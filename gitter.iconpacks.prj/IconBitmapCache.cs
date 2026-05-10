#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.IconPacks;

using System;
using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// LRU cache of rasterized icon bitmaps keyed by (icon-name, pixel-size).
/// Bytes are estimated as 4 * size * size (BGRA premultiplied).
/// </summary>
public sealed class IconBitmapCache : IDisposable
{
	private readonly long _maxBytes;
	private readonly Dictionary<Key, LinkedListNode<Entry>> _map;
	private readonly LinkedList<Entry> _lru = new();
	private long _currentBytes;
	private bool _disposed;

	public IconBitmapCache(long maxBytes = 32L * 1024 * 1024)
	{
		if(maxBytes <= 0) throw new ArgumentOutOfRangeException(nameof(maxBytes));
		_maxBytes = maxBytes;
		_map = new Dictionary<Key, LinkedListNode<Entry>>();
	}

	public long MaxBytes     => _maxBytes;
	public long CurrentBytes => _currentBytes;
	public int  Count        => _map.Count;

	public Bitmap GetOrAdd(string iconName, int pixelSize, Func<int, Bitmap> factory)
	{
		if(iconName is null) throw new ArgumentNullException(nameof(iconName));
		if(pixelSize <= 0)   throw new ArgumentOutOfRangeException(nameof(pixelSize));
		if(factory is null)  throw new ArgumentNullException(nameof(factory));
		ThrowIfDisposed();

		var key = new Key(iconName, pixelSize);
		lock(_map)
		{
			if(_map.TryGetValue(key, out var node))
			{
				_lru.Remove(node);
				_lru.AddFirst(node);
				return node.Value.Bitmap;
			}
		}

		var bitmap = factory(pixelSize);
		var bytes  = EstimateBytes(pixelSize);

		lock(_map)
		{
			if(_map.TryGetValue(key, out var existing))
			{
				bitmap.Dispose();
				_lru.Remove(existing);
				_lru.AddFirst(existing);
				return existing.Value.Bitmap;
			}
			var entry = new Entry(key, bitmap, bytes);
			var node  = new LinkedListNode<Entry>(entry);
			_lru.AddFirst(node);
			_map[key] = node;
			_currentBytes += bytes;
			Evict();
			return bitmap;
		}
	}

	public Bitmap? TryGet(string iconName, int pixelSize)
	{
		if(iconName is null) throw new ArgumentNullException(nameof(iconName));
		ThrowIfDisposed();
		var key = new Key(iconName, pixelSize);
		lock(_map)
		{
			if(_map.TryGetValue(key, out var node))
			{
				_lru.Remove(node);
				_lru.AddFirst(node);
				return node.Value.Bitmap;
			}
		}
		return null;
	}

	public void Clear()
	{
		lock(_map)
		{
			foreach(var node in _map.Values) node.Value.Bitmap.Dispose();
			_map.Clear();
			_lru.Clear();
			_currentBytes = 0;
		}
	}

	public void Dispose()
	{
		if(_disposed) return;
		_disposed = true;
		Clear();
	}

	private void Evict()
	{
		while(_currentBytes > _maxBytes && _lru.Last is { } last)
		{
			_lru.RemoveLast();
			_map.Remove(last.Value.Key);
			_currentBytes -= last.Value.Bytes;
			last.Value.Bitmap.Dispose();
		}
	}

	private void ThrowIfDisposed()
	{
		if(_disposed) throw new ObjectDisposedException(nameof(IconBitmapCache));
	}

	private static long EstimateBytes(int pixelSize) => 4L * pixelSize * pixelSize;

	private readonly record struct Key(string Name, int PixelSize);

	private sealed class Entry
	{
		public Entry(Key key, Bitmap bitmap, long bytes)
		{
			Key    = key;
			Bitmap = bitmap;
			Bytes  = bytes;
		}
		public Key    Key    { get; }
		public Bitmap Bitmap { get; }
		public long   Bytes  { get; }
	}
}

#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Globalization;
	using System.Reflection;

	public class CachedScaledImageResources
	{
		readonly struct Key : IEquatable<Key>
		{
			sealed class EqualityComparerImpl : IEqualityComparer<Key>
			{
				public bool Equals(Key x, Key y) => x == y;

				public int GetHashCode(Key obj) => obj.GetHashCode();
			}

			public static IEqualityComparer<Key> EqualityComparer { get; } = new EqualityComparerImpl();

			public Key(string name, int size)
			{
				Name = name;
				Size = size;
			}

			public string Name { get; }

			public int Size { get; }

			public override int GetHashCode()
				=> Name.GetHashCode() ^ Size;

			public override bool Equals(object obj)
				=> obj is Key other && this == other;

			public bool Equals(Key other)
				=> this == other;

			public static bool operator ==(Key a, Key b)
				=> a.Size == b.Size && a.Name == b.Name;

			public static bool operator !=(Key a, Key b)
				=> a.Size != b.Size && a.Name != b.Name;
		}

		private static readonly int[] Sizes = new int[]
			{
				16, 24, 32, 48, 64, 128, 256
			};

		private readonly Dictionary<Key, Bitmap> _cache = new(Key.EqualityComparer);

		public CachedScaledImageResources(Assembly assembly, string root)
		{
			Assembly = assembly;
			Root     = root;
		}

		private Assembly Assembly { get; }

		private string Root { get; }

		private static Bitmap Rescale(Bitmap original, int size)
		{
			var rescaled = new Bitmap(size, size);
			using var graphics = Graphics.FromImage(rescaled);
			graphics.Clear(Color.Transparent);
			graphics.DrawImage(original, new Rectangle(0, 0, size, size));
			return rescaled;
		}

		private Bitmap TryLoadExact(Key key)
		{
			var name = Root + "." + key.Name + "." + key.Size.ToString(CultureInfo.InvariantCulture) + ".png";
			using var stream = Assembly.GetManifestResourceStream(name);
			if(stream is null) return default;
			return new Bitmap(stream);
		}

		private Bitmap LoadBitmap(Key key)
		{
			var bitmap = TryLoadExact(key);
			if(bitmap is not null)
			{
				_cache.Add(key, bitmap);
				return bitmap;
			}

			int a = -1, b = Sizes.Length;
			for(int i = 0; i < Sizes.Length; ++i)
			{
				if(Sizes[i] == key.Size)
				{
					a = i - 1;
					b = i + 1;
					break;
				}
				if(Sizes[i] < key.Size)
				{
					a = i;
				}
				if(Sizes[i] > key.Size)
				{
					b = i;
					break;
				}
			}

			for(int i = b; i < Sizes.Length; ++i)
			{
				var sub = new Key(key.Name, Sizes[i]);
				if(_cache.TryGetValue(sub, out bitmap))
				{
					break;
				}
				bitmap = TryLoadExact(sub);
				if(bitmap is not null)
				{
					_cache.Add(sub, bitmap);
					break;
				}
			}

			if(bitmap is not null)
			{
				bitmap = Rescale(bitmap, key.Size);
				_cache.Add(key, bitmap);
				return bitmap;
			}

			for(int i = a; i >= 0; --i)
			{
				var sub = new Key(key.Name, Sizes[i]);
				if(_cache.TryGetValue(sub, out bitmap))
				{
					break;
				}
				bitmap = TryLoadExact(sub);
				if(bitmap is not null)
				{
					_cache.Add(sub, bitmap);
					break;
				}
			}

			if(bitmap is not null)
			{
				bitmap = Rescale(bitmap, key.Size);
				_cache.Add(key, bitmap);
				return bitmap;
			}

			return default;
		}

		public Bitmap this[string name, int size]
		{
			get
			{
				var key = new Key(name, size);
				if(!_cache.TryGetValue(key, out var bitmap))
				{
					bitmap = LoadBitmap(key);
				}
				return bitmap;
			}
		}
	}
}

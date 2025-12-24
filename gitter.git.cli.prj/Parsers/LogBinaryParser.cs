#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

#if NETCOREAPP

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

using gitter.Framework;
using gitter.Framework.CLI;

sealed class LogBinaryParser(
	Encoding encoding,
	Dictionary<Sha1Hash, RevisionData>? cache = null) : IBinaryParser<IList<RevisionData>>
{
	private readonly List<RevisionData> _log = [];
	private readonly Dictionary<Sha1Hash, RevisionData> _cache = cache ?? new(Sha1Hash.EqualityComparer);

	private byte[]? _buffer;
	private int _offset = 0;
	private readonly List<RevisionData> _parents = [];

	private RevisionData GetRevisionData(Sha1Hash hash)
	{
		if(!_cache.TryGetValue(hash, out var data))
		{
			_cache.Add(hash, data = new(hash));
		}
		return data;
	}

	private static Sha1Hash ParseHash(ref ReadOnlySpan<byte> data)
	{
		if(!Sha1Hash.TryParse(data[..Sha1Hash.HexStringLength], out var hash))
		{
			throw new FormatException();
		}
		data = data[(Sha1Hash.HexStringLength + 1)..];
		return hash;
	}

	private RevisionData ParseRevisionByHash(ref ReadOnlySpan<byte> data)
		=> GetRevisionData(ParseHash(ref data));

	private static Many<T> CaptureAndReset<T>(List<T> values)
	{
		var result = new Many<T>(values);
		values.Clear();
		return result;
	}

	private Many<RevisionData> ParseHashList(ref ReadOnlySpan<byte> data)
	{
		var f = data[0];
		if(f == (byte)'\n')
		{
			data = data[1..];
			return Many<RevisionData>.None;
		}
		while(data.Length != 0)
		{
			if(!Sha1Hash.TryParse(data[..Sha1Hash.HexStringLength], out var hash))
			{
				throw new FormatException();
			}
			_parents.Add(GetRevisionData(hash));
			data = data[Sha1Hash.HexStringLength..];
			f = data[0];
			if(f == (byte)'\n')
			{
				data = data[1..];
				break;
			}
			if(f != (byte)' ') throw new FormatException();
			data = data[1..];
		}
		return CaptureAndReset(_parents);
	}

	private static DateTimeOffset ParseTimestamp(ref ReadOnlySpan<byte> data)
	{
		var size = data[StrictISO8601TimestampParser.OffsetOf.UtcOffsetSign] == 'Z'
			? 20
			: 25;
		var ts = StrictISO8601TimestampParser.Parse(data[..size]);
		data = data[(size + 1)..];
		return ts;
	}

	private string ParseLine(ref ReadOnlySpan<byte> data)
	{
		var index = data.IndexOf((byte)'\n');
		if(index < 0)
		{
			var value = encoding.GetString(data);
			data = default;
			return value;
		}
		else
		{
			var value = encoding.GetString(data[..index]);
			data = data[(index + 1)..];
			return value;
		}
	}

	private RevisionData ParseRevision(ReadOnlySpan<byte> data)
	{
		var rev            = ParseRevisionByHash(ref data);
		rev.TreeHash       = ParseHash          (ref data);
		rev.Parents        = ParseHashList      (ref data);
		rev.CommitDate     = ParseTimestamp     (ref data);
		rev.CommitterName  = ParseLine          (ref data);
		rev.CommitterEmail = ParseLine          (ref data);
		rev.AuthorDate     = ParseTimestamp     (ref data);
		rev.AuthorName     = ParseLine          (ref data);
		rev.AuthorEmail    = ParseLine          (ref data);
		rev.Subject        = ParseLine          (ref data);
		if(data.Length != 0)
		{
			rev.Body = encoding.GetString(data);
		}
		return rev;
	}

	private void EnsureBufferSize(int size)
	{
		if(_buffer is not null && _buffer.Length >= size) return;

		var next = ArrayPool<byte>.Shared.Rent(Math.Max(size, 1024 * 4));
		if(_buffer is not null)
		{
			if(_offset != 0)
			{
				_buffer.AsSpan(0, _offset).CopyTo(next);
			}
			ArrayPool<byte>.Shared.Return(_buffer);
		}
		_buffer = next;
	}

	private void BufferAppend(in ReadOnlySpan<byte> data)
	{
		EnsureBufferSize(_offset + data.Length);
		data.CopyTo(_buffer.AsSpan(_offset));
		_offset += data.Length;
	}

	public void Parse(ReadOnlySpan<byte> data)
	{
		while(data.Length != 0)
		{
			var index = data.IndexOf((byte)'\0');
			if(index < 0)
			{
				BufferAppend(data);
				break;
			}
			if(_offset == 0)
			{
				_log.Add(ParseRevision(data[..index]));
			}
			else
			{
				BufferAppend(data[..index]);
				_log.Add(ParseRevision(_buffer.AsSpan(0, _offset)));
				_offset = 0;
			}
			data = data[(index + 1)..];
		}
	}

	public void Complete()
	{
		if(_buffer is not null)
		{
			if(_offset != 0)
			{
				_log.Add(ParseRevision(_buffer.AsSpan(0, _offset)));
				_offset = 0;
			}
			ArrayPool<byte>.Shared.Return(_buffer);
			_buffer = default;
		}
	}

	public void Reset()
	{
		_log.Clear();
	}

	public IList<RevisionData> GetResult() => _log;
}

#endif

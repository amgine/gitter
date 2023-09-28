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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using gitter.Framework;
using gitter.Framework.CLI;

sealed partial class LogParser : ITextParser<IList<RevisionData>>
{
	#region Helpers

	interface ITextFieldParser
	{
#if NETCOREAPP
		bool Parse(ref ReadOnlySpan<char> text);
#else
		bool Parse(ITextSegment textSegment);
#endif

		void Reset();
	}

	interface ITextFieldParser<out T> : ITextFieldParser
	{
		T GetValue();
	}

	enum FieldParserState
	{
		Initial,
		Buffering,
		WaitingTerminator,
		Completed,
	}

	readonly record struct CommitMessage(string Subject, string Body);

#if NETCOREAPP

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool FillBufferExcludeLastChar(char[] buffer, int total, ref int offset, ref ReadOnlySpan<char> text)
	{
		if(offset < total)
		{
			int need = total - offset;
			if(text.Length > need)
			{
				text[..need].CopyTo(new(buffer, offset, need));
				text = text[(need + 1)..];
				offset = total + 1;
				return true;
			}
			else
			{
				text.CopyTo(new(buffer, offset, text.Length));
				offset += text.Length;
				text = default;
				return false;
			}
		}
		if(offset == total && text.Length > 0)
		{
			offset = total + 1;
			text = text[1..];
			return true;
		}
		return false;
	}

#else

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool FillBufferExcludeLastChar(char[] buffer, int total, ref int offset, ITextSegment textSegment)
	{
		if(offset < total)
		{
			int c = Math.Min(textSegment.Length, total - offset);
			textSegment.MoveTo(buffer, offset, c);
			offset += c;
		}
		if(offset == total && textSegment.Length > 0)
		{
			offset = total + 1;
			textSegment.Skip();
			return true;
		}
		return false;
	}

#endif

	sealed class UnixTimestampField : ITextFieldParser<DateTimeOffset>
	{
		const char Terminator = '\n';

		private long _timestamp;
		private bool _isCompleted;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendDigit(char digit)
		{
			int value = digit - '0';
			if(value is >= 0 and <= 9)
			{
				_timestamp = _timestamp * 10 + value;
			}
		}

#if NETCOREAPP

		public bool Parse(ref ReadOnlySpan<char> text)
		{
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			for(int i = 0; i < text.Length; ++i)
			{
				var c = text[i];
				if(c == Terminator)
				{
					text = text[(i + 1)..];
					_isCompleted = true;
					return true;
				}
				AppendDigit(c);
			}
			text = ReadOnlySpan<char>.Empty;
			return false;
		}

#else

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			while(textSegment.Length > 0)
			{
				var c = textSegment.ReadChar();
				if(c == Terminator)
				{
					_isCompleted = true;
					return true;
				}
				AppendDigit(c);
			}
			return false;
		}

#endif

		public DateTimeOffset GetValue() => DateTimeOffset.FromUnixTimeSeconds(_timestamp).ToLocalTime();

		public void Reset()
		{
			_timestamp = 0;
			_isCompleted = false;
		}
	}

	sealed class ISO8601TimestampField : ITextFieldParser<DateTimeOffset>
	{
		// 0123456789012345678901234
		// 2020-11-17 00:13:52 +0300

		private readonly char[] _buffer;
		private int _offset;

		public ISO8601TimestampField()
		{
			_buffer = new char[25];
		}

#if NETCOREAPP

		public bool Parse(ref ReadOnlySpan<char> text)
		{
			Verify.State.IsFalse(_offset > _buffer.Length, "Field is already completed.");

			return FillBufferExcludeLastChar(_buffer, _buffer.Length, ref _offset, ref text);
		}

#else

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_offset > _buffer.Length, "Field is already completed.");

			return FillBufferExcludeLastChar(_buffer, _buffer.Length, ref _offset, textSegment);
		}

#endif

		public void Reset() => _offset = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Digit(char c) => c - '0';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetYear() =>
			Digit(_buffer[0]) * 1000 +
			Digit(_buffer[1]) *  100 +
			Digit(_buffer[2]) *   10 +
			Digit(_buffer[3]) *    1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int Get2Digits(int offset) =>
			Digit(_buffer[offset + 0]) * 10 +
			Digit(_buffer[offset + 1]);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetMonth() => Get2Digits(5);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetDay() => Get2Digits(8);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetHours() => Get2Digits(11);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetMinutes() => Get2Digits(14);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetSeconds() => Get2Digits(17);

		const int OffsetSignIndex    = 20;
		const int OffsetHoursIndex   = 21;
		const int OffsetMinutesIndex = 23;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetOffsetSign()
			=> _buffer[OffsetSignIndex] switch
			{
				'+' =>  1,
				'-' => -1,
				_ => throw new FormatException($"Unexpected character at TZ offset sign: {_buffer[OffsetSignIndex]}"),
			};

		private TimeSpan GetOffset() => new(
			GetOffsetSign() *
			(Get2Digits(OffsetHoursIndex) * 60 + Get2Digits(OffsetMinutesIndex)) *
			TimeSpan.TicksPerMinute);

		public DateTimeOffset GetValue() => new(
			GetYear(), GetMonth(), GetDay(),
			GetHours(), GetMinutes(), GetSeconds(),
			GetOffset());
	}

	#endregion

	#region Data

	private readonly Dictionary<Hash, RevisionData> _cache;
	private readonly List<RevisionData> _log;
	private readonly ITextFieldParser<Hash> _commitHash;
	private readonly ITextFieldParser<Hash> _treeHash;
	private readonly ITextFieldParser<List<Hash>> _parents;
	private readonly ITextFieldParser<DateTimeOffset> _commitDate;
	private readonly ITextFieldParser<string> _committerName;
	private readonly ITextFieldParser<string> _committerEmail;
	private readonly ITextFieldParser<DateTimeOffset> _authorDate;
	private readonly ITextFieldParser<string> _authorName;
	private readonly ITextFieldParser<string> _authorEmail;
	private readonly ITextFieldParser<CommitMessage> _commitMessage;
	private readonly ITextFieldParser[] _fields;
	private int _currentFieldIndex;

	#endregion

	#region .ctor

	private static ITextFieldParser<DateTimeOffset> CreateTimestampParser(TimestampFormat timestampFormat)
		=> timestampFormat switch
		{
			TimestampFormat.Unix          => new UnixTimestampField(),
			TimestampFormat.ISO8601       => new ISO8601TimestampField(),
			TimestampFormat.StrictISO8601 => new StrictISO8601TimestampField(),
			_ => throw new ArgumentException($"Unsupported timestamp format: {timestampFormat}.", nameof(timestampFormat)),
		};

	public LogParser(Dictionary<Hash, RevisionData> cache = null, TimestampFormat timestampFormat = TimestampFormat.StrictISO8601)
	{
		_cache = cache ?? new Dictionary<Hash, RevisionData>(Hash.EqualityComparer);
		_log   = new List<RevisionData>();

		_fields = new ITextFieldParser[]
		{
			_commitHash     = new HashField(),
			_treeHash       = new HashField(),
			_parents        = new MultiHashFieldParser(),
			_commitDate     = CreateTimestampParser(timestampFormat),
			_committerName  = new StringLineFieldParser(),
			_committerEmail = new StringLineFieldParser(),
			_authorDate     = CreateTimestampParser(timestampFormat),
			_authorName     = new StringLineFieldParser(),
			_authorEmail    = new StringLineFieldParser(),
			_commitMessage  = new CommitMessageFieldParser(),
		};
	}

	#endregion

	#region Methods

	private RevisionData GetRevisionData(Hash sha1)
	{
		if(!_cache.TryGetValue(sha1, out var revisionData))
		{
			_cache.Add(sha1, revisionData = new(sha1));
		}
		return revisionData;
	}

	private RevisionData[] GetParents()
	{
		var parentHashes = _parents.GetValue();
		if(parentHashes.Count == 0)
		{
			return Preallocated<RevisionData>.EmptyArray;
		}
		var parents = new RevisionData[parentHashes.Count];
		for(int i = 0; i < parents.Length; ++i)
		{
			parents[i] = GetRevisionData(parentHashes[i]);
		}
		return parents;
	}

	private RevisionData BuildRevision()
	{
		var revisionData = GetRevisionData(_commitHash.GetValue());

		revisionData.TreeHash       = _treeHash.GetValue();
		revisionData.Parents        = GetParents();
		revisionData.CommitDate     = _commitDate.GetValue();
		revisionData.CommitterName  = _committerName.GetValue();
		revisionData.CommitterEmail = _committerEmail.GetValue();
		revisionData.AuthorDate     = _authorDate.GetValue();
		revisionData.AuthorName     = _authorName.GetValue();
		revisionData.AuthorEmail    = _authorEmail.GetValue();

		var message = _commitMessage.GetValue();

		revisionData.Subject        = message.Subject;
		revisionData.Body           = message.Body;

		return revisionData;
	}

	private void CompleteRevision()
	{
		_log.Add(BuildRevision());
		for(int i = 0; i < _fields.Length; ++i)
		{
			_fields[i].Reset();
		}
		_currentFieldIndex = 0;
	}

	private void NextField()
	{
		++_currentFieldIndex;
		if(_currentFieldIndex == _fields.Length)
		{
			CompleteRevision();
		}
	}

	#endregion

	#region IParser<IList<RevisionData>> Members

	public IList<RevisionData> GetResult() => _log;

	#endregion

	#region IParser Members

#if NETCOREAPP

	public void Parse(ReadOnlySpan<char> text)
	{
		while(text.Length > 0)
		{
			if(_fields[_currentFieldIndex].Parse(ref text))
			{
				NextField();
			}
		}
	}

#else

	public void Parse(ITextSegment textSegment)
	{
		Verify.Argument.IsNotNull(textSegment);

		while(textSegment.Length > 0)
		{
			if(_fields[_currentFieldIndex].Parse(textSegment))
			{
				NextField();
			}
		}
	}

#endif

	public void Complete()
	{
		if(_currentFieldIndex >= _fields.Length - 1)
		{
			_log.Add(BuildRevision());
			_currentFieldIndex = 0;
		}
	}

	public void Reset()
	{
		for(int i = 0; i <= _currentFieldIndex; ++i)
		{
			_fields[i].Reset();
		}
		_currentFieldIndex = 0;
		_log.Clear();
	}

	#endregion
}

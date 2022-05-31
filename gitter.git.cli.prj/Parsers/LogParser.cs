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
using System.Text;

using gitter.Framework.CLI;

sealed class LogParser : IParser<IList<RevisionData>>
{
	#region Helpers

	interface IField
	{
		bool Parse(ITextSegment textSegment);

		void Reset();
	}

	interface IField<out T> : IField
	{
		T GetValue();
	}

	record struct CommitMessage(string Subject, string Body);

	sealed class HashField : IField<Hash>
	{
		private readonly char[] _buffer;
		private int _offset;

		public HashField()
		{
			_buffer = new char[Hash.HexStringLength];
		}

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_offset > Hash.HexStringLength, "Field is already completed.");

			if(_offset < Hash.HexStringLength)
			{
				int c = Math.Min(textSegment.Length, Hash.HexStringLength - _offset);
				textSegment.MoveTo(_buffer, _offset, c);
				_offset += c;
			}
			if(_offset == Hash.HexStringLength && textSegment.Length > 0)
			{
				_offset = Hash.HexStringLength + 1;
				textSegment.Skip();
				return true;
			}
			return false;
		}

		public void Reset() => _offset = 0;

		public Hash GetValue() => new(_buffer);
	}

	sealed class MultiHashField : IField<List<Hash>>
	{
		private readonly char[] _buffer;
		private readonly List<Hash> _hashes;
		private int _offset;
		private bool _isCompleted;

		public MultiHashField()
		{
			_buffer = new char[Hash.HexStringLength];
			_hashes = new List<Hash>();
		}

		public void Reset()
		{
			_isCompleted = false;
			_offset = 0;
			_hashes.Clear();
		}

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			if(_offset == 0 && textSegment.Length > 0)
			{
				var term = textSegment.PeekChar();
				if(term == '\n')
				{
					textSegment.Skip(1);
					_isCompleted = true;
					return true;
				}
			}
			if(_offset < Hash.HexStringLength && textSegment.Length > 0)
			{
				int c = Math.Min(textSegment.Length, Hash.HexStringLength - _offset);
				textSegment.MoveTo(_buffer, _offset, c);
				_offset += c;
			}
			if(_offset == Hash.HexStringLength && textSegment.Length > 0)
			{
				_offset = 0;
				_hashes.Add(new Hash(_buffer));
				var separator = textSegment.ReadChar();
				if(separator == '\n')
				{
					_isCompleted = true;
					return true;
				}
			}
			return false;
		}

		public List<Hash> GetValue() => _hashes;
	}

	sealed class UnixTimestampField : IField<DateTimeOffset>
	{
		private long _timestamp;
		private bool _isCompleted;

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			while(textSegment.Length > 0)
			{
				var c = textSegment.ReadChar();
				if(c == '\n')
				{
					_isCompleted = true;
					return true;
				}
				int value = c - '0';
				if(value >= 0 && value <= 9)
				{
					_timestamp = _timestamp * 10 + value;
				}
			}
			return false;
		}

		public DateTimeOffset GetValue() => DateTimeOffset.FromUnixTimeSeconds(_timestamp).ToLocalTime();

		public void Reset()
		{
			_timestamp = 0;
			_isCompleted = false;
		}
	}

	sealed class ISO8601TimestampField : IField<DateTimeOffset>
	{
		// 0123456789012345678901234
		// 2020-11-17 00:13:52 +0300

		private readonly char[] _buffer;
		private int _offset;

		public ISO8601TimestampField()
		{
			_buffer = new char[25];
		}

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_offset > _buffer.Length, "Field is already completed.");

			if(_offset < _buffer.Length)
			{
				int c = Math.Min(textSegment.Length, _buffer.Length - _offset);
				textSegment.MoveTo(_buffer, _offset, c);
				_offset += c;
			}
			if(_offset == _buffer.Length && textSegment.Length > 0)
			{
				_offset = _buffer.Length + 1;
				textSegment.Skip();
				return true;
			}
			return false;
		}

		public void Reset() => _offset = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Digit(char c) => c - '0';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetYear() =>
			Digit(_buffer[0]) * 1000 +
			Digit(_buffer[1]) * 100 +
			Digit(_buffer[2]) * 10 +
			Digit(_buffer[3]);

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

		private TimeSpan GetOffset()
		{
			var sign = _buffer[20] switch
			{
				'+' =>  1,
				'-' => -1,
				_ => throw new FormatException(),
			};

			var h = Get2Digits(21);
			var m = Get2Digits(23);

			return new TimeSpan(sign * (h * 60 + m) * 60 * 10000000L);
		}

		public DateTimeOffset GetValue() => new(
			GetYear(), GetMonth(), GetDay(),
			GetHours(), GetMinutes(), GetSeconds(),
			GetOffset());
	}

	sealed class StrictISO8601TimestampField : IField<DateTimeOffset>
	{
		// 0123456789012345678901234
		// 2020-06-02T16:21:00+03:00

		private readonly char[] _buffer;
		private int _offset;

		public StrictISO8601TimestampField()
		{
			_buffer = new char[25];
		}

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_offset > _buffer.Length, "Field is already completed.");

			if(_offset < _buffer.Length)
			{
				int c = Math.Min(textSegment.Length, _buffer.Length - _offset);
				textSegment.MoveTo(_buffer, _offset, c);
				_offset += c;
			}
			if(_offset == _buffer.Length && textSegment.Length > 0)
			{
				_offset = _buffer.Length + 1;
				textSegment.Skip();
				return true;
			}
			return false;
		}

		public void Reset() => _offset = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Digit(char c) => c - '0';

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetYear() =>
			Digit(_buffer[0]) * 1000 +
			Digit(_buffer[1]) * 100 +
			Digit(_buffer[2]) * 10 +
			Digit(_buffer[3]);

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

		private TimeSpan GetOffset()
		{
			var sign = _buffer[19] switch
			{
				'+' =>  1,
				'-' => -1,
				_ => throw new FormatException(),
			};

			var h = Get2Digits(20);
			var m = Get2Digits(23);

			return new TimeSpan(sign * (h * 60 + m) * 60 * TimeSpan.TicksPerSecond);
		}

		public DateTimeOffset GetValue() => new(
			GetYear(), GetMonth(), GetDay(),
			GetHours(), GetMinutes(), GetSeconds(),
			GetOffset());
	}

	sealed class StringLineField : IField<string>
	{
		private readonly StringBuilder _line;
		private bool _isCompleted;

		public StringLineField()
		{
			_line = new StringBuilder();
		}

		public string GetValue() => _line.ToString();

		#region IField Members

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			if(textSegment.Length > 0)
			{
				int eol = textSegment.IndexOf('\n');
				if(eol == -1)
				{
					textSegment.MoveTo(_line, textSegment.Length);
				}
				else
				{
					if(eol != 0)
					{
						textSegment.MoveTo(_line, eol);
					}
					textSegment.Skip(1);
					return true;
				}
			}
			return false;
		}

		public void Reset()
		{
			_line.Clear();
			_isCompleted = false;
		}

		#endregion
	}

	sealed class CommitMessageField : IField<CommitMessage>
	{
		sealed class EmptyLineSeparator
		{
			private readonly char[] _buffer;
			private int _length;
			private int _lineEndings;

			public EmptyLineSeparator()
			{
				_buffer = new char[4];
			}

			public int Length => _length;

			public bool Append(char c)
			{
				if(c == '\n')
				{
					if(_length == 0)
					{
						++_lineEndings;
					}
					else
					{
						if(_buffer[_length - 1] != '\r')
						{
							++_lineEndings;
						}
					}
				}
				_buffer[_length++] = c;
				return _lineEndings == 2;
			}

			public void Dump(StringBuilder stringBuilder)
			{
				Assert.IsNotNull(stringBuilder);

				stringBuilder.Append(_buffer, 0, _length);
				Reset();
			}

			public void Reset()
			{
				_length = 0;
				_lineEndings = 0;
			}
		}

		static readonly char[] Separators = new[] { '\0', '\r', '\n' };

		private readonly StringBuilder _subject;
		private readonly StringBuilder _body;
		private readonly EmptyLineSeparator _separator;
		private bool _isSubjectCompleted;
		private bool _isCompleted;

		public CommitMessageField()
		{
			_subject   = new StringBuilder();
			_body      = new StringBuilder();
			_separator = new EmptyLineSeparator();
		}

		public CommitMessage GetValue() => new(_subject.ToString(), _body.ToString());

		private static void RemoveTrailingWhitespace(StringBuilder stringBuilder)
		{
			Assert.IsNotNull(stringBuilder);

			int offset = stringBuilder.Length - 1;
			while(offset >= 0 && char.IsWhiteSpace(stringBuilder[offset]))
			{
				--offset;
			}
			++offset;
			if(offset < stringBuilder.Length)
			{
				stringBuilder.Remove(offset, stringBuilder.Length - offset);
			}
		}

		#region IField Members

		public bool Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment);
			Verify.State.IsFalse(_isCompleted, "Field is already completed.");

			while(textSegment.Length > 0)
			{
				int separatorIndex = _isSubjectCompleted ?
					textSegment.IndexOf('\0') :
					textSegment.IndexOfAny(Separators);
				if(separatorIndex == -1)
				{
					if(_isSubjectCompleted)
					{
						textSegment.MoveTo(_body, textSegment.Length);
					}
					else
					{
						if(_separator.Length != 0)
						{
							_separator.Dump(_subject);
						}
						textSegment.MoveTo(_subject, textSegment.Length);
					}
					return false;
				}
				else
				{
					if(_isSubjectCompleted)
					{
						textSegment.MoveTo(_body, separatorIndex);
					}
					else
					{
						if(separatorIndex != 0)
						{
							if(_separator.Length != 0)
							{
								_separator.Dump(_subject);
							}
							textSegment.MoveTo(_subject, separatorIndex);
						}
					}
					var separatorChar = textSegment.ReadChar();
					switch(separatorChar)
					{
						case '\0':
							RemoveTrailingWhitespace(_subject);
							if(_isSubjectCompleted)
							{
								RemoveTrailingWhitespace(_body);
							}
							_isCompleted = true;
							return true;
						case '\r':
						case '\n':
							if(_isSubjectCompleted)
							{
								_body.Append(separatorChar);
							}
							else
							{
								if(_separator.Append(separatorChar))
								{
									_isSubjectCompleted = true;
									_separator.Reset();
								}
							}
							break;
					}
				}
			}
			return false;
		}

		public void Reset()
		{
			_subject.Clear();
			_body.Clear();
			_separator.Reset();
			_isSubjectCompleted = false;
			_isCompleted = false;
		}

		#endregion
	}

	#endregion

	#region Data

	private readonly Dictionary<Hash, RevisionData> _cache;
	private readonly List<RevisionData> _log;
	private readonly IField<Hash> _commitHash;
	private readonly IField<Hash> _treeHash;
	private readonly IField<List<Hash>> _parents;
	private readonly IField<DateTimeOffset> _commitDate;
	private readonly IField<string> _committerName;
	private readonly IField<string> _committerEmail;
	private readonly IField<DateTimeOffset> _authorDate;
	private readonly IField<string> _authorName;
	private readonly IField<string> _authorEmail;
	private readonly IField<CommitMessage> _commitMessage;
	private readonly IField[] _fields;
	private int _currentFieldIndex;

	#endregion

	#region .ctor

	private static IField<DateTimeOffset> CreateTimestampParser(TimestampFormat timestampFormat)
		=> timestampFormat switch
		{
			TimestampFormat.Unix          => new UnixTimestampField(),
			TimestampFormat.ISO8601       => new ISO8601TimestampField(),
			TimestampFormat.StrictISO8601 => new StrictISO8601TimestampField(),
			_ => throw new ArgumentException("Unsupported timestamp format.", nameof(timestampFormat)),
		};

	public LogParser(Dictionary<Hash, RevisionData> cache = null, TimestampFormat timestampFormat = TimestampFormat.StrictISO8601)
	{
		_cache = cache ?? new Dictionary<Hash, RevisionData>(Hash.EqualityComparer);
		_log   = new List<RevisionData>();

		_fields = new IField[]
		{
			_commitHash     = new HashField(),
			_treeHash       = new HashField(),
			_parents        = new MultiHashField(),
			_commitDate     = CreateTimestampParser(timestampFormat),
			_committerName  = new StringLineField(),
			_committerEmail = new StringLineField(),
			_authorDate     = CreateTimestampParser(timestampFormat),
			_authorName     = new StringLineField(),
			_authorEmail    = new StringLineField(),
			_commitMessage  = new CommitMessageField(),
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

	#endregion

	#region IParser<IList<RevisionData>> Members

	public IList<RevisionData> GetResult() => _log;

	#endregion

	#region IParser Members

	public void Parse(ITextSegment textSegment)
	{
		Verify.Argument.IsNotNull(textSegment);

		while(textSegment.Length > 0)
		{
			if(_fields[_currentFieldIndex].Parse(textSegment))
			{
				++_currentFieldIndex;
				if(_currentFieldIndex == _fields.Length)
				{
					_log.Add(BuildRevision());
					for(int i = 0; i < _fields.Length; ++i)
					{
						_fields[i].Reset();
					}
					_currentFieldIndex = 0;
				}
			}
		}
	}

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

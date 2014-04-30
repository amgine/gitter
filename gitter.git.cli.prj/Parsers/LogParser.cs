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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
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

		sealed class HashField : IField
		{
			private readonly char[] _buffer;
			private int _offset;

			public HashField()
			{
				_buffer = new char[40];
			}

			public bool Parse(ITextSegment textSegment)
			{
				Verify.Argument.IsNotNull(textSegment, "textSegment");
				Verify.State.IsFalse(_offset == 41, "Field is already completed.");

				if(_offset < 40)
				{
					int c = Math.Min(textSegment.Length, 40 - _offset);
					textSegment.MoveTo(_buffer, _offset, c);
					_offset += c;
				}
				if(_offset == 40 && textSegment.Length > 0)
				{
					_offset = 41;
					textSegment.Skip(1);
					return true;
				}
				return false;
			}

			public void Reset()
			{
				_offset = 0;
			}

			public Hash GetValue()
			{
				return new Hash(_buffer);
			}
		}

		sealed class MultiHashField : IField
		{
			private readonly char[] _buffer;
			private readonly List<Hash> _hashes;
			private int _offset;
			private bool _isCompleted;

			public MultiHashField()
			{
				_buffer = new char[40];
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
				Verify.Argument.IsNotNull(textSegment, "textSegment");
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
				if(_offset < 40 && textSegment.Length > 0)
				{
					int c = Math.Min(textSegment.Length, 40 - _offset);
					textSegment.MoveTo(_buffer, _offset, c);
					_offset += c;
				}
				if(_offset == 40 && textSegment.Length > 0)
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

			public List<Hash> GetValue()
			{
				return _hashes;
			}
		}

		sealed class UnixTimestampField : IField
		{
			private long _timestamp;
			private bool _isCompleted;

			public bool Parse(ITextSegment textSegment)
			{
				Verify.Argument.IsNotNull(textSegment, "textSegment");
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

			public DateTime GetValue()
			{
				return GitConstants.UnixEraStart.AddSeconds(_timestamp).ToLocalTime();
			}

			public void Reset()
			{
				_timestamp = 0;
				_isCompleted = false;
			}
		}

		sealed class StringLineField : IField
		{
			private readonly StringBuilder _line;
			private bool _isCompleted;

			public StringLineField()
			{
				_line = new StringBuilder();
			}

			public string GetValue()
			{
				return _line.ToString();
			}

			#region IField Members

			public bool Parse(ITextSegment textSegment)
			{
				Verify.Argument.IsNotNull(textSegment, "textSegment");
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

		sealed class SubjectAndBodyField : IField
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

				public int Length
				{
					get { return _length; }
				}

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

			static readonly char[] Separators = new char[] { '\0', '\r', '\n' };

			private readonly StringBuilder _subject;
			private readonly StringBuilder _body;
			private readonly EmptyLineSeparator _separator;
			private bool _isSubjectCompleted;
			private bool _isCompleted;

			public SubjectAndBodyField()
			{
				_subject   = new StringBuilder();
				_body      = new StringBuilder();
				_separator = new EmptyLineSeparator();
			}

			public string GetSubject()
			{
				return _subject.ToString();
			}

			public string GetBody()
			{
				return _body.ToString();
			}

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
				Verify.Argument.IsNotNull(textSegment, "textSegment");
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
		private readonly HashField _commitHash;
		private readonly HashField _treeHash;
		private readonly MultiHashField _parents;
		private readonly UnixTimestampField _commitDate;
		private readonly StringLineField _committerName;
		private readonly StringLineField _committerEmail;
		private readonly UnixTimestampField _authorDate;
		private readonly StringLineField _authorName;
		private readonly StringLineField _authorEmail;
		private readonly SubjectAndBodyField _subjectAndBody;
		private readonly IField[] _fields;
		private int _currentFieldIndex;

		#endregion

		#region .ctor

		public LogParser(Dictionary<Hash, RevisionData> cache)
		{
			Verify.Argument.IsNotNull(cache, "cache");

			_cache = cache;
			_log   = new List<RevisionData>();

			_fields = new IField[]
			{
				_commitHash     = new HashField(),
				_treeHash       = new HashField(),
				_parents        = new MultiHashField(),
				_commitDate     = new UnixTimestampField(),
				_committerName  = new StringLineField(),
				_committerEmail = new StringLineField(),
				_authorDate     = new UnixTimestampField(),
				_authorName     = new StringLineField(),
				_authorEmail    = new StringLineField(),
				_subjectAndBody = new SubjectAndBodyField(),
			};
		}

		public LogParser()
			: this(new Dictionary<Hash, RevisionData>(Hash.EqualityComparer))
		{
		}

		#endregion

		#region Methods

		private RevisionData GetRevisionData(Hash sha1)
		{
			RevisionData revisionData;
			if(!_cache.TryGetValue(sha1, out revisionData))
			{
				revisionData = new RevisionData(sha1);
				_cache.Add(sha1, revisionData);
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
			revisionData.Subject        = _subjectAndBody.GetSubject();
			revisionData.Body           = _subjectAndBody.GetBody();

			return revisionData;
		}

		#endregion

		#region IParser<IList<RevisionData>> Members

		public IList<RevisionData> GetResult()
		{
			return _log;
		}

		#endregion

		#region IParser Members

		public void Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment, "textSegment");

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
}

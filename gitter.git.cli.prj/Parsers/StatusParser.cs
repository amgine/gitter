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

	sealed class StatusParser : IParser<StatusData>
	{
		#region Helpers

		sealed class StatusLine
		{
			private char _x;
			private char _y;
			private readonly StringBuilder _from;
			private readonly StringBuilder _to;
			private int _offset;

			public StatusLine()
			{
				_from   = new StringBuilder(260);
				_to     = new StringBuilder(260);
				_offset = -3;
			}

			private static void RemoveTrailingSlash(StringBuilder stringBuilder)
			{
				Assert.IsNotNull(stringBuilder);

				if(stringBuilder.Length != 0 && stringBuilder[stringBuilder.Length - 1] == '/')
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
				}
			}

			private static bool ParseFileName(ITextSegment textSegment, StringBuilder stringBuilder)
			{
				Assert.IsNotNull(textSegment);
				Assert.IsNotNull(stringBuilder);

				int terminator = textSegment.IndexOf('\0');
				if(terminator == -1)
				{
					textSegment.MoveTo(stringBuilder, textSegment.Length);
				}
				else
				{
					if(terminator != 0)
					{
						textSegment.MoveTo(stringBuilder, terminator);
					}
					textSegment.Skip(1);
					RemoveTrailingSlash(stringBuilder);
					return true;
				}
				return false;
			}

			public bool Parse(ITextSegment textSegment)
			{
				Assert.IsNotNull(textSegment);

				while(textSegment.Length > 0)
				{
					switch(_offset)
					{
						case -3:
							_x = textSegment.ReadChar();
							++_offset;
							break;
						case -2:
							_y = textSegment.ReadChar();
							++_offset;
							break;
						case -1:
							textSegment.Skip(1);
							++_offset;
							break;
						case 0:
							if(ParseFileName(textSegment, _to))
							{
								++_offset;
								if(_x != 'C' && _x != 'R')
								{
									++_offset;
									return true;
								}
							}
							break;
						case 1:
							if(ParseFileName(textSegment, _from))
							{
								++_offset;
								return true;
							}
							break;
					}
				}
				return false;
			}

			public char X
			{
				get { return _x; }
			}

			public char Y
			{
				get { return _y; }
			}

			public string From
			{
				get { return _from.ToString(); }
			}

			public string To
			{
				get { return _to.ToString(); }
			}

			public void Reset()
			{
				_from.Clear();
				_to.Clear();
				_offset = -3;
			}
		}

		#endregion

		#region Data

		private readonly StatusLine _line;
		private readonly Dictionary<string, TreeFileData> _stagedFiles;
		private readonly Dictionary<string, TreeFileData> _unstagedFiles;
		private int _unstagedUntrackedCount;
		private int _unstagedRemovedCount;
		private int _unstagedModifiedCount;
		private int _unmergedCount;
		private int _stagedAddedCount;
		private int _stagedModifiedCount;
		private int _stagedRemovedCount;

		#endregion

		#region .ctor

		public StatusParser()
		{
			_line          = new StatusLine();
			_stagedFiles   = new Dictionary<string, TreeFileData>();
			_unstagedFiles = new Dictionary<string, TreeFileData>();
		}

		#endregion

		#region Methods

		private static FileStatus CharToFileStatus(char c)
		{
			switch(c)
			{
				case 'M':
					return FileStatus.Modified;
				case 'A':
					return FileStatus.Added;
				case 'D':
					return FileStatus.Removed;
				case '?':
					return FileStatus.Added;
				case 'U':
					return FileStatus.Unmerged;
				case ' ':
					return FileStatus.Unknown;
				case '!':
					return FileStatus.Ignored;
				default:
					return FileStatus.Unknown;
			}
		}

		private static ConflictType GetConflictType(char x, char y)
		{
			if(IsUnmergedState(x, y))
			{
				if(x == y)
				{
					if(x == 'A')
						return ConflictType.BothAdded;
					if(x == 'D')
						return ConflictType.BothDeleted;
					if(x == 'U')
						return ConflictType.BothModified;
					return ConflictType.Unknown;
				}
				else
				{
					switch(x)
					{
						case 'U':
							switch(y)
							{
								case 'A':
									return ConflictType.AddedByThem;
								case 'D':
									return ConflictType.DeletedByThem;
								default:
									return ConflictType.Unknown;
							}
						case 'A':
							switch(y)
							{
								case 'U':
									return ConflictType.AddedByUs;
								default:
									return ConflictType.Unknown;
							}
						case 'D':
							switch(y)
							{
								case 'U':
									return ConflictType.DeletedByUs;
								default:
									return ConflictType.Unknown;
							}
						default:
							return ConflictType.Unknown;
					}
				}
			}
			else
			{
				return ConflictType.None;
			}
		}

		private static bool IsUnmergedState(char x, char y)
		{
			return (x == 'U' || y == 'U') || (x == 'A' && y == 'A') || (x == 'D' && y == 'D');
		}

		private void AddUnstagedStats(FileStatus fileStatus, int count)
		{
			switch(fileStatus)
			{
				case FileStatus.Added:
					_unstagedUntrackedCount += count;
					break;
				case FileStatus.Removed:
					_unstagedRemovedCount += count;
					break;
				case FileStatus.Modified:
					_unstagedModifiedCount += count;
					break;
			}
		}

		private void AddStagedStats(FileStatus fileStatus, int count)
		{
			switch(fileStatus)
			{
				case FileStatus.Added:
					_stagedAddedCount += count;
					break;
				case FileStatus.Removed:
					_stagedRemovedCount += count;
					break;
				case FileStatus.Modified:
					_stagedModifiedCount += count;
					break;
			}
		}

		#endregion

		#region IParser<StatusData> Members

		public StatusData GetResult()
		{
			return new StatusData(
				_stagedFiles, _unstagedFiles,
				_unstagedUntrackedCount,
				_unstagedRemovedCount,
				_unstagedModifiedCount,
				_unmergedCount,
				_stagedAddedCount,
				_stagedModifiedCount,
				_stagedRemovedCount);
		}

		#endregion

		#region IParser Members

		public void Parse(ITextSegment textSegment)
		{
			Verify.Argument.IsNotNull(textSegment, "textSegment");

			while(textSegment.Length > 0)
			{
				if(_line.Parse(textSegment))
				{
					bool staged            = false;
					bool unstaged          = false;
					var conflictType       = ConflictType.None;
					var stagedFileStatus   = FileStatus.Unknown;
					var unstagedFileStatus = FileStatus.Unknown;

					var x  = _line.X;
					var y  = _line.Y;
					var to = _line.To;

					if(x == '?')
					{
						staged             = false;
						unstaged           = true;
						unstagedFileStatus = FileStatus.Added;
						++_unstagedUntrackedCount;
					}
					else
					{
						if(x == 'C' || x == 'R')
						{
							var from = _line.From;
							if(x == 'C')
							{
								x = 'A';
								stagedFileStatus = FileStatus.Added;
							}
							else
							{
								if(!_stagedFiles.ContainsKey(from))
								{
									var file = new TreeFileData(from, FileStatus.Removed, ConflictType.None, StagedStatus.Staged);
									_stagedFiles.Add(from, file);
									++_stagedRemovedCount;
								}
								x = 'A';
								stagedFileStatus = FileStatus.Added;
							}
						}
						conflictType = GetConflictType(x, y);
						if(conflictType != ConflictType.None)
						{
							staged             = false;
							unstaged           = true;
							unstagedFileStatus = FileStatus.Unmerged;
							++_unmergedCount;
						}
						else
						{
							if(x != ' ')
							{
								staged = true;
								stagedFileStatus = CharToFileStatus(x);
								AddStagedStats(stagedFileStatus, 1);
							}
							if(y != ' ')
							{
								unstaged = true;
								unstagedFileStatus = CharToFileStatus(y);
								AddUnstagedStats(unstagedFileStatus, 1);
							}
						}
					}

					if(staged)
					{
						var file = new TreeFileData(to, stagedFileStatus, ConflictType.None, StagedStatus.Staged);
						TreeFileData existing;
						if(_stagedFiles.TryGetValue(to, out existing))
						{
							AddStagedStats(existing.FileStatus, -1);
							_stagedFiles[to] = file;
						}
						else
						{
							_stagedFiles.Add(to, file);
						}
					}
					if(unstaged)
					{
						var file = new TreeFileData(to, unstagedFileStatus, conflictType, StagedStatus.Unstaged);
						TreeFileData existing;
						if(_unstagedFiles.TryGetValue(to, out existing))
						{
							if(existing.FileStatus == FileStatus.Removed)
							{
								--_unstagedRemovedCount;
								_unstagedFiles[to] = file;
							}
						}
						else
						{
							_unstagedFiles.Add(to, file);
						}
					}

					_line.Reset();
				}
			}
		}

		public void Complete()
		{
		}

		public void Reset()
		{
			_stagedFiles.Clear();
			_unstagedFiles.Clear();
			_unstagedUntrackedCount = 0;
			_unstagedRemovedCount = 0;
			_unstagedModifiedCount = 0;
			_unmergedCount = 0;
			_stagedAddedCount = 0;
			_stagedModifiedCount = 0;
			_stagedRemovedCount = 0;
		}

		#endregion
	}
}

#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.Globalization;

	using gitter.Framework;

	/// <summary>Parser for git output.</summary>
	internal class GitParser : Parser
	{
		/// <summary>Create <see cref="GitParser"/>.</summary>
		/// <param name="string">String to parse.</param>
		public GitParser(string @string)
			: base(@string)
		{
		}

		private static byte ByteFromOctString(string str, int offset, int length)
		{
			int res = 0;
			for(int i = offset + length - 1, n = 1; i >= offset; --i)
			{
				res += (str[i] - '0') * n;
				n *= 8;
			}
			return (byte)res;
		}

		public string DecodeEscapedString(int end)
		{
			return DecodeEscapedString(end, 0, GitProcess.DefaultEncoding);
		}

		public string DecodeEscapedString(int end, Encoding encoding)
		{
			return DecodeEscapedString(end, 0, encoding);
		}

		public string DecodeEscapedString(int end, int skip)
		{
			return DecodeEscapedString(end, skip, GitProcess.DefaultEncoding);
		}

		private static void HandleEscapeCode(StringBuilder sb, char nc)
		{
			switch(nc)
			{
				case '\\':
					sb.Append('\\');
					break;
				case '\"':
					sb.Append('\"');
					break;
				case 't':
					sb.Append('\t');
					break;
				default:
					sb.Append(nc);
					break;
			}
		}

		private sealed class ByteString
		{
			private byte[] _buffer;
			private int _length;

			public static void Dump(ByteString str, StringBuilder sb, Encoding encoding)
			{
				if(str != null && !str.IsEmpty)
				{
					sb.Append(str.GetString(encoding));
					str.Clear();
				}
			}

			public ByteString(int bufferSize)
			{
				Verify.Argument.IsPositive(bufferSize, "bufferSize");

				_buffer = new byte[bufferSize];
			}

			public void AppendByte(byte @byte)
			{
				if(_length == _buffer.Length)
				{
					var buffer = new byte[_buffer.Length * 2];
					Array.Copy(_buffer, buffer, _buffer.Length);
					_buffer = buffer;
				}
				_buffer[_length++] = @byte;
			}

			public bool IsEmpty
			{
				get { return _length == 0; }
			}

			public int Length
			{
				get { return _length; }
			}

			public void Clear()
			{
				_length = 0;
			}

			public string GetString(Encoding encoding)
			{
				Verify.Argument.IsNotNull(encoding, "encoding");

				if(_length == 0)
				{
					return string.Empty;
				}
				else
				{
					return encoding.GetString(_buffer, 0, _length);
				}
			}
		}

		public string DecodeEscapedString(int end, int skip, Encoding encoding)
		{
			if(CheckValue('\"'))
			{
				var sb = new StringBuilder(end - Position);
				ByteString bs = null;
				var bytes = new byte[end - Position + 1];
				Skip();
				while(Position < end)
				{
					var c = ReadChar();
					if(c == '\\')
					{
						var nc = CurrentChar;
						if(nc.IsOctDigit())
						{
							if(bs == null)
							{
								var bufferSize = end - Position;
								if(bufferSize < 1) bufferSize = 1;
								bs = new ByteString(end - Position);
							}
							int len = 1;
							int start = Position;
							Skip();
							while(CurrentChar.IsOctDigit())
							{
								Skip();
								++len;
							}
							bs.AppendByte(ByteFromOctString(String, start, len));
						}
						else
						{
							ByteString.Dump(bs, sb, encoding);
							HandleEscapeCode(sb, nc);
							Skip();
						}
					}
					else if(c == '\"')
					{
						ByteString.Dump(bs, sb, encoding);
						break;
					}
					else
					{
						ByteString.Dump(bs, sb, encoding);
						sb.Append(c);
					}
				}
				Position = end + skip;
				return sb.ToString();
			}
			else
			{
				return ReadStringUpTo(end, skip);
			}
		}

		public BranchesData ParseBranches(QueryBranchRestriction restriction, bool allowFakeBranch)
		{
			var heads = new List<BranchData>();
			var remotes = new List<BranchData>();
			while(!IsAtEndOfString)
			{
				var branch = ParseBranch(restriction);
				if(branch != null && (allowFakeBranch || !branch.IsFake))
				{
					if(branch.IsRemote)
					{
						remotes.Add(branch);
					}
					else
					{
						heads.Add(branch);
					}
				}
			}
			return new BranchesData(heads, remotes);
		}

		public BranchData ParseBranch(QueryBranchRestriction restriction)
		{
			BranchData res;
			bool current = CheckValue('*');
			Skip(2);
			int eol = FindNewLineOrEndOfString();
			int space = FindSpace();
			if(current && (space == Position + 3) && CheckValue(GitConstants.NoBranch))
			{
				Skip(GitConstants.NoBranch.Length);
				Skip(' ');
				var sha1 = new Hash(String, Position);
				res = new BranchData(GitConstants.NoBranch, sha1, true, false, true);
			}
			else
			{
				var name = ReadStringUpTo(space, 1);
				Skip(' ');
				if(!(restriction == QueryBranchRestriction.Local) && CheckValue('-')) // it's a remote head indicator, skip it
				{
					res = null;
				}
				else
				{
					var sha1 = new Hash(String, Position);
					bool remote;
					switch(restriction)
					{
						case QueryBranchRestriction.All:
							remote = !current && name.StartsWith(GitConstants.RemoteBranchShortPrefix);
							if(remote) name = name.Substring(8);
							break;
						case QueryBranchRestriction.Local:
							remote = false;
							break;
						case QueryBranchRestriction.Remote:
							remote = true;
							break;
						default:
							throw new ArgumentException("restriction");
					}
					res = new BranchData(name, sha1, remote, current);
				}
			}
			Position = eol + 1;
			return res;
		}

		private void TrimProgressMessageEnd(ref int p, ref int trimEnd)
		{
			if(p - Position + 1 > 3)
			{
				if(String[p] == 'K' && String[p - 1] == '[' && String[p - 2] == '\u001B')
				{
					p -= 3;
					trimEnd += 3;
				}
			}
			if(p < Length)
			{
				while(p > Position && char.IsWhiteSpace(String[p]))
				{
					--p;
					++trimEnd;
				}
			}
		}

		public OperationProgress ParseProgress()
		{
			int trimEnd = 1;
			int p = FindNewLineOrEndOfString() - trimEnd;
			TrimProgressMessageEnd(ref p, ref trimEnd);

			int c = String.LastIndexOf(':', p);
			if(c == -1) return new OperationProgress(ReadStringUpTo(p + 1, trimEnd));
			int p1 = String.IndexOf('(', c);
			if(p1 == -1) return new OperationProgress(ReadStringUpTo(p + 1, trimEnd));
			int s = String.IndexOf('/', p1);
			if(s == -1) return new OperationProgress(ReadStringUpTo(p + 1, trimEnd));
			int p2 = String.IndexOf(')', s);
			if(p2 == -1) return new OperationProgress(ReadStringUpTo(p + 1, trimEnd));

			int max = 0;
			int cur = 0;
			if(!int.TryParse(String.Substring(p1 + 1, s - p1 - 1), NumberStyles.None, CultureInfo.InvariantCulture, out cur))
			{
				return new OperationProgress(ReadStringUpTo(p + 1, trimEnd));
			}
			if(!int.TryParse(String.Substring(s + 1, p2 - s - 1), NumberStyles.None, CultureInfo.InvariantCulture, out max))
			{
				return new OperationProgress(ReadStringUpTo(p + 1, trimEnd));
			}
			if(cur > max)
			{
				return new OperationProgress(ReadStringUpTo(p + 1, trimEnd));
			}

			var stage = ReadStringUpToNoAdvance(p1).Trim();
			Position = p + trimEnd;
			return new OperationProgress
			{
				ActionName		= stage,
				CurrentProgress	= cur,
				MaxProgress		= max,
				IsIndeterminate	= false,
				IsCompleted		= false,
			};
		}

		protected DateTime ReadUnixTimestampLine()
		{
			var timestampStr = ReadLine();
			long timestamp;
			if(string.IsNullOrWhiteSpace(timestampStr) || !long.TryParse(timestampStr, NumberStyles.None, CultureInfo.InvariantCulture, out timestamp))
			{
				return GitConstants.UnixEraStart;
			}
			else
			{
				var date = GitConstants.UnixEraStart.AddSeconds(timestamp).ToLocalTime();
				return date;
			}
		}

		public void ParseCommitParentsFromRaw(IEnumerable<RevisionData> revs, Dictionary<Hash, RevisionData> cache)
		{
			var parents = new List<RevisionData>();
			foreach(var rev in revs)
			{
				parents.Clear();
				int start = Position;
				int eoc = FindNullOrEndOfString();
				SkipLine();
				while(Position < eoc)
				{
					bool hasParents = false;
					while(CheckValue("parent ") && Position < eoc)
					{
						Skip(7);
						var p = ReadHash();
						SkipLine();
						RevisionData prd;
						if(cache != null)
						{
							if(!cache.TryGetValue(p, out prd))
							{
								prd = new RevisionData(p);
								cache.Add(p, prd);
							}
						}
						else
						{
							prd = new RevisionData(p);
						}
						parents.Add(prd);
						hasParents = true;
					}
					SkipLine();
					if(hasParents) break;
				}
				rev.Parents = parents.ToArray();
				Position = eoc + 1;
			}
		}

		public RevisionData ParseRevision()
		{
			var hash     = ReadHash(skip: 1);
			var revision = new RevisionData(hash);
			ParseRevisionData(revision, null);
			return revision;
		}

		private RevisionData[] ReadRevisionParents(Dictionary<Hash, RevisionData> cache)
		{
			int end = FindNewLineOrEndOfString();
			int numParents = (end - Position + 1) / 41;
			var parents = new RevisionData[numParents];
			if(numParents == 0)
			{
				Position = end + 1;
			}
			else
			{
				for(int i = 0; i < numParents; ++i)
				{
					var sha1 = ReadHash(skip: 1);
					if(cache == null)
					{
						parents[i] = new RevisionData(sha1);
					}
					else
					{
						if(!cache.TryGetValue(sha1, out parents[i]))
						{
							parents[i] = new RevisionData(sha1);
							cache.Add(sha1, parents[i]);
						}
					}
				}
			}
			return parents;
		}

		public Hash ReadHash()
		{
			var hash = new Hash(String, Position);
			Skip(40);
			return hash;
		}

		public Hash ReadHash(int skip)
		{
			Verify.Argument.IsNotNegative(skip, "skip");

			var hash = new Hash(String, Position);
			Skip(40 + skip);
			return hash;
		}

		public void ParseRevisionData(RevisionData rev, Dictionary<Hash, RevisionData> cache)
		{
			rev.TreeHash       = ReadHash(skip: 1);
			rev.Parents        = ReadRevisionParents(cache);
			rev.CommitDate     = ReadUnixTimestampLine();
			rev.CommitterName  = ReadLine();
			rev.CommitterEmail = ReadLine();
			rev.AuthorDate     = ReadUnixTimestampLine();
			rev.AuthorName     = ReadLine();
			rev.AuthorEmail    = ReadLine();

			// Subject + Body
			int eoc = FindNullOrEndOfString();
			int bodyStart;
			int subjectEnd = FindSeparatingEmptyLine(eoc, out bodyStart);
			if(subjectEnd == -1)
			{
				int eos = eoc - 1;
				char c = String[eos];
				while((c == '\r') || (c == '\n'))
				{
					c = String[--eos];
				}
				if(eos > Position)
				{
					rev.Subject = ReadStringUpToNoAdvance(eos + 1);
				}
				else
				{
					rev.Subject = string.Empty;
				}
				rev.Body = string.Empty;
			}
			else
			{
				rev.Subject = ReadStringUpToNoAdvance(subjectEnd);
				Position = bodyStart;
				int eob = eoc - 1;
				char c = String[eob];
				while((c == '\r') || (c == '\n'))
				{
					c = String[--eob];
				}
				if(eob > Position)
				{
					rev.Body = ReadStringUpToNoAdvance(eob + 1);
				}
				else
				{
					rev.Body = string.Empty;
				}
			}
			Position = eoc + 1;
		}
	}
}

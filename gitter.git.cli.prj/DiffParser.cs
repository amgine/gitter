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
	using System.Globalization;
	using System.Collections.Generic;

	/// <summary>Parser to use with git diffs.</summary>
	internal sealed class DiffParser : GitParser
	{
		#region Constants

		private const string FileHeader = "diff --";
		private const string GitHeader = "diff --git";
		private const string CombinedHeader = "diff --combined";
		private const string CCHeader = "diff --cc";

		private const string BinaryFilesHeader = "Binary files ";
		private const string BinaryPatchHeader = "GIT binary patch";

		private const string OldModeHeader = "old mode";
		private const string NewModeHeader = "new mode";
		private const string ModeHeader = "mode";
		private const string IndexHeader = "index";
		private const string DeletedFileModeHeader = "deleted file mode";
		private const string NewFileModeHeader = "new file mode";
		private const string CopyFromHeader = "copy from";
		private const string CopyToHeader = "copy to";
		private const string RenameFromHeader = "rename from";
		private const string RenameToHeader = "rename to";
		private const string SimilarityIndexHeader = "similarity index";
		private const string DissimilarityIndexHeader = "dissimilarity index";

		private const string DefaultPrefixA = "a/";
		private const string DefaultPrefixB = "b/";

		private const char NoNewlineHeader = '\\';
		private const char HunkHeader = '@';

		#endregion

		#region Headers

		private struct DiffFileHeader
		{
			public FileStatus Status;
			public string OldIndex;
			public string NewIndex;
			public int OldMode;
			public int NewMode;
			public string CopyTo;
			public string CopyFrom;
			public string RenameTo;
			public string RenameFrom;
			public int SimilarityIndex;
			public int DissimilarityIndex;

			public DiffFileHeader(FileStatus status)
			{
				Status = status;
				OldIndex = string.Empty;
				NewIndex = string.Empty;
				OldMode = 0;
				NewMode = 0;
				CopyTo = string.Empty;
				CopyFrom = string.Empty;
				RenameFrom = string.Empty;
				RenameTo = string.Empty;
				SimilarityIndex = -1;
				DissimilarityIndex = -1;
			}
		}

		private enum Header
		{
			Unknown = -1,

			OldMode = 0,
			NewMode = 1,
			Mode = 2,
			DeletedFileMode = 3,
			NewFileMode = 4,
			CopyFrom = 5,
			CopyTo = 6,
			RenameFrom = 7,
			RenameTo = 8,
			SimilarityIndex = 9,
			DissimilarityIndex = 10,
			Index = 11,
		}

		private static readonly string[] DiffHeaders = new[]
		{
			OldModeHeader,
			NewModeHeader,
			ModeHeader,
			DeletedFileModeHeader,
			NewFileModeHeader,
			CopyFromHeader,
			CopyToHeader,
			RenameFromHeader,
			RenameToHeader,
			SimilarityIndexHeader,
			DissimilarityIndexHeader,
			IndexHeader,
		};

		#endregion

		#region ReadDiffLine()

		private DiffLine ReadDiffLine(int[] nums)
		{
			int[] lineNums = (int[])nums.Clone();
			int nc = nums.Length;
			var states = new DiffLineState[nc];
			if(CheckValue(NoNewlineHeader) || CheckValue(HunkHeader))
			{
				for(int i = 0; i < nc; ++i)
				{
					states[i] = DiffLineState.Header;
				}
				return new DiffLine(DiffLineState.Header, states, lineNums, ReadLine());
			}
			var state = DiffLineState.Context;
			for(int i = 0; i < nc - 1; ++i)
			{
				switch(ReadChar())
				{
					case '-':
						state = DiffLineState.Removed;
						states[i] = DiffLineState.Removed;
						break;
					case '+':
						state = DiffLineState.Added;
						states[i] = DiffLineState.Added;
						break;
					case ' ':
						states[i] = DiffLineState.Context;
						break;
					default:
						states[i] = DiffLineState.Context;
						break;
				}
			}
			states[nc - 1] = DiffLineState.Context;
			switch(state)
			{
				case DiffLineState.Context:
					for(int i = 0; i < nc; ++i)
					{
						++nums[i];
					}
					break;
				case DiffLineState.Added:
					for(int i = 0; i < nc; ++i)
					{
						if(states[i] == DiffLineState.Context)
						{
							++nums[i];
						}
					}
					break;
				case DiffLineState.Removed:
					for(int i = 0; i < nc - 1; ++i)
					{
						if(states[i] == DiffLineState.Context)
						{
							states[i] = DiffLineState.NotPresent;
						}
						else if(states[i] == DiffLineState.Removed)
						{
							states[i] = DiffLineState.Context;
							++nums[i];
						}
					}
					states[nc - 1] = DiffLineState.Removed;
					break;
			}
			int eol = FindLfLineEnding();
			string ending;
			if(String[eol - 1] == '\r')
			{
				--eol;
				ending = LineEnding.CrLf;
			}
			else
			{
				ending = LineEnding.Lf;
			}
			var line = ReadStringUpTo(eol, ending.Length);
			return new DiffLine(state, states, lineNums, line, ending);
		}

		#endregion

		#region ReadDiffHunk()

		private DiffColumnHeader[] ReadDiffHunkHeader()
		{
			PushPosition();
			int colNum = Skip(HunkHeader);
			Skip();
			var headers = new DiffColumnHeader[colNum];
			for(int i = 0; i < colNum; ++i)
			{
				var action = DiffColumnAction.Remove;
				if(CheckValue('-'))
				{
					action = DiffColumnAction.Remove;
					Skip();
				}
				else if(CheckValue('+'))
				{
					action = DiffColumnAction.Add;
					Skip();
				}
				int space = FindSpace();
				int comma = FindComma(space - Position);
				if(comma != -1)
				{
					var lineNumber = int.Parse(
						ReadStringUpTo(comma, 1),
						NumberStyles.Integer,
						CultureInfo.InvariantCulture);
					var lineCount = int.Parse(
						ReadStringUpTo(space, 1),
						NumberStyles.Integer,
						CultureInfo.InvariantCulture);
					headers[i] = new DiffColumnHeader(action, lineNumber, lineCount);
				}
				else
				{
					var lineNumberStr = ReadStringUpTo(space, 1);
					var lineNumber = int.Parse(
						lineNumberStr,
						NumberStyles.Integer,
						CultureInfo.InvariantCulture);
					headers[i] = new DiffColumnHeader(action, lineNumber, 1);
				}
			}
			PopPosition();
			return headers;
		}

		private DiffHunk ReadTextPatchHunk()
		{
			var headers = ReadDiffHunkHeader();
			var nums = new int[headers.Length];
			int lineCount = 0;
			for(int i = 0; i < headers.Length; ++i)
			{
				nums[i] = headers[i].StartLine;
				lineCount += headers[i].LineCount;
			}
			var lines = new List<DiffLine>(lineCount);
			var stats = new DiffStats();
			while(!IsAtEndOfString)
			{
				var line = ReadDiffLine(nums);
				stats.Increment(line.State);
				lines.Add(line);
				if(CheckValue(HunkHeader) || CheckValue(FileHeader))
				{
					break;
				}
			}
			return new DiffHunk(headers, lines, stats, false);
		}

		private DiffHunk ReadBinaryFilesDifferHunk()
		{
			return new DiffHunk(
				new DiffColumnHeader[0],
				new DiffLine[]
				{
					new DiffLine(
						DiffLineState.Header,
						new DiffLineState[0],
						new int[0],
						ReadLine())
				},
				new DiffStats(0, 0, 0, 1),
				true);
		}

		private DiffHunk ReadBinaryPatchHunk()
		{
			var lines = new List<DiffLine>();
			int headers = 0;
			if(CheckValue(BinaryPatchHeader))
			{
				lines.Add(new DiffLine(DiffLineState.Header, new DiffLineState[0], new int[0], ReadLine()));
				++headers;
			}
			var state = CheckValue("literal ") || CheckValue("delta ") ? DiffLineState.Header : DiffLineState.Context;
			if(state == DiffLineState.Header) ++headers;
			while(!IsAtEndOfString)
			{
				lines.Add(new DiffLine(state, new DiffLineState[0], new int[0], ReadLine()));
				if(IsAtEndOfLine)
				{
					Skip();
					break;
				}
			}
			return new DiffHunk(new DiffColumnHeader[0], lines, new DiffStats(0, 0, lines.Count - headers, headers), true);
		}

		#endregion

		#region ReadDiffFile()

		private bool ReadDiffFileHeader1(out string source, out string target)
		{
			bool combined = CheckValue(CombinedHeader);
			bool cc = (!combined && CheckValue(CCHeader));
			int eol = FindNewLineOrEndOfString();

			if(combined)
			{
				Skip(CombinedHeader.Length + 1);
				source = target = DecodeEscapedString(eol, 1).Trim();
			}
			else
			{
				if(cc)
				{
					Skip(CCHeader.Length + 1);
					source = target = DecodeEscapedString(eol, 1).Trim();
				}
				else
				{
					int offset = 10;
					int len = (eol - Position - offset) / 2;
					offset += 1;
					len -= 1;
					Skip(offset);
					source = target = DecodeEscapedString(Position + len).Trim();
					if(target.StartsWith(DefaultPrefixA))
					{
						source = target = target.Substring(2);
					}
				}
			}
			Position = eol + 1;
			return combined || cc;
		}

		private DiffFileHeader ReadDiffFileHeader2(bool combinedDiff)
		{
			var fileHeader = new DiffFileHeader(FileStatus.Modified);

			bool[] headerPresent = new bool[DiffHeaders.Length];

			if(combinedDiff)
			{
				headerPresent[(int)Header.OldMode] = true;
				headerPresent[(int)Header.NewMode] = true;
				headerPresent[(int)Header.CopyFrom] = true;
				headerPresent[(int)Header.CopyTo] = true;
				headerPresent[(int)Header.RenameFrom] = true;
				headerPresent[(int)Header.RenameTo] = true;
				headerPresent[(int)Header.SimilarityIndex] = true;
				headerPresent[(int)Header.DissimilarityIndex] = true;
			}
			else
			{
				headerPresent[(int)Header.Mode] = true;
			}

			bool completed = false;
			while(!completed)
			{
				var header = Header.Unknown;
				bool dataPresent = false;
				for(int i = 0; i < DiffHeaders.Length; ++i)
				{
					if(!headerPresent[i])
					{
						if(CheckValue(DiffHeaders[i]))
						{
							Skip(DiffHeaders[i].Length);
							if(CheckValue(' '))
							{
								dataPresent = true;
							}
							Skip();
							headerPresent[i] = true;
							header = (Header)i;
							break;
						}
					}
				}
				switch(header)
				{
					case Header.Index:
						if(dataPresent)
						{
							if(combinedDiff)
							{
								// index <hash>,<hash>..<hash>
								SkipLine();
							}
							else
							{
								int pos = FindNoAdvance("..");
								fileHeader.OldIndex = ReadStringUpTo(pos, 2);
								pos = FindNewLineOrEndOfString();
								int space = FindSpace(pos - Position);
								if(space == -1)
								{
									// index <hash>..<hash>
									fileHeader.NewIndex = ReadStringUpTo(pos, 1);
								}
								else
								{
									// index <hash>..<hash> <mode>
									fileHeader.NewIndex = ReadStringUpTo(space, 1);
									fileHeader.NewMode = int.Parse(ReadStringUpTo(pos, 1), NumberStyles.None, CultureInfo.InvariantCulture);
								}
							}
						}
						break;
					case Header.OldMode:
						// old mode <mode>
						if(dataPresent)
						{
							fileHeader.OldMode = int.Parse(ReadLine(), NumberStyles.None, CultureInfo.InvariantCulture);
						}
						break;
					case Header.NewMode:
						// new mode <mode>
						if(dataPresent)
						{
							fileHeader.NewMode = int.Parse(ReadLine(), NumberStyles.None, CultureInfo.InvariantCulture);
						}
						break;
					case Header.Mode:
						// mode <mode>,<mode>..<mode>
						if(dataPresent)
						{
							SkipLine();
						}
						break;
					case Header.NewFileMode:
						// new file mode <mode>
						fileHeader.Status = FileStatus.Added;
						if(dataPresent)
						{
							fileHeader.NewMode = int.Parse(ReadLine(), NumberStyles.None, CultureInfo.InvariantCulture);
						}
						break;
					case Header.DeletedFileMode:
						fileHeader.Status = FileStatus.Removed;
						if(dataPresent)
						{
							if(combinedDiff)
							{
								// deleted file mode <mode>,<mode>
								SkipLine();
							}
							else
							{
								// deleted file mode <mode>
								fileHeader.OldMode = int.Parse(ReadLine(), NumberStyles.None, CultureInfo.InvariantCulture);
							}
						}
						break;
					case Header.CopyFrom:
						fileHeader.Status = FileStatus.Copied;
						if(dataPresent)
						{
							// copy from <path>
							fileHeader.CopyFrom = DecodeEscapedString(FindNewLineOrEndOfString(), 1);
						}
						break;
					case Header.CopyTo:
						fileHeader.Status = FileStatus.Copied;
						if(dataPresent)
						{
							// copy to <path>
							fileHeader.CopyTo = DecodeEscapedString(FindNewLineOrEndOfString(), 1);
						}
						break;
					case Header.RenameFrom:
						fileHeader.Status = FileStatus.Renamed;
						if(dataPresent)
						{
							// rename from <path>
							fileHeader.RenameFrom = DecodeEscapedString(FindNewLineOrEndOfString(), 1);
						}
						break;
					case Header.RenameTo:
						fileHeader.Status = FileStatus.Renamed;
						if(dataPresent)
						{
							// rename to <path>
							fileHeader.RenameTo = DecodeEscapedString(FindNewLineOrEndOfString(), 1);
						}
						break;
					case Header.SimilarityIndex:
						{
							int index;
							string strIndex = ReadStringUpTo(FindNewLineOrEndOfString() - 1);
							Skip(2);
							if(int.TryParse(strIndex, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
							{
								fileHeader.SimilarityIndex = index;
							}
						}
						break;
					case Header.DissimilarityIndex:
						{
							int index;
							string strIndex = ReadStringUpTo(FindNewLineOrEndOfString() - 1);
							Skip(2);
							if(int.TryParse(strIndex, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
							{
								fileHeader.DissimilarityIndex = index;
							}
						}
						break;
					case Header.Unknown:
						if(IsAtEndOfString ||
							CheckValue("---") ||
							CheckValue(HunkHeader) ||
							CheckValue(FileHeader) ||
							CheckValue(BinaryFilesHeader) ||
							CheckValue(BinaryPatchHeader))
						{
							completed = true;
						}
						else
						{
							SkipLine();
						}
						break;
					default:
						if(dataPresent)
						{
							SkipLine();
						}
						break;
				}
			}
			return fileHeader;
		}

		private void ReadDiffFileHeader3(ref string source, ref string target)
		{
			if(CheckValue("---"))
			{
				Skip(4);
				source = DecodeEscapedString(FindNewLineOrEndOfString(), 1).Trim();
				if(source.StartsWith(DefaultPrefixA))
				{
					source = source.Substring(2);
				}
				Skip(4);
				target = DecodeEscapedString(FindNewLineOrEndOfString(), 1).Trim();
				if(target.StartsWith(DefaultPrefixB))
				{
					target = target.Substring(2);
				}
			}
		}

		private DiffFile ReadDiffFile()
		{
			string source, target;
			bool isCombined = ReadDiffFileHeader1(out source, out target);
			var header = ReadDiffFileHeader2(isCombined);

			ReadDiffFileHeader3(ref source, ref target);

			bool isBinary = false;
			var stats = new DiffStats();
			var hunks = new List<DiffHunk>();
			while(!IsAtEndOfString)
			{
				if(CheckValue(HunkHeader))
				{
					var hunk = ReadTextPatchHunk();
					stats.Add(hunk.Stats);
					hunks.Add(hunk);
				}
				else if(CheckValue(BinaryFilesHeader))
				{
					var hunk = ReadBinaryFilesDifferHunk();
					stats.Add(hunk.Stats);
					hunks.Add(hunk);
					isBinary = true;
					break;
				}
				else if(CheckValue(BinaryPatchHeader))
				{
					while(!IsAtEndOfString && !CheckValue(FileHeader))
					{
						var hunk = ReadBinaryPatchHunk();
						stats.Add(hunk.Stats);
						hunks.Add(hunk);
					}
					isBinary = true;
					break;
				}
				else if(CheckValue(FileHeader))
				{
					break;
				}
				else
				{
					SkipLine();
				}
			}
			switch(header.Status)
			{
				case FileStatus.Renamed:
					source = header.RenameFrom;
					target = header.RenameTo;
					break;
				case FileStatus.Copied:
					source = header.CopyFrom;
					target = header.CopyTo;
					break;
			}
			return new DiffFile(
				header.OldIndex, header.NewIndex, header.OldMode, header.NewMode,
				source, target, header.Status, hunks, isBinary, stats);
		}

		#endregion

		/// <summary>Read git diff.</summary>
		/// <param name="type">Diff type.</param>
		/// <returns>Parsed <see cref="Diff"/>.</returns>
		public Diff ReadDiff(DiffType type)
		{
			FindStartOfLine(FileHeader);
			var files = new List<DiffFile>();
			while(!IsAtEndOfString)
			{
				files.Add(ReadDiffFile());
			}
			return new Diff(type, files);
		}

		/// <summary>Create <see cref="DiffParser"/>.</summary>
		/// <param name="string">Diff to parse.</param>
		public DiffParser(string @string)
			: base(@string)
		{
		}
	}
}

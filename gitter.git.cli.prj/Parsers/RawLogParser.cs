namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using gitter.Framework.CLI;

#if NOTINMPLEMENTED
	class RawLogParser
	{
		private sealed class RawCommitParser
		{
			private readonly char[] _buffer;
			private int _bufferOffset;

			enum State
			{
				ReadingHeader,
				ReadingContent,
			}

			private Hash _commit;
			private Hash _tree;
			private List<Hash> _parents;
			private string _committer;
			private string _committerEmail;
			private DateTime _commitTimestamp;
			private string _author;
			private string _authorEmail;
			private DateTime _authorTimestamp;
			private string _subject;
			private string _body;

			private State _state;
			private Part _part;

			enum Part
			{
				None,
				Commit,
				Tree,
				Parent,
				Author,
				Committer,
				Message,
			}

			public RawCommitParser()
			{
				_buffer = new char[40];
			}

			private bool CurrentHeaderIs(string header)
			{
				if(_bufferOffset != header.Length)
				{
					return false;
				}
				for(int i = 0; i < header.Length; ++i)
				{
					if(_buffer[i] != header[i])
					{
						return false;
					}
				}
				return true;
			}

			private bool ReadHash(ITextSegment textSegment)
			{
				int count = Math.Min(textSegment.Length, 40 - _bufferOffset);
				if(count != 0)
				{
					textSegment.MoveTo(_buffer, _bufferOffset, count);
					_bufferOffset += count;
				}
				if(_bufferOffset == 40)
				{
					if(textSegment.Length > 0)
					{
						textSegment.Skip(1);
						return true;
					}
				}
				return false;
			}

			public bool Parse(ITextSegment textSegment)
			{
				while(textSegment.Length > 0)
				{
					switch(_state)
					{
						case State.ReadingHeader:
							{
								int terminator = textSegment.IndexOf(' ');
								if(terminator == -1)
								{
									int count = textSegment.Length;
									if(_bufferOffset + count > _buffer.Length)
									{
										throw new Exception("header is too large");
									}
									textSegment.MoveTo(_buffer, _bufferOffset, count);
									_bufferOffset += count;
								}
								else
								{
									if(terminator != 0)
									{
										if(_bufferOffset + terminator > _buffer.Length)
										{
											throw new Exception("header is too large");
										}
										textSegment.MoveTo(_buffer, _bufferOffset, terminator);
										_bufferOffset += terminator;
									}
									textSegment.Skip(1);
									switch(_part)
									{
										case Part.None:
											if(CurrentHeaderIs(@"commit"))
											{
												_part = Part.Commit;
											}
											else
											{
												throw new Exception("Expected header: commit");
											}
											break;
										case Part.Commit:
											if(CurrentHeaderIs(@"tree"))
											{
												_part = Part.Tree;
											}
											else
											{
												throw new Exception("Expected header: tree");
											}
											break;
										case Part.Tree:
										case Part.Parent:
											if(CurrentHeaderIs(@"parent"))
											{
												_part = Part.Parent;
											}
											else if(CurrentHeaderIs(@"author"))
											{
												_part = Part.Author;
											}
											else
											{
												throw new Exception("Expected header: parent or author");
											}
											break;
										case Part.Author:
											if(CurrentHeaderIs("committer"))
											{
												_part = Part.Committer;
											}
											else
											{
												throw new Exception("Expected header: committer");
											}
											break;
										default:
											throw new Exception("Invalid parser state.");
									}
									_bufferOffset = 0;
									_state = State.ReadingContent;
								}
							}
							break;
						case State.ReadingContent:
							switch(_part)
							{
								case Part.Commit:
									if(ReadHash(textSegment))
									{
										_commit = new Hash(_buffer);
										_state = State.ReadingHeader;
									}
									break;
								case Part.Tree:
									if(ReadHash(textSegment))
									{
										_tree = new Hash(_buffer);
										_state = State.ReadingHeader;
									}
									break;
								case Part.Parent:
									if(ReadHash(textSegment))
									{
										_parents.Add(new Hash(_buffer));
										_state = State.ReadingHeader;
									}
									break;
							}
							break;
					}
				}
				return false;
			}

			public void Reset()
			{
				_bufferOffset = 0;
				_state = State.ReadingHeader;
				_part  = Part.None;
			}
		}

		private readonly RawCommitParser _commitParser;

		public RawLogParser()
		{
			_commitParser = new RawCommitParser();
		}

		public void Parse(ITextSegment textSegment)
		{
			while(textSegment.Length > 0)
			{
				if(_commitParser.Parse(textSegment))
				{
					_commitParser.Reset();
				}
			}
		}
	}
#endif
}

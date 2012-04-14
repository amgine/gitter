namespace gitter.Git
{
	using System;

	public struct DiffColumnHeader
	{
		private readonly DiffColumnAction _action;
		private readonly int _startLine;
		private readonly int _lineCount;

		public DiffColumnHeader(DiffColumnAction action, int startLine, int lineCount)
		{
			_action = action;
			_startLine = startLine;
			_lineCount = lineCount;
		}

		public DiffColumnAction Action
		{
			get { return _action; }
		}

		public int StartLine
		{
			get { return _startLine; }
		}

		public int LineCount
		{
			get { return _lineCount; }
		}

		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}{1},{2}", Action == DiffColumnAction.Add ? '+' : '-', _startLine, _lineCount);
		}
	}
}

namespace gitter.Git
{
	public sealed class DiffOptions
	{
		internal static readonly DiffOptions Default = GetDefault();

		public static DiffOptions GetDefault()
		{
			return new DiffOptions();
		}

		private int _context;
		private bool _patience;
		private bool _ignoreWhitespace;

		public DiffOptions()
		{
			_context = 3;
		}

		public int Context
		{
			get { return _context; }
			set { _context = value; }
		}

		public bool UsePatienceAlgorithm
		{
			get { return _patience; }
			set { _patience = value; }
		}

		public bool IgnoreWhitespace
		{
			get { return _ignoreWhitespace; }
			set { _ignoreWhitespace = value; }
		}
	}
}

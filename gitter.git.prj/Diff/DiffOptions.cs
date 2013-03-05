namespace gitter.Git
{
	public sealed class DiffOptions
	{
		#region Static

		internal static readonly DiffOptions Default = CreateDefault();

		public static DiffOptions CreateDefault()
		{
			return new DiffOptions();
		}

		#endregion

		#region Data

		private int _context;
		private bool _usePatienceAlgorithm;
		private bool _ignoreWhitespace;
		private bool _binary;

		#endregion

		#region .ctor

		public DiffOptions()
		{
			_context = 3;
		}

		#endregion

		#region Properties

		public int Context
		{
			get { return _context; }
			set { _context = value; }
		}

		public bool UsePatienceAlgorithm
		{
			get { return _usePatienceAlgorithm; }
			set { _usePatienceAlgorithm = value; }
		}

		public bool IgnoreWhitespace
		{
			get { return _ignoreWhitespace; }
			set { _ignoreWhitespace = value; }
		}

		public bool Binary
		{
			get { return _binary; }
			set { _binary = value; }
		}

		#endregion
	}
}

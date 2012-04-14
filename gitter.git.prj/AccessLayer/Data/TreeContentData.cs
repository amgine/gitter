namespace gitter.Git
{
	using System;

	using gitter.Framework;

	public abstract class TreeContentData : INamedObject
	{
		private readonly string _hash;
		private readonly int _mode;
		private readonly string _name;

		internal TreeContentData(string hash, int mode, string name)
		{
			_hash = hash; ;
			_name = name;
			_mode = mode;
		}

		public string SHA1
		{
			get { return _hash; }
		}

		public string Name
		{
			get { return _name; }
		}

		public int Mode
		{
			get { return _mode; }
		}

		public abstract TreeContentType Type { get; }
	}
}

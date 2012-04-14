namespace gitter.Git
{
	internal abstract class GitFeature
	{
		private readonly string _name;

		protected GitFeature(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public abstract bool IsAvailable { get; }
	}
}

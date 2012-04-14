namespace gitter.Framework.Options
{
	public sealed class SelectableColorCategory
	{
		private readonly string _id;
		private readonly string _name;

		public SelectableColorCategory(string id, string name)
		{
			_id = id;
			_name = name;
		}

		public string Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
		}

		public override string ToString()
		{
			return _name;
		}
	}
}

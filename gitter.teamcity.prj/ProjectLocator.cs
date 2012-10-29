namespace gitter.TeamCity
{
	public sealed class ProjectLocator : ObjectLocator
	{
		public ProjectLocator()
		{
		}

		public string Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public override string ToString()
		{
			if(!string.IsNullOrWhiteSpace(Id))
			{
				return "id:" + Id;
			}
			if(!string.IsNullOrWhiteSpace(Name))
			{
				return "name:" + Name;
			}
			return string.Empty;
		}
	}
}

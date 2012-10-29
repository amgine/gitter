namespace gitter.TeamCity
{
	public sealed class UserLocator : ObjectLocator
	{
		public UserLocator()
		{
		}

		public string Id
		{
			get;
			set;
		}

		public string Username
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
			if(!string.IsNullOrWhiteSpace(Username))
			{
				return "username:" + Username;
			}
			return string.Empty;
		}
	}
}

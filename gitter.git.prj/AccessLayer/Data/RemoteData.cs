namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class RemoteData : IObjectData<Remote>, INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _fetchUrl;
		private readonly string _pushUrl;

		#endregion

		#region .ctor

		public RemoteData(string name, string fetchUrl, string pushUrl)
		{
			if(name == null) throw new ArgumentNullException("name");
			if(name.Length == 0) throw new ArgumentException("name");
			if(fetchUrl == null) throw new ArgumentNullException("fetchUrl");
			if(fetchUrl.Length == 0) throw new ArgumentException("fetchUrl");
			_name = name;
			_fetchUrl = fetchUrl;
			_pushUrl = pushUrl;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
		}

		public string FetchUrl
		{
			get { return _fetchUrl; }
		}

		public string PushUrl
		{
			get { return _pushUrl; }
		}

		#endregion

		public void Update(Remote obj)
		{
			obj.SetPushUrl(_pushUrl);
			obj.SetFetchUrl(_fetchUrl);
		}

		public Remote Construct(IRepository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			var repo = (Repository)repository;
			return new Remote(repo, _name, _fetchUrl, _pushUrl);
		}

		public override string ToString()
		{
			return _name;
		}
	}
}

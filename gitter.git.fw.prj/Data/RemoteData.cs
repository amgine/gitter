namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class RemoteData : INamedObject
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

		public override string ToString()
		{
			return _name;
		}
	}
}

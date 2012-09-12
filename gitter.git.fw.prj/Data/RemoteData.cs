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
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(fetchUrl, "fetchUrl");
			Verify.Argument.IsNotNull(pushUrl, "pushUrl");

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

namespace gitter.TeamCity
{
	using System;
	using System.Text;
	using System.IO;
	using System.Net;
	using System.Xml;

	public sealed class TeamCityServiceContext
	{
		#region Data

		private readonly object _syncRoot;
		private Uri _serviceUri;
		private string _passphrase;

		private readonly ProjectsCollection _projects;
		private readonly BuildTypesCollection _buildTypes;
		private readonly BuildsCollection _builds;

		#endregion

		private enum ResponseContentType
		{
			Default,
			PlainText,
			Xml,
			Json,
		}

		private static string GetPassphrase(string username, string password)
		{
			return Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
		}

		#region .ctor

		public TeamCityServiceContext(Uri serviceUri, string username, string password)
		{
			_syncRoot = new object();
			_serviceUri = serviceUri;
			_passphrase = GetPassphrase(username, password);

			_projects	= new ProjectsCollection(this);
			_buildTypes	= new BuildTypesCollection(this);
			_builds		= new BuildsCollection(this);
		}

		#endregion

		#region Properties

		internal object SyncRoot
		{
			get { return _syncRoot; }
		}

		public ProjectsCollection Projects
		{
			get { return _projects; }
		}

		public BuildTypesCollection BuildTypes
		{
			get { return _buildTypes; }
		}

		public BuildsCollection Builds
		{
			get { return _builds; }
		}

		public string DefaultProjectId
		{
			get;
			set;
		}

		#endregion

		private Uri GetUri(string relativeUri)
		{
			return new Uri(_serviceUri, @"/httpAuth/app/rest/" + relativeUri);
		}

		private void SetupHttpBasicAuth(WebRequest request)
		{
			request.Headers.Add("Authorization: Basic " + _passphrase);
		}

		private static void SetupContentType(WebRequest request, ResponseContentType contentType)
		{
			switch(contentType)
			{
				case ResponseContentType.PlainText:
					request.ContentType = "text/plain";
					break;
				case ResponseContentType.Xml:
					request.ContentType = "application/xml";
					break;
				case ResponseContentType.Json:
					request.ContentType = "application/json";
					break;
			}
		}

		private WebResponse GetResponse(string relativeUri, ResponseContentType contentType)
		{
			var uri = GetUri(relativeUri);
			var request = WebRequest.Create(uri);
			SetupHttpBasicAuth(request);
			SetupContentType(request, contentType);
			return request.GetResponse();
		}

		internal XmlDocument GetXml(string relativeUri)
		{
			using(var response = GetResponse(relativeUri, ResponseContentType.Xml))
			using(var stream = response.GetResponseStream())
			{
				var xml = new XmlDocument();
				xml.Load(stream);
				return xml;
			}
		}

		internal string GetPlainText(string relativeUri)
		{
			using(var response = GetResponse(relativeUri, ResponseContentType.PlainText))
			using(var stream = response.GetResponseStream())
			using(var streamReader = new StreamReader(stream))
			{
				return streamReader.ReadToEnd();
			}
		}
	}
}

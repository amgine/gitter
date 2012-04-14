namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;

	public sealed class RedmineServiceEndpoint
	{
		private readonly GCCache<RedmineServiceContext> _context;

		private string _name;
		private string _url;
		private string _apiKey;

		public RedmineServiceEndpoint(string name, string url, string apiKey)
		{
			_name = name;
			_url = url;
			_apiKey = apiKey;
			_context = new GCCache<RedmineServiceContext>(
				() => new RedmineServiceContext(new Uri(_url), _apiKey));
		}

		public string Name
		{
			get { return _name; }
		}

		public RedmineServiceContext Context
		{
			get { return _context.Value; }
		}
	}
}

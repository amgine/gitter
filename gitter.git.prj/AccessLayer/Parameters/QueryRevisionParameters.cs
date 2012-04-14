namespace gitter.Git.AccessLayer
{
	using System;

	public sealed class QueryRevisionParameters
	{
		public QueryRevisionParameters()
		{
		}

		public QueryRevisionParameters(string sha1)
		{
			SHA1 = sha1;
		}

		public string SHA1 { get; set; }
	}
}

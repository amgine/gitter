namespace gitter.Git.AccessLayer
{
	using System;

	public sealed class QueryReflogParameters
	{
		public QueryReflogParameters()
		{
		}

		public QueryReflogParameters(string reference)
		{
			Reference = reference;
		}

		public string Reference { get; set; }

		public int MaxCount { get; set; }
	}
}

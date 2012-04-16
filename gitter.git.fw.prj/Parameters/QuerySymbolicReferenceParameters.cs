namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public class QuerySymbolicReferenceParameters
	{
		public QuerySymbolicReferenceParameters()
		{
		}

		public QuerySymbolicReferenceParameters(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}

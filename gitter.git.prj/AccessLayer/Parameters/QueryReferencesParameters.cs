namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class QueryReferencesParameters
	{
		public QueryReferencesParameters()
		{
		}

		public QueryReferencesParameters(ReferenceType referenceTypes)
		{
			ReferenceTypes = referenceTypes;
		}

		public ReferenceType ReferenceTypes { get; set; }
	}
}

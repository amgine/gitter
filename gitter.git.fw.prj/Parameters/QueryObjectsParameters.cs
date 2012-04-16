namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryObjects"/> opearation.</summary>
	public sealed class QueryObjectsParameters
	{
		/// <summary>Create <see cref="QueryObjectsParameters"/>.</summary>
		public QueryObjectsParameters()
		{
		}

		/// <summary>Create <see cref="QueryObjectsParameters"/>.</summary>
		/// <param name="object">Object to query.</param>
		public QueryObjectsParameters(string @object)
		{
			Objects = new[] { @object };
		}

		/// <summary>Create <see cref="QueryObjectsParameters"/>.</summary>
		/// <param name="objects">Objects to query.</param>
		public QueryObjectsParameters(IList<string> objects)
		{
			Objects = objects;
		}

		/// <summary>List of requested objects.</summary>
		public IList<string> Objects { get; set; }
	}
}

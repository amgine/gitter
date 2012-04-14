namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="ITagAccessor.QueryTag"/> operation.</summary>
	public sealed class QueryTagParameters
	{
		/// <summary>Create <see cref="QueryTagParameters"/>.</summary>
		public QueryTagParameters()
		{
		}

		/// <summary>Create <see cref="QueryTagParameters"/>.</summary>
		/// <param name="tagName">Name of the queried tag.</param>
		public QueryTagParameters(string tagName)
		{
			TagName = tagName;
		}

		/// <summary>Name of the queried tag.</summary>
		public string TagName { get; set; }
	}
}

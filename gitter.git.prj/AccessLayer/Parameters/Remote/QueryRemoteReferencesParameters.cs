namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRemoteAccessor.QueryRemoteReferences"/> operation.</summary>
	public sealed class QueryRemoteReferencesParameters
	{
		/// <summary>Create <see cref="QueryRemoteReferencesParameters"/>.</summary>
		public QueryRemoteReferencesParameters()
		{
		}

		/// <summary>Create <see cref="QueryRemoteReferencesParameters"/>.</summary>
		/// <param name="remoteName">Remote to query.</param>
		public QueryRemoteReferencesParameters(string remoteName)
		{
			RemoteName = remoteName;
		}

		/// <summary>Create <see cref="QueryRemoteReferencesParameters"/>.</summary>
		/// <param name="remoteName">Remote to query.</param>
		///	<param name="heads">Query /refs/heads.</param>
		///	<param name="tags">Query /refs/tags.</param>
		public QueryRemoteReferencesParameters(string remoteName, bool heads, bool tags)
		{
			RemoteName = remoteName;
			Heads = heads;
			Tags = tags;
		}

		/// <summary>Remote to query.</summary>
		public string RemoteName { get; set; }

		/// <summary>Query /refs/heads.</summary>
		public bool Heads { get; set; }

		/// <summary>Query /refs/tags.</summary>
		public bool Tags { get; set; }

		/// <summary>Ref name pattern.</summary>
		public string Pattern { get; set; }
	}
}

namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRemoteAccessor.QueryRemote"/> operation.</summary>
	public sealed class QueryRemoteParameters
	{
		/// <summary>Create <see cref="QueryRemoteParameters"/>.</summary>
		public QueryRemoteParameters()
		{
		}

		/// <summary>Create <see cref="QueryRemoteParameters"/>.</summary>
		/// <param name="remoteName">Remote to query.</param>
		public QueryRemoteParameters(string remoteName)
		{
			RemoteName = remoteName;
		}

		/// <summary>Remote to query.</summary>
		public string RemoteName { get; set; }
	}
}

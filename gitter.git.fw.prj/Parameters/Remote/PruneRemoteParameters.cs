namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRemoteAccessor.PruneRemote"/> operation.</summary>
	public sealed class PruneRemoteParameters
	{
		/// <summary>Create <see cref="PruneRemoteParameters"/>.</summary>
		public PruneRemoteParameters()
		{
		}

		/// <summary>Create <see cref="PruneRemoteParameters"/>.</summary>
		/// <param name="remoteName">Remote to prune.</param>
		public PruneRemoteParameters(string remoteName)
		{
			RemoteName = remoteName;
		}

		/// <summary>Remote to prune.</summary>
		public string RemoteName { get; set; }
	}
}

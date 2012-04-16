namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRemoteAccessor.RemoveRemote"/> operation.</summary>
	public sealed class RemoveRemoteParameters
	{
		/// <summary>Create <see cref="RemoveRemoteParameters"/>.</summary>
		public RemoveRemoteParameters()
		{
		}

		/// <summary>Create <see cref="RemoveRemoteParameters"/>.</summary>
		/// <param name="remoteName">Remote to remove.</param>
		public RemoveRemoteParameters(string remoteName)
		{
			RemoteName = remoteName;
		}

		/// <summary>Remote to remove.</summary>
		public string RemoteName { get; set; }
	}
}

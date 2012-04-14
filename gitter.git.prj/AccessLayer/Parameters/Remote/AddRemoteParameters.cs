namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IRemoteAccessor.AddRemote"/> operation.</summary>
	public sealed class AddRemoteParameters
	{
		/// <summary>Create <see cref="AddRemoteParameters"/>.</summary>
		public AddRemoteParameters()
		{
		}

		/// <summary>Create <see cref="AddRemoteParameters"/>.</summary>
		/// <param name="remoteName">Remote name.</param>
		/// <param name="url">Remote URL.</param>
		public AddRemoteParameters(string remoteName, string url)
		{
			RemoteName = remoteName;
			Url = url;
		}

		/// <summary>Name of the remote.</summary>
		public string RemoteName { get; set; }

		/// <summary>Remote URL.</summary>
		public string Url { get; set; }

		/// <summary>List of branches to track.</summary>
		public IList<string> Branches { get; set; }

		/// <summary>HEAD of remote.</summary>
		public string MasterBranch { get; set; }

		/// <summary>Mirror-mode remote.</summary>
		public bool Mirror { get; set; }

		/// <summary>Fetch immediately after adding.</summary>
		public bool Fetch { get; set; }

		/// <summary>Tag fetch mode.</summary>
		public TagFetchMode TagFetchMode { get; set; }
	}
}

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="IRemoteAccessor.RemoveRemoteReferences"/> operation.</summary>
	public sealed class RemoveRemoteReferencesParameters
	{
		/// <summary>Create <see cref="RemoveRemoteReferencesParameters"/>.</summary>
		public RemoveRemoteReferencesParameters()
		{
		}

		/// <summary>Create <see cref="RemoveRemoteReferencesParameters"/>.</summary>
		/// <param name="remoteName">Affected remote.</param>
		/// <param name="reference">Reference to remove.</param>
		public RemoveRemoteReferencesParameters(string remoteName, string reference)
		{
			RemoteName = remoteName;
			References = new[] { reference };
		}

		/// <summary>Create <see cref="RemoveRemoteReferencesParameters"/>.</summary>
		/// <param name="remoteName">Affected remote.</param>
		/// <param name="references">References to remove.</param>
		public RemoveRemoteReferencesParameters(string remoteName, IList<string> references)
		{
			RemoteName = remoteName;
			References = references;
		}

		/// <summary>Affected remote.</summary>
		public string RemoteName { get; set; }

		/// <summary>References to remove.</summary>
		public IList<string> References { get; set; }
	}
}

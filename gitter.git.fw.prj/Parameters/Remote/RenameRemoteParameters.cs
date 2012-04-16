namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRemoteAccessor.RenameRemote"/> operation.</summary>
	public sealed class RenameRemoteParameters
	{
		/// <summary>Create <see cref="RenameRemoteParameters"/>.</summary>
		public RenameRemoteParameters()
		{
		}

		/// <summary>Create <see cref="RenameRemoteParameters"/>.</summary>
		/// <param name="oldName">Remote to rename.</param>
		/// <param name="newName">New remote name.</param>
		public RenameRemoteParameters(string oldName, string newName)
		{
			OldName = oldName;
			NewName = newName;
		}

		/// <summary>Remote to rename.</summary>
		public string OldName { get; set; }

		/// <summary>New remote name.</summary>
		public string NewName { get; set; }
	}
}

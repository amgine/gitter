namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.RenameConfigSection"/> operation.</summary>
	public sealed class RenameConfigSectionParameters : BaseConfigParameters
	{
		/// <summary>Create <see cref="RenameConfigSectionParameters"/>.</summary>
		public RenameConfigSectionParameters()
		{
		}

		/// <summary>Section to rename.</summary>
		public string OldName { get; set; }

		/// <summary>New section's name.</summary>
		public string NewName { get; set; }
	}
}

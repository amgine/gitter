namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.DeleteConfigSection"/> operation.</summary>
	public sealed class DeleteConfigSectionParameters : BaseConfigParameters
	{
		/// <summary>Create <see cref="DeleteConfigSectionParameters"/>.</summary>
		public DeleteConfigSectionParameters()
		{
		}

		/// <summary>Section to remove.</summary>
		public string SectionName { get; set; }
	}
}

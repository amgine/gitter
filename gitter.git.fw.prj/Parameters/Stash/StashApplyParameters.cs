namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IStashAccessor.StashApply()"/> operation.</summary>
	public sealed class StashApplyParameters
	{
		/// <summary>Create <see cref="StashPopParameters"/>.</summary>
		public StashApplyParameters()
		{
		}

		/// <summary>Create <see cref="StashPopParameters"/>.</summary>
		/// <param name="stashName">Stash to apply.</param>
		/// <param name="restoreIndex">Restore index state.</param>
		public StashApplyParameters(string stashName, bool restoreIndex)
		{
			StashName = stashName;
			RestoreIndex = restoreIndex;
		}

		/// <summary>Create <see cref="StashPopParameters"/>.</summary>
		/// <param name="stashName">Stash to apply.</param>
		public StashApplyParameters(string stashName)
		{
			StashName = stashName;
		}

		/// <summary>Create <see cref="StashPopParameters"/>.</summary>
		/// <param name="restoreIndex">Restore index state.</param>
		public StashApplyParameters(bool restoreIndex)
		{
			RestoreIndex = restoreIndex;
		}

		/// <summary>Stash to apply.</summary>
		public string StashName { get; set; }

		/// <summary>Restore index state.</summary>
		public bool RestoreIndex { get; set; }
	}
}

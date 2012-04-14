namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IStashAccessor.StashPop()"/> operation.</summary>
	public sealed class StashPopParameters
	{
		/// <summary>Create <see cref="StashPopParameters"/>.</summary>
		public StashPopParameters()
		{
		}

		/// <summary>Create <see cref="StashPopParameters"/>.</summary>
		/// <param name="stashName">Stash to pop.</param>
		public StashPopParameters(string stashName, bool restoreIndex)
		{
			StashName = stashName;
			RestoreIndex = restoreIndex;
		}

		/// <summary>Create <see cref="StashPopParameters"/>.</summary>
		/// <param name="stashName">Stash to pop.</param>
		public StashPopParameters(string stashName)
		{
			StashName = stashName;
		}

		/// <summary>Create <see cref="StashPopParameters"/>.</summary>
		/// <param name="restoreIndex">Restore index state.</param>
		public StashPopParameters(bool restoreIndex)
		{
			RestoreIndex = restoreIndex;
		}

		/// <summary>Stash to pop.</summary>
		public string StashName { get; set; }

		/// <summary>Restore index state.</summary>
		public bool RestoreIndex { get; set; }
	}
}

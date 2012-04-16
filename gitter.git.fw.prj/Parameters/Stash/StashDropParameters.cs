namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IStashAccessor.StashDrop()"/> operation.</summary>
	public sealed class StashDropParameters
	{
		/// <summary>Create <see cref="StashDropParameters"/>.</summary>
		public StashDropParameters()
		{
		}

		/// <summary>Create <see cref="StashDropParameters"/>.</summary>
		/// <param name="stashName">Stash to drop.</param>
		public StashDropParameters(string stashName)
		{
			StashName = stashName;
		}

		/// <summary>Stash to drop.</summary>
		public string StashName { get; set; }
	}
}

namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IStashAccessor.StashSave()"/> operation.</summary>
	public sealed class StashSaveParameters
	{
		/// <summary>Create <see cref="StashSaveParameters"/>.</summary>
		public StashSaveParameters()
		{
		}

		/// <summary>Create <see cref="StashSaveParameters"/>.</summary>
		/// <param name="message">Custom stash message.</param>
		/// <param name="keepIndex">Do not stash staged changes.</param>
		/// <param name="includeUntracked">Include untracked files in stash.</param>
		public StashSaveParameters(string message, bool keepIndex, bool includeUntracked)
		{
			Message = message;
			KeepIndex = keepIndex;
			IncludeUntracked = includeUntracked;
		}

		/// <summary>Custom stash message.</summary>
		public string Message { get; set; }

		/// <summary>Do not stash staged changes.</summary>
		public bool KeepIndex { get; set; }

		/// <summary>Include untracked files in stash.</summary>
		public bool IncludeUntracked { get; set; }
	}
}

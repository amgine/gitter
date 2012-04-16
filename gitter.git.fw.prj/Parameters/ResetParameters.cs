namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.Reset"/> operation.</summary>
	public sealed class ResetParameters
	{
		/// <summary>Create <see cref="ResetParameters"/>.</summary>
		public ResetParameters()
		{
		}

		/// <summary>Create <see cref="ResetParameters"/>.</summary>
		/// <param name="revision">Revision to reset to.</param>
		public ResetParameters(string revision)
		{
			Revision = revision;
		}

		/// <summary>Create <see cref="ResetParameters"/>.</summary>
		/// <param name="mode">Reset mode.</param>
		public ResetParameters(ResetMode mode)
		{
			Mode = mode;
		}

		/// <summary>Create <see cref="ResetParameters"/>.</summary>
		/// <param name="revision">Revision to reset to.</param>
		/// <param name="mode">Reset mode.</param>
		public ResetParameters(string revision, ResetMode mode)
		{
			Revision = revision;
			Mode = mode;
		}

		/// <summary>Revision to reset to.</summary>
		public string Revision { get; set; }

		/// <summary>Reset mode.</summary>
		public ResetMode Mode { get; set; }
	}
}

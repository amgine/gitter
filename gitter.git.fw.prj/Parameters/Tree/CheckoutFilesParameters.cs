namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="ITreeAccessor.CheckoutFiles"/> operation.</summary>
	public sealed class CheckoutFilesParameters
	{
		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		public CheckoutFilesParameters()
		{
		}

		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		/// <param name="revision">Tree or commit referencing a tree.</param>
		/// <param name="paths">Paths to checkout.</param>
		public CheckoutFilesParameters(string revision, IList<string> paths)
		{
			Revision = revision;
			Paths = paths;
		}

		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		/// <param name="paths">Paths to checkout.</param>
		public CheckoutFilesParameters(IList<string> paths)
		{
			Paths = paths;
		}

		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		/// <param name="revision">Tree or commit referencing a tree.</param>
		/// <param name="path">Path to checkout.</param>
		public CheckoutFilesParameters(string revision, string path)
		{
			Revision = revision;
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		/// <param name="path">Path to checkout.</param>
		public CheckoutFilesParameters(string path)
		{
			Paths = new[] { path };
		}

		/// <summary>Tree or commit referencing a tree.</summary>
		public string Revision { get; set; }

		/// <summary>Paths to checkout.</summary>
		public IList<string> Paths { get; set; }

		/// <summary>Checkout mode.</summary>
		public CheckoutFileMode Mode { get; set; }
	}
}

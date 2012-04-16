namespace gitter.Git.AccessLayer
{
	using System;
	using gitter.Framework;

	public sealed class CloneRepositoryParameters
	{
		/// <summary>Create <see cref="CloneRepositoryParameters"/>.</summary>
		public CloneRepositoryParameters()
		{
		}

		public string Url { get; set; }

		public string Path { get; set; }

		public string Template { get; set; }

		public string RemoteName { get; set; }

		public bool Bare { get; set; }

		public bool Mirror { get; set; }

		public bool Recursive { get; set; }

		public bool Shallow { get; set; }

		public int Depth { get; set; }

		public bool NoCheckout { get; set; }

		public IAsyncProgressMonitor Monitor { get; set; }
	}
}

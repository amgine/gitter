namespace gitter.Git.AccessLayer
{
	using System;

	public sealed class SubmoduleUpdateParameters
	{
		public SubmoduleUpdateParameters()
		{
		}

		public string Path { get; set; }

		public bool Init { get; set; }

		public bool Recursive { get; set; }

		public bool NoFetch { get; set; }

		public SubmoduleUpdateMode Mode { get; set; }
	}
}

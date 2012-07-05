namespace gitter.Git.AccessLayer
{
	using System;

	public sealed class ArchiveParameters
	{
		public string Remote { get; set; }

		public string Tree { get; set; }

		public string OutputFile { get; set; }

		public string Format { get; set; }

		public string Path { get; set; }
	}
}

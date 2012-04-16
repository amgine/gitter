namespace gitter.Git.AccessLayer
{
	using System;

	public sealed class InitRepositoryParameters
	{
		/// <summary>Create <see cref="InitRepositoryParameters"/>.</summary>
		public InitRepositoryParameters()
		{
		}

		public InitRepositoryParameters(string path, string template, bool bare)
		{
			Path = path;
			Template = template;
			Bare = bare;
		}

		public string Path { get; set; }

		public bool Bare { get; set; }

		public string Template { get; set; }
	}
}

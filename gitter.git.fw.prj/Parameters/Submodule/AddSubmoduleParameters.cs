namespace gitter.Git.AccessLayer
{
	public sealed class AddSubmoduleParameters
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AddSubmoduleParameters"/> class.
		/// </summary>
		public AddSubmoduleParameters()
		{
		}

		public string Branch { get; set; }

		public bool Force { get; set; }

		public string ReferenceRepository { get; set; }

		public string Repository { get; set; }

		public string Path { get; set; }
	}
}

namespace gitter.Git.AccessLayer
{
	using System;

	public sealed class DereferenceParameters
	{
		public DereferenceParameters()
		{
		}

		public DereferenceParameters(string reference)
		{
			Reference = reference;
		}

		public string Reference { get; set; }

		public bool LoadRevisionData { get; set; }
	}
}

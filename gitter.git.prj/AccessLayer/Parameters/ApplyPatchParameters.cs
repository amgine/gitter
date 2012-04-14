namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class ApplyPatchParameters
	{
		public IList<string> Patches { get; set; }

		public ApplyPatchTo ApplyTo { get; set; }

		public bool Reverse { get; set; }
	}
}

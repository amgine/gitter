namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class RebaseParameters
	{
		public RebaseParameters(string onto, string branch, string target)
		{
			NewBase = onto;
			Branch = branch;
			Upstream = target;
		}

		public RebaseParameters(string branch, string target)
		{
			Branch = branch;
			Upstream = target;
		}

		public RebaseParameters(string target)
		{
			Upstream = target;
		}

		public string NewBase { get; set; }

		public string Branch { get; set; }

		public string Upstream { get; set; }
	}
}

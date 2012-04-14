namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class FormatMergeMessageParameters
	{
		public FormatMergeMessageParameters()
		{
		}

		public FormatMergeMessageParameters(string revision, string headRefName)
		{
			Revisions = new string[] { revision };
			HeadReference = headRefName;
		}

		public FormatMergeMessageParameters(IList<string> revisions, string headRefName)
		{
			Revisions = revisions;
			HeadReference = headRefName;
		}

		public IList<string> Revisions { get; set; }

		public string HeadReference { get; set; }
	}
}

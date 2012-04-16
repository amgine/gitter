namespace gitter.Git.AccessLayer
{
	using System;

	public sealed class DescribeParameters
	{
		public DescribeParameters()
		{
		}

		public DescribeParameters(string revision)
		{
			Revision = revision;
		}

		public string Revision { get; set; }
	}
}

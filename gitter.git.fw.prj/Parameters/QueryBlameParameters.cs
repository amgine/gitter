namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public sealed class QueryBlameParameters
	{
		public string FileName { get; set; }

		public string Revision { get; set; }
	}
}

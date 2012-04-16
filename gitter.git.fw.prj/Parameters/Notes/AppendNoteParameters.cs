namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class AppendNoteParameters
	{
		public string Revision { get; set; }

		public string Message { get; set; }
	}
}

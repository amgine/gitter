namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class GarbageCollectParameters
	{
		public GarbageCollectParameters()
		{
		}

		public IAsyncProgressMonitor Monitor { get; set; }
	}
}

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	
	/// <summary>Parameters for <see cref="IIndexAccessor.RunMergeTool"/> operation.</summary>
	public sealed class RunMergeToolParameters
	{
		/// <summary>Create <see cref="RunMergeToolParameters"/>.</summary>
		public RunMergeToolParameters()
		{
		}

		/// <summary>Create <see cref="RunMergeToolParameters"/>.</summary>
		/// <param name="tool">Tool to run.</param>
		/// <param name="file">File to run merge tool on.</param>
		public RunMergeToolParameters(MergeTool tool, string file)
		{
			Tool = tool.Name;
			Files = new[] { file };
		}

		/// <summary>Create <see cref="RunMergeToolParameters"/>.</summary>
		/// <param name="tool">Tool to run.</param>
		/// <param name="files">Files to run merge tool on.</param>
		public RunMergeToolParameters(MergeTool tool, IList<string> files)
		{
			Tool = tool.Name;
			Files = files;
		}

		/// <summary>Create <see cref="RunMergeToolParameters"/>.</summary>
		/// <param name="file">File to run merge tool on.</param>
		public RunMergeToolParameters(string file)
		{
			Files = new[] { file };
		}

		/// <summary>Create <see cref="RunMergeToolParameters"/>.</summary>
		/// <param name="files">Files to run merge tool on.</param>
		public RunMergeToolParameters(IList<string> files)
		{
			Files = files;
		}

		/// <summary>Tool to run.</summary>
		public string Tool { get; set; }

		/// <param name="files">Files to run merge tool on.</param>
		public IList<string> Files { get; set; }

		public IAsyncProgressMonitor Monitor { get; set; }
	}
}

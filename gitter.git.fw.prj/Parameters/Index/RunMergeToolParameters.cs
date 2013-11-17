#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
		public RunMergeToolParameters(string tool, string file)
		{
			Tool = tool;
			Files = new[] { file };
		}

		/// <summary>Create <see cref="RunMergeToolParameters"/>.</summary>
		/// <param name="tool">Tool to run.</param>
		/// <param name="files">Files to run merge tool on.</param>
		public RunMergeToolParameters(string tool, IList<string> files)
		{
			Tool = tool;
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
	}
}

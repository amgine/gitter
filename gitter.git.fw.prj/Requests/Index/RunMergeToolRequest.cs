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

namespace gitter.Git.AccessLayer;

using System;
using System.Collections.Generic;

using gitter.Framework;

/// <summary>Parameters for <see cref="IRepositoryAccessor.RunMergeTool"/> operation.</summary>
public sealed class RunMergeToolRequest
{
	/// <summary>Create <see cref="RunMergeToolRequest"/>.</summary>
	public RunMergeToolRequest()
	{
	}

	/// <summary>Create <see cref="RunMergeToolRequest"/>.</summary>
	/// <param name="tool">Tool to run.</param>
	/// <param name="file">File to run merge tool on.</param>
	public RunMergeToolRequest(string tool, string file)
	{
		Tool  = tool;
		Files = file;
	}

	/// <summary>Create <see cref="RunMergeToolRequest"/>.</summary>
	/// <param name="tool">Tool to run.</param>
	/// <param name="files">Files to run merge tool on.</param>
	public RunMergeToolRequest(string tool, Many<string> files)
	{
		Tool  = tool;
		Files = files;
	}

	/// <summary>Create <see cref="RunMergeToolRequest"/>.</summary>
	/// <param name="file">File to run merge tool on.</param>
	public RunMergeToolRequest(string file)
	{
		Files = file;
	}

	/// <summary>Create <see cref="RunMergeToolRequest"/>.</summary>
	/// <param name="files">Files to run merge tool on.</param>
	public RunMergeToolRequest(Many<string> files)
	{
		Files = files;
	}

	/// <summary>Tool to run.</summary>
	public string? Tool { get; set; }

	/// <summary>Files to run merge tool on.</summary>
	public Many<string> Files { get; set; }
}

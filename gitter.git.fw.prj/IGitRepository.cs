﻿#region Copyright Notice
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

using gitter.Framework;

/// <summary>Git repository.</summary>
public interface IGitRepository : IRepository, IDisposable
{
	/// <summary>GIT_DIR.</summary>
	string GitDirectory { get; }

	/// <summary>Returns object which provides raw access to this repository.</summary>
	/// <value>Object which provides raw access to this repository.</value>
	IRepositoryAccessor Accessor { get; }

	/// <summary>Repository monitor.</summary>
	IRepositoryMonitor Monitor { get; }

	/// <summary>Returns full file name for a file in GIT_DIR.</summary>
	/// <param name="fileName">File name.</param>
	/// <returns>Full file name.</returns>
	string GetGitFileName(string fileName);
}

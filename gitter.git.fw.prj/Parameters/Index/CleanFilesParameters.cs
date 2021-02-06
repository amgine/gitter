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

	/// <summary>Parameters for <see cref="IRepositoryAccessor.CleanFiles"/> operation.</summary>
	public sealed class CleanFilesParameters
	{
		/// <summary>Create <see cref="CleanFilesParameters"/>.</summary>
		public CleanFilesParameters()
		{
		}

		/// <summary>Create <see cref="CleanFilesParameters"/>.</summary>
		/// <param name="path">Path to clean.</param>
		public CleanFilesParameters(string path)
		{
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="CleanFilesParameters"/>.</summary>
		/// <param name="paths">Paths to clean.</param>
		public CleanFilesParameters(IList<string> paths)
		{
			Paths = paths;
		}

		/// <summary>
		/// If the git configuration variable clean.requireForce is not set to false,
		/// git clean will refuse to run unless given -f or -n. 
		/// </summary>
		public bool Force { get; set; }

		/// <summary>Clean files mode.</summary>
		public CleanFilesMode Mode { get; set; }

		/// <summary>Allow removing directories.</summary>
		public bool RemoveDirectories { get; set; }

		/// <summary>Paths to clean.</summary>
		public IList<string> Paths { get; set; }

		/// <summary>Exception paths that should not be cleaned.</summary>
		public IList<string> ExcludePatterns { get; set; }
	}
}

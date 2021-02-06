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
	
	/// <summary>Parameters for <see cref="IRepositoryAccessor.AddFiles"/> operation.</summary>
	public sealed class AddFilesParameters
	{
		/// <summary>Create <see cref="AddFilesParameters"/>.</summary>
		public AddFilesParameters()
		{
		}

		/// <summary>Create <see cref="AddFilesParameters"/>.</summary>
		/// <pparam name="mode">Add files mode.</pparam>
		public AddFilesParameters(AddFilesMode mode)
		{
			Mode = mode;
		}

		/// <summary>Create <see cref="AddFilesParameters"/>.</summary>
		/// <param name="path">Path to add.</param>
		public AddFilesParameters(string path)
		{
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="AddFilesParameters"/>.</summary>
		/// <param name="paths">Paths to add.</param>
		public AddFilesParameters(IList<string> paths)
		{
			Paths = paths;
		}

		/// <summary>Create <see cref="AddFilesParameters"/>.</summary>
		/// <pparam name="mode">Add files mode.</pparam>
		/// <param name="path">Path to add.</param>
		public AddFilesParameters(AddFilesMode mode, string path)
		{
			Mode = mode;
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="AddFilesParameters"/>.</summary>
		/// <param name="mode">Add files mode.</param>
		/// <param name="paths">Paths to add.</param>
		public AddFilesParameters(AddFilesMode mode, IList<string> paths)
		{
			Mode = mode;
			Paths = paths;
		}

		/// <summary>Paths to add.</summary>
		public IList<string> Paths { get; set; }

		/// <summary>Allow adding otherwise ignored files.</summary>
		public bool Force { get; set; }

		/// <summary>Add files mode.</summary>
		public AddFilesMode Mode { get; set; }

		/// <summary>Record only the fact that the path will be added later.</summary>
		public bool IntentToAdd { get; set; }

		/// <summary>Don't add the file(s), but only refresh their stat() information in the index.</summary>
		public bool Refresh { get; set; }

		/// <summary>
		/// If some files could not be added because of errors indexing them,
		/// do not abort the operation, but continue adding the others.
		/// </summary>
		public bool IgnoreErrors { get; set; }
	}
}

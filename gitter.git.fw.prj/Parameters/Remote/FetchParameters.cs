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
	using gitter.Framework;

	/// <summary>Parameters for <see cref="IRemoteAccessor.Fetch"/> operation.</summary>
	public class FetchParameters
	{
		/// <summary>Create <see cref="FetchParameters"/>.</summary>
		public FetchParameters()
		{
		}

		/// <summary>Create <see cref="FetchParameters"/>.</summary>
		/// <param name="all">Fetch all remotes.</param>
		public FetchParameters(bool all)
		{
			All = all;
		}

		/// <summary>Create <see cref="FetchParameters"/>.</summary>
		/// <param name="repository">Repository to fetch from.</param>
		public FetchParameters(string repository)
		{
			Repository = repository;
		}

		/// <summary>Fetch all remotes.</summary>
		public bool All { get; set; }

		/// <summary>
		///	Append ref names and object names of fetched refs to the existing contents of .git/FETCH_HEAD.
		///	Without this option old data in .git/FETCH_HEAD will be overwritten.
		/// </summary>
		public bool Append { get; set; }

		/// <summary>
		/// Deepen the history of a shallow repository created by git clone with --depth option
		/// by the specified number of commits. 
		/// </summary>
		public int Depth { get; set; }

		/// <summary>Repository to fetch from.</summary>
		public string Repository { get; set; }

		/// <summary>Keep downloaded pack.</summary>
		public bool KeepDownloadedPack { get; set; }

		/// <summary>Force update remote tracking branches.</summary>
		public bool Force { get; set; }

		/// <summary>Remove stale remote tracking branches.</summary>
		public bool Prune { get; set; }

		/// <summary>Tag feth mode.</summary>
		public TagFetchMode TagFetchMode { get; set; }

		/// <summary>
		/// --exec=upload-pack is passed to the command to specify
		/// non-default path for the command run on the other end. 
		/// </summary>
		public string UploadPack { get; set; }
	}
}

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

	/// <summary>Parameters for <see cref="IRepositoryAccessor.AddRemote"/> operation.</summary>
	public sealed class AddRemoteParameters
	{
		/// <summary>Create <see cref="AddRemoteParameters"/>.</summary>
		public AddRemoteParameters()
		{
		}

		/// <summary>Create <see cref="AddRemoteParameters"/>.</summary>
		/// <param name="remoteName">Remote name.</param>
		/// <param name="url">Remote URL.</param>
		public AddRemoteParameters(string remoteName, string url)
		{
			RemoteName = remoteName;
			Url = url;
		}

		/// <summary>Name of the remote.</summary>
		public string RemoteName { get; set; }

		/// <summary>Remote URL.</summary>
		public string Url { get; set; }

		/// <summary>List of branches to track.</summary>
		public IList<string> Branches { get; set; }

		/// <summary>HEAD of remote.</summary>
		public string MasterBranch { get; set; }

		/// <summary>Mirror-mode remote.</summary>
		public bool Mirror { get; set; }

		/// <summary>Fetch immediately after adding.</summary>
		public bool Fetch { get; set; }

		/// <summary>Tag fetch mode.</summary>
		public TagFetchMode TagFetchMode { get; set; }
	}
}

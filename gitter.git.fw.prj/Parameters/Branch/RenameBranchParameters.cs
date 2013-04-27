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

	/// <summary>Parameters for <see cref="IBranchAccessor.RenameBranch"/> operation.</summary>
	public sealed class RenameBranchParameters
	{
		/// <summary>Create <see cref="RenameBranchParameters"/>.</summary>
		public RenameBranchParameters()
		{
		}

		/// <summary>Create <see cref="RenameBranchParameters"/>.</summary>
		/// <param name="oldName">Name of the branch to rename.</param>
		/// <param name="newName">New branch name.</param>
		public RenameBranchParameters(string oldName, string newName)
		{
			OldName = oldName;
			NewName = newName;
		}

		/// <summary>Name of the branch to rename.</summary>
		public string OldName { get; set; }

		/// <summary>New branch name.</summary>
		public string NewName { get; set; }

		/// <summary>Force-overwrite existing branch.</summary>
		public bool Force { get; set; }
	}
}

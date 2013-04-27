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

	/// <summary>Parameters for <see cref="IBranchAccessor.DeleteBranch"/> operation.</summary>
	public sealed class DeleteBranchParameters
	{
		/// <summary>Create <see cref="DeleteBranchParameters"/>.</summary>
		public DeleteBranchParameters()
		{
		}

		/// <summary>Create <see cref="DeleteBranchParameters"/>.</summary>
		/// <param name="branchName">Name of the branch to delete.</param>
		/// <param name="force">Delete branch irrespective its merged status.</param>
		/// <param name="remote">Branch is remote tracking branch.</param>
		public DeleteBranchParameters(string branchName, bool remote, bool force)
		{
			BranchName = branchName;
			Remote = remote;
			Force = force;
		}

		/// <summary>Branch name.</summary>
		public string BranchName { get; set; }

		/// <summary>Branch is remote.</summary>
		public bool Remote { get; set; }

		/// <summary>Delete branch irrespective its merged status.</summary>
		public bool Force { get; set; }
	}
}

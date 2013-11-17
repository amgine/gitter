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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	/// <summary>Object which can perform various operations on git branches.</summary>
	public interface IBranchAccessor
	{
		/// <summary>Check if branch exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryBranchParameters"/>.</param>
		/// <returns><see cref="BranchData"/> or null, if requested branch doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		BranchData QueryBranch(QueryBranchParameters parameters);

		/// <summary>Check if branch exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryBranchParameters"/>.</param>
		/// <returns><see cref="BranchData"/> or null, if requested branch doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<BranchData> QueryBranchAsync(QueryBranchParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Query branch list.</summary>
		/// <param name="parameters"><see cref="QueryBranchesParameters"/>.</param>
		/// <returns>List of requested branches.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		BranchesData QueryBranches(QueryBranchesParameters parameters);

		/// <summary>Query branch list.</summary>
		/// <param name="parameters"><see cref="QueryBranchesParameters"/>.</param>
		/// <returns>List of requested branches.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<BranchesData> QueryBranchesAsync(QueryBranchesParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Create local branch.</summary>
		/// <param name="parameters"><see cref="CreateBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void CreateBranch(CreateBranchParameters parameters);

		/// <summary>Create local branch.</summary>
		/// <param name="parameters"><see cref="CreateBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task CreateBranchAsync(CreateBranchParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Reset local branch.</summary>
		/// <param name="parameters"><see cref="ResetBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void ResetBranch(ResetBranchParameters parameters);

		/// <summary>Reset local branch.</summary>
		/// <param name="parameters"><see cref="ResetBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task ResetBranchAsync(ResetBranchParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Rename local branch.</summary>
		/// <param name="parameters"><see cref="RenameBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RenameBranch(RenameBranchParameters parameters);

		/// <summary>Rename branch.</summary>
		/// <param name="parameters"><see cref="RenameBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task RenameBranchAsync(RenameBranchParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Delete branch.</summary>
		/// <param name="parameters"><see cref="DeleteBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void DeleteBranch(DeleteBranchParameters parameters);

		/// <summary>Remove branch <paramref name="branchName"/>.</summary>
		/// <param name="parameters"><see cref="DeleteBranchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task DeleteBranchAsync(DeleteBranchParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);
	}
}

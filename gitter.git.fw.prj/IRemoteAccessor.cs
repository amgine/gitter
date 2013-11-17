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

	/// <summary>Object which can perform various operations on git remotes.</summary>
	public interface IRemoteAccessor
	{
		/// <summary>Get information about remote.</summary>
		/// <param name="parameters"><see cref="QueryRemoteParameters"/>.</param>
		/// <returns>Requested remote.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		RemoteData QueryRemote(QueryRemoteParameters parameters);

		/// <summary>Get information about remote.</summary>
		/// <param name="parameters"><see cref="QueryRemoteParameters"/>.</param>
		/// <returns>Requested remote.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<RemoteData> QueryRemoteAsync(QueryRemoteParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Query list of remotes.</summary>
		/// <param name="parameters"><see cref="QueryRemotesParameters"/>.</param>
		/// <returns>List of remotes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<RemoteData> QueryRemotes(QueryRemotesParameters parameters);

		/// <summary>Query list of remotes.</summary>
		/// <param name="parameters"><see cref="QueryRemotesParameters"/>.</param>
		/// <returns>List of remotes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<IList<RemoteData>> QueryRemotesAsync(QueryRemotesParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Add remote repository.</summary>
		/// <param name="parameters"><see cref="AddRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void AddRemote(AddRemoteParameters parameters);

		/// <summary>Add remote repository.</summary>
		/// <param name="parameters"><see cref="AddRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task AddRemoteAsync(AddRemoteParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Rename remote repository.</summary>
		/// <param name="parameters"><see cref="RenameRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RenameRemote(RenameRemoteParameters parameters);

		/// <summary>Rename remote repository.</summary>
		/// <param name="parameters"><see cref="RenameRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task RenameRemoteAsync(RenameRemoteParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Get list of stale remote tracking branches that are subject to pruninig.</summary>
		/// <param name="parameters"><see cref="PruneRemoteParameters"/>.</param>
		/// <returns>List of stale remote tracking branches that are subject to pruninig.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<string> QueryPrunedBranches(PruneRemoteParameters parameters);

		/// <summary>Get list of stale remote tracking branches that are subject to pruninig.</summary>
		/// <param name="parameters"><see cref="PruneRemoteParameters"/>.</param>
		/// <returns>List of stale remote tracking branches that are subject to pruninig.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<IList<string>> QueryPrunedBranchesAsync(PruneRemoteParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Remove stale remote tracking branches.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void PruneRemote(PruneRemoteParameters parameters);

		/// <summary>Remove stale remote tracking branches.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task PruneRemoteAsync(PruneRemoteParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Remove remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RemoveRemote(RemoveRemoteParameters parameters);

		/// <summary>Remove remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task RemoveRemoteAsync(RemoveRemoteParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Get list of references on remote repository.</summary>
		/// <param name="parameters"><see cref="QueryRemoteReferencesParameters"/>.</param>
		/// <returns>List of remote references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<RemoteReferenceData> QueryRemoteReferences(QueryRemoteReferencesParameters parameters);

		/// <summary>Get list of references on remote repository.</summary>
		/// <param name="parameters"><see cref="QueryRemoteReferencesParameters"/>.</param>
		/// <returns>List of remote references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<IList<RemoteReferenceData>> QueryRemoteReferencesAsync(QueryRemoteReferencesParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Remove reference on remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteReferencesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RemoveRemoteReferences(RemoveRemoteReferencesParameters parameters);

		/// <summary>Remove reference on remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteReferencesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task RemoveRemoteReferencesAsync(RemoveRemoteReferencesParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Download objects and refs from another repository.</summary>
		/// <param name="parameters"><see cref="FetchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void Fetch(FetchParameters parameters);

		/// <summary>Download objects and refs from another repository.</summary>
		/// <param name="parameters"><see cref="FetchParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task FetchAsync(FetchParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Download objects and refs from another repository and merge with local branches configured for this.</summary>
		/// <param name="parameters"><see cref="PullParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void Pull(PullParameters parameters);

		/// <summary>Download objects and refs from another repository and merge with local branches configured for this.</summary>
		/// <param name="parameters"><see cref="PullParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task PullAsync(PullParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		///	<summary>Update remote refs along with associated objects.</summary>
		/// <param name="parameters"><see cref="PushParameters"/>.</param>
		///	<returns>List of pushed references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<ReferencePushResult> Push(PushParameters parameters);

		///	<summary>Update remote refs along with associated objects.</summary>
		/// <param name="parameters"><see cref="PushParameters"/>.</param>
		///	<returns>List of pushed references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<IList<ReferencePushResult>> PushAsync(PushParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);
	}
}

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Object which can perform various operations on git remotes.</summary>
	public interface IRemoteAccessor
	{
		/// <summary>Get information about remote.</summary>
		/// <param name="parameters"><see cref="QueryRemoteParameters"/>.</param>
		/// <returns>Requested remote.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		RemoteData QueryRemote(QueryRemoteParameters parameters);

		/// <summary>Query list of remotes.</summary>
		/// <param name="parameters"><see cref="QueryRemotesParameters"/>.</param>
		/// <returns>List of remotes.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<RemoteData> QueryRemotes(QueryRemotesParameters parameters);

		/// <summary>Add remote repository.</summary>
		/// <param name="parameters"><see cref="AddRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void AddRemote(AddRemoteParameters parameters);

		/// <summary>Rename remote repository.</summary>
		/// <param name="parameters"><see cref="RenameRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RenameRemote(RenameRemoteParameters parameters);

		/// <summary>Get list of stale remote tracking branches that are subject to pruninig.</summary>
		/// <param name="parameters"><see cref="PruneRemoteParameters"/>.</param>
		/// <returns>List of stale remote tracking branches that are subject to pruninig.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<string> QueryPrunedBranches(PruneRemoteParameters parameters);

		/// <summary>Remove stale remote tracking branches.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void PruneRemote(PruneRemoteParameters parameters);

		/// <summary>Remove remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RemoveRemote(RemoveRemoteParameters parameters);

		/// <summary>Get list of references on remote repository.</summary>
		/// <param name="parameters"><see cref="QueryRemoteReferencesParameters"/>.</param>
		/// <returns>List of remote references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<RemoteReferenceData> QueryRemoteReferences(QueryRemoteReferencesParameters parameters);

		/// <summary>Remove reference on remote repository.</summary>
		/// <param name="parameters"><see cref="RemoveRemoteReferencesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RemoveRemoteReferences(RemoveRemoteReferencesParameters parameters);

		/// <summary>Download objects and refs from another repository.</summary>
		/// <param name="parameters"><see cref="FetchParameters"/>.</param>
		/// <returns><c>true</c> if any objects were downloaded, <c>false</c> otherwise.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		bool Fetch(FetchParameters parameters);

		/// <summary>Download objects and refs from another repository and merge with local branches configured for this.</summary>
		/// <param name="parameters"><see cref="PullParameters"/>.</param>
		/// <returns><c>true</c> if any objects were downloaded, <c>false</c> otherwise.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		bool Pull(PullParameters parameters);

		///	<summary>Update remote refs along with associated objects.</summary>
		/// <param name="parameters"><see cref="PushParameters"/>.</param>
		///	<returns>List of pushed references.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<ReferencePushResult> Push(PushParameters parameters);
	}
}

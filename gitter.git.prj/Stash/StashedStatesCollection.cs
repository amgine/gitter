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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Repository's stash.</summary>
	public sealed class StashedStatesCollection : GitObject, IEnumerable<StashedState>
	{
		#region Events

		/// <summary>New <see cref="StashedState"/> was created.</summary>
		public event EventHandler<StashedStateEventArgs> StashedStateCreated;

		/// <summary><see cref="StashedState"/> was dropped.</summary>
		public event EventHandler<StashedStateEventArgs> StashedStateDeleted;

		/// <summary>Invokes <see cref="StashedStateCreated"/> event.</summary>
		/// <param name="state">Created stash.</param>
		private void InvokeStashedStateCreated(StashedState state)
		{
			var handler = StashedStateCreated;
			if(handler != null) handler(this, new StashedStateEventArgs(state));
		}

		/// <summary>Invokes <see cref="StashedStateDeleted"/> event.</summary>
		/// <param name="state">Dropped state.</param>
		private void InvokeStashedStateDeleted(StashedState state)
		{
			state.MarkAsDeleted();
			var handler = StashedStateDeleted;
			if(handler != null) handler(this, new StashedStateEventArgs(state));
		}

		#endregion

		#region Data

		private readonly List<StashedState> _stash;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="StashedStatesCollection"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		internal StashedStatesCollection(Repository repository)
			: base(repository)
		{
			_stash = new List<StashedState>();
		}

		#endregion

		#region Drop()

		private static StashDropParameters GetStashDropParameters(StashedState stashedState)
		{
			return new StashDropParameters(((IRevisionPointer)stashedState).Pointer);
		}

		private static StashDropParameters GetStashDropParameters()
		{
			return new StashDropParameters();
		}

		private void OnStashedStateDropped(StashedState stashedState)
		{
			lock(SyncRoot)
			{
				int index = _stash.IndexOf(stashedState);
				if(index >= 0)
				{
					_stash.RemoveAt(index);
					for(int i = index; i < _stash.Count; ++i)
					{
						_stash[i].Index = i;
					}
					InvokeStashedStateDeleted(stashedState);
				}
			}
		}

		private void OnStashedStateDropped()
		{
			lock(SyncRoot)
			{
				if(_stash.Count == 0)
				{
					return;
				}
				var stashedState = _stash[0];
				_stash.RemoveAt(0);
				for(int i = 0; i < _stash.Count; ++i)
				{
					_stash[i].Index = i;
				}
				InvokeStashedStateDeleted(stashedState);
			}
		}

		internal void Drop(StashedState stashedState)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			var parameters = GetStashDropParameters(stashedState);
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged))
			{
				Repository.Accessor.StashDrop.Invoke(parameters);
			}
			OnStashedStateDropped(stashedState);
		}

		internal Task DropAsync(StashedState stashedState, IProgress<OperationProgress> progress)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrPerformingStashDrop.AddEllipsis()));
			}
			var block = Repository.Monitor.BlockNotifications(RepositoryNotifications.StashChanged);
			var parameters = GetStashDropParameters(stashedState);
			return Repository.Accessor.StashDrop.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
					t =>
					{
						block.Dispose();
						TaskUtility.PropagateFaultedStates(t);
						OnStashedStateDropped(stashedState);
					},
					CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}

		public void Drop()
		{
			Verify.State.IsTrue(_stash.Count != 0);

			var parameters = GetStashDropParameters();
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged))
			{
				Repository.Accessor.StashDrop.Invoke(parameters);
			}
			OnStashedStateDropped();
		}

		public Task DropAsync(IProgress<OperationProgress> progress)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrPerformingStashDrop.AddEllipsis()));
			}
			var block = Repository.Monitor.BlockNotifications(RepositoryNotifications.StashChanged);
			var parameters = GetStashDropParameters();
			return Repository.Accessor.StashDrop.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
					t =>
					{
						block.Dispose();
						TaskUtility.PropagateFaultedStates(t);
						OnStashedStateDropped();
					},
					CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
		}

		#endregion

		#region Clear()

		private static StashClearParameters GetClearStashParameters()
		{
			return new StashClearParameters();
		}

		private void OnStashCleared()
		{
			lock(SyncRoot)
			{
				if(_stash.Count != 0)
				{
					for(int i = _stash.Count - 1; i >= 0; --i)
					{
						var ss = _stash[i];
						_stash.RemoveAt(i);
						InvokeStashedStateDeleted(ss);
					}
				}
			}
		}

		public void Clear()
		{
			var parameters = GetClearStashParameters();
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged))
			{
				Repository.Accessor.StashClear.Invoke(parameters);
			}
			OnStashCleared();
		}

		public Task ClearAsync(IProgress<OperationProgress> progress)
		{
			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsCleaningStash.AddEllipsis()));
			}
			var parameters = GetClearStashParameters();
			var block = Repository.Monitor.BlockNotifications(RepositoryNotifications.StashChanged);
			return Repository.Accessor
				.StashClear.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
				t =>
				{
					block.Dispose();
					TaskUtility.PropagateFaultedStates(t);
					OnStashCleared();
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		public StashedState MostRecentState
		{
			get
			{
				lock(SyncRoot)
				{
					if(_stash.Count == 0) return null;
					return _stash[0];
				}
			}
		}

		public StashedState this[int index]
		{
			get
			{
				lock(SyncRoot)
				{
					return _stash[index];
				}
			}
		}

		public StashedState this[string name]
		{
			get
			{
				const string ExcMessage = "Invalid stash name format.";

				Verify.Argument.IsNotNull(name, "name");
				Verify.Argument.IsTrue(name.StartsWith(GitConstants.StashFullName), "name", ExcMessage);

				var sfnl = GitConstants.StashFullName.Length;
				if(name.Length == sfnl)
				{
					lock(SyncRoot)
					{
						return _stash[0];
					}
				}
				else
				{
					Verify.Argument.IsTrue(name.Length - sfnl - 3 >= 1, "name", ExcMessage);
					Verify.Argument.IsTrue(name[sfnl + 0] == '@', "name", ExcMessage);
					Verify.Argument.IsTrue(name[sfnl + 1] == '{', "name", ExcMessage);
					Verify.Argument.IsTrue(name[name.Length - 1] == '}', "name", ExcMessage);
					var s = name.Substring(sfnl + 2, name.Length - sfnl - 3);
					int index;
					Verify.Argument.IsTrue(
						int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out index),
						"name", ExcMessage);
					lock(SyncRoot)
					{
						return _stash[index];
					}
				}
			}
		}

		public int Count
		{
			get
			{
				lock(SyncRoot)
				{
					return _stash.Count;
				}
			}
		}

		/// <summary>Returns if this stash is empty.</summary>
		/// <value><c>true</c> if this stash is empty, <c>false</c> otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				lock(SyncRoot)
				{
					return _stash.Count == 0;
				}
			}
		}

		/// <summary>Object used for cross-thread synchronization.</summary>
		public object SyncRoot
		{
			get { return _stash; }
		}

		/// <summary>Refresh stash content.</summary>
		public void Refresh()
		{
			var top = Repository.Accessor.QueryStashTop.Invoke(
				new QueryStashTopParameters(false));
			var stash = (top == null)?
				new StashedStateData[0] :
				Repository.Accessor.QueryStash.Invoke(new QueryStashParameters());
			lock(SyncRoot)
			lock(Repository.Revisions.SyncRoot)
			{
				if(_stash.Count != 0)
				{
					if(stash.Count == 0)
					{
						for(int i = _stash.Count - 1; i >= 0; --i)
						{
							var s = _stash[i];
							_stash.RemoveAt(i);
							InvokeStashedStateDeleted(s);
						}
					}
					else
					{
						var d = new Dictionary<Hash, StashedState>(_stash.Count, Hash.EqualityComparer);
						foreach(var s in _stash)
						{
							d.Add(s.Revision.Hash, s);
						}
						_stash.Clear();
						foreach(var ssinfo in stash)
						{
							StashedState stashedState;
							if(!d.TryGetValue(ssinfo.Revision.SHA1, out stashedState))
							{
								stashedState = ObjectFactories.CreateStashedState(Repository, ssinfo);
								_stash.Add(stashedState);
								InvokeStashedStateCreated(stashedState);
							}
							else
							{
								ObjectFactories.UpdateStashedState(stashedState, ssinfo);
								d.Remove(ssinfo.Revision.SHA1);
								_stash.Add(stashedState);
							}
						}
						if(d.Count != 0)
						{
							foreach(var ss in d.Values)
							{
								InvokeStashedStateDeleted(ss);
							}
						}
					}
				}
				else
				{
					if(stash.Count != 0)
					{
						foreach(var stashedStateData in stash)
						{
							var stashedState = ObjectFactories.CreateStashedState(Repository, stashedStateData);
							_stash.Add(stashedState);
							InvokeStashedStateCreated(stashedState);
						}
					}
				}
			}
		}

		internal void NotifyCleared()
		{
			lock(SyncRoot)
			{
				if(_stash.Count != 0)
				{
					for(int i = _stash.Count - 1; i >= 0; --i)
					{
						var ss = _stash[i];
						_stash.RemoveAt(i);
						InvokeStashedStateDeleted(ss);
					}
				}
			}
		}

		#region Apply()

		private static StashApplyParameters GetStashApplyParameters(StashedState stashedState, bool restoreIndex)
		{
			return new StashApplyParameters(((IRevisionPointer)stashedState).Pointer, restoreIndex);
		}

		private static StashApplyParameters GetStashApplyParameters(bool restoreIndex)
		{
			return new StashApplyParameters(restoreIndex);
		}

		internal void Apply(StashedState stashedState, bool restoreIndex)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			var parameters = GetStashApplyParameters(stashedState, restoreIndex);
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.StashApply.Invoke(parameters);
			}

			Repository.Status.Refresh();
		}

		internal Task ApplyAsync(StashedState stashedState, bool restoreIndex, IProgress<OperationProgress> progress)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrPerformingStashApply.AddEllipsis()));
			}
			var block = Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated);
			var parameters = GetStashApplyParameters(stashedState, restoreIndex);
			return Repository.Accessor
				.StashApply.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
				t =>
				{
					block.Dispose();
					Repository.Status.Refresh();
					TaskUtility.PropagateFaultedStates(t);
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		public void Apply(bool restoreIndex)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			var parameters = GetStashApplyParameters(restoreIndex);
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.StashApply.Invoke(parameters);
			}

			Repository.Status.Refresh();
		}

		public Task ApplyAsync(bool restoreIndex, IProgress<OperationProgress> progress)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrPerformingStashApply.AddEllipsis()));
			}
			var block = Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated);
			var parameters = GetStashApplyParameters(restoreIndex);
			return Repository.Accessor
				.StashApply.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
				t =>
				{
					block.Dispose();
					Repository.Status.Refresh();
					TaskUtility.PropagateFaultedStates(t);
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region Pop()

		private static StashPopParameters GetStashPopParameters(StashedState stashedState, bool restoreIndex)
		{
			return new StashPopParameters(((IRevisionPointer)stashedState).Pointer, restoreIndex);
		}

		private static StashPopParameters GetStashPopParameters(bool restoreIndex)
		{
			return new StashPopParameters(restoreIndex);
		}

		private void OnStashPopCompleted(StashedState stashedState)
		{
			lock(SyncRoot)
			{
				_stash.RemoveAt(stashedState.Index);
				for(int i = stashedState.Index; i < _stash.Count; ++i)
				{
					--_stash[i].Index;
				}
				InvokeStashedStateDeleted(stashedState);
			}

			Repository.Status.Refresh();
		}

		private void OnStashPopCompleted()
		{
			lock(SyncRoot)
			{
				if(_stash.Count == 0)
				{
					return;
				}
				var stashedState = _stash[0];
				_stash.RemoveAt(0);
				for(int i = 0; i < _stash.Count; ++i)
				{
					--_stash[i].Index;
				}
				InvokeStashedStateDeleted(stashedState);
			}

			Repository.Status.Refresh();
		}

		internal Task PopAsync(StashedState stashedState, bool restoreIndex, IProgress<OperationProgress> progress)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");
			Verify.State.IsTrue(_stash.Count != 0);

			var block = Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated);
			var parameters = GetStashPopParameters(stashedState, restoreIndex);
			return Repository.Accessor.StashPop.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
				t =>
				{
					block.Dispose();
					TaskUtility.PropagateFaultedStates(t);
					OnStashPopCompleted(stashedState);
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		internal void Pop(StashedState stashedState, bool restoreIndex)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");
			Verify.State.IsTrue(_stash.Count != 0);

			var parameters = GetStashPopParameters(stashedState, restoreIndex);
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.StashPop.Invoke(parameters);
			}

			OnStashPopCompleted(stashedState);
		}

		public Task PopAsync(bool restoreIndex, IProgress<OperationProgress> progress)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			var block = Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated);
			var parameters = GetStashPopParameters(restoreIndex);
			return Repository.Accessor.StashPop.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
				t =>
				{
					block.Dispose();
					TaskUtility.PropagateFaultedStates(t);
					OnStashPopCompleted();
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		public void Pop(bool restoreIndex)
		{
			Verify.State.IsTrue(_stash.Count != 0);

			var parameters = GetStashPopParameters(restoreIndex);
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.StashPop.Invoke(parameters);
			}

			OnStashPopCompleted();
		}

		#endregion

		#region ToBranch()

		internal Branch ToBranch(StashedState stashedState, string name)
		{
			Verify.Argument.IsValidGitObject(stashedState, Repository, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.StashChanged))
			{
				Repository.Accessor.StashToBranch.Invoke(
					new StashToBranchParameters(((IRevisionPointer)stashedState).Pointer, name));
			}

			lock(SyncRoot)
			{
				_stash.RemoveAt(stashedState.Index);
				for(int i = stashedState.Index; i < _stash.Count; ++i)
				{
					--_stash[i].Index;
				}
				InvokeStashedStateDeleted(stashedState);
			}

			Repository.Status.Refresh();

			var branchInformation = new BranchData(name,
				stashedState.Revision.Parents[0].Hash, false, true);
			var branch = Repository.Refs.Heads.NotifyCreated(branchInformation);
			Repository.Head.Pointer = branch;

			return branch;
		}

		#endregion

		#region Save()

		public StashedState Save(bool keepIndex)
		{
			return Save(keepIndex, false, null);
		}

		private StashSaveParameters GetStashSaveParameters(bool keepIndex, bool includeUntracked, string message)
		{
			return new StashSaveParameters(message, keepIndex, includeUntracked);
		}

		private StashedState OnStashSaveCompleted(bool created)
		{
			if(created)
			{
				var stashTopData = Repository.Accessor.QueryStashTop.Invoke(
					new QueryStashTopParameters(true));
				Revision revision;
				lock(Repository.Revisions.SyncRoot)
				{
					revision = ObjectFactories.CreateRevision(Repository, stashTopData);
				}
				var res = new StashedState(Repository, 0, revision);

				lock(SyncRoot)
				{
					_stash.Insert(0, res);
					for(int i = 1; i < _stash.Count; ++i)
					{
						++_stash[i].Index;
					}

					InvokeStashedStateCreated(res);
				}

				Repository.OnCommitCreated(res.Revision);
				Repository.Status.Refresh();

				return res;
			}
			else
			{
				return null;
			}
		}

		public StashedState Save(bool keepIndex, bool includeUntracked, string message)
		{
			Verify.State.IsFalse(Repository.IsEmpty,
				Resources.ExcCantDoOnEmptyRepository.UseAsFormat("stash save"));

			var parameters = GetStashSaveParameters(keepIndex, includeUntracked, message);
			bool created;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.StashChanged))
			{
				created = Repository.Accessor.StashSave.Invoke(parameters);
			}
			return OnStashSaveCompleted(created);
		}

		public Task<StashedState> SaveAsync(bool keepIndex, bool includeUntracked, string message, IProgress<OperationProgress> progress)
		{
			Verify.State.IsFalse(Repository.IsEmpty,
				Resources.ExcCantDoOnEmptyRepository.UseAsFormat("stash save"));

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrPerformingStashSave.AddEllipsis()));
			}
			var parameters = GetStashSaveParameters(keepIndex, includeUntracked, message);
			var block = Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.StashChanged);
			return Repository.Accessor.StashSave.InvokeAsync(parameters, progress, CancellationToken.None)
				.ContinueWith(
				t =>
				{
					block.Dispose();
					var created = TaskUtility.UnwrapResult(t);
					return OnStashSaveCompleted(created);
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region IEnumerable<StashedState> Members

		public IEnumerator<StashedState> GetEnumerator()
		{
			return _stash.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _stash.GetEnumerator();
		}

		#endregion
	}
}

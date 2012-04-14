namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

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

		internal IAsyncAction DropAsync(StashedState stashedState)
		{
			ValidateObject(stashedState, "stashedState");

			return AsyncAction.Create(
				stashedState,
				(state, monitor) =>
				{
					state.Drop();
				},
				Resources.StrStashDrop,
				Resources.StrPerformingStashDrop.AddEllipsis());
		}

		internal void Drop(StashedState stashedState)
		{
			ValidateObject(stashedState, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChangedNotification))
			{
				Repository.Accessor.StashDrop(
					new StashDropParameters(((IRevisionPointer)stashedState).Pointer));
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
		}

		public void Drop()
		{
			#region validate state

			if(_stash.Count == 0)
			{
				throw new InvalidOperationException();
			}

			#endregion

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChangedNotification))
			{
				Repository.Accessor.StashDrop(
					new StashDropParameters());
			}

			lock(SyncRoot)
			{
				var stashedState = _stash[0];

				_stash.RemoveAt(0);
				for(int i = 0; i < _stash.Count; ++i)
				{
					--_stash[i].Index;
				}

				InvokeStashedStateDeleted(stashedState);
			}
		}

		public IAsyncAction DropAsync()
		{
			if(_stash.Count == 0) throw new InvalidOperationException();
			return AsyncAction.Create(
				new
				{
					Repository = Repository,
					List = _stash,
					OnDeleted = (Action<StashedState>)InvokeStashedStateDeleted,
				},
				(data, monitor) =>
				{
					var repository = data.Repository;
					var stash = data.List;
					var onDeleted = data.OnDeleted;

					using(repository.Monitor.BlockNotifications(
						RepositoryNotifications.StashChangedNotification))
					{
						repository.Accessor.StashDrop(
							new StashDropParameters());
					}

					lock(stash)
					{
						var stashedState = stash[0];

						stash.RemoveAt(0);
						for(int i = 0; i < stash.Count; ++i)
							--stash[i].Index;

						onDeleted(stashedState);
					}
				},
				Resources.StrStashDrop,
				Resources.StrsRemovingStashedChanges.AddEllipsis());
		}

		#endregion

		#region Clear()

		public void Clear()
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChangedNotification))
			{
				Repository.Accessor.StashClear(
					new StashClearParameters());
			}
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

		public IAsyncAction ClearAsync()
		{
			return AsyncAction.Create(
				new
				{
					Repository = Repository,
					List = _stash,
					OnDeleted = (Action<StashedState>)InvokeStashedStateDeleted,
				},
				(data, monitor) =>
				{
					var repository = data.Repository;
					var stash = data.List;
					var onDeleted = data.OnDeleted;
					using(repository.Monitor.BlockNotifications(RepositoryNotifications.StashChangedNotification))
					{
						repository.Accessor.StashClear(
							new StashClearParameters());
					}
					lock(stash)
					{
						if(stash.Count != 0)
						{
							for(int i = stash.Count - 1; i >= 0; --i)
							{
								var stashedState = stash[i];
								stash.RemoveAt(i);
								onDeleted(stashedState);
							}
						}
					}
				},
				Resources.StrStashClear,
				Resources.StrsRemovingStashedChanges.AddEllipsis());
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
				if(name == null) throw new ArgumentNullException("name");
				if(name.StartsWith(GitConstants.StashFullName))
				{
					var l = GitConstants.StashFullName.Length;
					if(name.Length == l)
					{
						lock(SyncRoot)
						{
							return _stash[0];
						}
					}
					else
					{
						if(name.Length - l - 3 < 1)
							throw new ArgumentException("name");
						if(name[l + 0] != '@')
							throw new ArgumentException("name");
						if(name[l + 1] != '{')
							throw new ArgumentException("name");
						if(name[name.Length - 1] != '}')
							throw new ArgumentException("name");
						var s = name.Substring(l + 2, name.Length - l - 3);
						int index;
						if(!int.TryParse(s, out index))
							throw new ArgumentException("name");
						lock(SyncRoot)
						{
							return _stash[index];
						}
					}
				}
				else
				{
					throw new ArgumentException("name");
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
			var top = Repository.Accessor.QueryStashTop(
				new QueryStashTopParameters(false));
			var stash = (top == null)?
				new StashedStateData[0] : 
				Repository.Accessor.QueryStash(new QueryStashParameters());
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
						var d = new Dictionary<string, StashedState>(_stash.Count);
						foreach(var s in _stash)
						{
							d.Add(s.Revision.Name, s);
						}
						_stash.Clear();
						foreach(var ssinfo in stash)
						{
							StashedState ss;
							if(!d.TryGetValue(ssinfo.Revision.SHA1, out ss))
							{
								ss = ssinfo.Construct(Repository);
								_stash.Add(ss);
								InvokeStashedStateCreated(ss);
							}
							else
							{
								ssinfo.Update(ss);
								d.Remove(ssinfo.Revision.SHA1);
								_stash.Add(ss);
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
						foreach(var s in stash)
						{
							var state = s.Construct(Repository);
							_stash.Add(state);
							InvokeStashedStateCreated(state);
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

		internal IAsyncAction ApplyAsync(StashedState stashedState, bool restoreIndex)
		{
			ValidateObject(stashedState, "stashedState");

			return AsyncAction.Create(
				Tuple.Create(stashedState, restoreIndex),
				(data, monitor) =>
				{
					data.Item1.Apply(data.Item2);
				},
				Resources.StrStashApply,
				Resources.StrPerformingStashApply.AddEllipsis());
		}

		internal void Apply(StashedState stashedState, bool restoreIndex)
		{
			ValidateObject(stashedState, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChangedNotification,
				RepositoryNotifications.WorktreeUpdatedNotification,
				RepositoryNotifications.IndexUpdatedNotification))
			{
				Repository.Accessor.StashApply(
					new StashApplyParameters(((IRevisionPointer)stashedState).Pointer, restoreIndex));
			}

			Repository.Status.Refresh();
		}

		internal void Apply(StashedState state)
		{
			Apply(state, false);
		}

		public IAsyncAction ApplyAsync(bool restoreIndex)
		{
			#region validate state

			if(_stash.Count == 0)
			{
				throw new InvalidOperationException(Resources.ExcStashIsEmpty);
			}

			#endregion

			return AsyncAction.Create(
				new
				{
					Repository		= Repository,
					RestoreIndex	= restoreIndex,
				},
				(data, monitor) =>
				{
					data.Repository.Stash.Apply(data.RestoreIndex);
				},
				Resources.StrStashPop,
				Resources.StrPerformingStashApply.AddEllipsis());
		}

		public void Apply(bool restoreIndex)
		{
			#region validate state

			if(_stash.Count == 0)
			{
				throw new InvalidOperationException(Resources.ExcStashIsEmpty);
			}

			#endregion

			Apply(_stash[0], restoreIndex);
		}

		public void Apply()
		{
			Apply(false);
		}

		#endregion

		#region Pop

		internal IAsyncAction PopAsync(StashedState stashedState, bool restoreIndex)
		{
			ValidateObject(stashedState, "stashedState");

			return AsyncAction.Create(
				new
				{
					Repository		= Repository,
					StashedState	= stashedState,
					RestoreIndex	= restoreIndex,
				},
				(data, monitor) =>
				{
					data.Repository.Stash.Pop(
						data.StashedState, data.RestoreIndex);
				},
				Resources.StrStashPop,
				Resources.StrPerformingStashPop.AddEllipsis());
		}

		internal void Pop(StashedState stashedState, bool restoreIndex)
		{
			ValidateObject(stashedState, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.StashChangedNotification,
				RepositoryNotifications.WorktreeUpdatedNotification,
				RepositoryNotifications.IndexUpdatedNotification))
			{
				Repository.Accessor.StashPop(
					new StashPopParameters(((IRevisionPointer)stashedState).Pointer, restoreIndex));
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
		}

		internal void Pop(StashedState state)
		{
			Pop(state, false);
		}

		public IAsyncAction PopAsync(bool restoreIndex)
		{
			#region validate state

			if(_stash.Count == 0)
			{
				throw new InvalidOperationException(Resources.ExcStashIsEmpty);
			}

			#endregion

			return AsyncAction.Create(
				Tuple.Create(Repository, restoreIndex),
				(data, monitor) =>
				{
					data.Item1.Stash.Pop(data.Item2);
				},
				Resources.StrStashPop,
				Resources.StrPerformingStashPop.AddEllipsis());
		}

		public void Pop(bool restoreIndex)
		{
			#region validate state

			if(_stash.Count == 0)
			{
				throw new InvalidOperationException(Resources.ExcStashIsEmpty);
			}

			#endregion

			Pop(_stash[0], restoreIndex);
		}

		public void Pop()
		{
			Pop(false);
		}

		#endregion

		#region ToBranch()

		internal Branch ToBranch(StashedState stashedState, string name)
		{
			ValidateObject(stashedState, "stashedState");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification,
				RepositoryNotifications.CheckoutNotification,
				RepositoryNotifications.WorktreeUpdatedNotification,
				RepositoryNotifications.IndexUpdatedNotification,
				RepositoryNotifications.StashChangedNotification))
			{
				Repository.Accessor.StashToBranch(
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
				stashedState.Revision.Parents[0].Name, false, true);
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

		public StashedState Save(bool keepIndex, bool includeUntracked, string message)
		{
			#region validate state

			if(Repository.IsEmpty)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcCantDoOnEmptyRepository, "stash"));
			}

			#endregion

			bool created;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdatedNotification,
				RepositoryNotifications.WorktreeUpdatedNotification,
				RepositoryNotifications.StashChangedNotification))
			{
				created = Repository.Accessor.StashSave(
					new StashSaveParameters(message, keepIndex, includeUntracked));
			}

			if(created)
			{
				var stashTopData = Repository.Accessor.QueryStashTop(
					new QueryStashTopParameters(true));
				Revision revision;
				lock(Repository.Revisions.SyncRoot)
				{
					revision = stashTopData.Construct(Repository);
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

				Repository.InvokeCommitCreated(res.Revision);
				Repository.Status.Refresh();

				return res;
			}
			else
			{
				return null;
			}
		}

		public IAsyncFunc<StashedState> SaveAsync(bool keepIndex, bool includeUntracked, string message)
		{
			return new AsyncFunc<Repository, StashedState>(
				Repository,
				(repository, mon) =>
				{
					return repository.Stash.Save(keepIndex, includeUntracked, message);
				},
				Resources.StrStash,
				Resources.StrPerformingStashSave.AddEllipsis());
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

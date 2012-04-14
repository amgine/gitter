namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>git remote repository object.</summary>
	public sealed class Remote : GitLifeTimeNamedObject
	{
		#region Events

		/// <summary>Remote is renamed.</summary>
		public event EventHandler<NameChangeEventArgs> Renamed;

		/// <summary>Invoke <see cref="Renamed"/>.</summary>
		private void InvokeRenamed(string oldName, string newName)
		{
			var handler = Renamed;
			if(handler != null) handler(this, new NameChangeEventArgs(oldName, newName));
		}

		#endregion

		#region Data

		private string _fetchUrl;
		private string _pushUrl;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="Remote"/> object.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Remote name.</param>
		/// <param name="fetchUrl">Fetch URL.</param>
		/// <param name="pushUrl">Push URL.</param>
		internal Remote(Repository repository, string name, string fetchUrl, string pushUrl)
			: base(repository, name)
		{
			_fetchUrl = fetchUrl;
			_pushUrl = pushUrl;
		}

		/// <summary>Create <see cref="Remote"/> object.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Remote name.</param>
		internal Remote(Repository repository, string name)
			: this(repository, name, null, null)
		{
		}

		#endregion

		#region Properties

		/// <summary>Url used by fetch/pull commands.</summary>
		public string FetchUrl
		{
			get { return _fetchUrl; }
			set
			{
				if(_fetchUrl != value)
				{
					var pname = "remote." + Name + ".url";
					Repository.Configuration.SetValue(pname, value);
					pname = "remote." + Name + ".pushurl";
					if(_fetchUrl == _pushUrl && !Repository.Configuration.Exists(pname))
						_pushUrl = value;
					_fetchUrl = value;
				}
			}
		}

		internal void SetFetchUrl(string fetchUrl)
		{
			_fetchUrl = fetchUrl;
		}

		/// <summary>Url used by push command.</summary>
		public string PushUrl
		{
			get { return _pushUrl; }
			set
			{
				if(_pushUrl != value)
				{
					var pname = "remote." + Name + ".pushurl";
					Repository.Configuration.SetValue(pname, value);
					_pushUrl = value;
				}
			}
		}

		internal void SetPushUrl(string pushUrl)
		{
			_pushUrl = pushUrl;
		}

		/// <summary>Proxy.</summary>
		public string Proxy
		{
			get
			{
				var p = Repository.Configuration.TryGetParameter("remote." + Name + ".proxy");
				if(p != null) return p.Value;
				return string.Empty;
			}
			set
			{
				var pname = "remote." + Name + ".proxy";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
				{
					if(value == "") return;
					Repository.Configuration.SetValue(pname, value);
				}
				else
				{
					p.Value = value;
				}
			}
		}

		/// <summary>Version control system.</summary>
		public string VCS
		{
			get
			{
				var p = Repository.Configuration.TryGetParameter("remote." + Name + ".vcs");
				if(p != null) return p.Value;
				return string.Empty;
			}
			set
			{
				var pname = "remote." + Name + ".vcs";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
				{
					if(value == "") return;
					Repository.Configuration.SetValue(pname, value);
				}
				else
				{
					p.Value = value;
				}
			}
		}

		public string ReceivePack
		{
			get
			{
				var p = Repository.Configuration.TryGetParameter("remote." + Name + ".receivepack");
				if(p != null) return p.Value;
				return string.Empty;
			}
			set
			{
				var pname = "remote." + Name + ".receivepack";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
				{
					if(value == "") return;
					Repository.Configuration.SetValue(pname, value);
				}
				else
				{
					p.Value = value;
				}
			}
		}

		public string UploadPack
		{
			get
			{
				var p = Repository.Configuration.TryGetParameter("remote." + Name + ".uploadpack");
				if(p != null) return p.Value;
				return string.Empty;
			}
			set
			{
				var pname = "remote." + Name + ".uploadpack";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
				{
					if(value == "") return;
					Repository.Configuration.SetValue(pname, value);
				}
				else
				{
					p.Value = value;
				}
			}
		}

		public string FetchRefspec
		{
			get
			{
				var p = Repository.Configuration.TryGetParameter("remote." + Name + ".fetch");
				if(p != null) return p.Value;
				return string.Empty;
			}
			set
			{
				var pname = "remote." + Name + ".fetch";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
				{
					if(value == "") return;
					Repository.Configuration.SetValue(pname, value);
				}
				else
				{
					p.Value = value;
				}
			}
		}

		public string PushRefspec
		{
			get
			{
				var p = Repository.Configuration.TryGetParameter("remote." + Name + ".push");
				if(p != null) return p.Value;
				return string.Empty;
			}
			set
			{
				var pname = "remote." + Name + ".push";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
				{
					if(value == "") return;
					Repository.Configuration.SetValue(pname, value);
				}
				else
				{
					p.Value = value;
				}
			}
		}

		public bool Mirror
		{
			get
			{
				var pname = "remote." + Name + ".mirror";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
					return false;
				return p.Value == "true";
			}
			set
			{
				var pname = "remote." + Name + ".mirror";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
				{
					if(!value) return;
					Repository.Configuration.SetValue(pname, value ? "true" : "false");
				}
				else
				{
					p.Value = value ? "true" : "false";
				}
			}
		}

		public bool SkipFetchAll
		{
			get
			{
				var pname = "remote." + Name + ".skipfetchall";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
					return false;
				return p.Value == "true";
			}
			set
			{
				var pname = "remote." + Name + ".skipfetchall";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p == null)
				{
					if(!value) return;
					Repository.Configuration.SetValue(pname, value ? "true" : "false");
				}
				else
				{
					p.Value = value ? "true" : "false";
				}
			}
		}

		public TagFetchMode TagFetchMode
		{
			get
			{
				var p = Repository.Configuration.TryGetParameter("remote." + Name + ".tagopt");
				if(p != null)
				{
					switch(p.Value)
					{
						case "--tags":
							return TagFetchMode.AllTags;
						case "--no-tags":
							return TagFetchMode.NoTags;
					}
				}
				return TagFetchMode.Default;
			}
			set
			{
				string strvalue;
				switch(value)
				{
					case TagFetchMode.AllTags:
						strvalue = "--tags";
						break;
					case TagFetchMode.NoTags:
						strvalue = "--no-tags";
						break;
					default:
						strvalue = "";
						break;
				}
				var pname = "remote." + Name + ".tagopt";
				var p = Repository.Configuration.TryGetParameter(pname);
				if(p != null)
					p.Value = strvalue;
				else if(strvalue != "")
					Repository.Configuration.SetValue(pname, strvalue);
			}
		}

		#endregion

		#region Methods

		public RemoteReferencesCollection GetReferences()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			return new RemoteReferencesCollection(this);
		}

		public void Delete()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			Repository.Remotes.RemoveRemote(this);
		}

		/// <summary>Download new objects from remote repository.</summary>
		public void Fetch()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			bool fetchedSomething = false;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification,
				RepositoryNotifications.TagChangedNotification))
			{
				fetchedSomething = Repository.Accessor.Fetch(
					new FetchParameters(Name));
			}
			if(fetchedSomething)
			{
				Repository.Refs.Refresh(ReferenceType.RemoteBranch | ReferenceType.Tag);
				Repository.InvokeUpdated();
			}
		}

		/// <summary>Download new objects from remote repository.</summary>
		public IAsyncAction FetchAsync()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			return AsyncAction.Create(
				this,
				(remote, monitor) =>
				{
					var repository = remote.Repository;

					bool fetchedSomething = false;
					using(Repository.Monitor.BlockNotifications(
						RepositoryNotifications.BranchChangedNotification,
						RepositoryNotifications.TagChangedNotification))
					{
						fetchedSomething = repository.Accessor.Fetch(
							new FetchParameters(remote.Name)
							{
								Monitor = monitor,
							});
					}

					if(fetchedSomething)
					{
						monitor.SetAction(Resources.StrRefreshingReferences.AddEllipsis());
						Repository.Refs.Refresh(ReferenceType.RemoteBranch | ReferenceType.Tag);
						repository.InvokeUpdated();
					}
				},
				string.Format(Resources.StrFetch, Name),
				string.Format(Resources.StrFetchingDataFrom, Name),
				true);
		}

		/// <summary>Download new objects from remote repository and merge tracking branches.</summary>
		public void Pull()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			try
			{
				using(Repository.Monitor.BlockNotifications(
					RepositoryNotifications.BranchChangedNotification,
					RepositoryNotifications.TagChangedNotification))
				{
					Repository.Accessor.Pull(
						new PullParameters(Name));
				}
			}
			finally
			{
				Repository.Refs.Refresh();
				Repository.InvokeUpdated();
			}
		}

		public IAsyncAction PullAsync()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			return AsyncAction.Create(
				this,
				(remote, monitor) =>
				{
					var repository = remote.Repository;
					try
					{
						using(Repository.Monitor.BlockNotifications(
							RepositoryNotifications.BranchChangedNotification,
							RepositoryNotifications.TagChangedNotification))
						{

							if(repository.Accessor.Pull(
								new PullParameters(remote.Name)
								{
									Monitor = monitor,
								}))
							{
								monitor.SetAction(Resources.StrRefreshingReferences.AddEllipsis());
								repository.Refs.Refresh();
							}
						}
					}
					catch
					{
						monitor.SetAction(Resources.StrRefreshingReferences.AddEllipsis());
						repository.Refs.Refresh();
						throw;
					}
					finally
					{
						repository.InvokeUpdated();
					}
				},
				string.Format(Resources.StrPull, Name),
				string.Format(Resources.StrFetchingDataFrom, Name),
				true);
		}

		/// <summary>Send local objects to remote repository.</summary>
		public void Push(ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			#region validate arguments

			if(branches == null) throw new ArgumentNullException("branches");
			if(branches.Count == 0)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcCollectionMustContainAtLeastOneObject, "branch"), "branches");
			}

			var names = new List<string>(branches.Count);
			foreach(var b in branches)
			{
				if(b == null)
				{
					throw new ArgumentException(
						Resources.ExcCollectionMustNotContainNullElements, "branches");
				}
				if(b.Repository != Repository)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAllObjectsMustBeHandledByThisRepository, "branches"), "branches");
				}
				if(b.IsDeleted)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAtLeastOneOfSuppliedObjectIsDeleted, "branches"), "branches");
				}
				names.Add(b.Name);
			}

			#endregion

			IList<ReferencePushResult> res;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification))
			{
				res = Repository.Accessor.Push(
					new PushParameters(Name, sendTags?PushMode.Tags:PushMode.Default, names)
					{
						Force = forceOverwrite,
						ThinPack = thinPack,
					});
			}

			bool changed = false;
			for(int i = 0; i < res.Count; ++i)
			{
				if(res[i].Type != PushResultType.UpToDate && res[i].Type != PushResultType.Rejected)
				{
					changed = true;
					break;
				}
			}
			if(changed)
			{
				Repository.Refs.Remotes.Refresh();
			}
		}

		/// <summary>Send local objects to remote repository.</summary>
		public IAsyncAction PushAsync(ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			#region validate arguments

			if(branches == null) throw new ArgumentNullException("branches");
			if(branches.Count == 0)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcCollectionMustContainAtLeastOneObject, "branch"), "branches");
			}

			var names = new List<string>(branches.Count);
			foreach(var b in branches)
			{
				if(b == null)
				{
					throw new ArgumentException(
						Resources.ExcCollectionMustNotContainNullElements, "branches");
				}
				if(b.Repository != Repository)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAllObjectsMustBeHandledByThisRepository, "branches"), "branches");
				}
				if(b.IsDeleted)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAtLeastOneOfSuppliedObjectIsDeleted, "branches"), "branches");
				}
				names.Add(b.Name);
			}

			#endregion

			return AsyncAction.Create(
				Tuple.Create(this, names, forceOverwrite, thinPack, sendTags),
				(tuple, monitor) =>
				{
					var remote = tuple.Item1;
					var repository = remote.Repository;

					IList<ReferencePushResult> res;
					using(Repository.Monitor.BlockNotifications(
						RepositoryNotifications.BranchChangedNotification))
					{
						res = repository.Accessor.Push(
							new PushParameters(remote.Name, tuple.Item5 ? PushMode.Tags : PushMode.Default, tuple.Item2)
							{
								Force = tuple.Item3,
								ThinPack = tuple.Item4,
								Monitor = monitor,
							});
					}
					bool changed = false;
					for(int i = 0; i < res.Count; ++i)
					{
						if(res[i].Type != PushResultType.UpToDate && res[i].Type != PushResultType.Rejected)
						{
							changed = true;
							break;
						}
					}
					if(changed)
					{
						Repository.Refs.Remotes.Refresh();
					}
				},
				string.Format(Resources.StrPush, Name),
				string.Format(Resources.StrSendingDataTo.AddEllipsis(), Name),
				true);
		}

		/// <summary>Deletes all stale tracking branches.</summary>
		public void Prune()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChangedNotification))
			{
				Repository.Accessor.PruneRemote(
					new PruneRemoteParameters(Name));
			}
			Repository.Refs.Remotes.Refresh();
		}

		/// <summary>Deletes all stale tracking branches.</summary>
		public IAsyncAction PruneAsync()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			return AsyncAction.Create(
				this,
				(remote, monitor) =>
				{
					var repository = remote.Repository;
					using(repository.Monitor.BlockNotifications(
						RepositoryNotifications.BranchChangedNotification))
					{
						repository.Accessor.PruneRemote(
							new PruneRemoteParameters(remote.Name));
					}
					repository.Refs.Remotes.Refresh();
				},
				Resources.StrPruneRemote,
				Resources.StrsSearchingStaleBranches.AddEllipsis(),
				false);
		}

		#endregion

		#region Overrides

		/// <summary>Rename remote.</summary>
		/// <param name="newName">New name.</param>
		protected override void RenameCore(string newName)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Remote"));
			}

			#endregion

			Repository.Remotes.RenameRemote(this, newName);
		}

		/// <summary>Called after remote is renamed.</summary>
		/// <param name="oldName">Old name.</param>
		protected override void AfterRename(string oldName)
		{
			InvokeRenamed(oldName, Name);
			Repository.Remotes.NotifyRenamed(this, oldName);
		}

		#endregion
	}
}

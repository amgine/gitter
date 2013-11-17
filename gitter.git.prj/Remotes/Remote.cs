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
	using System.Globalization;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>git remote repository object.</summary>
	public sealed class Remote : GitNamedObjectWithLifetime
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
				Verify.Argument.IsNotNull(value, "value");
				Verify.State.IsNotDeleted(this);

				if(_fetchUrl != value)
				{
					var pname = "remote." + Name + ".url";
					Repository.Configuration.SetValue(pname, value);
					pname = "remote." + Name + ".pushurl";
					if(_fetchUrl == _pushUrl && !Repository.Configuration.Exists(pname))
					{
						_pushUrl = value;
					}
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
				Verify.Argument.IsNotNull(value, "value");
				Verify.State.IsNotDeleted(this);

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
				Verify.Argument.IsNotNull(value, "value");
				Verify.State.IsNotDeleted(this);

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
				Verify.Argument.IsNotNull(value, "value");
				Verify.State.IsNotDeleted(this);

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
				Verify.Argument.IsNotNull(value, "value");
				Verify.State.IsNotDeleted(this);

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
				Verify.Argument.IsNotNull(value, "value");
				Verify.State.IsNotDeleted(this);

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
				Verify.Argument.IsNotNull(value, "value");
				Verify.State.IsNotDeleted(this);

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
				Verify.Argument.IsNotNull(value, "value");
				Verify.State.IsNotDeleted(this);

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
				Verify.State.IsNotDeleted(this);

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
				Verify.State.IsNotDeleted(this);

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
				{
					p.Value = strvalue;
				}
				else if(strvalue.Length == 0)
				{
					Repository.Configuration.SetValue(pname, strvalue);
				}
			}
		}

		#endregion

		#region Methods

		public RemoteReferencesCollection GetReferences()
		{
			Verify.State.IsNotDeleted(this);

			return new RemoteReferencesCollection(this);
		}

		public void Delete()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Remotes.RemoveRemote(this);
		}

		/// <summary>Download new objects from remote repository.</summary>
		public void Fetch()
		{
			Verify.State.IsNotDeleted(this);

			RemotesUtility.FetchOrPull(Repository, this, false);
		}

		/// <summary>Download new objects from remote repository.</summary>
		public Task FetchAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsNotDeleted(this);

			return RemotesUtility.FetchOrPullAsync(Repository, this, false, progress, cancellationToken);
		}

		/// <summary>Download new objects from remote repository and merge tracking branches.</summary>
		public void Pull()
		{
			Verify.State.IsNotDeleted(this);

			RemotesUtility.FetchOrPull(Repository, this, true);
		}

		/// <summary>Download new objects from remote repository and merge tracking branches.</summary>
		public Task PullAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsNotDeleted(this);

			return RemotesUtility.FetchOrPullAsync(Repository, this, true, progress, cancellationToken);
		}

		/// <summary>Send local objects to remote repository.</summary>
		public void Push(ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags)
		{
			Verify.State.IsNotDeleted(this);
			Verify.Argument.IsValidRevisionPointerSequence(branches, Repository, "branches");
			Verify.Argument.IsTrue(branches.Count != 0, "branches",
				Resources.ExcCollectionMustContainAtLeastOneObject.UseAsFormat("branch"));

			var names = new List<string>(branches.Count);
			foreach(var branch in branches)
			{
				names.Add(branch.Name);
			}
			IList<ReferencePushResult> res;
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged))
			{
				res = Repository.Accessor.Push.Invoke(
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

		public Task PushAsync(ICollection<Branch> branches, bool forceOverwrite, bool thinPack, bool sendTags, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsNotDeleted(this);

			return RemotesUtility.PushAsync(Repository, this, branches, forceOverwrite, thinPack, sendTags, progress, cancellationToken);
		}

		private PruneRemoteParameters GetPruneParameters()
		{
			return new PruneRemoteParameters(Name);
		}

		/// <summary>Deletes all stale tracking branches.</summary>
		public void Prune()
		{
			Verify.State.IsNotDeleted(this);

			var state1 = RefsState.Capture(Repository, ReferenceType.RemoteBranch);
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged))
			{
				var parameters = GetPruneParameters();
				Repository.Accessor.PruneRemote.Invoke(parameters);
			}
			Repository.Refs.Remotes.Refresh();
			var state2 = RefsState.Capture(Repository, ReferenceType.RemoteBranch);
			var changes = RefsDiff.Calculate(state1, state2);
			Repository.Remotes.OnPruneCompleted(this, changes);
		}

		/// <summary>Deletes all stale tracking branches.</summary>
		public Task PruneAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsNotDeleted(this);

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsSearchingStaleBranches.AddEllipsis()));
			}
			var state1 = RefsState.Capture(Repository, ReferenceType.RemoteBranch);
			var block = Repository.Monitor.BlockNotifications(RepositoryNotifications.BranchChanged);
			var parameters = GetPruneParameters();
			return Repository.Accessor.PruneRemote.InvokeAsync(parameters, progress, cancellationToken)
				.ContinueWith(
				t =>
				{
					block.Dispose();
					Repository.Refs.Remotes.Refresh();
					var state2 = RefsState.Capture(Repository, ReferenceType.RemoteBranch);
					var changes = RefsDiff.Calculate(state1, state2);
					Repository.Remotes.OnPruneCompleted(this, changes);
					TaskUtility.PropagateFaultedStates(t);
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion

		#region Overrides

		/// <summary>Rename remote.</summary>
		/// <param name="newName">New name.</param>
		protected override void RenameCore(string newName)
		{
			Verify.State.IsNotDeleted(this);

			Repository.Remotes.RenameRemote(this, newName);
		}

		/// <summary>Called after remote is renamed.</summary>
		/// <param name="oldName">Old name.</param>
		protected override void AfterRename(string oldName)
		{
			Assert.IsNeitherNullNorWhitespace(oldName);

			InvokeRenamed(oldName, Name);
			Repository.Remotes.NotifyRenamed(this, oldName);
		}

		#endregion
	}
}

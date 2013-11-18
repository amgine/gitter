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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class RemoteReferencesCollection
	{
		#region Events

		public event EventHandler<RemoteReferenceEventArgs> TagDeleted;
		public event EventHandler<RemoteReferenceEventArgs> BranchDeleted;

		private void InvokeTagDeleted(RemoteRepositoryTag tag)
		{
			var handler = TagDeleted;
			if(handler != null) handler(this, new RemoteReferenceEventArgs(tag));
		}

		private void InvokeBranchDeleted(RemoteRepositoryBranch branch)
		{
			var handler = BranchDeleted;
			if(handler != null) handler(this, new RemoteReferenceEventArgs(branch));
		}

		public event EventHandler<RemoteReferenceEventArgs> TagCreated;
		public event EventHandler<RemoteReferenceEventArgs> BranchCreated;

		private void InvokeTagCreated(RemoteRepositoryTag tag)
		{
			var handler = TagCreated;
			if(handler != null) handler(this, new RemoteReferenceEventArgs(tag));
		}

		private void InvokeBranchCreated(RemoteRepositoryBranch branch)
		{
			var handler = BranchCreated;
			if(handler != null) handler(this, new RemoteReferenceEventArgs(branch));
		}

		#endregion

		#region Data

		private readonly Repository _repository;
		private readonly Remote _remote;
		private readonly Dictionary<string, RemoteRepositoryBranch> _remoteBranches;
		private readonly Dictionary<string, RemoteRepositoryTag> _remoteTags;
		private readonly object _syncRoot;

		#endregion

		#region .ctor

		internal RemoteReferencesCollection(Remote remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");

			_remote         = remote;
			_repository     = remote.Repository;
			_remoteBranches = new Dictionary<string, RemoteRepositoryBranch>();
			_remoteTags     = new Dictionary<string, RemoteRepositoryTag>();
			_syncRoot       = new object();
		}

		#endregion

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		public IEnumerable<RemoteRepositoryBranch> Branches
		{
			get { return _remoteBranches.Values; }
		}

		public int BranchCount
		{
			get { return _remoteBranches.Count; }
		}

		public IEnumerable<RemoteRepositoryTag> Tags
		{
			get { return _remoteTags.Values; }
		}

		public int TagCount
		{
			get { return _remoteTags.Count; }
		}

		public Remote Remote
		{
			get { return _remote; }
		}

		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		#endregion

		#region Methods

		private RemoveRemoteReferencesParameters GetRemoveRemoteReferenceParameters(BaseRemoteReference remoteReference)
		{
			return new RemoveRemoteReferencesParameters(_remote.Name, remoteReference.FullName);
		}

		internal void RemoveTag(RemoteRepositoryTag tag)
		{
			Verify.Argument.IsNotNull(tag, "tag");
			Verify.Argument.IsFalse(tag.IsDeleted, "tag",
				Resources.ExcSuppliedObjectIsDeleted.UseAsFormat("tag"));

			var parameters = GetRemoveRemoteReferenceParameters(tag);
			_remote.Repository.Accessor.RemoveRemoteReferences.Invoke(parameters);

			_remoteTags.Remove(tag.Name);
			tag.MarkAsDeleted();
			InvokeTagDeleted(tag);
		}

		internal Task RemoveTagAsync(RemoteRepositoryTag tag, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(tag, "tag");
			Verify.Argument.IsFalse(tag.IsDeleted, "tag",
				Resources.ExcSuppliedObjectIsDeleted.UseAsFormat("tag"));

			var parameters = GetRemoveRemoteReferenceParameters(tag);
			return _remote.Repository.Accessor
				.RemoveRemoteReferences.InvokeAsync(parameters, progress, cancellationToken)
				.ContinueWith(
				t =>
				{
					TaskUtility.PropagateFaultedStates(t);
					_remoteTags.Remove(tag.Name);
					tag.MarkAsDeleted();
					InvokeTagDeleted(tag);
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		internal void RemoveBranch(RemoteRepositoryBranch branch)
		{
			Verify.Argument.IsNotNull(branch, "branch");
			Verify.Argument.IsFalse(branch.IsDeleted, "branch",
				Resources.ExcSuppliedObjectIsDeleted.UseAsFormat("branch"));

			var parameters = GetRemoveRemoteReferenceParameters(branch);
			_remote.Repository.Accessor.RemoveRemoteReferences.Invoke(parameters);

			_remoteBranches.Remove(branch.Name);
			branch.MarkAsDeleted();
			InvokeBranchDeleted(branch);
		}

		internal Task RemoveBranchAsync(RemoteRepositoryBranch branch, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(branch, "branch");
			Verify.Argument.IsFalse(branch.IsDeleted, "branch",
				Resources.ExcSuppliedObjectIsDeleted.UseAsFormat("branch"));

			var parameters = GetRemoveRemoteReferenceParameters(branch);
			return _remote.Repository.Accessor
				.RemoveRemoteReferences.InvokeAsync(parameters, progress, cancellationToken)
				.ContinueWith(
				t =>
				{
					TaskUtility.PropagateFaultedStates(t);
					_remoteTags.Remove(branch.Name);
					branch.MarkAsDeleted();
					InvokeBranchDeleted(branch);
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		private QueryRemoteReferencesParameters GetQueryParameters()
		{
			return new QueryRemoteReferencesParameters(_remote.Name, true, true);
		}

		private void OnFetchCompleted(IList<RemoteReferenceData> refs)
		{
			var branches = new Dictionary<string, RemoteReferenceData>(refs.Count);
			var tags = new Dictionary<string, RemoteReferenceData>(refs.Count);
			foreach(var r in refs)
			{
				switch(r.ReferenceType)
				{
					case ReferenceType.LocalBranch:
						branches.Add(r.Name.Substring(GitConstants.LocalBranchPrefix.Length), r);
						break;
					case ReferenceType.Tag:
						tags.Add(r.Name.Substring(GitConstants.TagPrefix.Length), r);
						break;
				}
			}

			List<RemoteRepositoryBranch> deletedBranches;
			if(_remoteBranches.Count != 0)
			{
				deletedBranches = new List<RemoteRepositoryBranch>(_remoteBranches.Count);
				if(branches.Count == 0)
				{
					deletedBranches.AddRange(_remoteBranches.Values);
				}
				else
				{
					foreach(var b in _remoteBranches)
					{
						if(!branches.ContainsKey(b.Key))
						{
							deletedBranches.Add(b.Value);
						}
						else
						{
							branches.Remove(b.Key);
						}
					}
				}
			}
			else
			{
				deletedBranches = null;
				foreach(var b in _remoteBranches)
				{
					branches.Remove(b.Key);
				}
			}
			if(branches.Count != 0)
			{
				foreach(var b in branches)
				{
					var branch = new RemoteRepositoryBranch(this, b.Key, b.Value.Hash);
					_remoteBranches.Add(branch.Name, branch);
					InvokeBranchCreated(branch);
				}
			}
			if(deletedBranches != null && deletedBranches.Count != 0)
			{
				foreach(var b in deletedBranches)
				{
					_remoteBranches.Remove(b.Name);
					b.MarkAsDeleted();
					InvokeBranchDeleted(b);
				}
			}

			List<RemoteRepositoryTag> deletedTags;
			if(_remoteTags.Count != 0)
			{
				deletedTags = new List<RemoteRepositoryTag>(_remoteTags.Count);
				if(tags.Count == 0)
				{
					deletedTags.AddRange(_remoteTags.Values);
				}
				else
				{
					foreach(var t in _remoteTags)
					{
						if(!tags.ContainsKey(t.Key))
						{
							deletedTags.Add(t.Value);
						}
						else
						{
							tags.Remove(t.Key);
						}
					}
				}
			}
			else
			{
				deletedTags = null;
				foreach(var t in _remoteTags)
				{
					if(tags.ContainsKey(t.Key))
					{
						tags.Remove(t.Key);
					}
				}
			}
			if(tags.Count != 0)
			{
				foreach(var t in tags)
				{
					var tag = new RemoteRepositoryTag(this, t.Key, t.Value.TagType, t.Value.Hash);
					_remoteTags.Add(tag.Name, tag);
					InvokeTagCreated(tag);
				}
			}
			if(deletedTags != null && deletedTags.Count != 0)
			{
				foreach(var t in deletedTags)
				{
					_remoteTags.Remove(t.Name);
					t.MarkAsDeleted();
					InvokeTagDeleted(t);
				}
			}
		}

		public void Refresh()
		{
			var parameters = GetQueryParameters();
			var refs = Repository.Accessor.QueryRemoteReferences.Invoke(parameters);
			lock(SyncRoot)
			{
				OnFetchCompleted(refs);
			}
		}

		public Task RefreshAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrFetchingDataFromRemoteRepository));
			}
			var parameters = GetQueryParameters();
			return Repository.Accessor
				.QueryRemoteReferences.InvokeAsync(parameters, progress, cancellationToken)
				.ContinueWith(
				t =>
				{
					var refs = TaskUtility.UnwrapResult(t);
					lock(SyncRoot)
					{
						OnFetchCompleted(refs);
					}
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion
	}
}

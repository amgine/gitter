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

namespace gitter.Git;

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
		=> TagDeleted?.Invoke(this, new RemoteReferenceEventArgs(tag));

	private void InvokeBranchDeleted(RemoteRepositoryBranch branch)
		=> BranchDeleted?.Invoke(this, new RemoteReferenceEventArgs(branch));

	public event EventHandler<RemoteReferenceEventArgs> TagCreated;
	public event EventHandler<RemoteReferenceEventArgs> BranchCreated;

	private void InvokeTagCreated(RemoteRepositoryTag tag)
		=> TagCreated?.Invoke(this, new RemoteReferenceEventArgs(tag));

	private void InvokeBranchCreated(RemoteRepositoryBranch branch)
		=> BranchCreated?.Invoke(this, new RemoteReferenceEventArgs(branch));

	#endregion

	#region Data

	private readonly Dictionary<string, RemoteRepositoryBranch> _remoteBranches = new();
	private readonly Dictionary<string, RemoteRepositoryTag> _remoteTags = new();

	#endregion

	#region .ctor

	internal RemoteReferencesCollection(Remote remote)
	{
		Verify.Argument.IsNotNull(remote);

		Remote = remote;
		Repository = remote.Repository;
	}

	#endregion

	#region Properties

	public Repository Repository { get; }

	public IEnumerable<RemoteRepositoryBranch> Branches => _remoteBranches.Values;

	public int BranchCount => _remoteBranches.Count;

	public IEnumerable<RemoteRepositoryTag> Tags => _remoteTags.Values;

	public int TagCount => _remoteTags.Count;

	public Remote Remote { get; }

	public object SyncRoot { get; } = new();

	#endregion

	#region Methods

	private RemoveRemoteReferencesParameters GetRemoveRemoteReferenceParameters(BaseRemoteReference remoteReference)
	{
		return new RemoveRemoteReferencesParameters(Remote.Name, remoteReference.FullName);
	}

	private void OnTagRemoved(RemoteRepositoryTag tag)
	{
		Assert.IsNotNull(tag);

		_remoteTags.Remove(tag.Name);
		tag.MarkAsDeleted();
		InvokeTagDeleted(tag);
	}

	internal void RemoveTag(RemoteRepositoryTag tag)
	{
		Verify.Argument.IsNotNull(tag);
		Verify.Argument.IsFalse(tag.IsDeleted, nameof(tag),
			Resources.ExcSuppliedObjectIsDeleted.UseAsFormat(nameof(tag)));

		var parameters = GetRemoveRemoteReferenceParameters(tag);
		Remote.Repository.Accessor.RemoveRemoteReferences.Invoke(parameters);
		OnTagRemoved(tag);
	}

	internal async Task RemoveTagAsync(RemoteRepositoryTag tag,
		IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(tag);
		Verify.Argument.IsFalse(tag.IsDeleted, nameof(tag),
			Resources.ExcSuppliedObjectIsDeleted.UseAsFormat(nameof(tag)));

		var parameters = GetRemoveRemoteReferenceParameters(tag);
		await Remote.Repository.Accessor.RemoveRemoteReferences
			.InvokeAsync(parameters, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		OnTagRemoved(tag);
	}

	private void OnBranchRemoved(RemoteRepositoryBranch branch)
	{
		Assert.IsNotNull(branch);

		_remoteBranches.Remove(branch.Name);
		branch.MarkAsDeleted();
		InvokeBranchDeleted(branch);
	}

	internal void RemoveBranch(RemoteRepositoryBranch branch)
	{
		Verify.Argument.IsNotNull(branch);
		Verify.Argument.IsFalse(branch.IsDeleted, nameof(branch),
			Resources.ExcSuppliedObjectIsDeleted.UseAsFormat(nameof(branch)));

		var parameters = GetRemoveRemoteReferenceParameters(branch);
		Remote.Repository.Accessor.RemoveRemoteReferences.Invoke(parameters);
		OnBranchRemoved(branch);
	}

	internal async Task RemoveBranchAsync(RemoteRepositoryBranch branch,
		IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(branch);
		Verify.Argument.IsFalse(branch.IsDeleted, nameof(branch),
			Resources.ExcSuppliedObjectIsDeleted.UseAsFormat(nameof(branch)));

		var parameters = GetRemoveRemoteReferenceParameters(branch);
		await Remote.Repository.Accessor.RemoveRemoteReferences
			.InvokeAsync(parameters, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		OnBranchRemoved(branch);
	}

	private QueryRemoteReferencesParameters GetQueryParameters()
	{
		return new QueryRemoteReferencesParameters(Remote.Name, true, true);
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

	public async Task RefreshAsync(IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		progress?.Report(new OperationProgress(Resources.StrFetchingDataFromRemoteRepository));
		var parameters = GetQueryParameters();
		var refs = await Repository.Accessor
			.QueryRemoteReferences
			.InvokeAsync(parameters, progress, cancellationToken);
			
		lock(SyncRoot)
		{
			OnFetchCompleted(refs);
		}
	}

	#endregion
}

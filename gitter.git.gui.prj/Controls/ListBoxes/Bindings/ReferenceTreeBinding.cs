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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;

using gitter.Framework.Controls;

public sealed class ReferenceTreeBinding : IDisposable
{
	#region Data

	private readonly CustomListBoxItemsCollection _itemHost;
	private readonly bool _groupItems;
	private readonly bool _groupRemotes;
	private readonly Predicate<IRevisionPointer> _predicate;
	private readonly ReferenceType _referenceTypes;
	private readonly List<RemoteListItem> _remotes;

	#endregion

	public event EventHandler<RevisionPointerEventArgs> ReferenceItemActivated;

	private void InvokeReferenceItemActivated(IRevisionPointer revision)
		=> ReferenceItemActivated?.Invoke(this, new RevisionPointerEventArgs(revision));

	#region .ctor

	public ReferenceTreeBinding(CustomListBoxItemsCollection itemHost, Repository repository,
		bool groupItems, bool groupRemoteBranches, Predicate<IRevisionPointer> predicate, ReferenceType referenceTypes)
	{
		Verify.Argument.IsNotNull(itemHost);
		Verify.Argument.IsNotNull(repository);

		_itemHost = itemHost;
		Repository = repository;

		_groupItems = groupItems;
		_groupRemotes = groupRemoteBranches;
		_predicate = predicate;
		_referenceTypes = referenceTypes;

		_itemHost.Clear();
		if(groupItems)
		{
			Heads = new ReferenceGroupListItem(repository, ReferenceType.LocalBranch);
			Heads.Items.Comparison = BranchListItem.CompareByName;
			Remotes = new ReferenceGroupListItem(repository, ReferenceType.RemoteBranch);
			Remotes.Items.Comparison = groupRemoteBranches
				? RemoteListItem.CompareByName
				: RemoteBranchListItem.CompareByName;
			Tags = new ReferenceGroupListItem(repository, ReferenceType.Tag);
			Tags.Items.Comparison = TagListItem.CompareByName;
			_itemHost.Comparison = null;
		}
		else
		{
			Heads = null;
			Remotes = null;
			Tags = null;
			_itemHost.Comparison = ReferenceListItemBase.UniversalComparer;
		}

		_remotes = groupRemoteBranches ? new List<RemoteListItem>(repository.Remotes.Count) : null;

		if((referenceTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
		{
			var refs = Repository.Refs.Heads;
			lock(refs.SyncRoot)
			{
				foreach(var branch in refs)
				{
					if(predicate != null && !predicate(branch)) continue;
					var item = new BranchListItem(branch);
					item.Activated += OnItemActivated;
					var host = groupItems ? Heads.Items : _itemHost;
					host.Add(item);
				}
				refs.ObjectAdded += OnBranchCreated;
			}
		}

		if((referenceTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch)
		{
			var refs = repository.Refs.Remotes;
			lock(refs.SyncRoot)
			{
				foreach(var branch in refs)
				{
					if(predicate != null && !predicate(branch)) continue;
					var host = groupItems ? Remotes.Items : _itemHost;
					var item = new RemoteBranchListItem(branch);
					item.Activated += OnItemActivated;
					if(groupRemoteBranches)
					{
						var ritem = GetRemoteListItem(branch);
						if(ritem != null)
						{
							host = ritem.Items;
						}
					}
					host.Add(item);
				}
				refs.ObjectAdded += OnRemoteBranchCreated;
			}
		}

		if((referenceTypes & ReferenceType.Tag) == ReferenceType.Tag)
		{
			var refs = repository.Refs.Tags;
			lock(refs.SyncRoot)
			{
				foreach(var tag in refs)
				{
					if(predicate != null && !predicate(tag)) continue;
					var item = new TagListItem(tag);
					item.Activated += OnItemActivated;
					var host = groupItems ? Tags.Items : _itemHost;
					host.Add(item);
				}
				refs.ObjectAdded += OnTagCreated;
			}
		}

		if(groupItems)
		{
			_itemHost.Add(Heads);
			_itemHost.Add(Remotes);
			_itemHost.Add(Tags);
		}
	}

	#endregion

	public Repository Repository { get; }

	public ReferenceGroupListItem Heads { get; }

	public ReferenceGroupListItem Remotes { get; }

	public ReferenceGroupListItem Tags { get; }

	private RemoteListItem GetRemoteListItem(RemoteBranch branch)
	{
		lock(Repository.Remotes.SyncRoot)
		{
			foreach(var remote in Repository.Remotes)
			{
				if(branch.Name.StartsWith(remote.Name + "/"))
				{
					RemoteListItem ritem = null;
					foreach(var i in _remotes)
					{
						if(i.DataContext.Name == remote.Name)
						{
							ritem = i;
							break;
						}
					}
					if(ritem == null)
					{
						ritem = new RemoteListItem(remote);
						ritem.Items.Comparison = RemoteBranchListItem.CompareByName;
						_remotes.Add(ritem);
						var host = Remotes == null ? _itemHost : Remotes.Items;
						host.AddSafe(ritem);
					}
					return ritem;
				}
			}
		}
		return null;
	}

	#region Event Handlers

	private void OnBranchCreated(object sender, BranchEventArgs e)
	{
		var branch = e.Object;
		if(_predicate == null || _predicate(branch))
		{
			var item = new BranchListItem(branch);
			item.Activated += OnItemActivated;
			var host = _groupItems ? Heads.Items: _itemHost;
			host.AddSafe(item);
		}
	}

	private void OnRemoteBranchCreated(object sender, RemoteBranchEventArgs e)
	{
		var branch = e.Object;
		if(_predicate == null || _predicate(branch))
		{
			var item = new RemoteBranchListItem(branch);
			item.Activated += OnItemActivated;
			CustomListBoxItemsCollection host;
			if(_groupItems)
			{
				if(_groupRemotes)
				{
					var p = GetRemoteListItem(branch);
					if(p == null)
					{
						host = Remotes.Items;
					}
					else
					{
						host = p.Items;
					}
				}
				else
				{
					host = Remotes.Items;
				}
			}
			else
			{
				if(_groupRemotes)
				{
					var p = GetRemoteListItem(branch);
					if(p == null)
					{
						host = _itemHost;
					}
					else
					{
						host = p.Items;
					}
				}
				else
				{
					host = _itemHost;
				}
			}
			host.AddSafe(item);
		}
	}

	private void OnTagCreated(object sender, TagEventArgs e)
	{
		Assert.IsNotNull(e);

		var tag = e.Object;
		if(_predicate == null || _predicate(tag))
		{
			var item = new TagListItem(tag);
			item.Activated += OnItemActivated;
			var host = _groupItems
				? Tags.Items
				: _itemHost;
			host.AddSafe(item);
		}
	}

	private void OnItemActivated(object sender, EventArgs e)
	{
		var item = (IRevisionPointerListItem)sender;
		var revision = item.RevisionPointer;
		InvokeReferenceItemActivated(revision);
	}

	#endregion

	public void Dispose()
	{
		Repository.Refs.Heads.ObjectAdded -= OnBranchCreated;
		Repository.Refs.Remotes.ObjectAdded -= OnRemoteBranchCreated;
		Repository.Refs.Tags.ObjectAdded -= OnTagCreated;
		_itemHost.Clear();
	}
}

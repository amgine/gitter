namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	public sealed class ReferenceTreeBinding : IDisposable
	{
		#region Data

		private readonly CustomListBoxItemsCollection _itemHost;
		private readonly Repository _repository;

		private readonly bool _groupItems;
		private readonly bool _groupRemotes;
		private readonly Predicate<IRevisionPointer> _predicate;
		private readonly ReferenceType _referenceTypes;

		private readonly ReferenceGroupListItem _refsHeads;
		private readonly ReferenceGroupListItem _refsRemotes;
		private readonly ReferenceGroupListItem _refsTags;

		private readonly List<RemoteListItem> _remotes;

		#endregion

		public event EventHandler<RevisionPointerEventArgs> ReferenceItemActivated;

		private void InvokeReferenceItemActivated(IRevisionPointer revision)
		{
			var handler = ReferenceItemActivated;
			if(handler != null) handler(this, new RevisionPointerEventArgs(revision));
		}

		#region .ctor

		public ReferenceTreeBinding(CustomListBoxItemsCollection itemHost, Repository repository,
			bool groupItems, bool groupRemoteBranches, Predicate<IRevisionPointer> predicate, ReferenceType referenceTypes)
		{
			if(itemHost == null) throw new ArgumentNullException("itemHost");
			if(repository == null) throw new ArgumentNullException("repository");

			_itemHost = itemHost;
			_repository = repository;

			_groupItems = groupItems;
			_groupRemotes = groupRemoteBranches;
			_predicate = predicate;
			_referenceTypes = referenceTypes;

			_itemHost.Clear();
			if(groupItems)
			{
				_refsHeads = new ReferenceGroupListItem(repository, ReferenceType.LocalBranch);
				_refsHeads.Items.Comparison = BranchListItem.CompareByName;
				_refsRemotes = new ReferenceGroupListItem(repository, ReferenceType.RemoteBranch);
				if(groupRemoteBranches)
					_refsRemotes.Items.Comparison = RemoteListItem.CompareByName;
				else
					_refsRemotes.Items.Comparison = RemoteBranchListItem.CompareByName;
				_refsTags = new ReferenceGroupListItem(repository, ReferenceType.Tag);
				_refsTags.Items.Comparison = TagListItem.CompareByName;
				_itemHost.Comparison = null;
			}
			else
			{
				_refsHeads = null;
				_refsRemotes = null;
				_refsTags = null;
				_itemHost.Comparison = BaseReferenceListItem.UniversalComparer;
			}

			_remotes = groupRemoteBranches ? new List<RemoteListItem>(repository.Remotes.Count) : null;

			if((referenceTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
			{
				var refs = _repository.Refs.Heads;
				lock(refs.SyncRoot)
				{
					foreach(var branch in refs)
					{
						if(predicate != null && !predicate(branch)) continue;
						var item = new BranchListItem(branch);
						item.Activated += OnItemActivated;
						CustomListBoxItemsCollection host;
						if(groupItems)
							host = _refsHeads.Items;
						else
							host = _itemHost;
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
						var host = groupItems ? _refsRemotes.Items : _itemHost;
						var item = new RemoteBranchListItem(branch);
						item.Activated += OnItemActivated;
						if(groupRemoteBranches)
						{
							var ritem = GetRemoteListItem(branch);
							if(ritem != null)
								host = ritem.Items;
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
						CustomListBoxItemsCollection host;
						if(groupItems)
							host = _refsTags.Items;
						else
							host = _itemHost;
						host.Add(item);
					}
					refs.ObjectAdded += OnTagCreated;
				}
			}

			if(groupItems)
			{
				_itemHost.Add(_refsHeads);
				_itemHost.Add(_refsRemotes);
				_itemHost.Add(_refsTags);
			}
		}

		#endregion

		public ReferenceGroupListItem Heads
		{
			get { return _refsHeads; }
		}

		public ReferenceGroupListItem Remotes
		{
			get { return _refsRemotes; }
		}

		public ReferenceGroupListItem Tags
		{
			get { return _refsTags; }
		}

		private RemoteListItem GetRemoteListItem(RemoteBranch branch)
		{
			lock(_repository.Remotes.SyncRoot)
			{
				foreach(var remote in _repository.Remotes)
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
							CustomListBoxItemsCollection host;
							if(_refsRemotes == null)
								host = _itemHost;
							else
								host = _refsRemotes.Items;
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
				CustomListBoxItemsCollection host;
				if(_groupItems)
					host = _refsHeads.Items;
				else
					host = _itemHost;
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
							host = _refsRemotes.Items;
						else
							host = p.Items;
					}
					else
					{
						host = _refsRemotes.Items;
					}
				}
				else
				{
					if(_groupRemotes)
					{
						var p = GetRemoteListItem(branch);
						if(p == null)
							host = _itemHost;
						else
							host = p.Items;
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
			var tag = e.Object;
			if(_predicate == null || _predicate(tag))
			{
				CustomListBoxItemsCollection host;
				var item = new TagListItem(tag);
				item.Activated += OnItemActivated;
				if(_groupItems)
					host = _refsTags.Items;
				else
					host = _itemHost;
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
			_repository.Refs.Heads.ObjectAdded -= OnBranchCreated;
			_repository.Refs.Remotes.ObjectAdded -= OnRemoteBranchCreated;
			_repository.Refs.Tags.ObjectAdded -= OnTagCreated;
			_itemHost.Clear();
		}
	}
}

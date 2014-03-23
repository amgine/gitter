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

namespace gitter
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	sealed class RepositoryGroupBinding : IDisposable
	{
		#region Data

		private readonly RepositoryGroup _group;
		private readonly CustomListBoxItemsCollection _items;
		private readonly Dictionary<RepositoryGroup, RepositoryGroupListItem> _groupsMapping;
		private readonly Dictionary<RepositoryLink,  RepositoryListItem> _repositoriesMapping;

		#endregion

		#region .ctor

		public RepositoryGroupBinding(CustomListBoxItemsCollection items, RepositoryGroup group)
		{
			Verify.Argument.IsNotNull(items, "items");
			Verify.Argument.IsNotNull(group, "group");

			_items               = items;
			_group               = group;
			_groupsMapping       = new Dictionary<RepositoryGroup, RepositoryGroupListItem>();
			_repositoriesMapping = new Dictionary<RepositoryLink, RepositoryListItem>();

			foreach(var subGroup in group.Groups)
			{
				var item = new RepositoryGroupListItem(subGroup);
				_groupsMapping.Add(subGroup, item);
				items.Add(item);
			}
			foreach(var repository in group.Respositories)
			{
				var item = new RepositoryListItem(repository);
				_repositoriesMapping.Add(repository, item);
				items.Add(item);
			}

			group.Groups.Changed += OnGroupsChanged;
			group.Respositories.Changed += OnRepositoriesChanged;
		}

		#endregion

		#region Methods

		private void OnGroupsChanged(object sender, NotifyCollectionEventArgs e)
		{
			switch(e.Event)
			{
				case NotifyEvent.Insert:
					for(int i = e.StartIndex; i <= e.EndIndex; ++i)
					{
						var group = _group.Groups[i];
						var item  = new RepositoryGroupListItem(group);
						_groupsMapping.Add(group, item);
						_items.Insert(i, item);
					}
					break;
				case NotifyEvent.Remove:
					for(int i = e.StartIndex; i <= e.EndIndex; ++i)
					{
						var item = _items[i] as RepositoryGroupListItem;
						if(item != null)
						{
							_groupsMapping.Remove(item.DataContext);
						}
					}
					_items.RemoveRange(e.StartIndex, e.ModifiedItems);
					break;
				case NotifyEvent.Set:
					for(int i = e.StartIndex; i <= e.EndIndex; ++i)
					{
						var item = _items[i] as RepositoryGroupListItem;
						if(item != null)
						{
							_groupsMapping.Remove(item.DataContext);
						}
						var group = _group.Groups[i];
						item = new RepositoryGroupListItem(group);
						_groupsMapping.Add(group, item);
						_items[i] = item;
					}
					break;
				case NotifyEvent.Clear:
					_groupsMapping.Clear();
					_items.RemoveRange(0, e.ModifiedItems);
					break;
			}
		}

		private void OnRepositoriesChanged(object sender, NotifyCollectionEventArgs e)
		{
			switch(e.Event)
			{
				case NotifyEvent.Insert:
					for(int i = e.StartIndex; i <= e.EndIndex; ++i)
					{
						var repo = _group.Respositories[i];
						var item = new RepositoryListItem(repo);
						_repositoriesMapping.Add(repo, item);
						_items.Insert(i + _groupsMapping.Count, item);
					}
					break;
				case NotifyEvent.Remove:
					for(int i = e.StartIndex; i <= e.EndIndex; ++i)
					{
						var item = _items[i + _groupsMapping.Count] as RepositoryListItem;
						if(item != null)
						{
							_repositoriesMapping.Remove(item.DataContext);
						}
					}
					_items.RemoveRange(e.StartIndex + _groupsMapping.Count, e.ModifiedItems);
					break;
				case NotifyEvent.Set:
					for(int i = e.StartIndex; i <= e.EndIndex; ++i)
					{
						var item = _items[i + _groupsMapping.Count] as RepositoryListItem;
						if(item != null)
						{
							_repositoriesMapping.Remove(item.DataContext);
						}
						var repo = _group.Respositories[i];
						item = new RepositoryListItem(repo);
						_repositoriesMapping.Add(repo, item);
						_items[i + _groupsMapping.Count] = item;
					}
					break;
				case NotifyEvent.Clear:
					_repositoriesMapping.Clear();
					_items.RemoveRange(_groupsMapping.Count, e.ModifiedItems);
					break;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_groupsMapping.Clear();
			_repositoriesMapping.Clear();
			_group.Groups.Changed -= OnGroupsChanged;
			_group.Respositories.Changed -= OnRepositoriesChanged;
			_items.Clear();
		}

		#endregion
	}
}

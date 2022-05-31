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

using gitter.Git.AccessLayer;

/// <summary>Tag created/deleted.</summary>
sealed class TagChangedNotification : RepositoryChangedNotification
{
	public TagChangedNotification(string name)
	{
		Name = name;
	}

	public string Name { get; }

	public override object NotificationType => RepositoryNotifications.TagChanged;

	public override bool Apply(Repository repository)
	{
		var tagInformation = repository.Accessor.QueryTag.Invoke(
			new QueryTagParameters(Name));
		if(tagInformation is null)
		{
			var refs = repository.Refs.Tags;
			lock(refs.SyncRoot)
			{
				if(refs.Contains(Name))
				{
					refs.NotifyRemoved(Name);
				}
				else
				{
					return false;
				}
			}
		}
		else
		{
			var refs = repository.Refs.Tags;
			lock(refs.SyncRoot)
			{
				var tag = refs.TryGetItem(Name);
				if(tag is null)
				{
					refs.NotifyCreated(tagInformation);
				}
				else
				{
					if(tag.Revision.Hash != tagInformation.SHA1)
					{
						refs.NotifyRemoved(Name);
						refs.NotifyCreated(tagInformation);
					}
					else
					{
						return false;
					}
				}
			}
		}
		return true;
	}
}

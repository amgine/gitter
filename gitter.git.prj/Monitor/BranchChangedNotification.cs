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

/// <summary>Branch created/deleted/reset.</summary>
sealed class BranchChangedNotification : RepositoryChangedNotification
{
	public BranchChangedNotification(string name, bool remote)
	{
		Name     = name;
		IsRemote = remote;
	}

	public string Name { get; }

	public bool IsRemote { get; }

	public override object NotificationType => RepositoryNotifications.BranchChanged;

	public override bool Apply(Repository repository)
	{
		var branchInformation = repository.Accessor.QueryBranch.Invoke(
			new QueryBranchParameters(Name, IsRemote));
		if(branchInformation is null)
		{
			if(IsRemote)
			{
				var refs = repository.Refs.Remotes;
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
				var refs = repository.Refs.Heads;
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
		}
		else
		{
			if(IsRemote)
			{
				var refs = repository.Refs.Remotes;
				lock(refs.SyncRoot)
				{
					var branch = refs.TryGetItem(Name);
					if(branch is null)
					{
						refs.NotifyCreated(branchInformation);
					}
					else
					{
						if(branch.Revision.Hash != branchInformation.SHA1)
						{
							branch.NotifyReset(branchInformation);
						}
						else
						{
							return false;
						}
					}
				}
			}
			else
			{
				var refs = repository.Refs.Heads;
				lock(refs.SyncRoot)
				{
					var branch = refs.TryGetItem(Name);
					if(branch is null)
					{
						refs.NotifyCreated(branchInformation);
					}
					else
					{
						if(branch.Revision.Hash != branchInformation.SHA1)
						{
							branch.NotifyReset(branchInformation);
						}
						else
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}
}

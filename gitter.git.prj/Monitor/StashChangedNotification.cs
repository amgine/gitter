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

/// <summary>Stash removed/updated.</summary>
sealed class StashChangedNotification : RepositoryChangedNotification
{
	public override object NotificationType => RepositoryNotifications.StashChanged;

	public override bool Apply(Repository repository)
	{
		var top = repository.Accessor.QueryStashTop.Invoke(
			new QueryStashTopRequest(false));
		if(top is null)
		{
			repository.Stash.NotifyCleared();
		}
		else
		{
			repository.Stash.Refresh();
		}
		return true;
	}
}

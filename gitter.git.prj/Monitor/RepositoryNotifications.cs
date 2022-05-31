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

public static class RepositoryNotifications
{
	public static readonly object Checkout          = new();
	public static readonly object SubmodulesChanged = new();
	public static readonly object RepositoryRemoved = new();
	public static readonly object ConfigUpdated     = new();
	public static readonly object IndexUpdated      = new();
	public static readonly object WorktreeUpdated   = new();
	public static readonly object BranchChanged     = new();
	public static readonly object TagChanged        = new();
	public static readonly object StashChanged      = new();
	public static readonly object RemoteRemoved     = new();
	public static readonly object RemoteCreated     = new();
}

#region Copyright Notice
/*
* gitter - VCS repository management tool
* Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

#nullable enable

namespace gitter.Git;

public static class BranchHelper
{
	public static string TryFormatDefaultLocalBranchName(IRevisionPointer? revision)
	{
		switch(revision)
		{
			case RemoteBranch remoteBranch:
				var branchName = remoteBranch.Name;
				var remote     = remoteBranch.Remote;
				if(remote is not null)
				{
					var remoteName = remote.Name;
					if((branchName.Length > remoteName.Length + 1) && branchName.StartsWith(remoteName))
					{
						return branchName.Substring(remoteName.Length + 1);
					}
				}
				var slashPos = branchName.IndexOf('/');
				if(slashPos >= 0)
				{
					return branchName.Substring(slashPos + 1);
				}
				break;
		}
		return string.Empty;
	}
}

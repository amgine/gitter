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

using gitter.Framework;

static class RefsDiff
{
	public static Many<ReferenceChange> Calculate(RefsState before, RefsState after)
	{
		Verify.Argument.IsNotNull(before);
		Verify.Argument.IsNotNull(after);

		var changeset = new Many<ReferenceChange>.Builder();
		foreach(var afterState in after.States)
		{
			var beforeState = before.GetState(afterState.FullName, afterState.ReferenceType);
			if(beforeState is null)
			{
				changeset.Add(new(
					afterState.ReferenceType,
					afterState.FullName,
					afterState.Name,
					default,
					afterState.Hash,
					ReferenceChangeType.Added));
			}
			else if(afterState.Hash != beforeState.Hash)
			{
				changeset.Add(new(
					beforeState.ReferenceType,
					beforeState.FullName,
					beforeState.Name,
					beforeState.Hash,
					afterState.Hash,
					ReferenceChangeType.Moved));
			}
		}
		foreach(var beforeState in before.States)
		{
			var afterState = after.GetState(beforeState.FullName, beforeState.ReferenceType);
			if(afterState is null)
			{
				changeset.Add(new(
					beforeState.ReferenceType,
					beforeState.FullName,
					beforeState.Name,
					beforeState.Hash,
					default,
					ReferenceChangeType.Removed));
			}
		}
		return changeset;
	}
}

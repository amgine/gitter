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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	static class RefsDiff
	{
		public static ReferenceChange[] Calculate(RefsState state1, RefsState state2)
		{
			var changeset = new List<ReferenceChange>();
			foreach(var refState2 in state2.States)
			{
				var refState1 = state1.GetState(refState2.FullName, refState2.ReferenceType);
				if(refState1 == null)
				{
					changeset.Add(
						new ReferenceChange(
							refState2.ReferenceType,
							refState2.FullName,
							refState2.Name,
							default(Hash),
							refState2.Hash,
							ReferenceChangeType.Added));
				}
				else
				{
					if(refState2.Hash != refState1.Hash)
					{
						changeset.Add(
							new ReferenceChange(
								refState1.ReferenceType,
								refState1.FullName,
								refState1.Name,
								refState1.Hash,
								refState2.Hash,
								ReferenceChangeType.Moved));
					}
				}
			}
			foreach(var refState1 in state1.States)
			{
				var refState2 = state2.GetState(refState1.FullName, refState1.ReferenceType);
				if(refState2 == null)
				{
					changeset.Add(
						new ReferenceChange(
							refState1.ReferenceType,
							refState1.FullName,
							refState1.Name,
							refState1.Hash,
							default(Hash),
							ReferenceChangeType.Removed));
				}
			}
			return changeset.ToArray();
		}
	}
}

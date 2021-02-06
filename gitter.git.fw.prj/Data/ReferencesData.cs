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

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	public sealed class ReferencesData
	{
		public ReferencesData(IList<BranchData> heads, IList<BranchData> remotes, IList<TagData> tags, RevisionData stash)
		{
			Heads   = heads;
			Remotes = remotes;
			Tags    = tags;
			Stash   = stash;
		}

		public IList<BranchData> Heads { get; }

		public IList<BranchData> Remotes { get; }

		public IList<TagData> Tags { get; }

		public RevisionData Stash { get; }
	}
}

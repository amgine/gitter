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
	/// <summary>Reference types.</summary>
	[Flags]
	public enum ReferenceType
	{
		/// <summary>Invalid (none) reference.</summary>
		None = (0),

		/// <summary><see cref="Revision"/>.</summary>
		Revision = (1 << 0),
		/// <summary>Local <see cref="Branch"/>.</summary>
		LocalBranch = (1 << 1),
		/// <summary>Remote <see cref="Branch"/>.</summary>
		RemoteBranch = (1 << 2),
		/// <summary><see cref="Tag"/>.</summary>
		Tag = (1 << 3),
		/// <summary><see cref="StashedState"/>.</summary>
		Stash = (1 << 4),
		/// <summary><see cref="Remote"/>.</summary>
		Remote = (1 << 5),
		/// <summary><see cref="ReflogRecord"/>.</summary>
		ReflogRecord = (1 << 6),

		/// <summary>Local or remote <see cref="Branch"/>.</summary>
		Branch = LocalBranch | RemoteBranch,
		/// <summary>Tag or branch.</summary>
		Reference = Branch | Tag,

		/// <summary>Any valid reference.</summary>
		All = Revision | Branch | Tag,
	}
}

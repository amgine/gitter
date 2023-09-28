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

namespace gitter.Git.AccessLayer;

using System;

[Flags]
public enum RevisionField
{
	None = 0,

	CommitHash           = (1 <<  0),
	TreeHash       = (1 <<  1),
	Parents        = (1 <<  2),
	Children       = (1 <<  3),
	Boundary       = (1 <<  4),
	Subject        = (1 <<  5),
	Body           = (1 <<  6),
	CommitDate     = (1 <<  7),
	CommitterName  = (1 <<  8),
	CommitterEmail = (1 <<  9),
	AuthorDate     = (1 << 10),
	AuthorName     = (1 << 11),
	AuthorEmail    = (1 << 12),
}

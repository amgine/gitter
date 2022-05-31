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

/// <summary>Checkout file mode.</summary>
public enum CheckoutFileMode
{
	/// <summary>Default.</summary>
	Default,
	/// <summary>When checking out paths from the index, check out stage #2 (ours) for unmerged paths.</summary>
	Ours,
	/// <summary>When checking out paths from the index, check out stage #3 (theirs) for unmerged paths.</summary>
	Theirs,
	/// <summary>This option lets you recreate the conflicted merge in the specified paths.</summary>
	Merge,
	/// <summary>Do not fail upon unmerged entries; instead, unmerged entries are ignored.</summary>
	IgnoreUnmergedEntries,
}

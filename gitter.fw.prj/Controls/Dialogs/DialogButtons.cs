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

namespace gitter.Framework
{
	using System;

	/// <summary>Dialog form buttons.</summary>
	[Flags]
	public enum DialogButtons
	{
		/// <summary>No buttons.</summary>
		None = 0,

		/// <summary>OK button.</summary>
		Ok = (1 << 0),
		/// <summary>Cancel button.</summary>
		Cancel = (1 << 1),
		/// <summary>Apply button.</summary>
		Apply = (1 << 2),

		/// <summary>OK &amp; Cancel.</summary>
		OkCancel = Ok | Cancel,
		/// <summary>OK, Cancel &amp; Apply.</summary>
		All = Ok | Cancel | Apply,
	}
}

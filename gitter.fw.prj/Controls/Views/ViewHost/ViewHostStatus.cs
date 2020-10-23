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

namespace gitter.Framework.Controls
{
	/// <summary>Represents docking status of <see cref="ViewHost"/> control.</summary>
	public enum ViewHostStatus
	{
		/// <summary><see cref="ViewHost"/> is not present in dock model hierarchy and can be safely docked.</summary>
		Offscreen,
		/// <summary><see cref="ViewHost"/> is docked inside <see cref="ViewDockGrid"/> or <see cref="ViewSplit"/>.</summary>
		Docked,
		/// <summary><see cref="ViewHost"/> is docked inside <see cref="FloatingViewForm"/> with other hosts.</summary>
		DockedOnFloat,
		/// <summary><see cref="ViewHost"/> is docked inside <see cref="FloatingViewForm"/>.</summary>
		Floating,
		/// <summary><see cref="ViewHost"/> is docked inside <see cref="ViewDockSide"/>.</summary>
		AutoHide,
		/// <summary><see cref="ViewHost"/> is disposed as a result of docking operation.</summary>
		Disposed,
	}
}

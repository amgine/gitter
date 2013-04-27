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

namespace gitter.Native
{
	using System;

	[Flags]
	internal enum RedrawWindowFlags : uint
	{
		/// <summary>
		/// Invalidates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
		/// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_INVALIDATE invalidates the entire window.
		/// </summary>
		Invalidate = 0x1,

		/// <summary>Causes the OS to post a WM_PAINT message to the window regardless of whether a portion of the window is invalid.</summary>
		InternalPaint = 0x2,

		/// <summary>
		/// Causes the window to receive a WM_ERASEBKGND message when the window is repainted.
		/// Specify this value in combination with the RDW_INVALIDATE value; otherwise, RDW_ERASE has no effect.
		/// </summary>
		Erase = 0x4,

		/// <summary>
		/// Validates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
		/// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_VALIDATE validates the entire window.
		/// This value does not affect internal WM_PAINT messages.
		/// </summary>
		Validate = 0x8,

		NoInternalPaint = 0x10,

		/// <summary>Suppresses any pending WM_ERASEBKGND messages.</summary>
		NoErase = 0x20,

		/// <summary>Excludes child windows, if any, from the repainting operation.</summary>
		NoChildren = 0x40,

		/// <summary>Includes child windows, if any, in the repainting operation.</summary>
		AllChildren = 0x80,

		/// <summary>Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND and WM_PAINT messages before the RedrawWindow returns, if necessary.</summary>
		UpdateNow = 0x100,

		/// <summary>
		/// Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND messages before RedrawWindow returns, if necessary.
		/// The affected windows receive WM_PAINT messages at the ordinary time.
		/// </summary>
		EraseNow = 0x200,

		Frame = 0x400,

		NoFrame = 0x800
	}
}

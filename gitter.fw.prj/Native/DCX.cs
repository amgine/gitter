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

namespace gitter.Native;

using System;

[Flags]
internal enum DCX : uint
{
	/// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather
	/// than the client rectangle.</summary>
	WINDOW = 0x00000001,
	/// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC
	/// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
	CACHE = 0x00000002,
	/// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the
	/// default attributes when this DC is released.</summary>
	NORESETATTRS = 0x00000004,
	/// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows
	/// below the window identified by hWnd.</summary>
	CLIPCHILDREN = 0x00000008,
	/// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows
	/// above the window identified by hWnd.</summary>
	CLIPSIBLINGS = 0x00000010,
	/// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The
	/// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is
	/// set to the upper-left corner of the window identified by hWnd.</summary>
	PARENTCLIP = 0x00000020,
	/// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded
	/// from the visible region of the returned DC.</summary>
	EXCLUDERGN = 0x00000040,
	/// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is
	/// intersected with the visible region of the returned DC.</summary>
	INTERSECTRGN = 0x00000080,
	/// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
	EXCLUDEUPDATE = 0x00000100,
	/// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
	INTERSECTUPDATE = 0x00000200,
	/// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate
	/// call in effect that would otherwise exclude this window. Used for drawing during
	/// tracking.</summary>
	LOCKWINDOWUPDATE = 0x00000400,
	/// <summary>DCX_USESTYLE: Undocumented, something related to WM_NCPAINT message.</summary>
	USESTYLE = 0x00010000,
	/// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to
	/// be completely validated. Using this function with both DCX_INTERSECTUPDATE and
	/// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
	VALIDATE = 0x00200000,
}

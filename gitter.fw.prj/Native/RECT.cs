#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.Drawing;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		public static RECT FromXYWH(int x, int y, int width, int height)
		{
			return new RECT(x, y, x + width, y + height);
		}

		public int left;
		public int top;
		public int right;
		public int bottom;

		public RECT(int l, int t, int r, int b)
		{
			left   = l;
			top    = t;
			right  = r;
			bottom = b;
		}

		public RECT(Rectangle rect)
		{
			left   = rect.Left;
			top    = rect.Top;
			right  = rect.Right;
			bottom = rect.Bottom;
		}
	};
}

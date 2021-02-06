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

	internal static class Constants
	{
		public const int WS_EX_NOACTIVATE = 0x08000000;
		public const int HTTRANSPARENT = -1;

		public const int HTCAPTION = 2;
		public const int HTLEFT = 10;
		public const int HTRIGHT = 11;
		public const int HTTOP = 12;
		public const int HTTOPLEFT = 13;
		public const int HTTOPRIGHT = 14;
		public const int HTBOTTOM = 15;
		public const int HTBOTTOMLEFT = 16;
		public const int HTBOTTOMRIGHT = 17;

		public const int CBN_SELCHANGE = 1;
		public const int CBN_DROPDOWN = 7;
		public const int CBN_CLOSEUP = 8;
		public const int CBN_SELENDOK = 9;
		public const int CBN_SELENDCANCEL = 10;

		public const int GA_ROOT = 2;

		public const int CB_SHOWDROPDOWN = 0x014F;
		public const int CB_SELECTSTRING = 0x014D;
	}
}

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

	[Flags]
	public enum DT
	{
		TOP                  = 0x00000000,
		LEFT                 = 0x00000000,
		CENTER               = 0x00000001,
		RIGHT                = 0x00000002,
		VCENTER              = 0x00000004,
		BOTTOM               = 0x00000008,
		CALCRECT             = 0x00000400,
		EDITCONTROL          = 0x00002000,
		END_ELLIPSIS         = 0x00008000,
		EXPANDTABS           = 0x00000040,
		EXTERNALLEADING      = 0x00000200,
		HIDEPREFIX           = 0x00100000,
		INTERNAL             = 0x00001000,
		MODIFYSTRING         = 0x00010000,
		NOCLIP               = 0x00000100,
		NOFULLWIDTHCHARBREAK = 0x00080000,
		NOPREFIX             = 0x00000800,
		PATH_ELLIPSIS        = 0x00004000,
		PREFIXONLY           = 0x00200000,
		RTLREADING           = 0x00020000,
		SINGLELINE           = 0x00000020,
		TABSTOP              = 0x00000080,
		WORDBREAK            = 0x00000010,
		WORD_ELLIPSIS        = 0x00040000,
	}
}

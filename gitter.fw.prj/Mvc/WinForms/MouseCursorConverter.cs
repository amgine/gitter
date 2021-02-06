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

namespace gitter.Framework.Mvc.WinForms
{
	using System.Windows.Forms;

	public static class MouseCursorConverter
	{
		public static MouseCursor Convert(Cursor cursor)
		{
			if(cursor == Cursors.Default)
			{
				return MouseCursor.Default;
			}
			if(cursor == Cursors.AppStarting)
			{
				return MouseCursor.AppStarting;
			}
			if(cursor == Cursors.Arrow)
			{
				return MouseCursor.Arrow;
			}
			if(cursor == Cursors.Cross)
			{
				return MouseCursor.Cross;
			}
			if(cursor == Cursors.Hand)
			{
				return MouseCursor.Hand;
			}
			if(cursor == Cursors.Help)
			{
				return MouseCursor.Help;
			}
			if(cursor == Cursors.HSplit)
			{
				return MouseCursor.HSplit;
			}
			if(cursor == Cursors.IBeam)
			{
				return MouseCursor.IBeam;
			}
			if(cursor == Cursors.No)
			{
				return MouseCursor.No;
			}
			if(cursor == Cursors.NoMove2D)
			{
				return MouseCursor.NoMove2D;
			}
			if(cursor == Cursors.NoMoveHoriz)
			{
				return MouseCursor.NoMoveHoriz;
			}
			if(cursor == Cursors.NoMoveVert)
			{
				return MouseCursor.NoMoveVert;
			}
			if(cursor == Cursors.PanEast)
			{
				return MouseCursor.PanEast;
			}
			if(cursor == Cursors.PanNE)
			{
				return MouseCursor.PanNE;
			}
			if(cursor == Cursors.PanNorth)
			{
				return MouseCursor.PanNorth;
			}
			if(cursor == Cursors.PanNW)
			{
				return MouseCursor.PanNW;
			}
			if(cursor == Cursors.PanSE)
			{
				return MouseCursor.PanSE;
			}
			if(cursor == Cursors.PanSouth)
			{
				return MouseCursor.PanSouth;
			}
			if(cursor == Cursors.PanSW)
			{
				return MouseCursor.PanSW;
			}
			if(cursor == Cursors.PanWest)
			{
				return MouseCursor.PanWest;
			}
			if(cursor == Cursors.SizeAll)
			{
				return MouseCursor.SizeAll;
			}
			if(cursor == Cursors.SizeNESW)
			{
				return MouseCursor.SizeNESW;
			}
			if(cursor == Cursors.SizeNS)
			{
				return MouseCursor.SizeNS;
			}
			if(cursor == Cursors.SizeNWSE)
			{
				return MouseCursor.SizeNWSE;
			}
			if(cursor == Cursors.SizeWE)
			{
				return MouseCursor.SizeWE;
			}
			if(cursor == Cursors.UpArrow)
			{
				return MouseCursor.UpArrow;
			}
			if(cursor == Cursors.VSplit)
			{
				return MouseCursor.VSplit;
			}
			if(cursor == Cursors.WaitCursor)
			{
				return MouseCursor.WaitCursor;
			}
			return MouseCursor.Default;
		}

		public static Cursor Convert(MouseCursor cursor)
			=> cursor switch
			{
				MouseCursor.Default     => Cursors.Default,
				MouseCursor.AppStarting => Cursors.AppStarting,
				MouseCursor.Arrow       => Cursors.Arrow,
				MouseCursor.Cross       => Cursors.Cross,
				MouseCursor.Hand        => Cursors.Hand,
				MouseCursor.Help        => Cursors.Help,
				MouseCursor.HSplit      => Cursors.HSplit,
				MouseCursor.IBeam       => Cursors.IBeam,
				MouseCursor.No          => Cursors.No,
				MouseCursor.NoMove2D    => Cursors.NoMove2D,
				MouseCursor.NoMoveHoriz => Cursors.NoMoveHoriz,
				MouseCursor.NoMoveVert  => Cursors.NoMoveVert,
				MouseCursor.PanEast     => Cursors.PanEast,
				MouseCursor.PanNE       => Cursors.PanNE,
				MouseCursor.PanNorth    => Cursors.PanNorth,
				MouseCursor.PanNW       => Cursors.PanNW,
				MouseCursor.PanSE       => Cursors.PanSE,
				MouseCursor.PanSouth    => Cursors.PanSouth,
				MouseCursor.PanSW       => Cursors.PanSW,
				MouseCursor.PanWest     => Cursors.PanWest,
				MouseCursor.SizeAll     => Cursors.SizeAll,
				MouseCursor.SizeNESW    => Cursors.SizeNESW,
				MouseCursor.SizeNS      => Cursors.SizeNS,
				MouseCursor.SizeNWSE    => Cursors.SizeNWSE,
				MouseCursor.SizeWE      => Cursors.SizeWE,
				MouseCursor.UpArrow     => Cursors.UpArrow,
				MouseCursor.VSplit      => Cursors.VSplit,
				MouseCursor.WaitCursor  => Cursors.WaitCursor,
				_ => Cursors.Default,
			};
	}
}

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
		{
			switch(cursor)
			{
				case MouseCursor.Default:
					return Cursors.Default;
				case MouseCursor.AppStarting:
					return Cursors.AppStarting;
				case MouseCursor.Arrow:
					return Cursors.Arrow;
				case MouseCursor.Cross:
					return Cursors.Cross;
				case MouseCursor.Hand:
					return Cursors.Hand;
				case MouseCursor.Help:
					return Cursors.Help;
				case MouseCursor.HSplit:
					return Cursors.HSplit;
				case MouseCursor.IBeam:
					return Cursors.IBeam;
				case MouseCursor.No:
					return Cursors.No;
				case MouseCursor.NoMove2D:
					return Cursors.NoMove2D;
				case MouseCursor.NoMoveHoriz:
					return Cursors.NoMoveHoriz;
				case MouseCursor.NoMoveVert:
					return Cursors.NoMoveVert;
				case MouseCursor.PanEast:
					return Cursors.PanEast;
				case MouseCursor.PanNE:
					return Cursors.PanNE;
				case MouseCursor.PanNorth:
					return Cursors.PanNorth;
				case MouseCursor.PanNW:
					return Cursors.PanNW;
				case MouseCursor.PanSE:
					return Cursors.PanSE;
				case MouseCursor.PanSouth:
					return Cursors.PanSouth;
				case MouseCursor.PanSW:
					return Cursors.PanSW;
				case MouseCursor.PanWest:
					return Cursors.PanWest;
				case MouseCursor.SizeAll:
					return Cursors.SizeAll;
				case MouseCursor.SizeNESW:
					return Cursors.SizeNESW;
				case MouseCursor.SizeNS:
					return Cursors.SizeNS;
				case MouseCursor.SizeNWSE:
					return Cursors.SizeNWSE;
				case MouseCursor.SizeWE:
					return Cursors.SizeWE;
				case MouseCursor.UpArrow:
					return Cursors.UpArrow;
				case MouseCursor.VSplit:
					return Cursors.VSplit;
				case MouseCursor.WaitCursor:
					return Cursors.WaitCursor;
				default:
					return Cursors.Default;
			}
		}
	}
}

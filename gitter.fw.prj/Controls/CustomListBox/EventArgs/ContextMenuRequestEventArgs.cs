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
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class ContextMenuRequestEventArgs : EventArgs
	{
		#region Data

		private readonly CustomListBoxColumn _column;
		private readonly int _columnIndex;
		private readonly int _x;
		private readonly int _y;

		#endregion

		public ContextMenuRequestEventArgs(CustomListBoxColumn column, int columnIndex, int x, int y)
		{
			_column = column;
			_columnIndex = columnIndex;
			_x = x;
			_y = y;
		}

		public CustomListBoxColumn Column
		{
			get { return _column; }
		}

		public int ColumnIndex
		{
			get { return _columnIndex; }
		}

		public int SubItemId
		{
			get { return _column.Id; }
		}

		public Point Location
		{
			get { return new Point(_x, _y); }
		}

		public int X
		{
			get { return _x; }
		}

		public int Y
		{
			get { return _y; }
		}

		public ContextMenuStrip ContextMenu { get; set; }

		public bool OverrideDefaultMenu { get; set; }
	}
}

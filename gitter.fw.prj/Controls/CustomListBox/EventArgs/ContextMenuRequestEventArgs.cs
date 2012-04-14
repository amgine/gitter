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

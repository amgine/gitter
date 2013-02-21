namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	public abstract class CustomListBoxRenderer
	{
		public virtual Color BackColor
		{
			get { return SystemColors.Window; }
		}

		public virtual Color ForeColor
		{
			get { return SystemColors.WindowText;  }
		}

		public virtual Color ColumnHeaderForeColor
		{
			get { return SystemColors.GrayText; }
		}

		private Brush _foregroundBrush;
		public Brush ForegroundBrush
		{
			get
			{
				if(_foregroundBrush == null)
				{
					_foregroundBrush = new SolidBrush(ForeColor);
				}
				return _foregroundBrush;
			}
		}

		private Brush _columnHeaderForegroundBrush;
		public Brush ColumnHeaderForegroundBrush
		{
			get
			{
				if(_columnHeaderForegroundBrush == null)
				{
					_columnHeaderForegroundBrush = new SolidBrush(ColumnHeaderForeColor);
				}
				return _columnHeaderForegroundBrush;
			}
		}

		public abstract void OnPaintColumnBackground(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs);

		public abstract void OnPaintColumnContent(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs);

		public abstract void OnPaintItemBackground(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs);

		public abstract void OnPaintItemContent(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs);
	}
}

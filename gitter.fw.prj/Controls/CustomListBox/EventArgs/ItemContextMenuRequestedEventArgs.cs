namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class ItemContextMenuRequestEventArgs : ContextMenuRequestEventArgs
	{
		#region Data

		private readonly CustomListBoxItem _item;

		#endregion

		public ItemContextMenuRequestEventArgs(CustomListBoxItem item, CustomListBoxColumn column, int columnIndex, int x, int y)
			: base(column, columnIndex, x, y)
		{
			_item = item;
		}

		public CustomListBoxItem Item
		{
			get { return _item; }
		}
	}
}

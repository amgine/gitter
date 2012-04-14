namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	public class ItemsContextMenuRequestEventArgs : ContextMenuRequestEventArgs
	{
		#region Data

		private readonly ICollection<CustomListBoxItem> _items;

		#endregion

		public ItemsContextMenuRequestEventArgs(ICollection<CustomListBoxItem> items, CustomListBoxColumn column, int columnIndex, int x, int y)
			: base(column, columnIndex, x, y)
		{
			_items = items;
		}

		public ICollection<CustomListBoxItem> Items
		{
			get { return _items; }
		}
	}
}

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Atomic listbox element - single cell.</summary>
	public abstract class CustomListBoxSubItem
	{
		#region Data

		private readonly int _id;
		private CustomListBoxItem _item;

		#endregion

		#region .ctor

		protected CustomListBoxSubItem(int id)
		{
			_id = id;
		}

		#endregion

		#region Properties

		public int Id
		{
			get { return _id; }
		}

		public CustomListBoxItem Item
		{
			get { return _item; }
			internal set { _item = value; }
		}

		#endregion

		#region Methods

		public void Invalidate()
		{
			if(_item != null) _item.InvalidateSubItem(_id);
		}

		public void InvalidateSafe()
		{
			if(_item != null) _item.InvalidateSubItemSafe(_id);
		}

		public void Paint(SubItemPaintEventArgs paintEventArgs)
		{
			OnPaint(paintEventArgs);
		}

		public Size Measure(SubItemMeasureEventArgs measureEventArgs)
		{
			return OnMeasure(measureEventArgs);
		}

		protected abstract void OnPaint(SubItemPaintEventArgs paintEventArgs);

		protected abstract Size OnMeasure(SubItemMeasureEventArgs measureEventArgs);

		#endregion
	}
}

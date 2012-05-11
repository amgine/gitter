namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Content item which can be directly hosted by <see cref="CustomListBox"/>.</summary>
	/// <seealso cref="CustomListBoxColumn"/>
	/// <seealso cref="CustomListBoxItem"/>
	public abstract class CustomListBoxHostedItem
	{
		#region Data

		/// <summary>Listbox which is currently hosting this item.</summary>
		private CustomListBox _listBox;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="CustomListBoxHostedItem"/>.</summary>
		internal CustomListBoxHostedItem()
		{
		}

		#endregion

		#region Properties

		/// <summary>Returns if this item is attached to a listbox.</summary>
		public bool IsAttachedToListBox
		{
			get { return _listBox != null; }
		}

		/// <summary>Returns listbox which is currently hosting this item.</summary>
		/// <value>Listbox which is currently hosting this item.</value>
		public CustomListBox ListBox
		{
			get { return _listBox; }
			internal set
			{
				if(_listBox != value)
				{
					if(_listBox != null)
					{
						OnListBoxDetached();
					}
					_listBox = value;
					if(_listBox != null)
					{
						OnListBoxAttached();
					}
				}
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>Called when item is attached to listbox.</summary>
		protected virtual void OnListBoxAttached()
		{
		}

		/// <summary>Called when item is detached from listbox.</summary>
		protected virtual void OnListBoxDetached()
		{
		}

		/// <summary>Paints item background.</summary>
		/// <param name="paintEventArgs">Painting options.</param>
		protected abstract void OnPaintBackground(ItemPaintEventArgs paintEventArgs);

		/// <summary>Paints item content.</summary>
		/// <param name="paintEventArgs">Painting options.</param>
		protected abstract void OnPaintContent(ItemPaintEventArgs paintEventArgs);

		/// <summary>Perform item hit-testing.</summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <returns>Hit area id.</returns>
		protected abstract int OnHitTest(int x, int y);

		#endregion

		#region Methods

		/// <summary>Draw item.</summary>
		/// <param name="paintEventArgs">Paint options.</param>
		public void Paint(ItemPaintEventArgs paintEventArgs)
		{
			OnPaintBackground(paintEventArgs);
			OnPaintContent(paintEventArgs);
		}

		/// <summary>Perform item hit-testing.</summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <returns>Hit area id.</returns>
		public int HitTest(int x, int y)
		{
			return OnHitTest(x, y);
		}

		/// <summary>Perform item hit-testing.</summary>
		/// <param name="location">Coordinates.</param>
		/// <returns>Hit area id.</returns>
		public int HitTest(Point location)
		{
			return OnHitTest(location.X, location.Y);
		}

		#endregion
	}
}

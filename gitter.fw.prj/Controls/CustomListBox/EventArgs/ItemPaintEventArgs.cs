namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Item paint event args.</summary>
	public class ItemPaintEventArgs : EventArgs
	{
		#region Data

		private readonly Graphics _graphics;
		private readonly Rectangle _clipRectangle;
		private readonly Rectangle _bounds;
		private readonly int _index;
		private readonly ItemState _itemState;
		private readonly int _hoveredPart;
		private readonly bool _hostControlFocused;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ItemPaintEventArgs"/>.</summary>
		/// <param name="graphics">Graphics surface to draw the item on.</param>
		/// <param name="clipRectangle">Clipping rectangle.</param>
		/// <param name="bounds">Rectangle that represents the bounds of the item that is being drawn.</param>
		/// <param name="index">Index value of the item that is being drawn.</param>
		/// <param name="state">State of the item being drawn.</param>
		/// <param name="hoveredPart">Hovered part of the item.</param>
		/// <param name="hostControlFocused">Host control is focused.</param>
		public ItemPaintEventArgs(
			Graphics graphics, Rectangle clipRectangle, Rectangle bounds, int index,
			ItemState state, int hoveredPart, bool hostControlFocused)
		{
			_graphics = graphics;
			_clipRectangle = clipRectangle;
			_bounds = bounds;
			_index = index;
			_itemState = state;
			_hoveredPart = hoveredPart;
			_hostControlFocused = hostControlFocused;
		}

		#endregion

		#region Properties

		/// <summary>Gets the graphics surface to draw the item on.</summary>
		public Graphics Graphics
		{
			get { return _graphics; }
		}

		/// <summary>Clipping rectangle.</summary>
		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
		}

		/// <summary>Gets the rectangle that represents the bounds of the item that is being drawn.</summary>
		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		/// <summary>Gets the index value of the item that is being drawn.</summary>
		public int Index
		{
			get { return _index; }
		}
		/// <summary>Gets the state of the item being drawn.</summary>
		public ItemState State
		{
			get { return _itemState; }
		}

		/// <summary>Hovered part of the item.</summary>
		public int HoveredPart
		{
			get { return _hoveredPart; }
		}

		/// <summary>Host control is focused.</summary>
		public bool HostControlFocused
		{
			get { return _hostControlFocused; }
		}

		#endregion

		#region Methods()

		/// <summary>Prepare rectangle <paramref name="rect"/> for painting by applying content ofsets.</summary>
		/// <param name="rect">Rectangle to prepare.</param>
		public static void PrepareContentRectangle(ref Rectangle rect)
		{
			rect.X += ListBoxConstants.ContentSpacing;
			rect.Width -= ListBoxConstants.ContentSpacing * 2;
		}

		/// <summary>Prepare rectangle <paramref name="rect"/> for painting text by applying text offsets.</summary>
		/// <param name="listBoxFont">Fonf of hosing <see cref="CustomListBox"/>.</param>
		/// <param name="itemFont">Text font.</param>
		/// <param name="rect">Rectangle to prepare.</param>
		public void PrepareTextRectangle(Font listBoxFont, Font itemFont, ref Rectangle rect)
		{
			if(listBoxFont == itemFont)
			{
				var h = GitterApplication.TextRenderer.GetFontHeight(Graphics, listBoxFont);
				var d = (int)((rect.Height - h) / 2.0f);
				rect.Y += d;
				rect.Height -= d;
			}
			else
			{
				var h1 = GitterApplication.TextRenderer.GetFontHeight(Graphics, listBoxFont);
				var h = GitterApplication.TextRenderer.GetFontHeight(Graphics, itemFont);
				var d = (int)((rect.Height - h1) / 2.0f + h1 - h);
				rect.Y += d;
				rect.Height -= d;
			}
		}

		#endregion
	}
}

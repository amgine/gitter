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

	using gitter.Native;

	using Resources = gitter.Framework.Properties.Resources;

	public class CustomHScrollbar : CustomScrollBar
	{
		#region Data

		private Rectangle _decreaseButtonBounds;
		private Rectangle _decreaseTrackBarBounds;
		private Rectangle _thumbBounds;
		private Rectangle _increaseTrackBarBounds;
		private Rectangle _increaseButtonBounds;
		private int _initialScrollX;
		private bool _isArranged;
		private Point _mouseDownLocation;

		#endregion

		#region .ctor

		public CustomHScrollbar()
		{
			Height = SystemInformation.HorizontalScrollBarHeight;
		}

		#endregion

		#region Methods

		private void Arrange()
		{
			var size = Size;
			if(size.Width <= 0 || size.Height <= 0) return;
			var buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
			var physicalRange = Maximum - Minimum;
			var trackBarSize = size.Width - buttonWidth * 2;
			int thumbSize;
			int thumbOffset;
			if(physicalRange <= LargeChange)
			{
				thumbSize = 0;
				thumbOffset = 0;
			}
			else
			{
				if(trackBarSize < MinThumbSize)
				{
					thumbSize = 0;
				}
				else
				{
					thumbSize = Kernel32.MulDiv(trackBarSize, LargeChange, physicalRange);
					if(thumbSize < MinThumbSize) thumbSize = MinThumbSize;
				}
				var freeTrackBarSize = trackBarSize - thumbSize;
				thumbOffset = Kernel32.MulDiv(freeTrackBarSize, ClampValue(Value), (physicalRange - LargeChange + 1));
			}

			_decreaseButtonBounds	= new Rectangle(0, 0, buttonWidth, size.Height);
			_decreaseTrackBarBounds	= new Rectangle(buttonWidth, 0, thumbOffset, size.Height);
			_thumbBounds			= new Rectangle(buttonWidth + thumbOffset, 0, thumbSize, size.Height);
			_increaseTrackBarBounds	= new Rectangle(buttonWidth + thumbOffset + thumbSize, 0, trackBarSize - thumbSize - thumbOffset, size.Height);
			_increaseButtonBounds	= new Rectangle(size.Width - buttonWidth, 0, buttonWidth, size.Height);
			_isArranged = true;
		}

		private int ThumbPositionToValue(int thumbPosition)
		{
			int visualRange = _decreaseTrackBarBounds.Width + _increaseTrackBarBounds.Width;
			int physicalRange = Maximum - Minimum - LargeChange + 1;
			return Kernel32.MulDiv(physicalRange, thumbPosition, visualRange);
		}

		#endregion

		#region Event Handlers

		private void OnScrollHereClick(object sender, EventArgs e)
		{
			if(!_isArranged)
			{
				Arrange();
			}
			var x = _mouseDownLocation.X - _decreaseButtonBounds.Width;
			var thumbPosition = x - _thumbBounds.Width / 2;
			Value = ClampValue(ThumbPositionToValue(thumbPosition));
		}

		private void OnLeftEdgeClick(object sender, EventArgs e)
		{
			Value = ClampValue(Minimum);
		}

		private void OnRightEdgeClick(object sender, EventArgs e)
		{
			Value = ClampValue(Maximum);
		}

		private void OnScrollLeftClick(object sender, EventArgs e)
		{
			Value = ClampValue(Value - SmallChange);
		}

		private void OnScrollRightClick(object sender, EventArgs e)
		{
			Value = ClampValue(Value + SmallChange);
		}

		private void OnPageLeftClick(object sender, EventArgs e)
		{
			Value = ClampValue(Value - LargeChange);
		}

		private void OnPageRightClick(object sender, EventArgs e)
		{
			Value = ClampValue(Value + LargeChange);
		}

		#endregion

		#region CustomScrollBar Overrides

		public override Orientation Orientation => Orientation.Horizontal;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_mouseDownLocation = e.Location;
			switch(e.Button)
			{
				case MouseButtons.Right:
					var menu = new ContextMenuStrip();
					menu.Items.Add(new ToolStripMenuItem(Resources.StrScrollHere, null, OnScrollHereClick));
					menu.Items.Add(new ToolStripSeparator());
					menu.Items.Add(new ToolStripMenuItem(Resources.StrLeftEdge, null, OnLeftEdgeClick));
					menu.Items.Add(new ToolStripMenuItem(Resources.StrRightEdge, null, OnRightEdgeClick));
					menu.Items.Add(new ToolStripSeparator());
					menu.Items.Add(new ToolStripMenuItem(Resources.StrPageLeft, null, OnPageLeftClick));
					menu.Items.Add(new ToolStripMenuItem(Resources.StrPageRight, null, OnPageRightClick));
					menu.Items.Add(new ToolStripSeparator());
					menu.Items.Add(new ToolStripMenuItem(Resources.StrScrollLeft, null, OnScrollLeftClick));
					menu.Items.Add(new ToolStripMenuItem(Resources.StrScrollRight, null, OnScrollRightClick));
					Utility.MarkDropDownForAutoDispose(menu);
					menu.Show(this, e.X, e.Y);
					break;
			}
			base.OnMouseDown(e);
		}

		protected override Rectangle DecreaseButtonBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _decreaseButtonBounds;
			}
		}

		protected override Rectangle DecreaseTrackBarBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _decreaseTrackBarBounds;
			}
		}

		protected override Rectangle ThumbBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _thumbBounds;
			}
		}

		protected override Rectangle IncreaseTrackBarBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _increaseTrackBarBounds;
			}
		}

		protected override Rectangle IncreaseButtonBounds
		{
			get
			{
				if(!_isArranged)
				{
					Arrange();
				}
				return _increaseButtonBounds;
			}
		}

		protected override void ArrangeInvalidate()
		{
			_isArranged = false;
		}

		protected override void BeginScroll(Point from)
		{
			if(!_isArranged)
			{
				Arrange();
			}
			_initialScrollX = _thumbBounds.X;
		}

		protected override bool PerformScroll(Point from, Point to)
		{
			var dx = to.X - from.X;
			if(dx == 0) return false;
			int x = _initialScrollX + dx;
			int maxX = Width - (_decreaseButtonBounds.Width + _thumbBounds.Width);
			if(x > maxX)
			{
				x = maxX;
			}
			if(x < _decreaseButtonBounds.Width)
			{
				x = _decreaseButtonBounds.Width;
			}
			if(_thumbBounds.X != x)
			{
				dx = x - _thumbBounds.X;
				_thumbBounds.X = x;
				_decreaseTrackBarBounds.Width += dx;
				_increaseTrackBarBounds.X += dx;
				_increaseTrackBarBounds.Width -= dx;
				return true;
			}
			else
			{
				return false;
			}
		}

		protected override bool EndScroll(Point from, Point to)
		{
			return PerformScroll(from, to);
		}

		protected override int ThumbPositionToValue()
		{
			if(!_isArranged)
			{
				Arrange();
			}
			return ThumbPositionToValue(_decreaseTrackBarBounds.Width);
		}

		#endregion
	}
}

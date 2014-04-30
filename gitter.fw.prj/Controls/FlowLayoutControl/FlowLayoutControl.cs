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
	using System.ComponentModel;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	using gitter.Framework.Services;

	/// <summary>Control which hosts <see cref="FlowPanel"/>'s.</summary>
	[DefaultProperty("Panels")]
	public class FlowLayoutControl : ScrollableControl
	{
		#region Data

		private readonly FlowPanelCollection _panels;
		private readonly Dictionary<FlowPanel, Size> _sizes;
		private readonly TrackingService<FlowPanel> _panelHover;
		private FlowPanel _mouseDownPanel;

		#endregion

		/// <summary>Create <see cref="FlowLayoutControl"/>.</summary>
		public FlowLayoutControl()
		{
			_panels = new FlowPanelCollection(this);
			_panels.Changing += OnPanelsChanging;
			_panels.Changed += OnPanelsChanged;

			_sizes = new Dictionary<FlowPanel, Size>();
			_panelHover = new TrackingService<FlowPanel>();
			_panelHover.Changed += OnPanelHoverChanged;

			BackColor = Style.Colors.Window;
			ForeColor = Style.Colors.WindowText;
		}

		protected override void OnStyleChanged()
		{
			BackColor = Style.Colors.Window;
			ForeColor = Style.Colors.WindowText;
			base.OnStyleChanged();
		}

		internal void InvalidatePanelSize(FlowPanel panel)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new Action<FlowPanel>(InvalidatePanelSize), panel);
			}
			else
			{
				_sizes.Remove(panel);
				NotifyContentSizeChanged();
			}
		}

		internal Rectangle GetPanelBounds(FlowPanel panel)
		{
			Verify.Argument.IsNotNull(panel, "panel");
			Verify.Argument.IsTrue(panel.FlowControl == this, "panel", "Panel is not owned by this FlowLayoutControl.");

			var graphics = GraphicsUtility.MeasurementGraphics;
			int y = ClientArea.Y;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var p = _panels[i];
				var size = GetPanelSize(graphics, p);
				int maxY = y + size.Height;
				if(p == panel)
				{
					return new Rectangle(ClientArea.X, y, size.Width, size.Height);
				}
				y = maxY;
			}
			Assert.Fail("Panel not found.");
			return Rectangle.Empty;
		}

		internal void InvalidatePanel(FlowPanel panel, Rectangle rect)
		{
			Verify.Argument.IsNotNull(panel, "panel");
			Verify.Argument.IsTrue(panel.FlowControl == this, "panel", "Panel is not owned by this FlowLayoutControl.");

			var graphics = GraphicsUtility.MeasurementGraphics;
			int y = ClientArea.Y - VScrollPos;
			int x = ClientArea.X - HScrollPos;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var p = _panels[i];
				var size = GetPanelSize(graphics, p);
				int maxY = y + size.Height;
				if(p == panel)
				{
					if(maxY >= ClientArea.Y)
					{
						var bounds = new Rectangle(x + rect.X, y + rect.Y, rect.Width, rect.Height);
						var clip = Rectangle.Intersect(bounds, ClientArea);
						if(clip.Width > 0 && clip.Height > 0)
						{
							Invalidate(clip);
						}
					}
					break;
				}
				if(maxY >= ClientArea.Bottom)
				{
					break;
				}
				y = maxY;
			}
		}

		internal void InvalidatePanel(FlowPanel panel)
		{
			Verify.Argument.IsNotNull(panel, "panel");
			Verify.Argument.IsTrue(panel.FlowControl == this, "panel", "Panel is not owned by this FlowLayoutControl.");

			var graphics = GraphicsUtility.MeasurementGraphics;
			int y = ClientArea.Y - VScrollPos;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var p = _panels[i];
				var size = GetPanelSize(graphics, p);
				int maxY = y + size.Height;
				if(p == panel)
				{
					if(maxY >= ClientArea.Y)
					{
						var bounds = new Rectangle(ClientArea.X, y, ClientArea.Width, size.Height);
						var clip = Rectangle.Intersect(bounds, ClientArea);
						if(clip.Width > 0 && clip.Height > 0)
						{
							Invalidate(clip);
						}
					}
					break;
				}
				if(maxY >= ClientArea.Bottom)
				{
					break;
				}
				y = maxY;
			}
		}

		internal void ScrollIntoView(FlowPanel p)
		{
			int panelY = 0;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var panel = _panels[i];
				var size = GetPanelSize(panel);
				int maxY = panelY + size.Height;
				if(p == panel)
				{
					if(panelY > MaxVScrollPos)
					{
						panelY = MaxVScrollPos;
					}
					VScrollBar.Value = panelY;
					break;
				}
				panelY = maxY;
			}
		}

		protected override void EndUpdate(bool refresh)
		{
			if(UpdateCounter == 1 && refresh)
			{
				NotifyContentSizeChanged();
			}
			base.EndUpdate(refresh);
		}

		/// <summary>Collection of hosted <see cref="FlowPanel"/>'s.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FlowPanelCollection Panels
		{
			get { return _panels; }
		}

		private void OnPanelHoverChanged(object sender, TrackingEventArgs<FlowPanel> e)
		{
			if(e.IsTracked)
			{
				e.Item.MouseEnter();
			}
			else
			{
				e.Item.MouseLeave();
			}
		}

		private void OnPanelsChanging(object sender, NotifyCollectionEventArgs e)
		{
			switch(e.Event)
			{
				case NotifyEvent.Clear:
					_panelHover.Drop();
					break;
				case NotifyEvent.Remove:
					if(_panelHover.Index >= e.StartIndex)
					{
						if(_panelHover.Index <= e.EndIndex)
						{
							_panelHover.Drop();
						}
						else
						{
							_panelHover.ResetIndex(_panelHover.Index - e.ModifiedItems);
						}
					}
					break;
				case NotifyEvent.Set:
					if(_panelHover.Index >= e.StartIndex)
					{
						if(_panelHover.Index <= e.EndIndex)
						{
							_panelHover.Drop();
						}
						else
						{
							_panelHover.ResetIndex(_panelHover.Index - e.ModifiedItems);
						}
					}
					break;
			}
		}

		private void OnPanelsChanged(object sender, NotifyCollectionEventArgs e)
		{
			_sizes.Clear();
			if(!IsUpdating)
			{
				NotifyContentSizeChanged();
			}
		}

		private Size GetPanelSize(Graphics graphics, FlowPanel panel)
		{
			Size size;
			if(!_sizes.TryGetValue(panel, out size))
			{
				size = panel.Measure(
					new FlowPanelMeasureEventArgs(graphics, ClientArea.Width));
				_sizes.Add(panel, size);
			}
			return size;
		}

		private Size GetPanelSize(FlowPanel panel)
		{
			Size size;
			if(!_sizes.TryGetValue(panel, out size))
			{
				var graphics = GraphicsUtility.MeasurementGraphics;
				size = panel.Measure(
					new FlowPanelMeasureEventArgs(graphics, ClientArea.Width));
				_sizes.Add(panel, size);
			}
			return size;
		}

		private Size GetPanelSize(Graphics graphics, FlowPanel panel, int width)
		{
			Size size;
			if(!_sizes.TryGetValue(panel, out size))
			{
				size = panel.Measure(
					new FlowPanelMeasureEventArgs(graphics, width));
				_sizes.Add(panel, size);
			}
			return size;
		}

		protected override Size MeasureContent()
		{
			var res = Size.Empty;
			if(_panels.Count != 0)
			{
				var graphics = GraphicsUtility.MeasurementGraphics;
				foreach(var panel in _panels)
				{
					var size = GetPanelSize(graphics, panel);
					if(size.Width > res.Width)
					{
						res.Width = size.Width;
					}
					res.Height += size.Height;
				}
			}
			return res;
		}

		protected override Size MeasureContent(Rectangle rect)
		{
			var res = Size.Empty;
			_sizes.Clear();
			if(_panels.Count != 0)
			{
				var graphics = GraphicsUtility.MeasurementGraphics;
				foreach(var panel in _panels)
				{
					var size = GetPanelSize(graphics, panel, rect.Width);
					if(size.Width > res.Width)
					{
						res.Width = size.Width;
					}
					res.Height += size.Height;
				}
			}
			return res;
		}

		protected override int GetVScrollSmallChange()
		{
			return 16;
		}

		protected override void UpdateHover(int x, int y)
		{
			if(_mouseDownPanel != null && _mouseDownPanel.FlowControl == this)
			{
				int panelY = ClientArea.Y - VScrollPos;
				for(int i = 0; i < _panels.Count; ++i)
				{
					var panel = _panels[i];
					var size = GetPanelSize(panel);
					int maxY = panelY + size.Height;
					if(panel == _mouseDownPanel)
					{
						_panelHover.Track(i, panel);
						panel.MouseMove(x - ClientArea.X + HScrollPos, y - panelY);
						break;
					}
					panelY = maxY;
				}
			}
			else
			{
				if(x >= ClientArea.X && x < ClientArea.Right)
				{
					bool hover = false;
					Graphics graphics = null;
					int panelY = ClientArea.Y - VScrollPos;
					for(int i = 0; i < _panels.Count; ++i)
					{
						var panel = _panels[i];
						var size = GetPanelSize(panel);
						int maxY = panelY + size.Height;
						if(maxY >= y)
						{
							hover = true;
							_panelHover.Track(i, panel);
							panel.MouseMove(x - ClientArea.X + HScrollPos, y - panelY);
							break;
						}
						if(maxY >= ClientRectangle.Bottom)
						{
							break;
						}
						panelY = maxY;
					}
					if(!hover)
					{
						_panelHover.Drop();
					}
					if(graphics != null)
					{
						graphics.Dispose();
					}
				}
				else
				{
					_panelHover.Drop();
				}
			}
			base.UpdateHover(x, y);
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			e.IsInputKey = true;
			switch(e.KeyCode)
			{
				case Keys.Home:
					VScrollBar.Value = 0;
					break;
				case Keys.End:
					VScrollBar.Value = MaxVScrollPos;
					break;
				case Keys.PageUp:
					{
						var pos = VScrollPos - ClientRectangle.Height;
						if(pos < 0) pos = 0;
						VScrollBar.Value = pos;
					}
					break;
				case Keys.PageDown:
					{
						var pos = VScrollPos + ClientRectangle.Height;
						if(pos > MaxVScrollPos) pos = MaxVScrollPos;
						VScrollBar.Value = pos;
					}
					break;
				case Keys.Up:
					{
						var pos = VScrollPos - GetVScrollSmallChange();
						if(pos < 0) pos = 0;
						VScrollBar.Value = pos;
					}
					break;
				case Keys.Down:
					{
						var pos = VScrollPos + GetVScrollSmallChange();
						if(pos > MaxVScrollPos) pos = MaxVScrollPos;
						VScrollBar.Value = pos;
					}
					break;
				default:
					e.IsInputKey = false;
					break;
			}
			base.OnPreviewKeyDown(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_panelHover.Drop();
			base.OnMouseLeave(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			UpdateHover(e.X, e.Y);
			base.OnMouseMove(e);
		}

		protected virtual void OnPanelMouseDown(FlowPanel panel, int x, int y, MouseButtons button)
		{
			panel.MouseDown(x, y, button);
		}

		protected virtual void OnFreeSpaceMouseDown(int x, int y, MouseButtons button)
		{
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			int x = e.X;
			int y = e.Y;
			int panelY = ClientArea.Y - VScrollPos;
			_mouseDownPanel = null;
			bool found = false;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var panel = _panels[i];
				var size = GetPanelSize(panel);
				int maxY = panelY + size.Height;
				if(maxY >= y)
				{
					_mouseDownPanel = panel;
					_panelHover.Track(i, panel);
					OnPanelMouseDown(panel, x - ClientArea.X + HScrollPos, y - panelY, e.Button);
					found = true;
					break;
				}
				if(maxY >= ClientRectangle.Bottom)
				{
					break;
				}
				panelY = maxY;
			}
			if(!found)
			{
				OnFreeSpaceMouseDown(x, y, e.Button);
			}
			Focus();
			base.OnMouseDown(e);
		}

		protected virtual void OnPanelDoubleClick(FlowPanel panel, int x, int y, MouseButtons button)
		{
			panel.DoubleClick(x, y, button);
		}

		protected virtual void OnFreeSpaceDoubleClick(int x, int y, MouseButtons button)
		{
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			int x = e.X;
			int y = e.Y;
			int panelY = ClientArea.Y - VScrollPos;
			_mouseDownPanel = null;
			bool found = false;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var panel = _panels[i];
				var size = GetPanelSize(panel);
				int maxY = panelY + size.Height;
				if(maxY >= y)
				{
					_mouseDownPanel = panel;
					_panelHover.Track(i, panel);
					OnPanelDoubleClick(panel, x - ClientArea.X + HScrollPos, y - panelY, e.Button);
					found = true;
					break;
				}
				if(maxY >= ClientRectangle.Bottom)
				{
					break;
				}
				panelY = maxY;
			}
			if(!found)
			{
				OnFreeSpaceDoubleClick(x, y, e.Button);
			}
			Focus();
			base.OnMouseDoubleClick(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if(_mouseDownPanel != null)
			{
				if(_mouseDownPanel.FlowControl == this)
				{
					int x = e.X;
					int y = e.Y;
					var bounds = GetPanelBounds(_mouseDownPanel);
					_mouseDownPanel.MouseUp(x - ClientArea.X + HScrollPos, y - bounds.Y + VScrollPos, e.Button);
				}
				_mouseDownPanel = null;
			}
			base.OnMouseUp(e);
		}

		protected override void OnResize(EventArgs e)
		{
			_sizes.Clear();
			base.OnResize(e);
		}

		protected override void OnPaintClientArea(PaintEventArgs e)
		{
			var clientArea = ClientArea;
			int y = clientArea.Y - VScrollPos;
			var graphics = e.Graphics;
			var clip = Rectangle.Intersect(clientArea, e.ClipRectangle);
			int clippingEdge = clip.Bottom;
			graphics.SetClip(clip);
			graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
			graphics.TextContrast = GraphicsUtility.TextContrast;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var panel = _panels[i];
				var size = GetPanelSize(graphics, panel);
				if(size.Width == 0) size.Width = ContentSize.Width;
				int maxY = y + size.Height;
				if(maxY >= ContentArea.Y)
				{
					if(y < ContentArea.Bottom)
					{
						if(maxY >= clip.Y && y < clippingEdge)
						{
							var bounds = new Rectangle(clientArea.X - HScrollPos, y, size.Width, size.Height);
							var extBounds = bounds;
							extBounds.Width = Math.Max(ClientArea.Width, size.Width);
							var panelClip = Rectangle.Intersect(clip, extBounds);
							if(panelClip.Width > 0 && panelClip.Height > 0)
							{
								graphics.SetClip(panelClip);
								panel.Paint(new FlowPanelPaintEventArgs(graphics, panelClip, bounds));
							}
						}
					}
					else
					{
						break;
					}
				}
				y = maxY;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_panels.Clear();
			}
			base.Dispose(disposing);
		}
	}
}

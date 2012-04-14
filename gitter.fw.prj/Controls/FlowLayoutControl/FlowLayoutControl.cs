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
			if(panel == null) throw new ArgumentNullException("panel");
			if(panel.FlowControl != this) throw new ArgumentException("panel");

			using(var graphics = CreateGraphics())
			{
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
			}
			throw new ArgumentException("panel");
		}

		internal void InvalidatePanel(FlowPanel panel, Rectangle rect)
		{
			using(var graphics = CreateGraphics())
			{
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
							Invalidate(new Rectangle(x + rect.X, y + rect.Y, rect.Width, rect.Height));
						break;
					}
					if(maxY >= ClientArea.Bottom)
						break;
					y = maxY;
				}
			}
		}

		internal void InvalidatePanel(FlowPanel panel)
		{
			using(var graphics = CreateGraphics())
			{
				int y = ClientArea.Y - VScrollPos;
				for(int i = 0; i < _panels.Count; ++i)
				{
					var p = _panels[i];
					var size = GetPanelSize(graphics, p);
					int maxY = y + size.Height;
					if(p == panel)
					{
						if(maxY >= ClientArea.Y)
							Invalidate(new Rectangle(ClientArea.X, y, ClientArea.Width, size.Height));
						break;
					}
					if(maxY >= ClientArea.Bottom)
						break;
					y = maxY;
				}
			}
		}

		internal void ScrollIntoView(FlowPanel p)
		{
			Graphics graphics = null;
			int panelY = 0;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var panel = _panels[i];
				var size = GetPanelSize(ref graphics, panel);
				int maxY = panelY + size.Height;
				if(p == panel)
				{
					if(panelY > MaxVScrollPos)
						panelY = MaxVScrollPos;
					VScrollBar.Value = panelY;
					break;
				}
				panelY = maxY;
			}
			if(graphics != null)
				graphics.Dispose();
		}

		protected override void EndUpdate(bool refresh)
		{
			if(UpdateCounter == 1 && refresh)
				NotifyContentSizeChanged();
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
				e.Item.MouseEnter();
			else
				e.Item.MouseLeave();
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
							_panelHover.Drop();
						else
							_panelHover.ResetIndex(_panelHover.Index - e.ModifiedItems);
					}
					break;
				case NotifyEvent.Set:
					if(_panelHover.Index >= e.StartIndex)
					{
						if(_panelHover.Index <= e.EndIndex)
							_panelHover.Drop();
						else
							_panelHover.ResetIndex(_panelHover.Index - e.ModifiedItems);
					}
					break;
			}
		}

		private void OnPanelsChanged(object sender, NotifyCollectionEventArgs e)
		{
			_sizes.Clear();
			if(!IsUpdating)
				NotifyContentSizeChanged();
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

		private Size GetPanelSize(ref Graphics graphics, FlowPanel panel)
		{
			Size size;
			if(!_sizes.TryGetValue(panel, out size))
			{
				if(graphics == null)
					graphics = CreateGraphics();
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
				using(var graphics = CreateGraphics())
				{
					foreach(var panel in _panels)
					{
						var size = GetPanelSize(graphics, panel);
						if(size.Width > res.Width)
							res.Width = size.Width;
						res.Height += size.Height;
					}
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
				using(var graphics = CreateGraphics())
				{
					foreach(var panel in _panels)
					{
						var size = GetPanelSize(graphics, panel, rect.Width);
						if(size.Width > res.Width)
							res.Width = size.Width;
						res.Height += size.Height;
					}
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
				Graphics graphics = null;
				int panelY = ClientArea.Y - VScrollPos;
				for(int i = 0; i < _panels.Count; ++i)
				{
					var panel = _panels[i];
					var size = GetPanelSize(ref graphics, panel);
					int maxY = panelY + size.Height;
					if(panel == _mouseDownPanel)
					{
						_panelHover.Track(i, panel);
						panel.MouseMove(x - ClientArea.X + HScrollPos, y - panelY);
						break;
					}
					panelY = maxY;
				}
				if(graphics != null)
					graphics.Dispose();
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
						var size = GetPanelSize(ref graphics, panel);
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
			Graphics graphics = null;
			int panelY = ClientArea.Y - VScrollPos;
			_mouseDownPanel = null;
			bool found = false;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var panel = _panels[i];
				var size = GetPanelSize(ref graphics, panel);
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
					break;
				panelY = maxY;
			}
			if(graphics != null)
				graphics.Dispose();
			if(!found)
				OnFreeSpaceMouseDown(x, y, e.Button);
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
			Graphics graphics = null;
			int panelY = ClientArea.Y - VScrollPos;
			_mouseDownPanel = null;
			bool found = false;
			for(int i = 0; i < _panels.Count; ++i)
			{
				var panel = _panels[i];
				var size = GetPanelSize(ref graphics, panel);
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
					break;
				panelY = maxY;
			}
			if(graphics != null)
				graphics.Dispose();
			if(!found)
				OnFreeSpaceDoubleClick(x, y, e.Button);
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
			int endy = e.ClipRectangle.Bottom;
			var graphics = e.Graphics;
			var clip = Rectangle.Intersect(ClientArea, e.ClipRectangle);
			graphics.SetClip(clip);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.TextRenderingHint = Utility.TextRenderingHint;
			graphics.TextContrast = Utility.TextContrast;
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
						if(maxY >= clip.Y && y < clip.Bottom)
						{
							var bounds = new Rectangle(clientArea.X - HScrollPos, y, size.Width, size.Height);
							panel.Paint(new FlowPanelPaintEventArgs(
								graphics, clip, bounds));
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

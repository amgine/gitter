namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public abstract class FlowPanel
	{
		private FlowLayoutControl _flowControl;

		/// <summary>Create <see cref="FlowPanel"/>.</summary>
		protected FlowPanel()
		{
		}

		/// <summary>Host <see cref="FlowLayoutControl"/>.</summary>
		public FlowLayoutControl FlowControl
		{
			get { return _flowControl; }
			internal set
			{
				if(_flowControl != value)
				{
					if(_flowControl != null)
					{
						OnFlowControlDetached();
					}
					_flowControl = value;
					if(_flowControl != null)
					{
						OnFlowControlAttached();
					}
				}
			}
		}

		protected IGitterStyle Style
		{
			get
			{
				if(_flowControl != null)
				{
					return _flowControl.Style;
				}
				else
				{
					return GitterApplication.DefaultStyle;
				}
			}
		}

		public virtual FlowPanelHeader Header
		{
			get { return null; }
		}

		public Rectangle Bounds
		{
			get
			{
				if(FlowControl == null) return Rectangle.Empty;
				return FlowControl.GetPanelBounds(this);
			}
		}

		public virtual void InvalidateSize()
		{
			if(_flowControl != null)
			{
				_flowControl.InvalidatePanelSize(this);
			}
		}

		public void Invalidate()
		{
			if(FlowControl != null)
			{
				FlowControl.InvalidatePanel(this);
			}
		}

		public void Invalidate(Rectangle rect)
		{
			if(FlowControl != null)
			{
				FlowControl.InvalidatePanel(this, rect);
			}
		}

		public void InvalidateSafe()
		{
			var control = FlowControl;
			if(control != null && FlowControl.Created)
			{
				if(control.InvokeRequired)
				{
					control.BeginInvoke(new Action(Invalidate), null);
				}
				else
				{
					Invalidate();
				}
			}
		}

		public void InvalidateSafe(Rectangle rect)
		{
			var control = FlowControl;
			if(control != null && FlowControl.Created)
			{
				if(control.InvokeRequired)
				{
					control.BeginInvoke(new Action<Rectangle>(Invalidate), rect);
				}
				else
				{
					Invalidate(rect);
				}
			}
		}

		public void ScrollIntoView()
		{
			FlowControl.ScrollIntoView(this);
		}

		public void Remove()
		{
			Verify.State.IsTrue(FlowControl != null);

			FlowControl.Panels.Remove(this);
		}

		public void RemoveSafe()
		{
			var control = FlowControl;
			Verify.State.IsTrue(control != null);

			control.BeginInvoke(new Func<FlowPanel, bool>(control.Panels.Remove), new object[] { this });
		}

		protected virtual void OnFlowControlAttached()
		{
		}

		protected virtual void OnFlowControlDetached()
		{
		}

		protected abstract Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs);

		protected abstract void OnPaint(FlowPanelPaintEventArgs paintEventArgs);

		protected virtual void OnMouseEnter()
		{
		}

		protected virtual void OnMouseLeave()
		{
		}

		protected virtual void OnMouseMove(int x, int y)
		{
		}

		protected virtual void OnMouseDown(int x, int y, MouseButtons button)
		{
		}

		protected virtual void OnMouseUp(int x, int y, MouseButtons button)
		{
		}

		protected virtual void OnMouseDoubleClick(int x, int y, MouseButtons button)
		{
		}

		public void ShowContextMenu(ContextMenuStrip menu, int x, int y)
		{
			Verify.Argument.IsNotNull(menu, "menu");
			Verify.State.IsTrue(FlowControl != null);

			var bounds = FlowControl.GetPanelBounds(this);
			menu.Show(FlowControl,
				bounds.X + x - FlowControl.HScrollPos,
				bounds.Y + y - FlowControl.VScrollPos);
		}

		internal void MouseEnter()
		{
			OnMouseEnter();
		}

		internal void MouseLeave()
		{
			OnMouseLeave();
		}

		internal void MouseMove(int x, int y)
		{
			OnMouseMove(x, y);
		}

		internal void MouseDown(int x, int y, MouseButtons button)
		{
			OnMouseDown(x, y, button);
		}

		internal void DoubleClick(int x, int y, MouseButtons button)
		{
			OnMouseDoubleClick(x, y, button);
		}

		internal void MouseUp(int x, int y, MouseButtons button)
		{
			OnMouseUp(x, y, button);
		}

		public Size Measure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			return OnMeasure(measureEventArgs);
		}

		public void Paint(FlowPanelPaintEventArgs paintEventArgs)
		{
			OnPaint(paintEventArgs);
		}
	}
}

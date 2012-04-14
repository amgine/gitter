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
						OnFlowControlDetached();
					_flowControl = value;
					if(_flowControl != null)
						OnFlowControlAttached();
				}
			}
		}

		public virtual void InvalidateSize()
		{
			if(_flowControl != null)
				_flowControl.InvalidatePanelSize(this);
		}

		public void Invalidate()
		{
			if(FlowControl != null)
				FlowControl.InvalidatePanel(this);
		}

		public void Invalidate(Rectangle rect)
		{
			if(FlowControl != null)
				FlowControl.InvalidatePanel(this, rect);
		}

		public void InvalidateSafe()
		{
			var control = FlowControl;
			if(control != null && FlowControl.Created)
			{
				control.BeginInvoke(new Action(Invalidate), null);
			}
		}

		public void ScrollIntoView()
		{
			FlowControl.ScrollIntoView(this);
		}

		public void Remove()
		{
			if(FlowControl == null) throw new InvalidOperationException();
			FlowControl.Panels.Remove(this);
		}

		public void RemoveSafe()
		{
			var control = FlowControl;
			if(control == null) throw new InvalidOperationException();
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
			if(menu == null) throw new ArgumentNullException("menu");
			if(FlowControl == null) throw new InvalidOperationException();

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

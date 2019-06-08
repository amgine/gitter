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

		public virtual FlowPanelHeader Header => null;

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
			if(control != null && control.Created && !control.IsDisposed)
			{
				if(control.InvokeRequired)
				{
					try
					{
						control.BeginInvoke(new MethodInvoker(Invalidate), null);
					}
					catch(ObjectDisposedException)
					{
					}
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
			if(control != null && control.Created && !control.IsDisposed)
			{
				if(control.InvokeRequired)
				{
					try
					{
						control.BeginInvoke(new Action<Rectangle>(Invalidate), rect);
					}
					catch(ObjectDisposedException)
					{
					}
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
			Verify.Argument.IsNotNull(menu, nameof(menu));
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

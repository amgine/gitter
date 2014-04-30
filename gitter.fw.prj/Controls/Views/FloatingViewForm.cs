#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.Windows.Forms;
	using System.Drawing;

	using gitter.Native;

	/// <summary>Hosts floating views.</summary>
	internal sealed class FloatingViewForm : Form
	{
		#region Data

		private readonly ViewDockGrid _dockGrid;
		private bool _isInMulticontrolMode;
		private Control _rootControl;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="FloatingViewForm"/> class.</summary>
		/// <param name="viewHost">Floating <see cref="ViewHost"/>.</param>
		public FloatingViewForm(ViewDockGrid dockGrid, ViewHost viewHost)
		{
			Verify.Argument.IsNotNull(dockGrid, "dockGrid");
			Verify.Argument.IsNotNull(viewHost, "viewHost");

			Font			= GitterApplication.FontManager.UIFont;
			Text			= viewHost.Text;
			FormBorderStyle	= FormBorderStyle.None;
			BackColor		= Renderer.BackgroundColor;
			StartPosition	= FormStartPosition.Manual;
			Padding			= new Padding(Renderer.FloatBorderSize);
			Bounds			= GetBoundsForControl(viewHost);
			if(viewHost.Width < ViewConstants.MinimumHostWidth)
			{
				if(viewHost.Height < ViewConstants.MinimumHostHeight)
				{
					viewHost.Size = new Size(ViewConstants.MinimumHostWidth, ViewConstants.MinimumHostHeight);
				}
				else
				{
					viewHost.Width = ViewConstants.MinimumHostWidth;
				}
			}
			else if(viewHost.Height < ViewConstants.MinimumHostHeight)
			{
				viewHost.Height = ViewConstants.MinimumHostHeight;
			};
			MinimumSize		= new Size(
				ViewConstants.MinimumHostWidth + Renderer.FloatBorderSize * 2,
				ViewConstants.MinimumHostHeight + Renderer.FloatBorderSize * 2);
			ShowInTaskbar	= false;
			ShowIcon		= false;
			ControlBox		= false;
			MinimizeBox		= false;
			MaximizeBox		= true;
			_rootControl	= viewHost;
			_dockGrid		= dockGrid;

			_dockGrid.AddFloatingForm(this);
		}

		private ViewRenderer Renderer
		{
			get { return ViewManager.Renderer; }
		}

		private Rectangle GetBoundsForControl(Control control)
		{
			var loc = control.PointToScreen(Point.Empty);
			var size = control.Size;
			loc.X -= Renderer.FloatBorderSize;
			loc.Y -= Renderer.FloatBorderSize;
			size.Width += Renderer.FloatBorderSize * 2;
			size.Height += Renderer.FloatBorderSize * 2;
			return new Rectangle(loc, size);
		}

		internal bool IsInMulticontrolMode
		{
			get { return _isInMulticontrolMode; }
		}

		internal void EnterMulticontrolMode()
		{
			Verify.State.IsFalse(IsInMulticontrolMode);

			if(WindowState == FormWindowState.Maximized)
			{
				Padding = new Padding(
					0,
					0 + Renderer.FloatTitleHeight,
					0,
					0);
			}
			else
			{
				Padding = new Padding(
					Renderer.FloatBorderSize,
					Renderer.FloatBorderSize + Renderer.FloatTitleHeight,
					Renderer.FloatBorderSize,
					Renderer.FloatBorderSize);
			}
			_isInMulticontrolMode = true;
		}

		internal void LeaveMulticontrolMode()
		{
			Verify.State.IsTrue(IsInMulticontrolMode);

			if(WindowState == FormWindowState.Maximized)
			{
				Padding = new Padding(
					0,
					0,
					0,
					0);
			}
			else
			{
				Padding = new Padding(
					Renderer.FloatBorderSize,
					Renderer.FloatBorderSize,
					Renderer.FloatBorderSize,
					Renderer.FloatBorderSize);
			}
			_isInMulticontrolMode = false;
		}

		public Control RootControl
		{
			get { return _rootControl; }
			internal set { _rootControl = value; }
		}

		/// <summary>
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Windows.Forms.CreateParams"/> that contains the required creation parameters when the handle to the control is created.</returns>
		protected override CreateParams CreateParams
		{
			get
			{
				//const int WS_SIZEBOX = 0x00040000;
				const int WS_EX_TOOLWINDOW = 0x80;
				var cp = base.CreateParams;
				//cp.Style |= WS_SIZEBOX;
				cp.ExStyle |= WS_EX_TOOLWINDOW;
				return cp;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
		protected override void WndProc(ref Message m)
		{
			bool processed = false;
			switch((WM)m.Msg)
			{
				case WM.NCHITTEST:
					processed = OnNcHitTest(ref m);
					break;
			}
			if(!processed)
			{
				base.WndProc(ref m);
			}
		}

		private bool OnNcHitTest(ref Message m)
		{
			int x = NativeUtility.LOWORD(m.LParam);
			int y = NativeUtility.HIWORD(m.LParam);

			var point = PointToClient(new Point(x, y));
			var rc = ClientRectangle;
			bool isMaximized = WindowState == FormWindowState.Maximized;

			if(_isInMulticontrolMode)
			{
				if(isMaximized)
				{
					if((new Rectangle(
						rc.X, rc.Y,
						rc.Width, Renderer.FloatTitleHeight)).Contains(point))
					{
						m.Result = (IntPtr)Constants.HTCAPTION;
						return true;
					}
				}
				else
				{
					if((new Rectangle(
						rc.X + Renderer.FloatBorderSize,
						rc.Y + Renderer.FloatBorderSize,
						rc.Width - Renderer.FloatBorderSize * 2,
						Renderer.FloatTitleHeight)).Contains(point))
					{
						m.Result = (IntPtr)Constants.HTCAPTION;
						return true;
					}
				}
			}

			if(isMaximized) return false;

			var grip = new GripBounds(rc);
			if(grip.TopLeft.Contains(point))
			{
				m.Result = (IntPtr)Constants.HTTOPLEFT;
				return true;
			}
			if(grip.TopRight.Contains(point))
			{
				m.Result = (IntPtr)Constants.HTTOPRIGHT;
				return true;
			}
			if(grip.Top.Contains(point))
			{
				m.Result = (IntPtr)Constants.HTTOP;
				return true;
			}
			if(grip.BottomLeft.Contains(point))
			{
				m.Result = (IntPtr)Constants.HTBOTTOMLEFT;
				return true;
			}
			if(grip.BottomRight.Contains(point))
			{
				m.Result = (IntPtr)Constants.HTBOTTOMRIGHT;
				return true;
			}
			if(grip.Bottom.Contains(point))
			{
				m.Result = (IntPtr)Constants.HTBOTTOM;
				return true;
			}
			if(grip.Left.Contains(point))
			{
				m.Result = (IntPtr)Constants.HTLEFT;
				return true;
			}
			if(grip.Right.Contains(point))
			{
				m.Result = (IntPtr)Constants.HTRIGHT;
				return true;
			}
			return false;
		}

		/// <summary>
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnStyleChanged(EventArgs e)
		{
			MaximizedBounds = Screen.GetWorkingArea(this);
			base.OnStyleChanged(e);
		}

		/// <summary>
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnResize(EventArgs e)
		{
			if(WindowState == FormWindowState.Maximized)
			{
				if(_isInMulticontrolMode)
				{
					Padding = new Padding(
						0,
						0 + Renderer.FloatTitleHeight,
						0,
						0);
				}
				else
				{
					Padding = new Padding(0);
				}
				Region = null;
			}
			else
			{
				if(_isInMulticontrolMode)
				{
					Padding = new Padding(
						Renderer.FloatBorderSize,
						Renderer.FloatBorderSize + Renderer.FloatTitleHeight,
						Renderer.FloatBorderSize,
						Renderer.FloatBorderSize);
				}
				else
				{
					Padding = new Padding(Renderer.FloatBorderSize);
				}
				var cornderRadius = Renderer.FloatCornerRadius;
				if(cornderRadius != 0)
				{
					Region = GraphicsUtility.GetRoundedRegion(ClientRectangle, Renderer.FloatCornerRadius);
				}
			}
			base.OnResize(e);
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the <see cref="T:FloatingViewForm"/>.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_dockGrid.RemoveFloatingForm(this);
				_rootControl = null;
			}
			base.Dispose(disposing);
		}
	}
}

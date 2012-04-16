namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Runtime.InteropServices;

	[ToolboxItem(true)]
	public partial class CustomPopupComboBox : ComboBox
	{
		#region Data

		private Popup _dropDown;
		private Control _dropDownControl;
		private DateTime _dropDownHideTime;

		#endregion

		#region .ctor

		public CustomPopupComboBox()
		{
			_dropDownHideTime = DateTime.UtcNow;
			base.IntegralHeight = false;
			base.DropDownHeight = 1;
			base.DropDownWidth = 1;

			_listBoxWndProc = new NativeMethods.WNDPROC(ListBoxWndProc);
		}

		#endregion

		#region Overrides

		private readonly NativeMethods.WNDPROC _listBoxWndProc;
		private IntPtr _listBoxDefaultWndProc;

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			var x = new NativeMethods.COMBOBOXINFO();
			x.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.COMBOBOXINFO));
			var b = NativeMethods.GetComboBoxInfo(Handle, ref x);
			if(IntPtr.Size == 8)
			{
				_listBoxDefaultWndProc = NativeMethods.SetWindowLongPtr(
					x.hwndList, NativeMethods.GWLP_WNDPROC, _listBoxWndProc);
			}
			else
			{
				_listBoxDefaultWndProc = NativeMethods.SetWindowLong(
					x.hwndList, NativeMethods.GWLP_WNDPROC, _listBoxWndProc);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if(DropDown != null)
				DropDown.Width = Width;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_dropDown != null)
				{
					_dropDown.Closed -= dropDown_Closed;
					_dropDown.Dispose();
					_dropDown = null;
				}
			}
			base.Dispose(disposing);
		}

		private IntPtr ListBoxWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			switch((WindowsMessage)msg)
			{
				case WindowsMessage.WM_MOUSEMOVE:
					NativeMethods.ReleaseCapture();
					break;
				case WindowsMessage.WM_CAPTURECHANGED:
					return IntPtr.Zero;
			}
			return NativeMethods.CallWindowProc(_listBoxDefaultWndProc, hWnd, msg, wParam, lParam);
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				case ((int)WindowsMessage.WM_COMMAND + (int)WindowsMessage.WM_REFLECT):
					{
						switch(NativeMethods.HIWORD(m.WParam))
						{
							case NativeMethods.CBN_DROPDOWN:
								ShowDropDownCore();
								return;
						}
					}
					break;
			}
			base.WndProc(ref m);
		}

		#endregion

		protected new Popup DropDown
		{
			get { return _dropDown; }
		}

		public Control DropDownControl
		{
			get { return _dropDownControl; }
			set
			{
				if(_dropDownControl != value)
				{
					_dropDownControl = value;
					if(_dropDown != null)
					{
						_dropDown.Closed -= dropDown_Closed;
						_dropDown.Dispose();
					}
					_dropDown = new Popup(value)
					{
						PopupAnimation = PopupAnimations.Slide | PopupAnimations.TopToBottom,
					};
					_dropDown.Closed += dropDown_Closed;
				}
			}
		}

		private void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			_dropDownHideTime = DateTime.UtcNow;
			NativeMethods.ShowDropDown(Handle, false);
		}

		public new bool DroppedDown
		{
			get { return _dropDown != null?_dropDown.Visible:false; }
			set
			{
				if(value)
					ShowDropDown();
				else
					HideDropDown();
			}
		}

		private void ShowDropDownCore()
		{
			if(_dropDown != null)
			{
				_dropDown.Show(this);
			}
		}

		private void HideDropDownCore()
		{
			if(_dropDown != null)
			{
				_dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
			}
		}

		public void ShowDropDown()
		{
			NativeMethods.ShowDropDown(Handle, true);
		}

		public void HideDropDown()
		{
			HideDropDownCore();
		}

		#region Disabled Properties

		/// <summary>This property is not relevant for this class.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new int DropDownWidth
		{
			get { return base.DropDownWidth; }
			set { base.DropDownWidth = value; }
		}

		/// <summary>This property is not relevant for this class.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual new int DropDownHeight
		{
			get { return base.DropDownHeight; }
			set { base.DropDownHeight = value; }
		}

		/// <summary>This property is not relevant for this class.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new bool IntegralHeight
		{
			get { return base.IntegralHeight; }
			set { base.IntegralHeight = value; }
		}

		/// <summary>This property is not relevant for this class.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new ObjectCollection Items
		{
			get { return base.Items; }
		}

		/// <summary>This property is not relevant for this class.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new int ItemHeight
		{
			get { return base.ItemHeight; }
			set { base.ItemHeight = value; }
		}

		#endregion
	}
}

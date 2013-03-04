namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Runtime.InteropServices;

	using gitter.Native;

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

			_listBoxWndProc = new WNDPROC(ListBoxWndProc);
		}

		#endregion

		#region Overrides

		private readonly WNDPROC _listBoxWndProc;
		private IntPtr _listBoxDefaultWndProc;

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			var x = new COMBOBOXINFO();
			x.cbSize = (uint)Marshal.SizeOf(typeof(COMBOBOXINFO));
			var b = User32.GetComboBoxInfo(Handle, ref x);
			_listBoxDefaultWndProc = NativeUtility.SetWindowProc(x.hwndList, _listBoxWndProc);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if(DropDown != null)
			{
				DropDown.Width = Width;
			}
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
			switch((WM)msg)
			{
				case WM.MOUSEMOVE:
					User32.ReleaseCapture();
					break;
				case WM.CAPTURECHANGED:
					return IntPtr.Zero;
			}
			return User32.CallWindowProc(_listBoxDefaultWndProc, hWnd, msg, wParam, lParam);
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				case ((int)WM.COMMAND + (int)WM.REFLECT):
					{
						switch(NativeUtility.HIWORD(m.WParam))
						{
							case Constants.CBN_DROPDOWN:
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
			NativeUtility.ShowDropDown(Handle, false);
		}

		public new bool DroppedDown
		{
			get { return _dropDown != null?_dropDown.Visible:false; }
			set
			{
				if(value)
				{
					ShowDropDown();
				}
				else
				{
					HideDropDown();
				}
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
			NativeUtility.ShowDropDown(Handle, true);
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

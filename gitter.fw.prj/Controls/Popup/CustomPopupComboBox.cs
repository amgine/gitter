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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using gitter.Native;

[ToolboxItem(true)]
[DesignerCategory("")]
public partial class CustomPopupComboBox : ComboBox
{
	#region Data

	private Popup? _dropDown;
	private Control? _dropDownControl;
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

	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		var x = new COMBOBOXINFO
		{
			cbSize = (uint)Marshal.SizeOf<COMBOBOXINFO>()
		};
		var b = User32.GetComboBoxInfo(Handle, ref x);
		_listBoxDefaultWndProc = NativeUtility.SetWindowProc(x.hwndList, _listBoxWndProc);
	}

	/// <inheritdoc/>
	protected override void OnSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		if(DropDown is not null)
		{
			DropDown.Width = Width;
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_dropDown is not null)
			{
				_dropDown.Closed -= OnDropDownClosed;
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

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		switch(m.Msg)
		{
			case ((int)WM.COMMAND + (int)WM.REFLECT):
				{
					switch(Macro.HIWORD(m.WParam))
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

	protected new Popup? DropDown => _dropDown;

	public Control? DropDownControl
	{
		get => _dropDownControl;
		set
		{
			if(_dropDownControl == value) return;

			_dropDownControl = value;
			if(_dropDown is not null)
			{
				_dropDown.Closed -= OnDropDownClosed;
				_dropDown.Dispose();
				_dropDown = default;
			}
			if(value is not null)
			{
				_dropDown = new Popup(value)
				{
					PopupAnimation = PopupAnimations.Slide | PopupAnimations.TopToBottom,
				};
				_dropDown.Closed += OnDropDownClosed;
			}
		}
	}

	private void OnDropDownClosed(object? sender, ToolStripDropDownClosedEventArgs e)
	{
		_dropDownHideTime = DateTime.UtcNow;
		NativeUtility.ShowDropDown(Handle, false);
	}

	public new bool DroppedDown
	{
		get => _dropDown is { Visible: true };
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
		_dropDown?.Show(this);
	}

	private void HideDropDownCore()
	{
		_dropDown?.Close(ToolStripDropDownCloseReason.ItemClicked);
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
		get => base.DropDownWidth;
		set => base.DropDownWidth = value;
	}

	/// <summary>This property is not relevant for this class.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual new int DropDownHeight
	{
		get => base.DropDownHeight;
		set => base.DropDownHeight = value;
	}

	/// <summary>This property is not relevant for this class.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool IntegralHeight
	{
		get => base.IntegralHeight;
		set => base.IntegralHeight = value;
	}

	/// <summary>This property is not relevant for this class.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ObjectCollection Items => base.Items;

	/// <summary>This property is not relevant for this class.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new int ItemHeight
	{
		get => base.ItemHeight;
		set => base.ItemHeight = value;
	}

	#endregion
}

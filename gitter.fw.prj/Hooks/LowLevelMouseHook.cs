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

namespace gitter.Framework.Hooks
{
	using System;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;
	using gitter.Native;

	public class LowLevelMouseHook : WindowsHook
	{
		public event EventHandler<MouseEventArgs> MouseWheel;

		public event EventHandler<MouseEventArgs> MouseMove;

		protected virtual void OnMouseWheel(MouseEventArgs args)
		{
			var handler = MouseWheel;
			if(handler != null) handler(this, args);
		}

		protected virtual void OnMouseMove(MouseEventArgs args)
		{
			var handler = MouseMove;
			if(handler != null) handler(this, args);
		}

		public LowLevelMouseHook()
			: base(Native.WH.MOUSE_LL)
		{
		}

		protected override void HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			var msg  = (WM)(wParam.ToInt32());
			var info = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

			switch(msg)
			{
				case WM.MOUSEWHEEL:
					OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, info.pt.X, info.pt.Y, info.mouseData >> 16));
					break;
				case WM.MOUSEMOVE:
					OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, info.pt.X, info.pt.Y, 0));
					break;
			}
		}
	}
}

namespace gitter.Native
{
	using System;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;

	internal static class NativeUtility
	{
		public static IntPtr SetWindowProc(IntPtr hwnd, WNDPROC newWndProc)
		{
			const int GWLP_WNDPROC = -4;

			if(IntPtr.Size == 8)
			{
				return User32.SetWindowLongPtr(
					hwnd, GWLP_WNDPROC, newWndProc);
			}
			else
			{
				return User32.SetWindowLong(
					hwnd, GWLP_WNDPROC, newWndProc);
			}
		}

		public static void ShowDropDown(IntPtr handle, bool show)
		{
			User32.SendMessage(handle, Constants.CB_SHOWDROPDOWN, (IntPtr)(show ? 1 : 0), IntPtr.Zero);
		}

		public static void AnimateWindow(Control control, int time, AnimationFlags flags)
		{
			User32.AnimateWindow(new HandleRef(control, control.Handle), time, flags);
		}

		public static int HIWORD(int n)
		{
			return (n >> 16) & 0xffff;
		}

		public static int HIWORD(IntPtr n)
		{
			return HIWORD(unchecked((int)(long)n));
		}

		public static int LOWORD(int n)
		{
			return n & 0xffff;
		}

		public static int LOWORD(IntPtr n)
		{
			return LOWORD(unchecked((int)(long)n));
		}
	}
}

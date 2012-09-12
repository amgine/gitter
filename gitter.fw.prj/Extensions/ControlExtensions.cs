namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	/// <summary>Extension methods for <see cref="System.Windows.Forms.Control"/>.</summary>
	public static class ControlExtensions
	{
		/// <summary>Disables control redrawing.</summary>
		/// <param name="control">Control to disable redraw for.</param>
		public static void DisableRedraw(this Control control)
		{
			Verify.Argument.IsNotNull(control, "control");

			NativeMethods.SendMessage(control.Handle, WindowsMessage.WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);
		}

		/// <summary>Enables control redrawing.</summary>
		/// <param name="control">Control to disable redraw for.</param>
		public static void EnableRedraw(this Control control)
		{
			Verify.Argument.IsNotNull(control, "control");

			NativeMethods.SendMessage(control.Handle, WindowsMessage.WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
		}

		/// <summary>Forces control redraw.</summary>
		/// <param name="control">Control to force-redraw.</param>
		public static void RedrawWindow(this Control control)
		{
			Verify.Argument.IsNotNull(control, "control");

			NativeMethods.RedrawWindow(control.Handle, IntPtr.Zero, IntPtr.Zero,
				NativeMethods.RedrawWindowFlags.Erase |
				NativeMethods.RedrawWindowFlags.Frame |
				NativeMethods.RedrawWindowFlags.Invalidate |
				NativeMethods.RedrawWindowFlags.AllChildren);
		}
	}
}

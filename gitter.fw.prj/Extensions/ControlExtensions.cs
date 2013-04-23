namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	using gitter.Native;

	/// <summary>Extension methods for <see cref="System.Windows.Forms.Control"/>.</summary>
	public static class ControlExtensions
	{
		public struct CursorChangeToken : IDisposable
		{
			private readonly Control _control;
			private readonly Cursor _cursor;

			internal CursorChangeToken(Control control, Cursor cursor)
			{
				_control = control;
				_cursor = cursor;
			}

			public void Dispose()
			{
				if(_control != null)
				{
					_control.Cursor = _cursor;
				}
			}
		}

		/// <summary>Disables control redrawing.</summary>
		/// <param name="control">Control to disable redraw for.</param>
		public static void DisableRedraw(this Control control)
		{
			Verify.Argument.IsNotNull(control, "control");

			User32.SendMessage(control.Handle, WM.SETREDRAW, (IntPtr)0, IntPtr.Zero);
		}

		/// <summary>Enables control redrawing.</summary>
		/// <param name="control">Control to disable redraw for.</param>
		public static void EnableRedraw(this Control control)
		{
			Verify.Argument.IsNotNull(control, "control");

			User32.SendMessage(control.Handle, WM.SETREDRAW, (IntPtr)1, IntPtr.Zero);
		}

		/// <summary>Forces control redraw.</summary>
		/// <param name="control">Control to force-redraw.</param>
		public static void RedrawWindow(this Control control)
		{
			Verify.Argument.IsNotNull(control, "control");

			User32.RedrawWindow(control.Handle, IntPtr.Zero, IntPtr.Zero,
				RedrawWindowFlags.Erase |
				RedrawWindowFlags.Frame |
				RedrawWindowFlags.Invalidate |
				RedrawWindowFlags.AllChildren);
		}

		/// <summary>Temporary changes control cursor.</summary>
		/// <param name="control">Control.</param>
		/// <param name="cursor">New cursor.</param>
		/// <returns>Cursor token, disposing which cursor is restored.</returns>
		public static CursorChangeToken ChangeCursor(this Control control, Cursor cursor)
		{
			if(control != null)
			{
				var oldCursor = control.Cursor;
				control.Cursor = cursor;
				return new CursorChangeToken(control, oldCursor);
			}
			else
			{
				return new CursorChangeToken();
			}
		}
	}
}

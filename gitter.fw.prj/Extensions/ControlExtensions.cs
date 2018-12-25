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
			Verify.Argument.IsNotNull(control, nameof(control));

			User32.SendMessage(control.Handle, WM.SETREDRAW, (IntPtr)0, IntPtr.Zero);
		}

		/// <summary>Enables control redrawing.</summary>
		/// <param name="control">Control to disable redraw for.</param>
		public static void EnableRedraw(this Control control)
		{
			Verify.Argument.IsNotNull(control, nameof(control));

			User32.SendMessage(control.Handle, WM.SETREDRAW, (IntPtr)1, IntPtr.Zero);
		}

		/// <summary>Forces control redraw.</summary>
		/// <param name="control">Control to force-redraw.</param>
		public static void RedrawWindow(this Control control)
		{
			Verify.Argument.IsNotNull(control, nameof(control));

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

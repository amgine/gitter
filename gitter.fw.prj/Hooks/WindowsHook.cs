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
	using System.ComponentModel;

	using gitter.Native;

	public abstract class WindowsHook : IDisposable
	{
		#region Data

		private readonly HookProc _hookProc;
		private readonly WH _hookType;
		private IntPtr _hook;
		private bool _isDisposed;

		#endregion

		#region .ctor

		protected WindowsHook(WH hookType)
		{
			_hookProc = OnHookTriggered;
			_hookType = hookType;
		}

		#endregion

		#region Properties

		public WH HookType
		{
			get { return _hookType; }
		}

		public bool IsActive
		{
			get { return Handle != IntPtr.Zero; }
		}

		protected IntPtr Handle
		{
			get { return _hook; }
			private set { _hook = value; }
		}

		protected HookProc HookProc
		{
			get { return _hookProc; }
		}

		#endregion

		#region Methods

		protected abstract void HookCallback(int nCode, IntPtr wParam, IntPtr lParam);

		private IntPtr OnHookTriggered(int nCode, IntPtr wParam, IntPtr lParam)
		{
			HookCallback(nCode, wParam, lParam);
			return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
		}

		public void Activate()
		{
			var threadId = Kernel32.GetCurrentThreadId();

			Activate(threadId);
		}

		public void Activate(int threadId)
		{
			Verify.State.IsFalse(IsActive, "Hook is already active.");
			Verify.State.IsFalse(IsDisposed, "Hook is disposed.");

			var module = typeof(WindowsHook).Module;
			var hInstance = Kernel32.GetModuleHandle(module.Name);

			Handle = User32.SetWindowsHookEx(HookType, HookProc, hInstance, threadId);
			if(Handle == IntPtr.Zero)
			{
				throw new Win32Exception("Failed to initialize hook.");
			}
		}

		public void Deactivate()
		{
			Verify.State.IsTrue(IsActive, "Hook is not active.");

			if(!User32.UnhookWindowsHookEx(Handle))
			{
				throw new Win32Exception("Failed to deinitialize hook.");
			}
			Handle = IntPtr.Zero;
		}

		#endregion

		#region IDisposable Members

		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		protected virtual void Dispose(bool disposing)
		{
			if(Handle != IntPtr.Zero)
			{
				User32.UnhookWindowsHookEx(Handle);
				Handle = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			if(!IsDisposed)
			{
				Dispose(true);
				GC.SuppressFinalize(this);
				IsDisposed = true;
			}
		}

		#endregion
	}
}

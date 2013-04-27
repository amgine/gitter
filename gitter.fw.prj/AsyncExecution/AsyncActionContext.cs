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
	using System.Threading;

	internal sealed class AsyncActionContext<TData> : AsyncContext
	{
		#region Data

		private readonly Action<TData, IAsyncProgressMonitor> _action;
		private readonly TData _data;

		#endregion

		#region .ctor

		internal AsyncActionContext(Action<TData, IAsyncProgressMonitor> action, TData data, IAsyncProgressMonitor monitor, bool ownsMonitor)
			: base(monitor, ownsMonitor)
		{
			_action = action;
			_data = data;
		}

		internal AsyncActionContext(Action<TData, IAsyncProgressMonitor> action, TData data, IAsyncProgressMonitor monitor, bool ownsMonitor, AsyncCallback callback, object asyncState)
			: base(monitor, ownsMonitor)
		{
			_action = action;
			_data = data;
			SetCallback(callback, asyncState);
		}

		#endregion

		/// <summary>Creates thread to execute related action.</summary>
		/// <returns>Created thread.</returns>
		protected override Thread CreateThreadCore()
		{
			return new Thread(ThreadProc)
			{
				IsBackground = true,
			};
		}

		private void ThreadProc()
		{
			try
			{
				_action(_data, Monitor);
			}
			catch(Exception exc)
			{
				Exception = exc;
			}
			finally
			{
				NotifyCompleted();
			}
		}

		/// <summary>Input data for action.</summary>
		public TData Data
		{
			get { return _data; }
		}
	}
}

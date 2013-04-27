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

	internal sealed class AsyncFuncContext<TData, TResult> : AsyncContext
	{
		#region Data

		private readonly Func<TData, IAsyncProgressMonitor, TResult> _action;
		private readonly TData _data;
		private TResult _result;

		#endregion

		#region .ctor

		internal AsyncFuncContext(Func<TData, IAsyncProgressMonitor, TResult> action, TData data, IAsyncProgressMonitor monitor, bool ownsMonitor)
			: base(monitor, ownsMonitor)
		{
			_action = action;
			_data = data;
		}

		internal AsyncFuncContext(Func<TData, IAsyncProgressMonitor, TResult> action, TData data, IAsyncProgressMonitor monitor, bool ownsMonitor, AsyncCallback callback, object asyncState)
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
				_result = _action(_data, Monitor);
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

		/// <summary>Input data for function.</summary>
		public TData Data
		{
			get { return _data; }
		}

		/// <summary>Result.</summary>
		public TResult Result
		{
			get { return _result; }
		}
	}
}

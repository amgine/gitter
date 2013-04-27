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

	/// <summary>Represents a function which can be executed sync/async with progress monitoring.</summary>
	/// <typeparam name="TResult">Function return type.</typeparam>
	public interface IAsyncFunc<out TResult>
	{
		/// <summary>Invoke synchronously without monitoring.</summary>
		/// <returns>Function return vaue.</returns>
		TResult Invoke();

		/// <summary>Invoke synchronously.</summary>
		/// <typeparam name="TMonitor">Monitor type.</typeparam>
		/// <param name="parent">Parent window reference.</param>
		/// <returns>Function return vaue.</returns>
		TResult Invoke<TMonitor>(IWin32Window parent)
			where TMonitor : IAsyncProgressMonitor, new();

		/// <summary>Invoke synchronously.</summary>
		/// <param name="parent">Parent window reference.</param>
		/// <param name="monitor">Progress monitor.</param>
		/// <returns>Function return vaue.</returns>
		TResult Invoke(IWin32Window parent, IAsyncProgressMonitor monitor);

		/// <summary>Begin async execution.</summary>
		/// <typeparam name="TMonitor">Monitor type.</typeparam>
		/// <param name="parent">Parent window reference.</param>
		/// <param name="callback">Callback to call on completion.</param>
		/// <param name="asyncState">Associated object reference.</param>
		/// <returns><see cref="IAsyncResult"/>.</returns>
		IAsyncResult BeginInvoke<TMonitor>(IWin32Window parent, AsyncCallback callback, object asyncState)
			where TMonitor : IAsyncProgressMonitor, new();

		/// <summary>Begin async execution.</summary>
		/// <param name="parent">Parent window reference.</param>
		/// <param name="monitor">Progress monitor.</param>
		/// <param name="callback">Callback to call on completion.</param>
		/// <param name="asyncState">Associated object reference.</param>
		/// <returns><see cref="IAsyncResult"/>.</returns>
		IAsyncResult BeginInvoke(IWin32Window parent, IAsyncProgressMonitor monitor, AsyncCallback callback, object asyncState);

		/// <summary>Complete async execution.</summary>
		/// <param name="asyncResult">
		/// <see cref="IAsyncResult"/>, returned by previous call to <see cref="BeginInvoke"/> or
		/// <see cref="BeginInvoke&lt;TMonitor&gt;"/>.
		/// </param>
		/// <returns>Function return vaue.</returns>
		TResult EndInvoke(IAsyncResult asyncResult);
	}
}

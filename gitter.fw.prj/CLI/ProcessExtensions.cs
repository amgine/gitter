#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.CLI;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>Extensions for <see cref="Process"/>.</summary>
public static class ProcessExtensions
{
	/// <summary>Starts <see cref="Process"/> ans represents it as a <see cref="Task"/>.</summary>
	/// <param name="process">Process to represent.</param>
	/// <returns>
	/// <see cref="Task"/>, representing <paramref name="process"/> lifetime
	/// and returning its <see cref="Process.ExitCode"/> as result.
	/// </returns>
	/// <exception cref="ArgumentNullException"><paramref name="process"/> == <c>null</c>.</exception>
	public static Task<int> StartAsync(this Process process)
	{
		Verify.Argument.IsNotNull(process);

#if NET46_OR_GREATER || NETCOREAPP
		var taskCompletionSource = new TaskCompletionSource<int>(
			TaskCreationOptions.RunContinuationsAsynchronously);
#else
		var taskCompletionSource = new TaskCompletionSource<int>();
#endif
		process.EnableRaisingEvents = true;
		process.Exited += (s, _) =>
		{
			if(taskCompletionSource.Task.IsCanceled) return;
			int exitCode;
			try
			{
				exitCode = ((Process)s).ExitCode;
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				taskCompletionSource.TrySetException(exc);
				return;
			}
			taskCompletionSource.TrySetResult(exitCode);
		};
		process.Start();
		return taskCompletionSource.Task;
	}
}

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
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public static class TaskUtility
	{
		public static Exception UnwrapException(Exception exc)
		{
			var ae = exc as AggregateException;
			if(ae != null && ae.InnerExceptions.Count == 1)
			{
				return UnwrapException(ae.InnerExceptions[0]);
			}
			return exc;
		}

		public static T UnwrapResult<T>(Task<T> task)
		{
			Assert.IsNotNull(task);

			PropagateFaultedStates(task);
			return task.Result;
		}

		public static void PropagateFaultedStates(Task task)
		{
			Assert.IsNotNull(task);

			if(task.IsCanceled)
			{
				throw new OperationCanceledException();
			}
			if(task.IsFaulted)
			{
				throw UnwrapException(task.Exception);
			}
		}

		public static Task<T> TaskFromResult<T>(T result)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult(result);
			return tcs.Task;
		}
	}
}

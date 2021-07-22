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

namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	public static class ExceptionExtensions
	{
		public static bool IsCritical(this Exception exception)
		{
			Assert.IsNotNull(exception);

			return exception
				is NullReferenceException
				or StackOverflowException
				or OutOfMemoryException
				or ThreadAbortException
				or IndexOutOfRangeException
				or AccessViolationException;
		}

		public static IEnumerable<Exception> AsEnumerable(this Exception exception)
		{
			while(exception is not null)
			{
				yield return exception;
				if(exception is AggregateException aggregate)
				{
					foreach(var innerException in aggregate.InnerExceptions)
					{
						foreach(var exc in innerException.AsEnumerable())
						{
							yield return exc;
						}
					}
				}
				else
				{
					exception = exception.InnerException;
				}
			}
		}
	}
}

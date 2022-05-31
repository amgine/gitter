#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System.Threading;

public abstract class OutputReceiverBase
{
	private readonly object _syncRoot = new();

	public abstract bool IsInitialized { get; }

	protected bool IsCanceled { get; private set; }

	protected bool IsCompleted { get; private set; }

	protected void WaitForCompleted()
	{
		if(IsCompleted) return;

		Monitor.Enter(_syncRoot);
		try
		{
			while(!IsCompleted)
			{
				Monitor.Wait(_syncRoot);
			}
		}
		finally
		{
			Monitor.Exit(_syncRoot);
		}
	}

	public void NotifyCanceled()
	{
		Verify.State.IsTrue(IsInitialized);

		if(IsCanceled) return;

		Monitor.Enter(_syncRoot);
		try
		{
			if(!IsCanceled)
			{
				IsCanceled = true;
				Monitor.PulseAll(_syncRoot);
			}
		}
		finally
		{
			Monitor.Exit(_syncRoot);
		}
	}

	protected void NotifyCompleted()
	{
		if(IsCompleted) return;

		Monitor.Enter(_syncRoot);
		try
		{
			if(!IsCompleted)
			{
				IsCompleted = true;
				Monitor.PulseAll(_syncRoot);
			}
		}
		finally
		{
			Monitor.Exit(_syncRoot);
		}
	}
}

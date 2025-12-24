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

namespace gitter.Framework;

using System;
using System.Threading;
using System.Threading.Tasks;

public abstract class AsyncDataBinding<T> : IDisposable
{
	private CancellationTokenSource? _cancellationTokenSource;
	private T? _data;

	public event EventHandler? DataChanged;

	protected virtual void OnDataChanged()
		=> DataChanged?.Invoke(this, EventArgs.Empty);

	~AsyncDataBinding() => Dispose(disposing: false);

	public T? Data
	{
		get => _data;
		private set
		{
			_data = value;
			OnDataChanged();
		}
	}

	public IProgress<OperationProgress>? Progress { get; set; }

	protected abstract Task<T> FetchDataAsync(IProgress<OperationProgress>? progress, CancellationToken cancellationToken);

	protected abstract void OnFetchCompleted(T data);

	protected abstract void OnFetchFailed(Exception exception);

	private void Restart(CancellationTokenSource? cts)
	{
		try
		{
			Interlocked.Exchange(ref _cancellationTokenSource, cts)?.Cancel(false);
		}
		catch(ObjectDisposedException)
		{
		}
	}

	private bool TryFinish(CancellationTokenSource? cts)
		=> Interlocked.CompareExchange(ref _cancellationTokenSource, null, cts) == cts;

	public async void ReloadData()
	{
		Verify.State.IsNotDisposed(IsDisposed, this);

		using var cts = new CancellationTokenSource();
		Restart(cts);
		var progress = Progress;
		T data;
		try
		{
			data = await FetchDataAsync(progress, cts.Token);
		}
		catch(OperationCanceledException)
		{
			TryFinish(cts);
			return;
		}
		catch(Exception exc)
		{
			if(TryFinish(cts))
			{
				Data = default;
				OnFetchFailed(exc);
			}
			return;
		}
		if(TryFinish(cts))
		{
			progress?.Report(OperationProgress.Completed);
			Data = data;
			OnFetchCompleted(data);
		}
	}

	public bool IsDisposed { get; private set; }

	protected virtual void Dispose(bool disposing)
	{
		Restart(null);
	}

	public void Dispose()
	{
		if(IsDisposed) return;

		GC.SuppressFinalize(this);
		Dispose(disposing: true);
		IsDisposed = true;
	}
}

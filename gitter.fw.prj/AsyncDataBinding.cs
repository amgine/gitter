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
	using System.Threading.Tasks;

	public abstract class AsyncDataBinding<T> : IDisposable
	{
		#region Data

		private CancellationTokenSource _cancellationTokenSource;
		private T _data;

		#endregion

		#region Events

		public event EventHandler DataChanged;

		protected virtual void OnDataChanged() => DataChanged?.Invoke(this, EventArgs.Empty);

		#endregion

		#region .ctor & finalizer

		public AsyncDataBinding()
		{
		}

		~AsyncDataBinding()
		{
			Dispose(false);
		}

		#endregion

		#region Properties

		public T Data
		{
			get { return _data; }
			private set
			{
				_data = value;
				OnDataChanged();
			}
		}

		public IProgress<OperationProgress> Progress { get; set; }

		#endregion

		#region Methods

		protected abstract Task<T> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		protected abstract void OnFetchCompleted(T data);

		protected abstract void OnFetchFailed(Exception exception);

		public async void ReloadData()
		{
			if(IsDisposed) throw new ObjectDisposedException(GetType().Name);

			using var cts = new CancellationTokenSource();
			try
			{
				Interlocked.Exchange(ref _cancellationTokenSource, cts)?.Cancel(false);
			}
			catch(ObjectDisposedException)
			{
			}
			var progress = Progress;
			T data;
			try
			{
				data = await FetchDataAsync(progress, cts.Token);
			}
			catch(OperationCanceledException)
			{
				Interlocked.CompareExchange(ref _cancellationTokenSource, null, cts);
				return;
			}
			catch(Exception exc)
			{
				if(Interlocked.CompareExchange(ref _cancellationTokenSource, null, cts) == cts)
				{
					Data = default;
					OnFetchFailed(exc);
				}
				return;
			}
			if(Interlocked.CompareExchange(ref _cancellationTokenSource, null, cts) == cts)
			{
				progress?.Report(OperationProgress.Completed);
				Data = data;
				OnFetchCompleted(data);
			}
		}

		#endregion

		#region IDisposable Members

		public bool IsDisposed { get; private set; }

		protected virtual void Dispose(bool disposing)
		{
			var cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
			if(cts != null)
			{
				try
				{
					cts.Cancel(false);
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public void Dispose()
		{
			if(!IsDisposed)
			{
				GC.SuppressFinalize(this);
				Dispose(true);
				IsDisposed = true;
			}
		}

		#endregion
	}
}

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

		private readonly TaskScheduler _scheduler;
		private IProgress<OperationProgress> _progress;
		private CancellationTokenSource _cancellationTokenSource;
		private T _data;
		private bool _isDisposed;

		#endregion

		#region Events

		public event EventHandler DataChanged;

		protected virtual void OnDataChanged()
		{
			var handler = DataChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor & finalizer

		public AsyncDataBinding()
		{
			_scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		}

		~AsyncDataBinding()
		{
			Dispose(false);
		}

		#endregion

		#region Properties

		protected TaskScheduler Scheduler
		{
			get { return _scheduler; }
		}

		public T Data
		{
			get { return _data; }
			private set
			{
				_data = value;
				OnDataChanged();
			}
		}

		public IProgress<OperationProgress> Progress
		{
			get { return _progress; }
			set { _progress = value; }
		}

		#endregion

		#region Methods

		protected abstract Task<T> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		protected abstract void OnFetchCompleted(T data);

		protected abstract void OnFetchFailed(Exception exception);

		public void ReloadData()
		{
			var cts = new CancellationTokenSource();
			var currentCts = Interlocked.Exchange(ref _cancellationTokenSource, cts);
			if(currentCts != null)
			{
				try
				{
					currentCts.Cancel(false);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			var progress = Progress;
			FetchDataAsync(progress, cts.Token).ContinueWith(t =>
				{
					cts.Dispose();
					if(Interlocked.CompareExchange(ref _cancellationTokenSource, null, cts) != cts)
					{
						return;
					}
					if(t.IsCanceled)
					{
						return;
					}
					else if(t.IsFaulted)
					{
						Data = default(T);
						OnFetchFailed(TaskUtility.UnwrapException(t.Exception));
					}
					else if(t.IsCompleted)
					{
						var data = t.Result;
						if(progress != null)
						{
							progress.Report(OperationProgress.Completed);
						}
						Data = data;
						OnFetchCompleted(data);
					}
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				Scheduler);
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

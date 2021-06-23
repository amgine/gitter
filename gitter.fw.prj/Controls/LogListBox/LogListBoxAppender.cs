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

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Services;

	sealed class LogListBoxAppender : ILogAppender, IObservable<LogEvent>
	{
		public static readonly LogListBoxAppender Instance = new();

		private sealed class SubscriberToken : IDisposable
		{
			private readonly LogListBoxAppender _appender;
			private readonly IObserver<LogEvent> _observer;
			private bool _isDisposed;

			public SubscriberToken(LogListBoxAppender appender, IObserver<LogEvent> observer)
			{
				_appender = appender;
				_observer = observer;
			}

			public void Dispose()
			{
				if(!_isDisposed)
				{
					lock(_appender._observers)
					{
						_appender._observers.Remove(_observer);
					}
					_isDisposed = true;
				}
			}
		}

		private readonly LinkedList<IObserver<LogEvent>> _observers;
		private readonly LogEvent[] _events;
		private int _pos;
		private int _count;

		/// <summary>Initializes a new instance of the <see cref="LogListBoxAppender"/> class.</summary>
		private LogListBoxAppender()
		{
			_observers = new LinkedList<IObserver<LogEvent>>();
			_events = new LogEvent[500];
		}

		public object SyncRoot => _events;

		public void DropCache()
		{
			lock(_events)
			{
				_pos = 0;
				_count = 0;
				Array.Clear(_events, 0, _events.Length);
			}
		}

		public void Append(LogEvent logEvent)
		{
			lock(_events)
			{
				if(_count < _events.Length) ++_count;
				_events[_pos] = logEvent;
				_pos = (_pos + 1) % _events.Length;
			}
			lock(_observers)
			{
				foreach(var observer in _observers)
				{
					observer.OnNext(logEvent);
				}
			}
		}

		public IDisposable Subscribe(IObserver<LogEvent> observer)
		{
			Verify.Argument.IsNotNull(observer, nameof(observer));

			lock(_observers)
			{
				_observers.AddLast(observer);
			}
			lock(_events)
			{
				int start = _pos - _count;
				if(start < 0) start += _events.Length;
				for(int i = start, c = 0; c < _count; i = (i + 1) % _events.Length, ++c)
				{
					observer.OnNext(_events[i]);
				}
			}
			return new SubscriberToken(this, observer);
		}
	}
}

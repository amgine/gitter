namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Services;

	sealed class LogListBoxAppender : ILogAppender, IObservable<LogEvent>
	{
		public static readonly LogListBoxAppender Instance = new LogListBoxAppender();

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

		public object SyncRoot
		{
			get { return _events; }
		}

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
			if(observer == null) throw new ArgumentNullException("observer");

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

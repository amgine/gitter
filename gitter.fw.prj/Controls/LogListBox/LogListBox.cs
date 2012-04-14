namespace gitter.Framework.Controls
{
	using System;
	using System.ComponentModel;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using System.Drawing;

	using gitter.Framework.Services;

	public class LogListBox : CustomListBox, IObserver<LogEvent>
	{
		private const int DefaultMessageLimit = 500;

		private int _messageLimit;
		private IDisposable _observerToken;

		/// <summary>Initializes a new instance of the <see cref="LogListBox"/> class.</summary>
		public LogListBox()
		{
			Columns.AddRange(new CustomListBoxColumn[]
				{
					new LogListBoxTypeColumn(),
					new LogListBoxTimestampColumn(),
					new LogListBoxSourceColumn(),
					new LogListBoxMessageColumn(),
					new LogListBoxExceptionColumn(),
				});

			_messageLimit = DefaultMessageLimit;
			_observerToken = LogListBoxAppender.Instance.Subscribe(this);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_observerToken != null)
				{
					_observerToken.Dispose();
					_observerToken = null;
				}
			}
			base.Dispose(disposing);
		}

		[DefaultValue(DefaultMessageLimit)]
		public int MessageLimit
		{
			get { return _messageLimit; }
			set { _messageLimit = value; }
		}

		public void AppendEvent(LogEvent logEvent)
		{
			var item = new LogEventListItem(logEvent);
			if(Items.Count == _messageLimit)
			{
				BeginUpdate();
				Items.RemoveAt(0);
				Items.Add(item);
				EnsureVisible(item);
				EndUpdate();
			}
			else
			{
				Items.Add(item);
				EnsureVisible(item);
			}
		}

		void IObserver<LogEvent>.OnCompleted()
		{
		}

		void IObserver<LogEvent>.OnError(Exception error)
		{
		}

		void IObserver<LogEvent>.OnNext(LogEvent value)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new Action<LogEvent>(AppendEvent), new object[] { value });
			}
			else
			{
				AppendEvent(value);
			}
		}
	}
}

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	public sealed class PopupNotificationsStack : IDisposable
	{
		#region Const

		/// <summary>Расстояние между соседними окнами сообщений.</summary>
		private const int WindowSpacing = 5;

		#endregion

		#region Data

		private readonly List<PopupNotificationForm> _windows;
		private readonly Queue<NotificationContent> _queue;
		private Point _origin;
		private bool _isDisposed;
		private int _maximumVisibleNotifications;
		private TimeSpan _defaultNotificationDuration;

		#endregion

		#region .ctor

		public PopupNotificationsStack(Point origin)
		{
			_maximumVisibleNotifications = 5;
			_defaultNotificationDuration = TimeSpan.FromSeconds(10.0);
			_windows = new List<PopupNotificationForm>();
			_queue = new Queue<NotificationContent>();
			_origin = origin;
		}

		public PopupNotificationsStack()
			: this(GetDefaultOrigin())
		{
		}

		private static Point GetDefaultOrigin()
		{
			var area = Screen.PrimaryScreen.WorkingArea;
			var w = area.Right;
			var h = area.Bottom;
			return new Point(w - ViewConstants.PopupWidth - 5, h - 5);
		}

		#endregion

		public int MaximumVisibleNotifications
		{
			get { return _maximumVisibleNotifications; }
			set
			{
				if(value < 1) throw new ArgumentOutOfRangeException();

				_maximumVisibleNotifications = value;
			}
		}

		public TimeSpan DefaultNotificationDuration
		{
			get { return _defaultNotificationDuration; }
			set
			{
				if(value.Ticks < 1) throw new ArgumentOutOfRangeException();

				_defaultNotificationDuration = value;
			}
		}

		public void PushNotification(NotificationContent content)
		{
			if(content == null) throw new ArgumentNullException("content");
			if(IsDisposed) throw new ObjectDisposedException(GetType().Name);

			if(_windows.Count < MaximumVisibleNotifications)
			{
				PushForce(content);
			}
			else
			{
				_queue.Enqueue(content);
			}
		}

		private void PushForce(NotificationContent notification)
		{
			var window = new PopupNotificationForm(notification);
			var h = window.Height;
			var x = _origin.X;
			var y = _origin.Y - h;
			window.Left = x;
			window.Top = y;
			_windows.Add(window);
			MakeRoomInStack(window);
			window.SizeChanged += OnWindowSizeChanged;
			AnimateFadeIn(window);
			window.Closed	+= OnWindowClosed;
			window.Show();
		}

		private void OnWindowSizeChanged(object sender, EventArgs e)
		{
			var window = (PopupNotificationForm)sender;
			MakeRoomInStack(window);
		}

		private void OnWindowClosed(object sender, EventArgs e)
		{
			var window = (PopupNotificationForm)sender;
			window.SizeChanged -= OnWindowSizeChanged;
			window.Closed -= OnWindowClosed;
			_windows.Remove(window);
			if(_windows.Count < MaximumVisibleNotifications && _queue.Count != 0)
			{
				PushForce(_queue.Dequeue());
			}
		}

		private void MakeRoomInStack(PopupNotificationForm window)
		{
			int index = _windows.IndexOf(window);
			if(index == -1) return;
			var desiredTop = _origin.Y - _windows[_windows.Count - 1].Height;
			for(int i = _windows.Count - 2; i >= index + 1; --i)
			{
				desiredTop -= _windows[i].Height - WindowSpacing;
			}
			if(window.Top > desiredTop)
			{
				AnimateVerticalSlide(window, desiredTop);
			}
			var w2 = window;
			for(int i = index - 1; i >= 0; --i)
			{
				var w1 = _windows[i];
				desiredTop = desiredTop - w1.Height - WindowSpacing;
				if(w1.Top > desiredTop)
				{
					AnimateVerticalSlide(w1, desiredTop);
				}
				else
				{
					break;
				}
				w2 = w1;
			}
		}

		private static void AnimateVerticalSlide(PopupNotificationForm window, int targetTop)
		{
			window.Top = targetTop;
		}

		private static void AnimateFadeIn(PopupNotificationForm window)
		{
		}

		#region IDisposable

		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		public void Dispose()
		{
			if(!IsDisposed)
			{
				foreach(var window in _windows)
				{
					window.SizeChanged -= OnWindowSizeChanged;
					window.Closed -= OnWindowClosed;
					window.Dispose();
				}
				_windows.Clear();
				foreach(var content in _queue)
				{
					content.Dispose();
				}
				_queue.Clear();
				IsDisposed = true;
			}
		}

		#endregion
	}
}

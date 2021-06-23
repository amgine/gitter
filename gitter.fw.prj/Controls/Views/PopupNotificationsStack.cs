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
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class PopupNotificationsStack : IDisposable
	{
		#region Const

		/// <summary>Расстояние между соседними окнами сообщений.</summary>
		private const int WindowSpacing = 5;

		#endregion

		#region Data

		private readonly List<PopupNotificationForm> _windows = new();
		private readonly Queue<NotificationContent> _queue = new();
		private Point _origin;
		private int _maximumVisibleNotifications = 5;
		private TimeSpan _defaultNotificationDuration = TimeSpan.FromSeconds(10.0);

		#endregion

		#region .ctor

		public PopupNotificationsStack(Point origin)
		{
			_origin = origin;
		}

		public PopupNotificationsStack()
			: this(GetDefaultOrigin())
		{
		}

		private static Point GetDefaultOrigin()
		{
			var conv = new DpiConverter(GraphicsUtility.MeasurementGraphics);
			var area = Screen.PrimaryScreen.WorkingArea;
			var w = area.Right;
			var h = area.Bottom;
			return new Point(w - conv.ConvertX(ViewConstants.PopupWidth) - conv.ConvertX(5), h - conv.ConvertY(5));
		}

		#endregion

		public int MaximumVisibleNotifications
		{
			get => _maximumVisibleNotifications;
			set
			{
				Verify.Argument.IsPositive(value, nameof(value));

				_maximumVisibleNotifications = value;
			}
		}

		public TimeSpan DefaultNotificationDuration
		{
			get => _defaultNotificationDuration;
			set
			{
				Verify.Argument.IsPositive(value.Ticks, nameof(value));

				_defaultNotificationDuration = value;
			}
		}

		public void PushNotification(NotificationContent content)
		{
			Verify.Argument.IsNotNull(content, nameof(content));
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
			window.Left = _origin.X;
			window.Top  = _origin.Y - window.Height;
			window.Prepare();
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

		public bool IsDisposed { get; private set; }

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

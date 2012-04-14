namespace gitter.Framework.Services
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class BalloonNotificationService : INotificationService, IDisposable
	{
		#region Data

		private ToolTip _toolTip;
		private Control _notifyControl;
		private Size _toolTipSize;
		private const int ToolTipTimeout = 5000;

		#endregion

		public BalloonNotificationService()
		{
			_toolTip = new ToolTip();
			_toolTip.IsBalloon = true;
		}

		~BalloonNotificationService()
		{
			Dispose(false);
		}

		private void Notify(Control control, int x, int y, NotificationType type, string title, string message, bool focus)
		{
			if(_notifyControl != null) _toolTip.Hide(_notifyControl);
			var tticon = ToolTipIcon.None;
			switch(type)
			{
				case NotificationType.Error:
					tticon = ToolTipIcon.Error;
					break;
				case NotificationType.Information:
					tticon = ToolTipIcon.Info;
					break;
				case NotificationType.Warning:
					tticon = ToolTipIcon.Warning;
					break;
			}
			_toolTip.ToolTipIcon = tticon;
			_toolTip.ToolTipTitle = title;
			_notifyControl = control;
			_toolTip.Popup += OnToolTipPopup;
			_toolTip.Show(message, _notifyControl, 0, -74, 1);
			_toolTip.Popup -= OnToolTipPopup;
			_toolTip.Hide(_notifyControl);
			_toolTip.Show(message, _notifyControl, x, y - _toolTipSize.Height, ToolTipTimeout);
			if(focus) control.Focus();
		}

		public void Notify(Control control, NotificationType type, string title, string message)
		{
			Notify(control, 0, 0, type, title, message, false);
		}

		public void Notify(ToolStripItem item, NotificationType type, string title, string message)
		{
			var control = Utility.GetParentControl(item);
			Notify(control, item.Bounds.X, item.Bounds.Y, type, title, message, false);
		}

		public void Notify(Control control, string title, string message)
		{
			Notify(control, 0, 0, NotificationType.Simple, title, message, false);
		}

		public void Notify(ToolStripItem item, string title, string message)
		{
			var control = Utility.GetParentControl(item);
			Notify(control, item.Bounds.X, item.Bounds.Y, NotificationType.Simple, title, message, false);
		}

		public void NotifyInputError(Control control, NotificationType type, string title, string message)
		{
			Notify(control, 0, 0, type, title, message, true);
		}

		public void NotifyInputError(Control control, string title, string message)
		{
			Notify(control, 0, 0, NotificationType.Simple, title, message, true);
		}

		private void OnToolTipPopup(object sender, PopupEventArgs e)
		{
			_toolTipSize = e.ToolTipSize;
		}

		private void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_toolTip != null)
				{
					if(_notifyControl != null)
					{
						_toolTip.Hide(_notifyControl);
						_notifyControl = null;
					}
					_toolTip.RemoveAll();
					_toolTip.Dispose();
					_toolTip = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}

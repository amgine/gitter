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

namespace gitter.Framework.Services;

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

	~BalloonNotificationService() => Dispose(disposing: false);

	private static ToolTipIcon GetIcon(NotificationType type)
		=> type switch
		{
			NotificationType.Error       => ToolTipIcon.Error,
			NotificationType.Warning     => ToolTipIcon.Warning,
			NotificationType.Information => ToolTipIcon.Info,
			_ => ToolTipIcon.None,
		};

	private void Notify(Control control, int x, int y, NotificationType type, string title, string message, bool focus)
	{
		if(_notifyControl is not null)
		{
			_toolTip.Hide(_notifyControl);
		}
		_toolTip.ToolTipIcon = GetIcon(type);
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
			if(_toolTip is not null)
			{
				if(_notifyControl is not null)
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
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

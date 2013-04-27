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
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	public class NotificationContent : Control
	{
		private TimeSpan _timeout;

		public NotificationContent()
		{
			_timeout = TimeSpan.FromSeconds(10.0);
			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.SupportsTransparentBackColor,
				false);
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer,
				true);
			if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
			{
				Font = GitterApplication.FontManager.UIFont;
				ForeColor = GitterApplication.Style.Colors.WindowText;
				BackColor = GitterApplication.Style.Colors.Window;
			}
			else
			{
				Font = SystemFonts.MessageBoxFont;
			}
		}

		public TimeSpan Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}
	}
}

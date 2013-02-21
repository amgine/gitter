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

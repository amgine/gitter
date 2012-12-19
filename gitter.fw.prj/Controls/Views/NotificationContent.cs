namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	public class NotificationContent : Control
	{
		private TimeSpan _timeout;

		public NotificationContent()
		{
			Font = GitterApplication.FontManager.UIFont;
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

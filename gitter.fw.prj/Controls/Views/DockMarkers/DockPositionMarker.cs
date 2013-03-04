namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using System.Drawing;
	using System.Drawing.Drawing2D;

	using gitter.Native;

	/// <summary>Dock position indicator.</summary>
	sealed class DockPositionMarker : Form
	{
		/// <summary>Initializes a new instance of the <see cref="DockPositionMarker"/> class.</summary>
		/// <param name="bounds">Dock position marker bounds.</param>
		public DockPositionMarker(Rectangle bounds)
		{
			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.ResizeRedraw |
				ControlStyles.SupportsTransparentBackColor,
				false);

			StartPosition = FormStartPosition.Manual;
			FormBorderStyle = FormBorderStyle.None;
			ControlBox = false;
			MaximizeBox = false;
			MinimizeBox = false;
			Text = string.Empty;
			ShowIcon = false;
			ShowInTaskbar = false;
			Enabled = false;
			ImeMode = ImeMode.Disable;

			MinimumSize = new Size(1, 1);

			Bounds = bounds;
			AllowTransparency = true;
			BackColor = Color.FromArgb(120, 170, 240);
			Opacity = ViewConstants.DockPositionMarkerOpacity;
		}

		/// <summary>Displays the control to the user.</summary>
		public new void Show()
		{
			User32.ShowWindow(this.Handle, 8);
		}

		/// <summary>
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
		protected override void DefWndProc(ref Message m)
		{
			const int WM_MOUSEACTIVATE = 0x21;
			const int MA_NOACTIVATE = 0x0003;

			switch(m.Msg)
			{
				case WM_MOUSEACTIVATE:
					m.Result = (IntPtr)MA_NOACTIVATE;
					return;
			}
			base.DefWndProc(ref m);
		}

		/// <summary>Raises the <see cref="E:System.Windows.Forms.Form.Shown"/> event.</summary>
		/// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnShown(EventArgs e)
		{
			TopMost = true;
		}

		/// <summary>
		/// Gets a value indicating whether the window will be activated when it is shown.
		/// </summary>
		/// <value></value>
		/// <returns>True if the window will not be activated when it is shown; otherwise, false. The default is false.</returns>
		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}

		/// <summary>
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Windows.Forms.CreateParams"/> that contains the required creation parameters when the handle to the control is created.</returns>
		protected override CreateParams CreateParams
		{
			get
			{
				const int WS_EX_NOACTIVATE = 0x08000000;
				var baseParams = base.CreateParams;
				baseParams.ExStyle |= WS_EX_NOACTIVATE;
				return baseParams;
			}
		}
	}
}

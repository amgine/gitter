namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework.Options;

	/// <summary>Form with Windows7 taskbar support.</summary>
	public partial class FormEx : Form
	{
		private bool _canUseWin7Api;

		/// <summary>Create <see cref="FormEx"/>.</summary>
		public FormEx()
		{
			SuspendLayout();
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(624, 435);
			if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
			{
				Font = GitterApplication.FontManager.UIFont;
			}
			else
			{
				Font = SystemFonts.MessageBoxFont;
			}
			Name = "FormEx";
			ResumeLayout(false);
		}

		protected override void WndProc(ref Message m)
		{
			if(Utility.TaskBarList != null)
			{
				if(m.Msg == Utility.WM_TASKBAR_BUTTON_CREATED)
				{
					_canUseWin7Api = true;
				}
			}
			base.WndProc(ref m);
		}

		protected bool CanUseWin7Api
		{
			get { return _canUseWin7Api; }
		}

		public void SetTaskbarOverlayIcon(Icon icon, string description)
		{
			if(icon == null) throw new ArgumentNullException("icon");

			if(CanUseWin7Api)
			{
				if(InvokeRequired)
				{
					BeginInvoke(new MethodInvoker(
						() => Utility.TaskBarList.SetOverlayIcon(Handle, icon.Handle, description)));
				}
				else
				{
					Utility.TaskBarList.SetOverlayIcon(Handle, icon.Handle, description);
				}
			}
		}

		public void RemoveTaskbarOverlayIcon()
		{
			if(CanUseWin7Api)
			{
				if(InvokeRequired)
				{
					BeginInvoke(new MethodInvoker(
						() => Utility.TaskBarList.SetOverlayIcon(Handle, IntPtr.Zero, null)));
				}
				else
				{
					Utility.TaskBarList.SetOverlayIcon(Handle, IntPtr.Zero, null);
				}
			}
		}

		public void SetTaskbarProgressState(TbpFlag state)
		{
			if(CanUseWin7Api)
			{
				if(InvokeRequired)
				{
					BeginInvoke(new MethodInvoker(
						() => Utility.TaskBarList.SetProgressState(Handle, state)));
				}
				else
				{
					Utility.TaskBarList.SetProgressState(Handle, state);
				}
			}
		}

		public void SetTaskbarProgressValue(long current, long total)
		{
			if(CanUseWin7Api)
			{
				if(InvokeRequired)
				{
					BeginInvoke(new MethodInvoker(
						() => Utility.TaskBarList.SetProgressValue(Handle, (ulong)current, (ulong)total)));
				}
				else
				{
					Utility.TaskBarList.SetProgressValue(Handle, (ulong)current, (ulong)total);
				}
			}
		}
	}
}

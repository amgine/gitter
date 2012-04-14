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
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			ClientSize = new System.Drawing.Size(624, 435);
			if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
				Font = GitterApplication.FontManager.UIFont;
			else
				Font = SystemFonts.MessageBoxFont;
			Name = "FormEx";
			Text = "FormEx";
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

		public void SetProgressState(TbpFlag state)
		{
			if(_canUseWin7Api)
			{
				if(InvokeRequired)
				{
					BeginInvoke(new Action<TbpFlag>((flag) =>
						{
							Utility.TaskBarList.SetProgressState(Handle, flag);
						}), state);
				}
				else
				{
					Utility.TaskBarList.SetProgressState(Handle, state);
				}
			}
		}

		public void SetProgressValue(long current, long total)
		{
			if(_canUseWin7Api)
			{
				if(InvokeRequired)
				{
					BeginInvoke(new Action<ulong, ulong>((cur, tot) =>
						{
							Utility.TaskBarList.SetProgressValue(Handle, cur, tot);
						}), (ulong)current, (ulong)total);
				}
				else
				{
					Utility.TaskBarList.SetProgressValue(Handle, (ulong)current, (ulong)total);
				}
			}
		}
	}
}

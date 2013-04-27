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
			Verify.Argument.IsNotNull(icon, "icon");

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

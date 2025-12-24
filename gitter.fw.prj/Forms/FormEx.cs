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

namespace gitter.Framework;

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

/// <summary>Form with Windows7 taskbar support.</summary>
[DesignerCategory("")]
public partial class FormEx : Form
{
	private bool _canUseWin7Api;

	/// <summary>Create <see cref="FormEx"/>.</summary>
	public FormEx()
	{
		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		ClientSize          = new Size(624, 435);
		if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
		{
			Font = GitterApplication.FontManager.UIFont;
		}
#if !NET6_0_OR_GREATER
		else
		{
			Font = SystemFonts.MessageBoxFont;
		}
#endif
		Name = nameof(FormEx);
		ResumeLayout(false);
	}

	/// <inheritdoc/>
	protected override void WndProc(ref Message m)
	{
		if(Utility.TaskBarList is not null)
		{
			if(m.Msg == Utility.WM_TASKBAR_BUTTON_CREATED)
			{
				_canUseWin7Api = true;
			}
		}
		base.WndProc(ref m);
	}

	protected bool CanUseWin7Api => _canUseWin7Api;

	public void SetTaskbarOverlayIcon(Icon? icon, string description)
	{
		if(!CanUseWin7Api || IsDisposed) return;

		if(icon is null)
		{
			RemoveTaskbarOverlayIcon();
			return;
		}

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(
					() => SetTaskbarOverlayIcon(icon, description)));
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			Utility.TaskBarList?.SetOverlayIcon(Handle, icon.Handle, description);
		}
	}

	public void RemoveTaskbarOverlayIcon()
	{
		if(!CanUseWin7Api || IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(
					() => RemoveTaskbarOverlayIcon()));
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			Utility.TaskBarList?.SetOverlayIcon(Handle, IntPtr.Zero, null);
		}
	}

	private TbpFlag _targetProgressState;

	public void SetTaskbarProgressState(TbpFlag state)
	{
		if(!CanUseWin7Api || IsDisposed) return;

		_targetProgressState = state;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(SetTaskbarProgressStateSync));
			}
			catch
			{
			}
		}
		else
		{
			SetTaskbarProgressStateSync();
		}
	}

	private void SetTaskbarProgressStateSync()
	{
		if(!IsHandleCreated || IsDisposed) return;
		Utility.TaskBarList?.SetProgressState(Handle, _targetProgressState);
	}

	public void SetTaskbarProgressValue(long current, long total)
	{
		if(!CanUseWin7Api || IsDisposed) return;

		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(
					() => SetTaskbarProgressValue(current, total)));
			}
			catch
			{
			}
		}
		else
		{
			Utility.TaskBarList?.SetProgressValue(Handle, (ulong)current, (ulong)total);
		}
	}
}

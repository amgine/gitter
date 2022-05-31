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

namespace gitter.Updater;

using System;
using System.Drawing;
using System.Windows.Forms;

public partial class MainForm : Form
{
	static Icon LoadWindowIcon()
	{
		using var stream = typeof(MainForm)
			.Assembly
			.GetManifestResourceStream(@"gitter.Updater.Resources.icons.app.ico");
		if(stream is null) return default;
		return new Icon(stream);
	}

	private readonly IUpdateProcess _process;
	private readonly UpdateProcessMonitor _monitor;

	public MainForm(IUpdateProcess process)
	{
		Verify.Argument.IsNotNull(process);

		InitializeComponent();
		Icon = LoadWindowIcon();

#if !NET5_0_OR_GREATER
		Font = SystemFonts.MessageBoxFont;
#endif

		_process = process;
		_monitor = new UpdateProcessMonitor();

		_monitor.MaximumProgressChanged += OnMaximumProgressChanged;
		_monitor.CurrentProgressChanged += OnCurrentProgressChanged;
		_monitor.StageChanged += OnStageChanged;
		_monitor.UpdateSuccessful += OnUpdateSuccessful;
		_monitor.UpdateFailed += OnUpdateFailed;
		_monitor.UpdateCancelled += OnUpdateCancelled;
		_monitor.CanCancelChanged += OnCanCancelChanged;
	}

	/// <inheritdoc/>
	protected override void OnShown(EventArgs e)
	{
		base.OnShown(e);
		_process.BeginUpdate(_monitor);
	}

	private void OnUpdateSuccessful(object sender, EventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new EventHandler(OnUpdateSuccessful), sender, e);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			Close();
		}
	}

	private void OnUpdateFailed(object sender, UpdateFailedEventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new EventHandler<UpdateFailedEventArgs>(OnUpdateFailed), sender, e);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			MessageBox.Show(this, e.Message, "Update failed.");
			Close();
		}
	}

	private void OnUpdateCancelled(object sender, EventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new EventHandler(OnUpdateCancelled), sender, e);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			Close();
		}
	}

	private void OnCurrentProgressChanged(object sender, EventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new EventHandler(OnCurrentProgressChanged), sender, e);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			_updateProgress.Value = _monitor.CurrentProgress;
		}
	}

	private void OnMaximumProgressChanged(object sender, EventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new EventHandler(OnMaximumProgressChanged), sender, e);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			_updateProgress.Maximum = _monitor.MaximumProgress;
		}
	}

	private void OnStageChanged(object sender, EventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new EventHandler(OnStageChanged), sender, e);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			_lblStage.Text = _monitor.Stage;
		}
	}

	private void OnCanCancelChanged(object sender, EventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new EventHandler(OnCanCancelChanged), sender, e);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			if(!_monitor.CancelRequested)
			{
				_btnCancel.Enabled = _monitor.CanCancel;
			}
		}
	}

	private void OnCancelClick(object sender, EventArgs e)
	{
		_btnCancel.Enabled = false;
		_monitor.CancelRequested = true;
	}
}

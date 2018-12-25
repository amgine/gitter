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

namespace gitter.Updater
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public partial class MainForm : Form
	{
		private readonly IUpdateProcess _process;
		private readonly UpdateProcessMonitor _monitor;

		public MainForm(IUpdateProcess process)
		{
			Verify.Argument.IsNotNull(process, nameof(process));

			InitializeComponent();

			Font = SystemFonts.MessageBoxFont;

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

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			_process.BeginUpdate(_monitor);
		}

		private void OnUpdateSuccessful(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new EventHandler(OnUpdateSuccessful), sender, e);
			}
			else
			{
				Close();
			}
		}

		private void OnUpdateFailed(object sender, UpdateFailedEventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new EventHandler<UpdateFailedEventArgs>(OnUpdateFailed), sender, e);
			}
			else
			{
				MessageBox.Show(this, e.Message, "Update failed.");
				Close();
			}
		}

		private void OnUpdateCancelled(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new EventHandler(OnUpdateCancelled), sender, e);
			}
			else
			{
				Close();
			}
		}

		private void OnCurrentProgressChanged(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new EventHandler(OnCurrentProgressChanged), sender, e);
			}
			else
			{
				_updateProgress.Value = _monitor.CurrentProgress;
			}
		}

		private void OnMaximumProgressChanged(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new EventHandler(OnMaximumProgressChanged), sender, e);
			}
			else
			{
				_updateProgress.Maximum = _monitor.MaximumProgress;
			}
		}

		private void OnStageChanged(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new EventHandler(OnStageChanged), sender, e);
			}
			else
			{
				_lblStage.Text = _monitor.Stage;
			}
		}

		private void OnCanCancelChanged(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new EventHandler(OnCanCancelChanged), sender, e);
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
}

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
	using System.Drawing.Drawing2D;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework.Properties;
	using gitter.Framework.Services;

	/// <summary>Form which hosts <see cref="DialogBase"/>.</summary>
	[System.ComponentModel.DesignerCategory("")]
	public partial class DialogForm : Form
	{
		#region Data

		private readonly DialogBase _dialog;
		private readonly IExecutableDialog _executable;
		private readonly IAsyncExecutableDialog _async;
		private readonly IElevatedExecutableDialog _elevated;
		private readonly IExpandableDialog _expandable;
		private readonly Control _expansionControl;
		private bool _isExecuting;

		private Bitmap _bmpEH;
		private Bitmap _bmpCH;
		private Bitmap _bmpEN;
		private Bitmap _bmpCN;

		private bool _btnHover;
		private bool _expanded;

		#endregion

		/// <summary>Create <see cref="DialogForm"/>.</summary>
		public DialogForm()
		{
			InitializeComponent();

			_btnOK.Text     = Resources.StrOk;
			_btnCancel.Text = Resources.StrCancel;
			_btnApply.Text  = Resources.StrApply;

			if(!Application.RenderWithVisualStyles)
			{
				_pnlContainer.BackColor = SystemColors.Control;
				_pnlLine.BackColor = SystemColors.Control;

				int d = _btnOK.Top - _pnlContainer.Bottom;

				_pnlContainer.Height += d;
				Height -= d;
			}
		}

		private DialogForm(DialogBase content)
			: this()
		{
			_dialog = content;
			if(content is not null)
			{
				UpdateSize();
				ShowContent();

				content.SizeChanged += OnContentSizeChanged;

				_expandable = content as IExpandableDialog;
				if(_expandable is not null)
				{
					_expanded = true;
					var exDialog = (IExpandableDialog)content;
					CreateBitmaps(exDialog.ExpansionName);
					_picAdvanced.Visible = true;
					_expansionControl = exDialog.ExpansionControl;
				}
				_elevated   = content as IElevatedExecutableDialog;
				_executable = content as IExecutableDialog;
				_async      = content as IAsyncExecutableDialog;
			}
		}

		public DialogForm(DialogBase content, DialogButtons buttons)
			: this(content)
		{
			bool btnOK     = (buttons & DialogButtons.Ok)     == DialogButtons.Ok;
			bool btnCancel = (buttons & DialogButtons.Cancel) == DialogButtons.Cancel;
			bool btnApply  = (buttons & DialogButtons.Apply)  == DialogButtons.Apply;

			if(!btnApply)
			{
				if(!btnCancel)
				{
					_btnOK.Left = _btnApply.Left;
				}
				else
				{
					_btnOK.Left = _btnCancel.Left;
					_btnCancel.Left = _btnApply.Left;
				}
			}
			else
			{
				if(!btnCancel)
				{
					_btnOK.Left = _btnCancel.Left;
				}
			}
			_btnOK.Visible = btnOK;
			_btnCancel.Visible = btnCancel;
			_btnApply.Visible = btnApply;
		}

		private void ShowContent()
		{
			var margin = _dialog.Margin;
			_dialog.Location = new Point(margin.Left, margin.Right);
			_dialog.Parent = _pnlContainer;
			Text = _dialog.Text;
		}

		private void UpdateSize()
		{
			var margin = _dialog.Margin;
			var dx = _pnlContainer.Width - _dialog.Width - margin.Left - margin.Right;
			var dy = _pnlContainer.Height - 1 - _dialog.Height - margin.Top - margin.Bottom;
			Width -= dx;
			Height -= dy;
		}

		private void OnContentSizeChanged(object sender, EventArgs e)
		{
			UpdateSize();
		}

		/// <inheritdoc/>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if(_isExecuting) e.Cancel = true;
			base.OnFormClosing(e);
		}

		/// <inheritdoc/>
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			_dialog.InvokeOnShown();
			if(_elevated is not null && !Utility.IsRunningWithAdministratorRights)
			{
				if(_elevated.RequireElevation)
				{
					if(_btnOK.Visible) _btnOK.ShowUACShield();
					if(_btnApply.Visible) _btnApply.ShowUACShield();
				}
				_elevated.RequireElevationChanged += OnRequireElevationExecutionChanged;
			}
		}

		/// <inheritdoc/>
		protected override void OnClosed(EventArgs e)
		{
			if(_dialog is not null)
			{
				_dialog.SizeChanged -= OnContentSizeChanged;
				_dialog.InvokeOnClosed(DialogResult);
			}
		}

		private void CreateBitmaps(string strname)
		{
			_bmpEH = RenderChevronButton(strname, Font, _picAdvanced.Width, _picAdvanced.Height, true,  true);
			_bmpCH = RenderChevronButton(strname, Font, _picAdvanced.Width, _picAdvanced.Height, false, true);
			_bmpEN = RenderChevronButton(strname, Font, _picAdvanced.Width, _picAdvanced.Height, true,  false);
			_bmpCN = RenderChevronButton(strname, Font, _picAdvanced.Width, _picAdvanced.Height, false, false);

			_picAdvanced.Image = _expanded?_bmpEN:_bmpCN;
		}

		private bool Execute()
		{
			bool okEnabled     = _btnOK.Enabled;
			bool cancelEnabled = _btnCancel.Enabled;
			bool applyEnabled  = _btnApply.Enabled;
			if(_executable is not null || _elevated is not null)
			{
				_btnOK.Enabled     = false;
				_btnCancel.Enabled = false;
				_btnApply.Enabled  = false;
				_isExecuting       = true;
			}
			try
			{
				if(_elevated is { RequireElevation: true })
				{
					var action = _elevated.ElevatedExecutionAction;
					if(action is not null)
					{
						try
						{
							HelperExecutables.ExecuteWithAdministartorRights(action);
						}
						catch(Exception exc) when(!exc.IsCritical())
						{
							GitterApplication.MessageBoxService.Show(
								this,
								Resources.ErrSomeOptionsCouldNotBeApplied,
								Resources.ErrFailedToRunElevatedProcess,
								MessageBoxButton.Close,
								MessageBoxIcon.Exclamation);
						}
					}
				}
				if(_executable is not null)
				{
					return _executable.Execute();
				}
			}
			finally
			{
				if(_executable is not null || _elevated is not null)
				{
					_btnOK.Enabled     = okEnabled;
					_btnCancel.Enabled = cancelEnabled;
					_btnApply.Enabled  = applyEnabled;
					_isExecuting       = false;
				}
			}
			return true;
		}

		private async Task<bool> ExecuteAsync()
		{
			var task = _async.ExecuteAsync();
			if(task.IsCompleted)
			{
				return task.Result;
			}

			bool okEnabled     = _btnOK.Enabled;
			bool cancelEnabled = _btnCancel.Enabled;
			bool applyEnabled  = _btnApply.Enabled;
			if(_async is not null)
			{
				_btnOK.Enabled     = false;
				_btnCancel.Enabled = false;
				_btnApply.Enabled  = false;
				_isExecuting       = true;
			}
			try
			{
				return await task;
			}
			finally
			{
				if(_async is not null)
				{
					_btnOK.Enabled     = okEnabled;
					_btnCancel.Enabled = cancelEnabled;
					_btnApply.Enabled  = applyEnabled;
					_isExecuting       = false;
				}
			}
		}

		private void _picAdvanced_MouseEnter(object sender, EventArgs e)
		{
			_btnHover = true;
			UpdateButtonState();
		}

		private void _picAdvanced_MouseLeave(object sender, EventArgs e)
		{
			_btnHover = false;
			UpdateButtonState();
		}

		private void _picAdvanced_Click(object sender, EventArgs e)
		{
			Expanded = !Expanded;
		}

		private void OnRequireElevationExecutionChanged(object sender, EventArgs e)
		{
			bool require = _elevated.RequireElevation;
			if(require)
			{
				if(_btnOK.Visible) _btnOK.ShowUACShield();
				if(_btnApply.Visible) _btnApply.ShowUACShield();
			}
			else
			{
				if(_btnOK.Visible) _btnOK.HideUACShield();
				if(_btnApply.Visible) _btnApply.HideUACShield();
			}
		}

		private void UpdateButtonState()
		{
			if(_btnHover)
			{
				_picAdvanced.Image = _expanded?_bmpEH:_bmpCH;
			}
			else
			{
				_picAdvanced.Image = _expanded?_bmpEN:_bmpCN;
			}
		}

		public bool Expanded
		{
			get => _expanded;
			set
			{
				if(_expanded != value)
				{
					_expanded = value;
					UpdateButtonState();
					if(value)
					{
						_expansionControl.Visible = true;
						Height += _expansionControl.Height + _expansionControl.Margin.Vertical;
					}
					else
					{
						_expansionControl.Visible = false;
						Height -= _expansionControl.Height + _expansionControl.Margin.Vertical;
					}
				}
			}
		}

		public bool OkButtonEnabled
		{
			get => _btnOK.Enabled;
			set => _btnOK.Enabled = value;
		}

		public bool CancelButtonEnabled
		{
			get => _btnCancel.Enabled;
			set => _btnCancel.Enabled = value;
		}

		public bool ApplyButtonEnabled
		{
			get => _btnApply.Enabled;
			set => _btnApply.Enabled = value;
		}

		public string OKButtonText
		{
			get => _btnOK.Text;
			set => _btnOK.Text = value;
		}

		public string CancelButtonText
		{
			get => _btnCancel.Text;
			set => _btnCancel.Text = value;
		}

		public string ApplyButtonText
		{
			get => _btnApply.Text;
			set => _btnApply.Text = value;
		}

		public void ClickOk() => _btnOK_Click(_btnOK, EventArgs.Empty);

		public void ClickCancel() => _btnCancel_Click(_btnCancel, EventArgs.Empty);

		public void ClickApply() => _btnApply_Click(_btnApply, EventArgs.Empty);

		private static Bitmap RenderChevronButton(string text, Font font, int width, int height, bool expanded, bool hover)
		{
			var bmp = new Bitmap(width, height);
			try
			{
				using var g = Graphics.FromImage(bmp);
				g.Clear(SystemColors.Control);
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
				var rc = new Rectangle(2, 2, width - 4, height - 4);
				g.SmoothingMode = SmoothingMode.HighQuality;
				if(hover)
				{
					using var img = expanded ? Resources.ImgChevronCollapseHover : Resources.ImgChevronExpandHover;
					g.DrawImage(img, (height - img.Width) / 2, (height - img.Height) / 2, img.Width, img.Height);
				}
				else
				{
					using var img = expanded ? Resources.ImgChevronCollapse : Resources.ImgChevronExpand;
					g.DrawImage(img, (height - img.Width) / 2, (height - img.Height) / 2, img.Width, img.Height);
				}
				rc.X += height + 2;
				rc.Width -= height + 2;
				using var sf = new StringFormat(StringFormat.GenericTypographic)
				{
					LineAlignment = StringAlignment.Center,
					Alignment = StringAlignment.Near,
				};
				GitterApplication.TextRenderer.DrawText(
					g, text, font, SystemBrushes.WindowText, rc, sf);
			}
			catch
			{
				bmp.Dispose();
				throw;
			}
			return bmp;
		}

		private async void _btnOK_Click(object sender, EventArgs e)
		{
			if(_async is not null)
			{
				if(await ExecuteAsync())
				{
					DialogResult = DialogResult.OK;
					Close();
				}
			}
			else
			{
				if(Execute())
				{
					DialogResult = DialogResult.OK;
					Close();
				}
			}
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private async void _btnApply_Click(object sender, EventArgs e)
		{
			if(_async is not null)
			{
				await ExecuteAsync();
			}
			else
			{
				Execute();
			}
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				components?.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}

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
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework.Properties;
using gitter.Framework.Services;
using gitter.Framework.Layout;
using gitter.Framework.Controls;

/// <summary>Form which hosts <see cref="DialogBase"/>.</summary>
[System.ComponentModel.DesignerCategory("")]
public partial class DialogForm : Form
{
	private readonly IButtonWidget _btnOK;
	private readonly IButtonWidget _btnCancel;
	private readonly IButtonWidget _btnApply;

	private readonly DialogBase _dialog;
	private readonly IExecutableDialog _executable;
	private readonly IAsyncExecutableDialog _async;
	private readonly IElevatedExecutableDialog _elevated;
	private bool _isExecuting;
	private bool _btnHover;

	/// <summary>Create <see cref="DialogForm"/>.</summary>
	public DialogForm(DialogBase content, DialogButtons buttons = DialogButtons.All)
	{
		AutoScaleDimensions = new SizeF(96F, 96F);
		AutoScaleMode = AutoScaleMode.Dpi;
		Font = GitterApplication.FontManager.UIFont;
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = nameof(DialogForm);
		ShowIcon = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;

		var noMargin = DpiBoundValue.Constant(Padding.Empty);

		_dialog = content;

		Panel pnlButtons;

		BackColor = Application.RenderWithVisualStyles
			? SystemColors.Window
			: SystemColors.Control;
		_ = new ControlLayout(this)
		{
			Content = new Grid(
				rows: new[]
				{
					SizeSpec.Everything(),
					Application.RenderWithVisualStyles ? SizeSpec.Absolute(1) : SizeSpec.Nothing(),
					SizeSpec.Absolute(39),
				},
				content: new[]
				{
					new GridContent(new ControlContent(new Panel
					{
						BackColor = Application.RenderWithVisualStyles
							? SystemColors.ControlLight
							: SystemColors.Control,
						Parent    = this,
					},
					marginOverride: noMargin,
					horizontalContentAlignment: HorizontalContentAlignment.Stretch,
					verticalContentAlignment:   VerticalContentAlignment.Stretch),
					row: 1),
					new GridContent(new ControlContent(pnlButtons = new Panel
					{
						BackColor = SystemColors.Control,
						Parent    = this,
					},
					marginOverride: noMargin,
					horizontalContentAlignment: HorizontalContentAlignment.Stretch,
					verticalContentAlignment:   VerticalContentAlignment.Stretch),
					row: 2),
				}),
		};

		var btnOK     = (buttons & DialogButtons.Ok)     == DialogButtons.Ok;
		var btnCancel = (buttons & DialogButtons.Cancel) == DialogButtons.Cancel;
		var btnApply  = (buttons & DialogButtons.Apply)  == DialogButtons.Apply;
		var btnCount  = (btnOK ? 1 : 0) + (btnCancel ? 1 : 0) + (btnApply ? 1 : 0);

		const int ButtonHeight  = 23;
		const int TopMargin     =  8;
		const int RightMargin   =  6;

		var buttonWidth   = SizeSpec.Absolute(75);
		var buttonSpacing = SizeSpec.Absolute(6);

		const int firstOffset = 1;

		var columns = new ISizeSpec[2 + btnCount * 2 - 1];
		columns[0] = SizeSpec.Everything();
		for(var i = 0; i < btnCount; ++i)
		{
			columns[i * 2 + firstOffset + 0] = buttonWidth;
			if(i < btnCount - 1)
			{
				columns[i * 2 + firstOffset + 1] = buttonSpacing;
			}
		}
		columns[columns.Length - 1] = SizeSpec.Absolute(RightMargin);

		if(btnOK)
		{
			_btnOK = new SystemButtonAdapter
			{
				Text = Resources.StrOk,
			};
			_btnOK.Control.Parent = pnlButtons;
			_btnOK.Click += _btnOK_Click;
			AcceptButton = _btnOK.Control as Button;
		}
		if(btnCancel)
		{
			_btnCancel = new SystemButtonAdapter
			{
				Text = Resources.StrCancel,
			};
			_btnCancel.Control.Parent = pnlButtons;
			_btnCancel.Click += _btnCancel_Click;
			CancelButton = _btnCancel.Control as Button;
		}
		if(btnApply)
		{
			_btnApply = new SystemButtonAdapter
			{
				Text = Resources.StrApply,
			};
			_btnApply.Control.Parent = pnlButtons;
			_btnApply.Click += _btnApply_Click;
		}

		GridContent WrapButton(IButtonWidget button, int index)
			=> new(new ControlContent(button.Control,
				marginOverride: noMargin,
				horizontalContentAlignment: HorizontalContentAlignment.Stretch,
				verticalContentAlignment:   VerticalContentAlignment.Stretch),
				row: 1, column: index * 2 + firstOffset);

		var buttonsContent = new GridContent[btnCount];
		var index = 0;
		if(btnOK)
		{
			buttonsContent[index] = WrapButton(_btnOK, index);
			++index;
		}
		if(btnCancel)
		{
			buttonsContent[index] = WrapButton(_btnCancel, index);
			++index;
		}
		if(btnApply)
		{
			buttonsContent[index] = WrapButton(_btnApply, index);
			++index;
		}

		_ = new ControlLayout(pnlButtons)
		{
			Content = new Grid(
				rows: new[]
				{
					SizeSpec.Absolute(TopMargin),
					SizeSpec.Absolute(ButtonHeight),
					SizeSpec.Everything(),
				},
				columns: columns,
				content: buttonsContent),
		};

		if(content is not null)
		{
			SuspendLayout();
			UpdateSize();
			ShowContent();
			ResumeLayout();

			_elevated   = content as IElevatedExecutableDialog;
			_executable = content as IExecutableDialog;
			_async      = content as IAsyncExecutableDialog;
		}
	}

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		if(_dialog is not null)
		{
			UpdateSize();
			_dialog.ScalableSizeChanged += OnContentSizeChanged;
		}
		base.OnLoad(e);
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	private void ShowContent()
	{
		_dialog.Parent = this;
		Text = _dialog.Text;
	}

	private void UpdateSize()
	{
		var dpi = Dpi.FromControl(this);

		var size   = _dialog.ScalableSize.GetValue(dpi);
		var margin = _dialog.ScalableMargin.GetValue(dpi);

		_dialog.Bounds = new Rectangle(
			margin.Left, margin.Right,
			size.Width,  size.Height);
	
		size.Width  += margin.Horizontal;
		size.Height += margin.Vertical;
		size.Height += 40 * dpi.Y / 96;

		ClientSize = size;
	}

	/// <inheritdoc/>
	protected override void OnDpiChanged(DpiChangedEventArgs e)
	{
		base.OnDpiChanged(e);
		UpdateSize();
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
				if(_btnOK is not null    && _btnOK.Control.Visible)    (_btnOK    as Button)?.ShowUACShield();
				if(_btnApply is not null && _btnApply.Control.Visible) (_btnApply as Button)?.ShowUACShield();
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

	private bool Execute()
	{
		bool okEnabled     = _btnOK     is not null && _btnOK.Control.Enabled;
		bool cancelEnabled = _btnCancel is not null && _btnCancel.Control.Enabled;
		bool applyEnabled  = _btnApply  is not null && _btnApply.Control.Enabled;
		if(_executable is not null || _elevated is not null)
		{
			if(okEnabled)     _btnOK.Control.Enabled     = false;
			if(cancelEnabled) _btnCancel.Control.Enabled = false;
			if(applyEnabled)  _btnApply.Control.Enabled  = false;
			_isExecuting = true;
		}
		try
		{
			if(_elevated is { RequireElevation: true, ElevatedExecutionActions: { Length: not 0 } actions })
			{
				try
				{
					HelperExecutables.ExecuteWithAdministartorRights(actions);
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
			if(_executable is not null)
			{
				return _executable.Execute();
			}
		}
		finally
		{
			if(_executable is not null || _elevated is not null)
			{
				if(okEnabled)     _btnOK.Control.Enabled     = okEnabled;
				if(cancelEnabled) _btnCancel.Control.Enabled = cancelEnabled;
				if(applyEnabled)  _btnApply.Control.Enabled  = applyEnabled;
				_isExecuting = false;
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

		bool okEnabled     = _btnOK     is not null && _btnOK.Control.Enabled;
		bool cancelEnabled = _btnCancel is not null && _btnCancel.Control.Enabled;
		bool applyEnabled  = _btnApply  is not null && _btnApply.Control.Enabled;
		if(_async is not null)
		{
			if(okEnabled)     _btnOK.Control.Enabled     = false;
			if(cancelEnabled) _btnCancel.Control.Enabled = false;
			if(applyEnabled)  _btnApply.Control.Enabled  = false;
			_isExecuting = true;
		}
		try
		{
			return await task;
		}
		finally
		{
			if(_async is not null)
			{
				if(okEnabled)     _btnOK.Control.Enabled     = okEnabled;
				if(cancelEnabled) _btnCancel.Control.Enabled = cancelEnabled;
				if(applyEnabled)  _btnApply.Control.Enabled  = applyEnabled;
				_isExecuting = false;
			}
		}
	}

	private void OnRequireElevationExecutionChanged(object sender, EventArgs e)
	{
		bool require = _elevated.RequireElevation;
		if(require)
		{
			if(_btnOK    is not null && _btnOK.Control.Visible)    (_btnOK.Control    as Button)?.ShowUACShield();
			if(_btnApply is not null && _btnApply.Control.Visible) (_btnApply.Control as Button)?.ShowUACShield();
		}
		else
		{
			if(_btnOK    is not null && _btnOK.Control.Visible)    (_btnOK.Control    as Button)?.HideUACShield();
			if(_btnApply is not null && _btnApply.Control.Visible) (_btnApply.Control as Button)?.HideUACShield();
		}
	}

	public bool OkButtonEnabled
	{
		get => _btnOK is not null && _btnOK.Control.Enabled;
		set
		{
			if(_btnOK is not null)
			{
				_btnOK.Control.Enabled = value;
			}
		}
	}

	public bool CancelButtonEnabled
	{
		get => _btnCancel is not null && _btnCancel.Control.Enabled;
		set
		{
			if(_btnCancel is not null)
			{
				_btnCancel.Control.Enabled = value;
			}
		}
	}

	public bool ApplyButtonEnabled
	{
		get => _btnApply is not null && _btnApply.Control.Enabled;
		set
		{
			if(_btnApply is not null)
			{
				_btnApply.Control.Enabled = value;
			}
		}
	}

	public string OKButtonText
	{
		get => _btnOK?.Text;
		set
		{
			if(_btnOK is not null)
			{
				_btnOK.Text = value;
			}
		}
	}

	public string CancelButtonText
	{
		get => _btnCancel?.Text;
		set
		{
			if(_btnCancel is not null)
			{
				_btnCancel.Text = value;
			}
		}
	}

	public string ApplyButtonText
	{
		get => _btnApply?.Text;
		set
		{
			if(_btnApply is not null)
			{
				_btnApply.Text = value;
			}
		}
	}

	public void ClickOk() => _btnOK_Click(_btnOK, EventArgs.Empty);

	public void ClickCancel() => _btnCancel_Click(_btnCancel, EventArgs.Empty);

	public void ClickApply() => _btnApply_Click(_btnApply, EventArgs.Empty);

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
}

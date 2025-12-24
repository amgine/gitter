#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class InitDialog : GitDialogBase, IExecutableDialog, IAsyncExecutableDialog, IInitView
{
	private readonly struct DialogControls
	{
        private readonly LabelControl _lblPath;
        private readonly TextBox _txtTemplate;
        public  readonly FolderPickerTextBoxDecorator _txtTemplateDecorator;
        public  readonly TextBox _txtPath;
        public  readonly ICheckBoxWidget _chkUseTemplate;
        public  readonly ICheckBoxWidget _chkBare;
        private readonly GroupSeparator _grpOptions;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			_lblPath = new();
			_txtTemplate = new();
			_txtTemplateDecorator = new(_txtTemplate) { Enabled = false };
			_txtPath = new();
            _chkUseTemplate = style.CheckBoxFactory.Create();
            _chkBare = style.CheckBoxFactory.Create();
			_grpOptions = new();

            GitterApplication.FontManager.InputFont.Apply(_txtPath, _txtTemplate);
        }

        public void Localize()
		{
            _lblPath.Text = Resources.StrPath.AddColon();
            _grpOptions.Text = Resources.StrOptions;
            _chkUseTemplate.Text = Resources.StrTemplate.AddColon();
            _chkBare.Text = Resources.StrBare;
        }

		public void Layout(Control parent)
		{
            var pathDec     = new FolderPickerTextBoxDecorator(_txtPath);

            _ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					rows:
					[
                        /* 0 */ LayoutConstants.TextInputRowHeight,
                        /* 1 */ LayoutConstants.GroupSeparatorRowHeight,
                        /* 2 */ LayoutConstants.TextInputRowHeight,
                        /* 3 */ LayoutConstants.CheckBoxRowHeight,
                        /* 4 */ SizeSpec.Everything(),
                    ],
					content:
					[
                        new GridContent(new ControlContent(_lblPath,              marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
                        new GridContent(new ControlContent(pathDec,               marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
                        new GridContent(new ControlContent(_grpOptions,           marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 1),
                        new GridContent(new WidgetContent(_chkUseTemplate,        marginOverride: LayoutConstants.GroupPadding), row: 2),
                        new GridContent(new ControlContent(_txtTemplateDecorator, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 2),
                        new GridContent(new WidgetContent(_chkBare,               marginOverride: LayoutConstants.GroupPadding), row: 3),
                    ]),
			};

			var tabIndex = 0;
            _lblPath.TabIndex = tabIndex++;
            pathDec.TabIndex = tabIndex++;
            _grpOptions.TabIndex = tabIndex++;
            _chkUseTemplate.TabIndex = tabIndex++;
			_txtTemplateDecorator.TabIndex = tabIndex++;
            _chkBare.TabIndex = tabIndex++;

            _lblPath.Parent = parent;
            pathDec.Parent = parent;
            _grpOptions.Parent = parent;
            _chkUseTemplate.Parent = parent;
			_txtTemplateDecorator.Parent = parent;
            _chkBare.Parent = parent;
        }
    }

	private readonly DialogControls _controls;
    private readonly IInitController _controller;

	public InitDialog(IGitRepositoryProvider gitRepositoryProvider)
	{
		Verify.Argument.IsNotNull(gitRepositoryProvider);

        Name = nameof(InitDialog);
        Text = Resources.StrInitRepository;

        SuspendLayout();
        AutoScaleDimensions = Dpi.Default;
        AutoScaleMode       = AutoScaleMode.Dpi;
        Size                = ScalableSize.GetValue(Dpi.Default);

		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);

		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			RepositoryPath    = new TextBoxInputSource(_controls._txtPath),
			Bare              = new CheckBoxWidgetInputSource(_controls._chkBare),
			UseCustomTemplate = new CheckBoxWidgetInputSource(_controls._chkUseTemplate),
			Template          = new TextBoxInputSource(_controls._txtTemplateDecorator.Decorated),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

        _controls._chkUseTemplate.IsCheckedChanged += _chkUseTemplate_CheckedChanged;

        _controller = new InitController(gitRepositoryProvider) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 106));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrInit;

	public IUserInputSource<string?> RepositoryPath { get; }

	public IUserInputSource<bool> Bare { get; }

	public IUserInputSource<bool> UseCustomTemplate { get; }

	public IUserInputSource<string?> Template { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtPath.Focus);
	}

	private void _chkUseTemplate_CheckedChanged(object? sender, EventArgs e)
	{
		bool check = _controls._chkUseTemplate.IsChecked;
		_controls._txtTemplateDecorator.Enabled = check;
	}

	public bool Execute() => _controller.TryInit();

	public Task<bool> ExecuteAsync() => _controller.TryInitAsync();
}

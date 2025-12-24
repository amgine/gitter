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
public partial class AddSubmoduleDialog : GitDialogBase, IAsyncExecutableDialog, IAddSubmoduleView
{
	readonly struct DialogControls
	{
		public readonly TextBox _txtRepository;
		public readonly TextBox _txtPath;
		public readonly TextBox _txtBranch;
		public readonly LabelControl _lblPath;
		public readonly LabelControl _lblUrl;
		public readonly ICheckBoxWidget _chkBranch;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_txtRepository = new();
			_txtPath       = new();
			_txtBranch     = new() { Enabled = false };
			_lblPath       = new();
			_lblUrl        = new();
			_chkBranch     = style.CheckBoxFactory.Create();

			GitterApplication.FontManager.InputFont.Apply(_txtBranch, _txtRepository, _txtPath);
		}

		public void Localize()
		{
			_lblPath.Text   = Resources.StrPath.AddColon();
			_lblUrl.Text    = Resources.StrUrl.AddColon();
			_chkBranch.Text = Resources.StrBranch.AddColon();
		}

		public void Layout(Control parent)
		{
			var pathDec   = new TextBoxDecorator(_txtPath);
			var urlDec    = new TextBoxDecorator(_txtRepository);
			var branchDec = new TextBoxDecorator(_txtBranch);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(100),
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblPath,   marginOverride: LayoutConstants.TextBoxLabelMargin), row: 0, column: 0),
						new GridContent(new ControlContent(pathDec,    marginOverride: LayoutConstants.TextBoxMargin),      row: 0, column: 1),
						new GridContent(new ControlContent(_lblUrl,    marginOverride: LayoutConstants.TextBoxLabelMargin), row: 1, column: 0),
						new GridContent(new ControlContent(urlDec,     marginOverride: LayoutConstants.TextBoxMargin),      row: 1, column: 1),
						new GridContent(new WidgetContent (_chkBranch, marginOverride: LayoutConstants.TextBoxLabelMargin), row: 2, column: 0),
						new GridContent(new ControlContent(branchDec,  marginOverride: LayoutConstants.TextBoxMargin),      row: 2, column: 1),
					]),
			};

			var tabIndex = 0;

			_lblPath.TabIndex   = tabIndex++;
			pathDec.TabIndex    = tabIndex++;
			_lblUrl.TabIndex    = tabIndex++;
			urlDec.TabIndex     = tabIndex++;
			_chkBranch.TabIndex = tabIndex++;
			branchDec.TabIndex  = tabIndex++;

			_lblPath.Parent   = parent;
			pathDec.Parent    = parent;
			_lblUrl.Parent    = parent;
			urlDec.Parent     = parent;
			_chkBranch.Parent = parent;
			branchDec.Parent  = parent;
		}
	}

	private readonly IAddSubmoduleController _controller;
	private readonly DialogControls _controls;

	/// <summary>Create <see cref="AddSubmoduleDialog"/>.</summary>
	public AddSubmoduleDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Name = nameof(AddSubmoduleDialog);
		Text = Resources.StrAddSubmodule;

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
			Path            = new TextBoxInputSource(_controls._txtPath),
			Url             = new TextBoxInputSource(_controls._txtRepository),
			UseCustomBranch = new CheckBoxWidgetInputSource(_controls._chkBranch),
			BranchName      = new TextBoxInputSource(_controls._txtBranch),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		_controls._chkBranch.IsCheckedChanged += OnBranchIsCheckedChanged;

		_controller = new AddSubmoduleController(repository) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(414, 82));

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrAdd;

	public IUserInputSource<string?> Path { get; }

	public IUserInputSource<string?> Url { get; }

	public IUserInputSource<bool> UseCustomBranch { get; }

	public IUserInputSource<string?> BranchName { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtPath.Focus);
	}

	private void OnBranchIsCheckedChanged(object? sender, EventArgs e)
	{
		_controls._txtBranch.Enabled = _controls._chkBranch.IsChecked;
	}

	/// <inheritdoc/>
	public Task<bool> ExecuteAsync() => _controller.TryAddSubmoduleAsync();
}

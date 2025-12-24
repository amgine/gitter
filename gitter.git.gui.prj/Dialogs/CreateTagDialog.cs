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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;
using gitter.Framework.Services;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Dialog for creating <see cref="Tag"/> object.</summary>
[ToolboxItem(false)]
public partial class CreateTagDialog : GitDialogBase, IExecutableDialog, ICreateTagView
{
	readonly struct DialogControls
	{
		public  readonly LabelControl _lblName;
		public  readonly TextBox _txtName;
		public  readonly LabelControl _lblRevision;
		public  readonly RevisionPicker _txtRevision;
		private readonly GroupSeparator _grpType;
		public  readonly IRadioButtonWidget _radAnnotated;
		public  readonly IRadioButtonWidget _radSimple;
		public  readonly IRadioButtonWidget _radSigned;
		private readonly GroupSeparator _grpMessage;
		public  readonly TextBox _txtMessage;
		private readonly GroupSeparator _grpSigning;
		public  readonly IRadioButtonWidget _radUseDefaultEmailKey;
		public  readonly IRadioButtonWidget _radUseKeyId;
		public  readonly TextBox _txtKeyId;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			var rbf = style.RadioButtonFactory;
			_txtName = new();
			_radAnnotated = rbf.Create();
			_radSimple = rbf.Create();
			_radSigned = rbf.Create();
			_lblRevision = new();
			_lblName = new();
			_grpType = new();
			_txtRevision = new();
			_txtKeyId = new();
			_radUseKeyId = rbf.Create();
			_radUseDefaultEmailKey = rbf.Create();
			_txtMessage = new();
			_grpMessage = new();
			_grpSigning = new();

			_radSimple.IsChecked = true;

			_txtMessage.AcceptsReturn = true;
			_txtMessage.AcceptsTab = true;
			_txtMessage.Enabled = false;
			_txtMessage.Multiline = true;

			_txtKeyId.Enabled = false;
			_radUseKeyId.Enabled = false;
			_radUseDefaultEmailKey.Enabled = false;
			_radUseDefaultEmailKey.IsChecked = true;
		}

		public void Localize()
		{
			_lblName.Text = Resources.StrName.AddColon();
			_lblRevision.Text = Resources.StrRevision.AddColon();

			_grpType.Text = Resources.StrType;
			_radSimple.Text = Resources.StrSimpleTag;
			_radAnnotated.Text = Resources.StrAnnotatedTag;
			_radSigned.Text = Resources.StrSigned;

			_grpMessage.Text = Resources.StrMessage;

			_grpSigning.Text = Resources.StrSigning;
			_radUseDefaultEmailKey.Text = Resources.StrUseDefaultEmailKey;
			_radUseKeyId.Text = Resources.StrUseKeyId.AddColon();
		}

		public void Layout(Control parent)
		{
			var nameDec      = new TextBoxDecorator(_txtName);
			var messageDec   = new TextBoxDecorator(_txtMessage);
			var keyIdDec     = new TextBoxDecorator(_txtKeyId);
			var panelTypes   = new Panel();
			var panelSigning = new Panel();

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
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.GroupSeparatorRowHeight,
						LayoutConstants.RadioButtonRowHeight,
						LayoutConstants.GroupSeparatorRowHeight,
						SizeSpec.Everything(),
						LayoutConstants.GroupSeparatorRowHeight,
						LayoutConstants.RadioButtonRowHeight,
						LayoutConstants.TextInputRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(_lblName,     marginOverride: DpiBoundValue.Padding(new(0, 6, 0, 6))), row: 0, column: 0),
						new GridContent(new ControlContent(nameDec,      marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lblRevision, marginOverride: DpiBoundValue.Padding(new(0, 6, 0, 6))), row: 1, column: 0),
						new GridContent(new ControlContent(_txtRevision, marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),
						new GridContent(new ControlContent(_grpType,     marginOverride: LayoutConstants.NoMargin), row: 2, columnSpan: 2),
						new GridContent(new ControlContent(panelTypes,   marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 3),
						new GridContent(new ControlContent(_grpMessage,  marginOverride: LayoutConstants.NoMargin), row: 4, columnSpan: 2),
						new GridContent(new ControlContent(messageDec,   marginOverride: DpiBoundValue.Padding(new(12, 0, 0, 0))), row: 5, columnSpan: 2),
						new GridContent(new ControlContent(_grpSigning,  marginOverride: LayoutConstants.NoMargin), row: 6, columnSpan: 2),
						new GridContent(new ControlContent(panelSigning, marginOverride: LayoutConstants.NoMargin), columnSpan: 2, row: 7, rowSpan: 2),
					]),
			};

			_ = new ControlLayout(panelTypes)
			{
				Content = new Grid(
					padding: LayoutConstants.GroupPadding,
					columns:
					[
						SizeSpec.Absolute(112),
						SizeSpec.Absolute(112),
						SizeSpec.Absolute(112),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new WidgetContent(_radSimple,    marginOverride: LayoutConstants.NoMargin), column: 0),
						new GridContent(new WidgetContent(_radAnnotated, marginOverride: LayoutConstants.NoMargin), column: 1),
						new GridContent(new WidgetContent(_radSigned,    marginOverride: LayoutConstants.NoMargin), column: 2),
					]),
			};

			_ = new ControlLayout(panelSigning)
			{
				Content = new Grid(
					padding: LayoutConstants.GroupPadding,
					rows:
					[
						LayoutConstants.RadioButtonRowHeight,
						LayoutConstants.TextInputRowHeight,
					],
					columns:
					[
						SizeSpec.Absolute(126),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new WidgetContent(_radUseDefaultEmailKey, marginOverride: LayoutConstants.NoMargin), columnSpan: 2),
						new GridContent(new WidgetContent(_radUseKeyId,           marginOverride: LayoutConstants.NoMargin), row: 1),
						new GridContent(new ControlContent(keyIdDec,              marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),
					]),
			};

			var tabIndex = 0;
			_lblName.TabIndex = tabIndex++;
			nameDec.TabIndex = tabIndex++;
			_lblRevision.TabIndex = tabIndex++;
			_txtRevision.TabIndex = tabIndex++;
			_grpType.TabIndex = tabIndex++;
			_radSimple.TabIndex = tabIndex++;
			_radAnnotated.TabIndex = tabIndex++;
			_radSigned.TabIndex = tabIndex++;
			panelTypes.TabIndex = tabIndex++;
			_grpMessage.TabIndex = tabIndex++;
			messageDec.TabIndex = tabIndex++;
			_grpSigning.TabIndex = tabIndex++;
			_radUseDefaultEmailKey.TabIndex = tabIndex++;
			_radUseKeyId.TabIndex = tabIndex++;
			keyIdDec.TabIndex = tabIndex++;
			panelSigning.TabIndex = tabIndex++;

			_lblName.Parent = parent;
			nameDec.Parent = parent;
			_lblRevision.Parent = parent;
			_txtRevision.Parent = parent;
			_grpType.Parent = parent;
			_radSimple.Parent = panelTypes;
			_radAnnotated.Parent = panelTypes;
			_radSigned.Parent = panelTypes;
			panelTypes.Parent = parent;
			_grpMessage.Parent = parent;
			messageDec.Parent = parent;
			_grpSigning.Parent = parent;
			_radUseDefaultEmailKey.Parent = panelSigning;
			_radUseKeyId.Parent = panelSigning;
			keyIdDec.Parent = panelSigning;
			panelSigning.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly Repository _repository;
	private TextBoxSpellChecker? _speller;
	private readonly ICreateTagController _controller;

	/// <summary>Create <see cref="CreateTagDialog"/>.</summary>
	/// <param name="repository"><see cref="Repository"/> to create <see cref="Tag"/> in.</param>
	/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
	public CreateTagDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		_repository = repository;

		Name = nameof(CreateTagDialog);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Layout(this);
		Localize();
		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			TagName   = new TextBoxInputSource(_controls._txtName),
			Revision  = new ControlInputSource(_controls._txtRevision),
			Message   = new TextBoxInputSource(_controls._txtMessage),
			Annotated = new RadioButtonWidgetInputSource(_controls._radAnnotated),
			Signed    = new RadioButtonWidgetInputSource(_controls._radSigned),
			UseKeyId  = new RadioButtonWidgetInputSource(_controls._radUseKeyId),
			KeyId     = new TextBoxInputSource(_controls._txtKeyId),
		};

		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_controls._txtName, ReferenceType.Tag);

		_controls._txtRevision.References.LoadData(
			_repository,
			ReferenceType.Reference,
			GlobalBehavior.GroupReferences,
			GlobalBehavior.GroupRemoteBranches);
		_controls._txtRevision.References.Items[0].IsExpanded = true;

		GitterApplication.FontManager.InputFont.Apply(_controls._txtKeyId, _controls._txtMessage, _controls._txtName, _controls._txtRevision);
		GlobalBehavior.SetupAutoCompleteSource((TextBox)_controls._txtRevision.Decorated, _repository, ReferenceType.Branch);
		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_controls._txtMessage, enable: true);
		}

		_controls._radSigned.IsCheckedChanged    += _radSigned_CheckedChanged;
		_controls._radAnnotated.IsCheckedChanged += _radAnnotated_CheckedChanged;
		_controls._radSimple.IsCheckedChanged    += _radSimple_CheckedChanged;
		_controls._radUseKeyId.IsCheckedChanged  += _radUseKeyId_CheckedChanged;

		_controller = new CreateTagController(repository) { View = this };
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DisposableUtility.Dispose(ref _speller);
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 364));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCreate;

	public IUserInputSource<string?> TagName { get; }

	public IUserInputSource<string> Revision { get; }

	public IUserInputSource<string?> Message { get; }

	public IUserInputSource<bool> Signed { get; }

	public IUserInputSource<bool> Annotated { get; }

	public IUserInputSource<bool> UseKeyId { get; }

	public IUserInputSource<string?> KeyId { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		BeginInvoke(_controls._txtName.Focus);
		base.OnLoad(e);
	}

	private void Localize()
	{
		Text = Resources.StrCreateTag;
		_controls.Localize();
	}

	private void SetControlStates()
	{
		bool signed = _controls._radSigned.IsChecked;
		bool annotated = signed || _controls._radAnnotated.IsChecked;
		_controls._txtMessage.Enabled = annotated;
		_controls._radUseDefaultEmailKey.Enabled = signed;
		_controls._radUseKeyId.Enabled = signed;
		_controls._txtKeyId.Enabled = signed & _controls._radUseKeyId.IsChecked;
	}

	private void _radSimple_CheckedChanged(object? sender, EventArgs e)
	{
		SetControlStates();
	}

	private void _radAnnotated_CheckedChanged(object? sender, EventArgs e)
	{
		SetControlStates();
	}

	private void _radSigned_CheckedChanged(object? sender, EventArgs e)
	{
		SetControlStates();
	}

	private void _radUseKeyId_CheckedChanged(object? sender, EventArgs e)
	{
		_controls._txtKeyId.Enabled = _controls._radUseKeyId.IsChecked;
	}

	/// <summary>Create <see cref="Tag"/>.</summary>
	/// <returns>true, if <see cref="Tag"/> was created successfully.</returns>
	public bool Execute() => _controller.TryCreateTag();
}

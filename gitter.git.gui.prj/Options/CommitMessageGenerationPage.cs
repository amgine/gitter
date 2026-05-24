#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 */
#endregion

namespace gitter.Git;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Options;
using gitter.Git.Gui;

[ToolboxItem(false)]
sealed class CommitMessageGenerationPage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("5B6A5620-4271-4E4E-B84C-3B0138C026BB");

	private readonly RepositoryProvider _repositoryProvider;
	private readonly IValueSource<CommitMessageGenerationOptions> _optionsSource;
	private readonly TextBox _txtApiKey = new() { UseSystemPasswordChar = true };
	private readonly TextBox _txtBaseAddress = new();
	private readonly TextBox _txtModel = new();
	private readonly TextBox _txtLanguage = new();
	private readonly TextBox _txtPrompt = new() { Multiline = true, AcceptsReturn = true, ScrollBars = ScrollBars.Vertical };
	private readonly TextBoxDecoratorWithUpDown _numMaxTokens = new(new() { TextAlign = HorizontalAlignment.Right }) { Minimum = 1, Maximum = 4000 };
	private readonly ICheckBoxWidget _chkSingleLine = GitterApplication.Style.CheckBoxFactory.Create();
	private readonly TextBoxDecorator _decApiKey;
	private readonly TextBoxDecorator _decBaseAddress;
	private readonly TextBoxDecorator _decModel;
	private readonly TextBoxDecorator _decLanguage;
	private readonly TextBoxDecorator _decPrompt;

	public CommitMessageGenerationPage(RepositoryProvider repositoryProvider, IValueSource<CommitMessageGenerationOptions> optionsSource)
		: base(Guid)
	{
		Verify.Argument.IsNotNull(repositoryProvider);
		Verify.Argument.IsNotNull(optionsSource);

		_repositoryProvider = repositoryProvider;
		_optionsSource = optionsSource;
		Text = "AI Commit";
		Name = nameof(CommitMessageGenerationPage);

		GitterApplication.FontManager.InputFont.Apply(_txtApiKey);
		GitterApplication.FontManager.InputFont.Apply(_txtBaseAddress);
		GitterApplication.FontManager.InputFont.Apply(_txtModel);
		GitterApplication.FontManager.InputFont.Apply(_txtLanguage);
		GitterApplication.FontManager.InputFont.Apply(_txtPrompt);
		_chkSingleLine.Text = "Generate single-line commit messages";
		_decApiKey      = new(_txtApiKey);
		_decBaseAddress = new(_txtBaseAddress);
		_decModel       = new(_txtModel);
		_decLanguage    = new(_txtLanguage);
		_decPrompt      = new(_txtPrompt);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Size = ScalableSize.GetValue(Dpi.Default);
		LayoutControls();
		ResumeLayout(performLayout: false);
		PerformLayout();
	}

	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(560, 360));

	protected override bool ScaleChildren => false;

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		Display(_optionsSource.Value);
	}

	private void LayoutControls()
	{
		var labels = new[]
		{
			CreateLabel("API key:"),
			CreateLabel("Base address:"),
			CreateLabel("Model:"),
			CreateLabel("Language:"),
			CreateLabel("Max tokens:"),
			CreateLabel("Prompt:"),
		};

		_ = new ControlLayout(this)
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
					LayoutConstants.TextInputRowHeight,
					LayoutConstants.TextInputRowHeight,
					LayoutConstants.CheckBoxRowHeight,
					LayoutConstants.LabelRowHeight,
					SizeSpec.Everything(),
				],
				content:
				[
					new GridContent(new ControlContent(labels[0], marginOverride: LayoutConstants.TextBoxLabelMargin), column: 0, row: 0),
					new GridContent(new ControlContent(_decApiKey, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
					new GridContent(new ControlContent(labels[1], marginOverride: LayoutConstants.TextBoxLabelMargin), column: 0, row: 1),
					new GridContent(new ControlContent(_decBaseAddress, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 1),
					new GridContent(new ControlContent(labels[2], marginOverride: LayoutConstants.TextBoxLabelMargin), column: 0, row: 2),
					new GridContent(new ControlContent(_decModel, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 2),
					new GridContent(new ControlContent(labels[3], marginOverride: LayoutConstants.TextBoxLabelMargin), column: 0, row: 3),
					new GridContent(new ControlContent(_decLanguage, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 3),
					new GridContent(new ControlContent(labels[4], marginOverride: LayoutConstants.TextBoxLabelMargin), column: 0, row: 4),
					new GridContent(new ControlContent(_numMaxTokens, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 4),
					new GridContent(new WidgetContent(_chkSingleLine, marginOverride: LayoutConstants.NoMargin), column: 1, row: 5),
					new GridContent(new ControlContent(labels[5], marginOverride: LayoutConstants.NoMargin), column: 0, row: 6),
					new GridContent(new ControlContent(_decPrompt, marginOverride: LayoutConstants.TextBoxMargin), column: 0, row: 7, columnSpan: 2),
				]),
		};

		var tabIndex = 0;
		labels[0].TabIndex = tabIndex++;
		_decApiKey.TabIndex = tabIndex++;
		labels[1].TabIndex = tabIndex++;
		_decBaseAddress.TabIndex = tabIndex++;
		labels[2].TabIndex = tabIndex++;
		_decModel.TabIndex = tabIndex++;
		labels[3].TabIndex = tabIndex++;
		_decLanguage.TabIndex = tabIndex++;
		labels[4].TabIndex = tabIndex++;
		_numMaxTokens.TabIndex = tabIndex++;
		_chkSingleLine.TabIndex = tabIndex++;
		labels[5].TabIndex = tabIndex++;
		_decPrompt.TabIndex = tabIndex++;

		_decApiKey.Parent = this;
		_decBaseAddress.Parent = this;
		_decModel.Parent = this;
		_decLanguage.Parent = this;
		_numMaxTokens.Parent = this;
		_chkSingleLine.Parent = this;
		_decPrompt.Parent = this;
	}

	private LabelControl CreateLabel(string text)
		=> new() { Text = text, Parent = this };

	private void Display(CommitMessageGenerationOptions options)
	{
		options = options.Normalize();
		_txtApiKey.Text = options.ApiKey;
		_txtBaseAddress.Text = options.BaseAddress;
		_txtModel.Text = options.Model;
		_txtLanguage.Text = options.Language;
		_txtPrompt.Text = options.PromptText;
		_numMaxTokens.Value = Math.Max(_numMaxTokens.Minimum, Math.Min(_numMaxTokens.Maximum, options.MaxTokens));
		_chkSingleLine.IsChecked = options.SingleLine;
	}

	private CommitMessageGenerationOptions MakeSnapshot()
		=> new CommitMessageGenerationOptions(
			ApiKey:      _txtApiKey.Text.Trim(),
			BaseAddress: _txtBaseAddress.Text.Trim(),
			Model:       _txtModel.Text.Trim(),
			Language:    _txtLanguage.Text.Trim(),
			PromptText:  _txtPrompt.Text.Trim(),
			MaxTokens:   _numMaxTokens.Value,
			SingleLine:  _chkSingleLine.IsChecked).Normalize();

	public bool Execute()
	{
		var options = MakeSnapshot();
		_optionsSource.Value = options;
		var section = _repositoryProvider.ConfigSection?.GetCreateSection(CommitMessageGenerationOptions.SectionName);
		if(section is not null)
		{
			CommitMessageGenerationOptions.SaveTo(options, section);
		}
		return true;
	}
}

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
using gitter.Framework.Services;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Dialog for stashing working directory changes.</summary>
[ToolboxItem(false)]
public partial class StashSaveDialog : GitDialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		private readonly LabelControl    _lblMessage;
		public  readonly TextBox         _txtMessage;
		public  readonly ICheckBoxWidget _chkKeepIndex;
		public  readonly ICheckBoxWidget _chkIncludeUntrackedFiles;
		private readonly GroupSeparator  _grpOptions;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_lblMessage = new();
			_txtMessage = new()
			{
				AcceptsReturn = true,
				AcceptsTab = true,
				Multiline = true,
			};
			_grpOptions = new();
			var cbf = style.CheckBoxFactory;
			_chkKeepIndex             = cbf.Create();
			_chkIncludeUntrackedFiles = cbf.Create();

			GitterApplication.FontManager.InputFont.Apply(_txtMessage);
		}

		public void Localize(Repository repository)
		{
			_lblMessage.Text = Resources.StrOptionalMessage.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_chkKeepIndex.Text = Resources.StrKeepIndex;
			_chkIncludeUntrackedFiles.Text = Resources.StrsIncludeUntrackedFiles;
			if(!GitFeatures.StashIncludeUntrackedOption.IsAvailableFor(repository))
			{
				_chkIncludeUntrackedFiles.Enabled = false;
				_chkIncludeUntrackedFiles.Text += " " +
					Resources.StrfVersionRequired
								.UseAsFormat(GitFeatures.StashIncludeUntrackedOption.RequiredVersion)
								.SurroundWithBraces();
			}
		}

		public void Layout(Control parent)
		{
			var messageDec = new TextBoxDecorator(_txtMessage);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
						LayoutConstants.GroupSeparatorRowHeight,
						LayoutConstants.CheckBoxRowHeight,
						LayoutConstants.CheckBoxRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(_lblMessage,               marginOverride: LayoutConstants.NoMargin),     row: 0),
						new GridContent(new ControlContent(messageDec,                marginOverride: LayoutConstants.NoMargin),     row: 2),
						new GridContent(new ControlContent(_grpOptions,               marginOverride: LayoutConstants.NoMargin),     row: 3),
						new GridContent(new WidgetContent (_chkKeepIndex,             marginOverride: LayoutConstants.GroupPadding), row: 4),
						new GridContent(new WidgetContent (_chkIncludeUntrackedFiles, marginOverride: LayoutConstants.GroupPadding), row: 5),
					]),
			};

			var tabIndex = 0;
			_lblMessage.TabIndex = tabIndex++;
			messageDec.TabIndex = tabIndex++;
			_grpOptions.TabIndex = tabIndex++;
			_chkKeepIndex.TabIndex = tabIndex++;
			_chkIncludeUntrackedFiles.TabIndex = tabIndex++;

			_lblMessage.Parent               = parent;
			messageDec.Parent                = parent;
			_grpOptions.Parent               = parent;
			_chkKeepIndex.Parent             = parent;
			_chkIncludeUntrackedFiles.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private TextBoxSpellChecker? _speller;

	/// <summary>Create <see cref="StashSaveDialog"/>.</summary>
	/// <param name="repository">Repository for performing stash save.</param>
	/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
	public StashSaveDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		Name = nameof(StashSaveDialog);
		Text = Resources.StrStashSave;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize(repository);
		_controls.Layout(this);
		ResumeLayout(false);
		PerformLayout();

		ToolTipService.Register(_controls._chkKeepIndex.Control, Resources.TipStashKeepIndex);

		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_controls._txtMessage, true);
		}
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

	public Repository Repository { get; private set; }

	/// <summary>Do not stash staged changes.</summary>
	public bool KeepIndex
	{
		get => _controls._chkKeepIndex.IsChecked;
		set => _controls._chkKeepIndex.IsChecked = value;
	}

	/// <summary>Include untracked files in stash.</summary>
	public bool IncludeUntrackedFiles
	{
		get => _controls._chkIncludeUntrackedFiles.IsChecked;
		set => _controls._chkIncludeUntrackedFiles.IsChecked = value;
	}

	/// <summary>Custom commit message (optional).</summary>
	public string Message
	{
		get => _controls._txtMessage.Text;
		set => _controls._txtMessage.Text = value;
	}

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrSave;

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 156));

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtMessage.Focus);
	}

	/// <summary>Perform stash save.</summary>
	/// <returns>true if stash save succeeded.</returns>
	public bool Execute()
	{
		bool keepIndex = KeepIndex;
		bool includeUntracked =
			GitFeatures.StashIncludeUntrackedOption.IsAvailableFor(Repository) &&
			IncludeUntrackedFiles;
		var message = Message ?? "";
		message = message.Trim();

		if(GuiCommands.SaveStash(this, Repository.Stash, keepIndex, includeUntracked, message) == GuiCommandStatus.Faulted)
		{
			return false;
		}
		return true;
	}
}

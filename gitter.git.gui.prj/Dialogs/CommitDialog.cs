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
using System.Drawing;
using System.Threading.Tasks;
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

/// <summary>Dialog for creating commit.</summary>
public partial class CommitDialog : GitDialogBase, IExecutableDialog, IAsyncExecutableDialog, ICommitView
{
	readonly struct DialogControls
	{
		public  readonly TreeListBox _lstStaged;
		public  readonly TextBox _txtMessage;
		private readonly LabelControl _lblStagedFiles;
		private readonly LabelControl _lblMessage;
		public  readonly ICheckBoxWidget _chkAmend;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			_lstStaged = new TreeListBox
			{
				HeaderStyle         = HeaderStyle.Hidden,
				DisableContextMenus = true,
				ShowTreeLines       = true,
			};
			for(int i = 0; i < _lstStaged.Columns.Count; ++i)
			{
				var col = _lstStaged.Columns[i];
				if(col.Id != (int)ColumnId.Name)
				{
					col.IsVisible = false;
					continue;
				}
				col.IsVisible = col.Id == (int)ColumnId.Name;
				col.SizeMode  = ColumnSizeMode.Auto;
			}

			_txtMessage = new TextBox
			{
				AcceptsReturn = true,
				AcceptsTab    = true,
				Multiline     = true,
				//ScrollBars    = ScrollBars.Vertical,
			};
			GitterApplication.FontManager.InputFont.Apply(_txtMessage);

			_lblStagedFiles = new()
			{
			};
			_lblMessage = new()
			{
			};
			_chkAmend = style.CheckBoxFactory.Create();
		}

		public void Localize()
		{
			_lblMessage.Text     = Resources.StrMessage.AddColon();
			_lblStagedFiles.Text = Resources.StrsStagedChanges.AddColon();
			_chkAmend.Text       = Resources.StrAmend;
		}

		public void Layout(Control parent)
		{
			var messageDecorator = new TextBoxDecorator(_txtMessage);

			var noMargin = DpiBoundValue.Constant(Padding.Empty);
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(230),
						SizeSpec.Absolute(4),
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
						SizeSpec.Absolute(4),
						LayoutConstants.CheckBoxRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(_lblStagedFiles,  marginOverride: noMargin), column: 0, row: 0),
						new GridContent(new ControlContent(_lstStaged,       marginOverride: noMargin), column: 0, row: 2, rowSpan: 3),
						new GridContent(new ControlContent(_lblMessage,      marginOverride: noMargin), column: 2, row: 0),
						new GridContent(new ControlContent(messageDecorator, marginOverride: noMargin), column: 2, row: 2),
						new GridContent(new WidgetContent(_chkAmend,         marginOverride: noMargin), column: 2, row: 4),
					]),
			};

			var tabIndex = 0;
			_lblStagedFiles.TabIndex  = tabIndex++;
			_lstStaged.TabIndex       = tabIndex++;
			_lblMessage.TabIndex      = tabIndex++;
			messageDecorator.TabIndex = tabIndex++;
			_chkAmend.TabIndex        = tabIndex++;

			_lblStagedFiles.Parent  = parent;
			_lstStaged.Parent       = parent;
			_lblMessage.Parent      = parent;
			messageDecorator.Parent = parent;
			_chkAmend.Parent        = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly ICommitController _controller;
	private readonly TextBoxSpellChecker? _speller;

	public CommitDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Name = nameof(CommitDialog);

		Repository = repository;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Layout(this);
		Localize();
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._chkAmend.IsCheckedChanged += OnAmendCheckedChanged;

		var inputs = new IUserInputSource[]
		{
			Message     = new TextBoxInputSource(_controls._txtMessage),
			Amend       = new CheckBoxWidgetInputSource(_controls._chkAmend),
			StagedItems = new ControlInputSource(_controls._lstStaged),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		_controls._lstStaged.SetTree(repository.Status.StagedRoot, TreeListBoxMode.ShowFullTree);
		_controls._lstStaged.ExpandAll();

		_controls._chkAmend.Enabled = !repository.Head.IsEmpty;

		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_controls._txtMessage, true);
		}

		_controls._txtMessage.Text = repository.Status.LoadCommitMessage();

		_controller = new CommitController(repository) { View = this };
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_speller?.Dispose();
		}
		base.Dispose(disposing);
	}

	public Repository Repository { get; }

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(642, 359));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCommit;

	public IUserInputSource<string?> Message { get; }

	public IUserInputSource<bool> Amend { get; }

	public IUserInputSource StagedItems { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtMessage.Focus);
	}

	private void Localize()
	{
		Text = Resources.StrCommitChanges;
		_controls.Localize();
	}

	/// <inheritdoc/>
	protected override void OnClosed(DialogResult result)
	{
		Repository.Status.SaveCommitMessage(result != DialogResult.OK
			? _controls._txtMessage.Text
			: string.Empty);
	}

	private void OnAmendCheckedChanged(object? sender, EventArgs e)
	{
		if(_controls._chkAmend.IsChecked && _controls._txtMessage.TextLength == 0)
		{
			var rev = Repository.Head.Revision;
			if(rev is not null)
			{
				_controls._txtMessage.AppendText(Utility.ExpandNewLineCharacters(rev.Subject));
				if(!string.IsNullOrEmpty(rev.Body))
				{
					_controls._txtMessage.AppendText(Environment.NewLine);
					_controls._txtMessage.AppendText(Environment.NewLine);
					_controls._txtMessage.AppendText(Utility.ExpandNewLineCharacters(rev.Body));
				}
				_controls._txtMessage.SelectAll();
			}
		}
	}

	/// <inheritdoc/>
	public bool Execute() => _controller.TryCommit();

	/// <inheritdoc/>
	public Task<bool> ExecuteAsync() => _controller.TryCommitAsync();
}

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
	private TextBoxSpellChecker _speller;
	private readonly ICommitController _controller;

	public CommitDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		InitializeComponent();
		Localize();

		var inputs = new IUserInputSource[]
		{
			Message     = new TextBoxInputSource(_txtMessage),
			Amend       = new CheckBoxInputSource(_chkAmend),
			StagedItems = new ControlInputSource(_lstStaged),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		for(int i = 0; i < _lstStaged.Columns.Count; ++i)
		{
			var col = _lstStaged.Columns[i];
			col.IsVisible = col.Id == (int)ColumnId.Name;
		}

		_lstStaged.Columns[0].SizeMode = ColumnSizeMode.Auto;
		_lstStaged.Style = GitterApplication.DefaultStyle;
		_lstStaged.SetTree(repository.Status.StagedRoot, TreeListBoxMode.ShowFullTree);
		_lstStaged.ExpandAll();

		_chkAmend.Enabled = !repository.Head.IsEmpty;

		GitterApplication.FontManager.InputFont.Apply(_txtMessage);
		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_txtMessage, true);
		}

		_txtMessage.Text = repository.Status.LoadCommitMessage();
		_txtMessage.Height = _chkAmend.Top - _txtMessage.Top - 2;

		_controller = new CommitController(repository) { View = this };
	}

	public Repository Repository { get; }

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(642, 359));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCommit;

	public IUserInputSource<string> Message { get; }

	public IUserInputSource<bool> Amend { get; }

	public IUserInputSource StagedItems { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_txtMessage.Focus);
	}

	private void Localize()
	{
		Text                 = Resources.StrCommitChanges;
		_lblMessage.Text     = Resources.StrMessage.AddColon();
		_lblStagedFiles.Text = Resources.StrsStagedChanges.AddColon();
		_chkAmend.Text       = Resources.StrAmend;
	}

	/// <inheritdoc/>
	protected override void OnClosed(DialogResult result)
	{
		Repository.Status.SaveCommitMessage(result != DialogResult.OK
			? _txtMessage.Text
			: string.Empty);
	}

	private void OnAmendCheckedChanged(object sender, EventArgs e)
	{
		if(_chkAmend.Checked && _txtMessage.TextLength == 0)
		{
			var rev = Repository.Head.Revision;
			_txtMessage.AppendText(Utility.ExpandNewLineCharacters(rev.Subject));
			if(!string.IsNullOrEmpty(rev.Body))
			{
				_txtMessage.AppendText(Environment.NewLine);
				_txtMessage.AppendText(Environment.NewLine);
				_txtMessage.AppendText(Utility.ExpandNewLineCharacters(rev.Body));
			}
			_txtMessage.SelectAll();
		}
	}

	/// <inheritdoc/>
	public bool Execute() => _controller.TryCommit();

	/// <inheritdoc/>
	public Task<bool> ExecuteAsync() => _controller.TryCommitAsync();
}

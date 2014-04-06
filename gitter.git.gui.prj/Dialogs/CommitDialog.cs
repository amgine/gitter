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

namespace gitter.Git.Gui.Dialogs
{
	using System;
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
	public partial class CommitDialog : GitDialogBase, IExecutableDialog, ICommitView
	{
		#region Data

		private readonly Repository _repository;
		private TextBoxSpellChecker _speller;
		private readonly IUserInputSource<string> _messageInput;
		private readonly IUserInputSource<bool> _amendInput;
		private readonly IUserInputSource _stagedItemsInput;
		private readonly IUserInputErrorNotifier _errorNotifier;
		private readonly ICommitController _controller;

		#endregion

		#region .ctor

		public CommitDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();
			Localize();

			var inputs = new IUserInputSource[]
			{
				_messageInput     = new TextBoxInputSource(_txtMessage),
				_amendInput       = new CheckBoxInputSource(_chkAmend),
				_stagedItemsInput = new ControlInputSource(_lstStaged),
			};
			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

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

		#endregion

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrCommit; }
		}

		public IUserInputSource<string> Message
		{
			get { return _messageInput; }
		}

		public IUserInputSource<bool> Amend
		{
			get { return _amendInput; }
		}

		public IUserInputSource StagedItems
		{
			get { return _stagedItemsInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

		#endregion

		#region Methods

		private void Localize()
		{
			Text = Resources.StrCommitChanges;
			_lblMessage.Text = Resources.StrMessage.AddColon();
			_lblStagedFiles.Text = Resources.StrsStagedChanges.AddColon();
			_chkAmend.Text = Resources.StrAmend;
		}

		protected override void OnClosed(DialogResult result)
		{
			if(result != DialogResult.OK)
			{
				_repository.Status.SaveCommitMessage(_txtMessage.Text);
			}
			else
			{
				_repository.Status.SaveCommitMessage(string.Empty);
			}
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

		#endregion

		#region IExecutableDialog Members

		public bool Execute()
		{
			return _controller.TryCommit();
		}

		#endregion
	}
}

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Dialog for creating commit.</summary>
	public partial class CommitDialog : GitDialogBase, IExecutableDialog
	{
		#region Data

		private readonly Repository _repository;
		private TextBoxSpellChecker _speller;

		#endregion

		public CommitDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();
			Localize();

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
		}

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

		public Repository Repository
		{
			get { return _repository; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrCommit; }
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

		#region IExecutableDialog Members

		public bool Execute()
		{
			string message = _txtMessage.Text.Trim();
			bool amend = _chkAmend.Checked;
			if(_lstStaged.Items.Count == 0 && !amend)
			{
				NotificationService.NotifyInputError(
					_lstStaged,
					Resources.ErrNothingToCommit,
					Resources.ErrNofilesStagedForCommit);
				return false;
			}
			if(message.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtMessage,
					Resources.ErrEmptyCommitMessage,
					Resources.ErrEnterCommitMessage);
				return false;
			}
			else if(message.Length < 2)
			{
				NotificationService.NotifyInputError(
					_txtMessage,
					Resources.ErrShortCommitMessage,
					Resources.ErrEnterLongerCommitMessage.UseAsFormat(2));
				return false;
			}
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					_repository.Status.Commit(message, amend);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToCommit,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}

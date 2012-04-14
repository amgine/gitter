namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Dialog for creating <see cref="Note"/> object.</summary>
	[ToolboxItem(false)]
	public partial class AddNoteDialog : GitDialogBase, IExecutableDialog
	{
		private Repository _repository;
		private TextBoxSpellChecker _speller;

		/// <summary>Create <see cref="AddNoteDialog"/>.</summary>
		/// <param name="repository">Repository to create note in.</param>
		public AddNoteDialog(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			InitializeComponent();

			Text = Resources.StrAddNote;

			_txtRevision.References.LoadData(
				_repository,
				ReferenceType.Reference,
				GlobalBehavior.GroupReferences,
				GlobalBehavior.GroupRemoteBranches);
			_txtRevision.References.Items[0].IsExpanded = true;

			_txtRevision.Text = GitConstants.HEAD;

			_lblRevision.Text = Resources.StrRevision.AddColon();
			_lblMessage.Text = Resources.StrMessage.AddColon();

			GitterApplication.FontManager.InputFont.Apply(_txtRevision, _txtMessage);
			if(SpellingService.Enabled)
			{
				_speller = new TextBoxSpellChecker(_txtMessage, true);
			}
		}

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public Repository Repository
		{
			get { return _repository; }
		}

		public string Revision
		{
			get { return _txtRevision.Text; }
			set { _txtRevision.Text = value; }
		}

		public bool AllowChangeRevision
		{
			get { return _txtRevision.Enabled; }
			set { _txtRevision.Enabled = value; }
		}

		public string Message
		{
			get { return _txtMessage.Text; }
			set { _txtMessage.Text = value; }
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			var revision = _txtRevision.Text.Trim();
			var message = _txtMessage.Text.Trim();
			if(revision.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtRevision,
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionCannotBeEmpty);
				return false;
			}
			if(message.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtMessage,
					Resources.ErrInvalidMessage,
					Resources.ErrMessageCannotBeEmpty);
				return false;
			}
			try
			{
				Cursor = Cursors.WaitCursor;
				var ptr = _repository.CreateRevisionPointer(revision);
				ptr.AddNote(message);
				Cursor = Cursors.Default;
			}
			catch(UnknownRevisionException)
			{
				Cursor = Cursors.Default;
				NotificationService.NotifyInputError(
					_txtRevision,
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown);
				return false;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToAddNote,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}

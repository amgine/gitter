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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Gui.Properties.Resources;

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
			Verify.Argument.IsNotNull(repository, "repository");

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
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					var ptr = _repository.GetRevisionPointer(revision);
					ptr.AddNote(message);
				}
			}
			catch(UnknownRevisionException)
			{
				NotificationService.NotifyInputError(
					_txtRevision,
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown);
				return false;
			}
			catch(GitException exc)
			{
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

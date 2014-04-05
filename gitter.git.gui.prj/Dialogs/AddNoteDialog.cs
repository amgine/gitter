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

	using gitter.Framework;
	using gitter.Framework.Mvc;
	using gitter.Framework.Mvc.WinForms;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controllers;
	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Dialog for creating <see cref="Note"/> object.</summary>
	[ToolboxItem(false)]
	public partial class AddNoteDialog : GitDialogBase, IExecutableDialog, IAddNoteView
	{
		#region Data

		private Repository _repository;
		private TextBoxSpellChecker _speller;
		private readonly IUserInputSource<string> _revisionInput;
		private readonly IUserInputSource<string> _messageInput;
		private readonly IUserInputErrorNotifier _errorNotifier;
		private readonly IAddNoteController _controller;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="AddNoteDialog"/>.</summary>
		/// <param name="repository">Repository to create note in.</param>
		public AddNoteDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			var inputs = new IUserInputSource[]
			{
				_revisionInput = new ControlInputSource(_txtRevision),
				_messageInput  = new TextBoxInputSource(_txtMessage),
			};
			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

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
			_controller = new AddNoteController(repository) { View = this };
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public Repository Repository
		{
			get { return _repository; }
		}

		public IUserInputSource<string> Revision
		{
			get { return _revisionInput; }
		}

		public IUserInputSource<string> Message
		{
			get { return _messageInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

		#endregion

		#region IExecutableDialog Members

		public bool Execute()
		{
			return _controller.TryAddNote();
		}

		#endregion
	}
}

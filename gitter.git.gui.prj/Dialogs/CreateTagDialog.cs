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

	/// <summary>Dialog for creating <see cref="Tag"/> object.</summary>
	[ToolboxItem(false)]
	public partial class CreateTagDialog : GitDialogBase, IExecutableDialog, ICreateTagView
	{
		#region Data

		private Repository _repository;
		private TextBoxSpellChecker _speller;
		private readonly ICreateTagController _controller;
		private readonly IUserInputSource<string> _tagNameInput;
		private readonly IUserInputSource<string> _revisionInput;
		private readonly IUserInputSource<string> _messageInput;
		private readonly IUserInputSource<bool> _annotatedInput;
		private readonly IUserInputSource<bool> _signedInput;
		private readonly IUserInputSource<bool> _useKeyIdInput;
		private readonly IUserInputSource<string> _keyIdInput;
		private readonly IUserInputErrorNotifier _errorNotifier;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="CreateTagDialog"/>.</summary>
		/// <param name="repository"><see cref="Repository"/> to create <see cref="Tag"/> in.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		public CreateTagDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();
			Localize();

			var inputs = new IUserInputSource[]
			{
				_tagNameInput   = new TextBoxInputSource(_txtName),
				_revisionInput  = new ControlInputSource(_txtRevision),
				_messageInput   = new TextBoxInputSource(_txtMessage),
				_annotatedInput = new RadioButtonInputSource(_radAnnotated),
				_signedInput    = new RadioButtonInputSource(_radSigned),
				_useKeyIdInput  = new RadioButtonInputSource(_radUseKeyId),
				_keyIdInput     = new TextBoxInputSource(_txtKeyId),
			};

			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

			SetupReferenceNameInputBox(_txtName, ReferenceType.Tag);

			_txtRevision.References.LoadData(
				_repository,
				ReferenceType.Reference,
				GlobalBehavior.GroupReferences,
				GlobalBehavior.GroupRemoteBranches);
			_txtRevision.References.Items[0].IsExpanded = true;

			GitterApplication.FontManager.InputFont.Apply(_txtKeyId, _txtMessage, _txtName, _txtRevision);
			GlobalBehavior.SetupAutoCompleteSource(_txtRevision, _repository, ReferenceType.Branch);
			if(SpellingService.Enabled)
			{
				_speller = new TextBoxSpellChecker(_txtMessage, true);
			}

			_controller = new CreateTagController(repository) { View = this };
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrCreate; }
		}

		public IUserInputSource<string> TagName
		{
			get { return _tagNameInput; }
		}

		public IUserInputSource<string> Revision
		{
			get { return _revisionInput; }
		}

		public IUserInputSource<string> Message
		{
			get { return _messageInput; }
		}

		public IUserInputSource<bool> Signed
		{
			get { return _signedInput; }
		}

		public IUserInputSource<bool> Annotated
		{
			get { return _annotatedInput; }
		}

		public IUserInputSource<bool> UseKeyId
		{
			get { return _useKeyIdInput; }
		}

		public IUserInputSource<string> KeyId
		{
			get { return _keyIdInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

		#endregion

		#region Methods

		private void Localize()
		{
			Text				= Resources.StrCreateTag;

			_lblName.Text		= Resources.StrName.AddColon();
			_lblRevision.Text	= Resources.StrRevision.AddColon();

			_grpOptions.Text	= Resources.StrType;
			_radSimple.Text		= Resources.StrSimpleTag;
			_radAnnotated.Text	= Resources.StrAnnotatedTag;
			_radSigned.Text		= Resources.StrSigned;
			
			_grpMessage.Text	= Resources.StrMessage;
			
			_grpSigning.Text	= Resources.StrSigning;
			_radUseDefaultEmailKey.Text = Resources.StrUseDefaultEmailKey;
			_radUseKeyId.Text	= Resources.StrUseKeyId.AddColon();
		}

		private void SetControlStates()
		{
			bool signed = _radSigned.Checked;
			bool annotated = signed || _radAnnotated.Checked;
			_txtMessage.Enabled = annotated;
			_radUseDefaultEmailKey.Enabled = signed;
			_radUseKeyId.Enabled = signed;
			_txtKeyId.Enabled = signed & _radUseKeyId.Checked;
		}

		private void _radSimple_CheckedChanged(object sender, EventArgs e)
		{
			SetControlStates();
		}

		private void _radAnnotated_CheckedChanged(object sender, EventArgs e)
		{
			SetControlStates();
		}

		private void _radSigned_CheckedChanged(object sender, EventArgs e)
		{
			SetControlStates();
		}

		private void _radUseKeyId_CheckedChanged(object sender, EventArgs e)
		{
			_txtKeyId.Enabled = _radUseKeyId.Checked;
		}

		#endregion

		#region IExecutableDialog

		/// <summary>Create <see cref="Tag"/>.</summary>
		/// <returns>true, if <see cref="Tag"/> was created successfully.</returns>
		public bool Execute()
		{
			return _controller.TryCreateTag();
		}

		#endregion
	}
}

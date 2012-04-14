namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Dialog for creating <see cref="Tag"/> object.</summary>
	[ToolboxItem(false)]
	public partial class CreateTagDialog : GitDialogBase, IExecutableDialog
	{
		#region Data

		private Repository _repository;
		private TextBoxSpellChecker _speller;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="CreateTagDialog"/>.</summary>
		/// <param name="repository"><see cref="Repository"/> to create <see cref="Tag"/> in.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		public CreateTagDialog(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			InitializeComponent();

			Localize();

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
		}

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

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrCreate; }
		}

		/// <summary>Tagged revision.</summary>
		public string Revision
		{
			get { return _txtRevision.Text; }
			set { _txtRevision.Text = value; }
		}

		/// <summary>Allow user to change starting revision.</summary>
		public bool AllowChangingRevision
		{
			get { return _txtRevision.Enabled; }
			set { _txtRevision.Enabled = value; }
		}

		/// <summary>Tag name.</summary>
		public string TagName
		{
			get { return _txtName.Text; }
			set { _txtName.Text = value; }
		}

		/// <summary>Allow user to edit <see cref="M:TagName"/>.</summary>
		public bool AllowChangingTagName
		{
			get { return !_txtName.ReadOnly; }
			set { _txtName.ReadOnly = !value; }
		}

		/// <summary>Make signed tag.</summary>
		public bool Signed
		{
			get { return _radSigned.Checked; }
			set { _radSigned.Checked = true; }
		}

		/// <summary>GPG key id.</summary>
		public string KeyId
		{
			get { return _txtKeyId.Text; }
			set { _txtKeyId.Text = value; }
		}

		/// <summary>Allow user to change <see cref="M:KeyId"/>.</summary>
		public bool AllowChangingKeyId
		{
			get { return !_txtKeyId.ReadOnly; }
			set { _txtKeyId.ReadOnly = !value; }
		}

		#endregion

		#region Event Handlers

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
			var name	= _txtName.Text.Trim();
			var refspec	= _txtRevision.Text.Trim();

			if(!ValidateNewTagName(name, _txtName, _repository))
			{
				return false;
			}
			if(!ValidateRefspec(refspec, _txtRevision))
			{
				return false;
			}

			string message	= null;
			bool signed		= _radSigned.Checked;
			bool annotated	= signed || _radAnnotated.Checked;
			if(annotated)
			{
				message = _txtMessage.Text.Trim();
				if(message.Length == 0)
				{
					NotificationService.NotifyInputError(
						_txtMessage,
						Resources.ErrNoMessageSpecified,
						Resources.ErrMessageCannotBeEmpty);
					return false;
				}
			}
			string keyId = null;
			if(signed)
			{
				if(_radUseKeyId.Checked)
				{
					keyId = _txtKeyId.Text.Trim();
					if(keyId.Length == 0)
					{
						NotificationService.NotifyInputError(
							_txtKeyId,
							Resources.ErrNoKeyIdSpecified,
							Resources.ErrKeyIdCannotBeEmpty);
						return false;
					}
				}
			}
			try
			{
				Cursor = Cursors.WaitCursor;
				var ptr = _repository.CreateRevisionPointer(refspec);
				if(annotated)
				{
					if(signed)
					{
						if(keyId == null)
							_repository.Refs.Tags.Create(name, ptr, message, true);
						else
							_repository.Refs.Tags.Create(name, ptr, message, keyId);
					}
					else
					{
						_repository.Refs.Tags.Create(name, ptr, message, false);
					}
				}
				else
				{
					_repository.Refs.Tags.Create(name, ptr);
				}
				Cursor = Cursors.Default;
			}
			catch(TagAlreadyExistsException)
			{
				Cursor = Cursors.Default;
				NotificationService.NotifyInputError(
					_txtName,
					Resources.ErrInvalidTagName,
					Resources.ErrTagAlreadyExists);
				return false;
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
			catch(InvalidTagNameException exc)
			{
				Cursor = Cursors.Default;
				NotificationService.NotifyInputError(
					_txtName,
					Resources.ErrInvalidTagName,
					exc.Message);
				return false;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToCreateTag, name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}

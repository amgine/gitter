namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Base class for git dialogs.</summary>
	[ToolboxItem(false)]
	public class GitDialogBase : DialogBase
	{
		protected void SetupReferenceNameInputBox(TextBox textBox, ReferenceType referenceType)
		{
			if(textBox == null) throw new ArgumentNullException("textBox");

			textBox.KeyPress += OnRevisionInputBoxKeyPress;
			textBox.Tag = referenceType;
		}

		private void OnRevisionInputBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			var textBox = (TextBox)sender;
			var refType = (ReferenceType)textBox.Tag;
			string refTypeName;
			switch(refType)
			{
				case ReferenceType.LocalBranch:
					refTypeName = Resources.StrBranch;
					break;
				case ReferenceType.Tag:
					refTypeName = Resources.StrTag;
					break;
				case ReferenceType.Remote:
					refTypeName = Resources.StrRemote;
					break;
				default:
					refTypeName = Resources.StrReference;
					break;
			}
			if((e.KeyChar != 8) && (e.KeyChar < 32 || e.KeyChar == 127))
			{
				e.Handled = true;
				NotificationService.NotifyInputError(
					textBox, string.Empty,
					Resources.ErrNameCannotContainASCIIControlCharacters.UseAsFormat(refTypeName));
			}
			else
			{
				switch(e.KeyChar)
				{
					case ' ':
					case '~':
					case ':':
					case '?':
					case '^':
					case '*':
					case '[':
					case '\\':
						e.Handled = true;
						NotificationService.NotifyInputError(
							textBox, string.Empty, Resources.ErrNameCannotContainCharacter.UseAsFormat(
								refTypeName, e.KeyChar));
						break;
				}
			}
		}

		protected bool ValidateNewBranchName(string branchName, Control inputControl, Repository repository)
		{
			if(branchName == null) throw new ArgumentNullException("branchName");
			if(repository == null) throw new ArgumentNullException("repository");

			if(string.IsNullOrWhiteSpace(branchName))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrNoBranchNameSpecified,
					Resources.ErrBranchNameCannotBeEmpty);
				return false;
			}
			if(repository.Refs.Heads.Contains(branchName) ||
				repository.Refs.Remotes.Contains(branchName))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidBranchName,
					Resources.ErrBranchAlreadyExists);
				return false;
			}
			string errmsg;
			if(!Branch.ValidateName(branchName, out errmsg))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidBranchName,
					errmsg);
				return false;
			}
			return true;
		}

		protected bool ValidateNewTagName(string tagName, Control inputControl, Repository repository)
		{
			if(tagName == null) throw new ArgumentNullException("tagName");
			if(repository == null) throw new ArgumentNullException("repository");
			
			if(string.IsNullOrWhiteSpace(tagName))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrNoTagNameSpecified,
					Resources.ErrTagNameCannotBeEmpty);
				return false;
			}
			if(repository.Refs.Tags.Contains(tagName))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidTagName,
					Resources.ErrTagAlreadyExists);
				return false;
			}
			string errmsg;
			if(!gitter.Git.Tag.ValidateName(tagName, out errmsg))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidTagName,
					errmsg);
				return false;
			}
			return true;
		}

		protected bool ValidateRemoteName(string remoteName, Control inputControl)
		{
			if(string.IsNullOrWhiteSpace(remoteName))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrNoRemoteNameSpecified,
					Resources.ErrRemoteNameCannotBeEmpty);
				return false;
			}
			string errmsg;
			if(!Reference.ValidateName(remoteName, out errmsg))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidRemoteName,
					errmsg);
				return false;
			}
			return true;
		}

		protected bool ValidateNewRemoteName(string remoteName, Control inputControl, Repository repository)
		{
			if(remoteName == null) throw new ArgumentNullException("remoteName");
			if(repository == null) throw new ArgumentNullException("repository");

			if(!ValidateRemoteName(remoteName, inputControl)) return false;
			if(repository.Remotes.Contains(remoteName))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidRemoteName,
					Resources.ErrRemoteAlreadyExists);
				return false;
			}
			return true;
		}

		protected bool ValidateUrl(string url, Control inputControl)
		{
			if(string.IsNullOrWhiteSpace(url))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidUrl,
					Resources.ErrUrlCannotBeEmpty);
				return false;
			}
			return true;
		}

		protected bool ValidatePath(string path, Control inputControl)
		{
			if(string.IsNullOrWhiteSpace(path))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrNoPathSpecified,
					Resources.ErrPathCannotBeEmpty);
				return false;
			}
			return true;
		}

		protected bool ValidateRefspec(string refspec, Control inputControl)
		{
			if(string.IsNullOrWhiteSpace(refspec))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrStartingRevisionCannotBeEmpty);
				return false;
			}
			if(refspec.ContainsAnyOf(' ', '\t'))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRefspecCannotContainSpaces);
				return false;
			}
			return true;
		}
	}
}

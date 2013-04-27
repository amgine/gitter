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
	using System.IO;
	using System.ComponentModel;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Base class for git dialogs.</summary>
	[ToolboxItem(false)]
	public class GitDialogBase : DialogBase
	{
		protected void SetupReferenceNameInputBox(TextBox textBox, ReferenceType referenceType)
		{
			Verify.Argument.IsNotNull(textBox, "textBox");

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

		protected bool ValidateBranchName(string branchName, Control inputControl)
		{
			if(string.IsNullOrWhiteSpace(branchName))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrNoBranchNameSpecified,
					Resources.ErrBranchNameCannotBeEmpty);
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

		protected bool ValidateTagName(string tagName, Control inputControl)
		{
			if(string.IsNullOrWhiteSpace(tagName))
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrNoTagNameSpecified,
					Resources.ErrTagNameCannotBeEmpty);
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

		protected bool ValidateNewBranchName(string branchName, Control inputControl, Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			if(!ValidateBranchName(branchName, inputControl))
			{
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
			return true;
		}

		protected bool ValidateNewTagName(string tagName, Control inputControl, Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			if(!ValidateTagName(tagName, inputControl))
			{
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
			Verify.Argument.IsNotNull(repository, "repository");

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

		private bool ValidatePartialPath(string path, Control inputControl, int start, int end)
		{
			var invalidPathChars = Path.GetInvalidFileNameChars();
			bool endsWithWhitespace = false;
			bool isPartStart = true;
			for(int i = start; i <= end; ++i)
			{
				char c = path[i];
				if(c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
				{
					if(i != start)
					{
						if(isPartStart)
						{
							NotificationService.NotifyInputError(
								inputControl,
								Resources.ErrInvalidPath,
								Resources.ErrPathCannotContainEmptyDirectoryName);
							return false;
						}
						if(endsWithWhitespace)
						{
							NotificationService.NotifyInputError(
								inputControl,
								Resources.ErrInvalidPath,
								Resources.ErrDirectoryNameCannotEndWithWhitespace);
							return false;
						}
						isPartStart = true;
					}
					continue;
				}
				if(invalidPathChars.Contains(c))
				{
					NotificationService.NotifyInputError(
						inputControl,
						Resources.ErrInvalidPath,
						Resources.ErrPathCannotContainCharacter.UseAsFormat(c));
					return false;
				}
				endsWithWhitespace = char.IsWhiteSpace(c);
				if(isPartStart)
				{
					if(endsWithWhitespace)
					{
						NotificationService.NotifyInputError(
							inputControl,
							Resources.ErrInvalidPath,
							Resources.ErrDirectoryNameCannotStartWithWhitespace);
						return false;
					}
					isPartStart = false;
				}
			}
			return true;
		}

		protected bool ValidateAbsolutePath(string path, Control inputControl)
		{
			int start = -1;
			int end = -1;
			for(int i = 0; i < path.Length; ++i)
			{
				if(!char.IsWhiteSpace(path[i]))
				{
					start = i;
					break;
				}
			}
			if(start == -1)
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrNoPathSpecified,
					Resources.ErrPathCannotBeEmpty);
				return false;
			}
			for(int i = path.Length - 1; i >= 0; --i)
			{
				if(!char.IsWhiteSpace(path[i]))
				{
					end = i;
					break;
				}
			}
			int length = end - start + 1;

			if(length >= 3)
			{
				var c0 = path[start + 0];
				var c1 = path[start + 1];
				var c2 = path[start + 2];
				if(c1 == Path.VolumeSeparatorChar)
				{
					if(!((c0 >= 'a' && c0 <= 'z') || (c0 >= 'A' && c0 <= 'Z')))
					{
						NotificationService.NotifyInputError(
							inputControl,
							Resources.ErrInvalidPath,
							Resources.ErrPathUnknownSchema);
						return false;
					}
					if(c2 != Path.DirectorySeparatorChar && c2 != Path.AltDirectorySeparatorChar)
					{
						NotificationService.NotifyInputError(
							inputControl,
							Resources.ErrInvalidPath,
							Resources.ErrPathUnknownSchema);
						return false;
					}
					start += 3;
					length -= 3;
				}
				else
				{
					if(c0 != Path.DirectorySeparatorChar && c0 != Path.AltDirectorySeparatorChar)
					{
						NotificationService.NotifyInputError(
							inputControl,
							Resources.ErrInvalidPath,
							Resources.ErrPathUnknownSchema);
						return false;
					}
					if(c1 != Path.DirectorySeparatorChar && c1 != Path.AltDirectorySeparatorChar)
					{
						NotificationService.NotifyInputError(
							inputControl,
							Resources.ErrInvalidPath,
							Resources.ErrPathUnknownSchema);
						return false;
					}
					start += 2;
					length += 2;
				}
			}
			else
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrInvalidPath,
					Resources.ErrPathIsTooShort);
				return false;
			}
			if(!ValidatePartialPath(path, inputControl, start, end))
			{
				return false;
			}
			return true;
		}

		protected bool ValidateRelativePath(string path, Control inputControl)
		{
			int start = -1;
			int end = -1;
			for(int i = 0; i < path.Length; ++i)
			{
				if(!char.IsWhiteSpace(path[i]))
				{
					start = i;
					break;
				}
			}
			if(start == -1)
			{
				NotificationService.NotifyInputError(
					inputControl,
					Resources.ErrNoPathSpecified,
					Resources.ErrPathCannotBeEmpty);
				return false;
			}
			for(int i = path.Length - 1; i >= 0; --i)
			{
				if(!char.IsWhiteSpace(path[i]))
				{
					end = i;
					break;
				}
			}
			if(!ValidatePartialPath(path, inputControl, start, end))
			{
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

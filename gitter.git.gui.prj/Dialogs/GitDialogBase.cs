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
	using System.ComponentModel;
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
	}
}

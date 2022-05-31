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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Base class for git dialogs.</summary>
[ToolboxItem(false)]
public abstract class GitDialogBase : DialogBase
{
	protected void SetupReferenceNameInputBox(TextBox textBox, ReferenceType referenceType)
	{
		Verify.Argument.IsNotNull(textBox);

		textBox.KeyPress += OnRevisionInputBoxKeyPress;
		textBox.Tag = referenceType;
	}

	private void OnRevisionInputBoxKeyPress(object sender, KeyPressEventArgs e)
	{
		Assert.IsNotNull(e);

		var textBox = (TextBox)sender;
		string refTypeName = (ReferenceType)textBox.Tag switch
		{
			ReferenceType.LocalBranch => Resources.StrBranch,
			ReferenceType.Tag         => Resources.StrTag,
			ReferenceType.Remote      => Resources.StrRemote,
			_ => Resources.StrReference,
		};
		if(!char.IsControl(e.KeyChar))
		{
			switch(e.KeyChar)
			{
				case ' ' or '~' or ':' or '?' or '^' or '*' or '[' or '\\':
					e.Handled = true;
					NotificationService.NotifyInputError(
						textBox, string.Empty, Resources.ErrNameCannotContainCharacter.UseAsFormat(
							refTypeName, e.KeyChar));
					break;
			}
		}
	}
}

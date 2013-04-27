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
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class RenameRemoteDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Remote _remote;

		public RenameRemoteDialog(Remote remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");
			Verify.Argument.IsFalse(remote.IsDeleted, "remote",
				Resources.ExcObjectIsDeleted.UseAsFormat("Remote"));

			_remote = remote;

			InitializeComponent();

			SetupReferenceNameInputBox(_txtNewName, ReferenceType.Remote);

			Text = Resources.StrRenameRemote;

			_lblOldName.Text = Resources.StrRemote.AddColon();
			_lblNewName.Text = Resources.StrNewName.AddColon();

			_txtOldName.Text = remote.Name;
			_txtNewName.Text = remote.Name;
			_txtNewName.SelectAll();

			GitterApplication.FontManager.InputFont.Apply(_txtNewName, _txtOldName);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrRename; }
		}

		public Remote Remote
		{
			get { return _remote; }
		}

		public string NewName
		{
			get { return _txtNewName.Text; }
			set { _txtNewName.Text = value; }
		}

		public bool Execute()
		{
			var repository	= _remote.Repository;
			var oldName		= _txtOldName.Text;
			var newName		= _txtNewName.Text.Trim();
			if(oldName == newName) return true;
			if(newName.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrNoRemoteNameSpecified,
					Resources.ErrRemoteNameCannotBeEmpty);
				return false;
			}
			if(repository.Remotes.Contains(newName))
			{
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrInvalidRemoteName,
					Resources.ErrRemoteAlreadyExists);
				return false;
			}
			string errmsg;
			if(!Reference.ValidateName(newName, out errmsg))
			{
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrInvalidRemoteName,
					errmsg);
				return false;
			}
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Remote.Name = newName;
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToRenameRemote, oldName),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}

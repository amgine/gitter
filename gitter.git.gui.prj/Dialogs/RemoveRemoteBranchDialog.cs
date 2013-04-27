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
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Dialog for removing remote tracking branches.</summary>
	public partial class RemoveRemoteBranchDialog : GitDialogBase
	{
		private readonly RemoteBranch _branch;
		private readonly Remote _remote;

		/// <summary>Create <see cref="RemoveRemoteBranchDialog"/>.</summary>
		/// <param name="branch"><see cref="RemoteBranch"/> to remove.</param>
		public RemoveRemoteBranchDialog(RemoteBranch branch)
		{
			Verify.Argument.IsNotNull(branch, "branch");
			Verify.Argument.IsFalse(branch.IsDeleted, "branch",
				Resources.ExcObjectIsDeleted.UseAsFormat("RemoteBranch"));

			InitializeComponent();

			_branch = branch;
			_remote = branch.Remote;

			Text = Resources.StrRemoveBranch;

			_lblRemoveBranch.Text = Resources.StrsRemoveBranchFrom.UseAsFormat(branch.Name).AddColon();

			_cmdRemoveLocalOnly.Text = Resources.StrsRemoveLocalOnly;
			_cmdRemoveLocalOnly.Description = Resources.StrsRemoveLocalOnlyDescription;

			_cmdRemoveFromRemote.Text = Resources.StrsRemoveFromRemote;
			_cmdRemoveFromRemote.Description = Resources.StrsRemoveFromRemoteDescription.UseAsFormat(_remote.Name);
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.Cancel; }
		}

		#region Event Handlers

		private void OnRemoveLocalOnlyClick(object sender, EventArgs e)
		{
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					_branch.Delete(true);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToRemoveBranch, _branch.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			ClickOk();
		}

		private void OnRemoveFromRemoteClick(object sender, EventArgs e)
		{
			string branchName = _branch.Name.Substring(_remote.Name.Length + 1);
			string remoteRefName = GitConstants.LocalBranchPrefix + branchName;
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					_branch.DeleteFromRemote();
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToRemoveBranchFrom.UseAsFormat(branchName, _remote.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			try
			{
				if(!_branch.IsDeleted)
				{
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						_branch.Delete(true);
					}
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToRemoveBranch, _branch.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			ClickOk();
		}

		#endregion
	}
}

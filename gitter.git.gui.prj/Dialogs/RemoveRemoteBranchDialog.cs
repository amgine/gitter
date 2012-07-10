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
			if(branch == null) throw new ArgumentNullException("branch");
			if(!branch.IsRemote) throw new ArgumentException("branch");

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

		private void _cmdRemoveLocalOnly_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			try
			{
				_branch.Delete(true);
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToRemoveBranch, _branch.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			ClickOk();
		}

		private void _cmdRemoveFromRemote_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			string branchName = _branch.Name.Substring(_remote.Name.Length + 1);
			string remoteRefName = GitConstants.LocalBranchPrefix + branchName;
			try
			{
				_branch.DeleteFromRemote();
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
					_branch.Delete(true);
				}
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
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

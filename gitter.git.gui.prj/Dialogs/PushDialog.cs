namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class PushDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Repository _repository;

		public PushDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			_picWarning.Image = CachedResources.Bitmaps["ImgWarning"];

			Text = Resources.StrPush;

			_lblBranches.Text = Resources.StrBranchesToPush.AddColon();
			_grpPushTo.Text = Resources.StrPushTo;
			_radRemote.Text = Resources.StrRemote;
			_radUrl.Text = Resources.StrUrl;
			_grpOptions.Text = Resources.StrOptions;
			_chkForceOverwriteBranches.Text = Resources.StrForceOverwriteRemoteBranches;
			_lblUseWithCaution.Text = Resources.StrUseWithCaution;
			_chkUseThinPack.Text = Resources.StrUseThinPack;
			_chkSendTags.Text = Resources.StrSendTags;

			ToolTipService.Register(_chkForceOverwriteBranches, Resources.TipPushForceOverwrite);
			ToolTipService.Register(_chkUseThinPack, Resources.TipUseTinPack);
			ToolTipService.Register(_chkSendTags, Resources.TipSendTags);

			_lstReferences.LoadData(_repository, ReferenceType.LocalBranch, false, false, null);
			_lstReferences.EnableCheckboxes();

			if(!_repository.Head.IsDetached)
			{
				foreach(BranchListItem item in _lstReferences.Items)
				{
					if(item.DataContext == _repository.Head.Pointer)
					{
						item.CheckedState = CheckedState.Checked;
						break;
					}
				}
			}

			_remotePicker.LoadData(repository);
			Remote remote = null;
			lock(repository.Remotes.SyncRoot)
			{
				foreach(var r in repository.Remotes)
				{
					remote = r;
					break;
				}
			}
			_remotePicker.SelectedRemote = remote;
		}

		protected override string ActionVerb
		{
			get { return Resources.StrPush; }
		}

		public ICollection<Branch> GetSelectedBranches()
		{
			var list = new List<Branch>(_lstReferences.Items.Count);
			foreach(BranchListItem item in _lstReferences.Items)
			{
				if(item.CheckedState == CheckedState.Checked) list.Add(item.DataContext);
			}
			return list;
		}

		public Remote Remote
		{
			get { return _remotePicker.SelectedRemote; }
			set { _remotePicker.SelectedRemote = value; }
		}

		public bool SendTags
		{
			get { return _chkSendTags.Checked; }
			set { _chkSendTags.Checked = value; }
		}

		public bool ForceOverwrite
		{
			get { return _chkForceOverwriteBranches.Checked; }
			set { _chkForceOverwriteBranches.Checked = value; }
		}

		public bool UseThinPack
		{
			get { return _chkUseThinPack.Checked; }
			set { _chkUseThinPack.Checked = value; }
		}

		private void _chkForceOverwriteBranches_CheckedChanged(object sender, EventArgs e)
		{
			_pnlWarning.Visible = _chkForceOverwriteBranches.Checked;
		}

		private void _txtUrl_TextChanged(object sender, EventArgs e)
		{
			if(_txtUrl.TextLength != 0)
				_radUrl.Checked = true;
		}

		private void _remotePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(_remotePicker.SelectedRemote != null)
				_radRemote.Checked = true;
		}

		public bool Execute()
		{
			bool pushToRemote = _radRemote.Checked;
			var url = _txtUrl.Text.Trim();
			if(!pushToRemote)
			{
				if(url.Length == 0)
				{
					NotificationService.NotifyInputError(
						_txtUrl,
						Resources.ErrInvalidUrl,
						Resources.ErrUrlCannotBeEmpty);
					return false;
				}
			}
			var remote = _remotePicker.SelectedRemote;
			var branches = GetSelectedBranches();
			if(branches.Count == 0)
			{
				NotificationService.NotifyInputError(
					_lstReferences,
					Resources.ErrNoBranchesSelected,
					Resources.ErrYouMustSelectBranchesToPush);
				return false;
			}
			bool forceOverwrite = _chkForceOverwriteBranches.Checked;
			bool thinPack = _chkUseThinPack.Checked;
			bool sendTags = _chkSendTags.Checked;
			try
			{
				if(pushToRemote)
					remote.PushAsync(
						branches, forceOverwrite, thinPack, sendTags).Invoke<ProgressForm>(this);
				else
					_repository.Remotes.PushToAsync(
						url, branches, forceOverwrite, thinPack, sendTags).Invoke<ProgressForm>(this);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrPushFailed, pushToRemote ? remote.Name : url),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class MergeDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Repository _repository;
		private readonly IList<BranchBase> _unmergedBranches;
		private bool _multipleBranchesMerge;
		private TextBoxSpellChecker _speller;

		public MergeDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsFalse(repository.IsEmpty, "repository",
				Resources.ExcCantDoOnEmptyRepository.UseAsFormat("merge"));

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrMerge;

			_references.Text = Resources.StrNoBranchesToMergeWith;
			_references.DisableContextMenus = true;

			_lblMergeWith.Text = Resources.StrMergeWith.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_lblMessage.Text = Resources.StrMessage.AddColon();
			_lnkAutoFormat.Text = Resources.StrAutoFormat;
			_chkNoFF.Text = Resources.StrNoFastForward;
			_chkNoCommit.Text = Resources.StrNoCommit;
			_chkSquash.Text = Resources.StrSquash;

			ToolTipService.Register(_chkNoFF, Resources.TipNoFF);
			ToolTipService.Register(_chkNoCommit, Resources.TipMergeNoCommit);
			ToolTipService.Register(_chkSquash, Resources.TipSquash);

			_unmergedBranches = _repository.Refs.GetUnmergedBranches();

			GitterApplication.FontManager.InputFont.Apply(_txtMessage, _txtRevision);
			GlobalBehavior.SetupAutoCompleteSource(_txtRevision, _unmergedBranches);

			if(SpellingService.Enabled)
			{
				_speller = new TextBoxSpellChecker(_txtMessage, true);
			}

			_references.Style = GitterApplication.DefaultStyle;
			_references.LoadData(_repository, ReferenceType.Branch, false, GlobalBehavior.GroupRemoteBranches,
				(reference) => _unmergedBranches.Contains((BranchBase)reference));
			_references.ItemActivated += OnItemActivated;
		}

		protected override string ActionVerb
		{
			get { return Resources.StrMerge; }
		}

		public void EnableMultipleBrunchesMerge()
		{
			if(!_multipleBranchesMerge)
			{
				_multipleBranchesMerge = true;
				_references.EnableCheckboxes();
				_lblMergeWith.Visible = false;
				_txtRevision.Visible = false;
				var d = _references.Top - _references.Left;
				_references.Top = _references.Left;
				_references.Height += d;
			}
		}

		public string Branch
		{
			get { return _txtRevision.Text; }
			set { _txtRevision.Text = value; }
		}

		public bool AllowChangingBranch
		{
			get { return !_txtRevision.ReadOnly; }
			set
			{
				if(_txtRevision.ReadOnly == value)
				{
					_txtRevision.ReadOnly = !value;
					var d = _references.Height + _references.Margin.Vertical;
					if(!value)
					{
						_references.Visible = false;
						_txtMessage.Top -= d;
						_txtMessage.Height += d;
						_lblMessage.Top -= d;
					}
					else
					{
						_txtMessage.Top += d;
						_txtMessage.Height -= d;
						_lblMessage.Top += d;
						_references.Visible = true;
					}
				}
			}
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as IRevisionPointerListItem;
			if(item != null)
			{
				_txtRevision.Text = item.RevisionPointer.Pointer;
			}
		}

		private void AutoFormatMessage()
		{
			if(_multipleBranchesMerge)
			{
				var branches = GetSelectedBranches();
				if(branches.Count != 0)
				{
					try
					{
						_txtMessage.Text = _repository.Head.FormatMergeMessage(branches);
					}
					catch { }
				}
			}
			else
			{
				var branchName = _txtRevision.Text.Trim();
				if(!string.IsNullOrEmpty(branchName))
				{
					var head = _repository.Refs.Heads.TryGetItem(branchName);
					if(head != null)
					{
						try
						{
							_txtMessage.Text = _repository.Head.FormatMergeMessage(head);
						}
						catch { }
					}
					else
					{
						var remote = _repository.Refs.Heads.TryGetItem(branchName);
						if(remote != null)
						{
							try
							{
								_txtMessage.Text = _repository.Head.FormatMergeMessage(remote);
							}
							catch { }
						}
					}
				}
			}
		}

		private void _lnkAutoFormat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AutoFormatMessage();
		}

		public string Message
		{
			get { return _txtMessage.Text; }
			set { _txtMessage.Text = value; }
		}

		private List<IRevisionPointer> GetSelectedBranches()
		{
			var res = new List<IRevisionPointer>();
			foreach(var item in _references.Items)
			{
				InspectItem(item, res);
			}
			return res;
		}

		private static void InspectItem(CustomListBoxItem item, List<IRevisionPointer> list)
		{
			if(item.CheckedState == CheckedState.Checked)
			{
				list.Add(((IRevisionPointerListItem)item).RevisionPointer);
			}
			foreach(var i in item.Items)
			{
				InspectItem(i, list);
			}
		}

		public bool Execute()
		{
			bool noCommit = _chkNoCommit.Checked;
			bool noFastForward = _chkNoFF.Checked;
			bool squash = _chkSquash.Checked;
			string message = _txtMessage.Text;

			if(_multipleBranchesMerge)
			{
				var branches = GetSelectedBranches();
				if(branches.Count == 0)
				{
					NotificationService.NotifyInputError(
						_references,
						Resources.ErrNoBranchNameSpecified,
						Resources.ErrYouMustSpecifyBranchToMergeWith);
					return false;
				}
				try
				{
					Cursor = Cursors.WaitCursor;
					_repository.Head.Merge(
						branches, noCommit, noFastForward, squash, message);
					Cursor = Cursors.Default;
				}
				catch(GitException exc)
				{
					Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						this,
						exc.Message,
						Resources.ErrFailedToMerge,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
					return false;
				}
				return true;
			}
			else
			{
				var mergeTarget = _txtRevision.Text.Trim();
				if(mergeTarget.Length == 0)
				{
					NotificationService.NotifyInputError(
						_txtRevision,
						Resources.ErrNoBranchNameSpecified,
						Resources.ErrYouMustSpecifyBranchToMergeWith);
					return false;
				}
				BranchBase branch = _repository.Refs.Heads.TryGetItem(mergeTarget);
				if(branch == null)
				{
					branch = _repository.Refs.Remotes.TryGetItem(mergeTarget);
					if(branch == null)
					{
						NotificationService.NotifyInputError(
							_txtRevision,
							Resources.ErrInvalidBranchName,
							Resources.ErrSpecifiedBranchNotFound);
						return false;
					}
				}
				if(!_unmergedBranches.Contains(branch))
				{
					NotificationService.NotifyInputError(
						_txtRevision,
						Resources.ErrInvalidBranchName,
						Resources.ErrAlreadyMerged);
					return false;
				}
				try
				{
					Cursor = Cursors.WaitCursor;
					_repository.Head.Merge(branch, noCommit, noFastForward, squash, message);
					Cursor = Cursors.Default;
				}
				catch(AutomaticMergeFailedException exc)
				{
					Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						this,
						exc.Message,
						Resources.StrMerge,
						MessageBoxButton.Close,
						MessageBoxIcon.Information);
					return true;
				}
				catch(GitException exc)
				{
					Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						this,
						exc.Message,
						string.Format(Resources.ErrFailedToMergeWith, _txtRevision.Text),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
					return false;
				}
				return true;
			}
		}
	}
}

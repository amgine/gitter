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
		private IRevisionPointer _mergeFrom;

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

			if(SpellingService.Enabled)
			{
				_speller = new TextBoxSpellChecker(_txtMessage, true);
			}

			GitterApplication.FontManager.InputFont.Apply(_txtMessage);

			_references.Style = GitterApplication.DefaultStyle;
			_references.LoadData(_repository, ReferenceType.Branch, false, GlobalBehavior.GroupRemoteBranches,
				reference => _unmergedBranches.Contains(reference as BranchBase));
			_references.SelectionChanged += OnReferencesSelectionChanged;
		}

		private void OnReferencesSelectionChanged(object sender, EventArgs e)
		{
			if(_references.SelectedItems.Count == 0)
			{
				_mergeFrom = null;
			}
			else
			{
				var item = _references.SelectedItems[0] as IRevisionPointerListItem;
				if(item == null)
				{
					_mergeFrom = null;
				}
				else
				{
					_mergeFrom = item.RevisionPointer;
				}
			}
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
				_references.SelectionChanged -= OnReferencesSelectionChanged;
				_mergeFrom = null;
			}
		}

		public IRevisionPointer MergeFrom
		{
			get { return _mergeFrom; }
			set
			{
				if(_mergeFrom != value)
				{
					_mergeFrom = value;
					if(!_multipleBranchesMerge)
					{
						_references.SelectionChanged -= OnReferencesSelectionChanged;
						if(value == null)
						{
							_references.SelectedItems.Clear();
						}
						else
						{
							var item = TryFindItem(_references.Items, value);
							if(item != null)
							{
								_references.SelectedItems.Clear();
								item.FocusAndSelect();
							}
						}
						_references.SelectionChanged += OnReferencesSelectionChanged;
					}
				}
			}
		}

		private static CustomListBoxItem TryFindItem(CustomListBoxItemsCollection items, IRevisionPointer revisionPointer)
		{
			if(revisionPointer == null) return null;
			foreach(var item in items)
			{
				var revItem = item as IRevisionPointerListItem;
				if(revItem != null)
				{
					if(revItem.RevisionPointer == revisionPointer)
					{
						return item;
					}
				}
				else
				{
					var res = TryFindItem(item.Items, revisionPointer);
					if(res != null)
					{
						return res;
					}
				}
			}
			return null;
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as IRevisionPointerListItem;
			if(item != null)
			{
				_mergeFrom = item.RevisionPointer;
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
				var branchName = _mergeFrom == null ? null : _mergeFrom.Pointer;
				if(MergeFrom != null)
				{
					try
					{
						_txtMessage.Text = _repository.Head.FormatMergeMessage(MergeFrom);
					}
					catch { }
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
				if(MergeFrom == null)
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
					_repository.Head.Merge(MergeFrom, noCommit, noFastForward, squash, message);
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
						string.Format(Resources.ErrFailedToMergeWith, MergeFrom.Pointer),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
					return false;
				}
				return true;
			}
		}
	}
}

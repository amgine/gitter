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
			Localize();
			SetupTooltips();

			if(SpellingService.Enabled)
			{
				_speller = new TextBoxSpellChecker(_txtMessage, true);
			}

			GitterApplication.FontManager.InputFont.Apply(_txtMessage);

			_unmergedBranches = _repository.Refs.GetUnmergedBranches();
			_references.DisableContextMenus = true;
			_references.Style = GitterApplication.DefaultStyle;
			_references.LoadData(_repository, ReferenceType.Branch, false, GlobalBehavior.GroupRemoteBranches,
				reference => _unmergedBranches.Contains(reference as BranchBase));
			_references.SelectionChanged += OnReferencesSelectionChanged;
		}

		private void Localize()
		{
			Text = Resources.StrMergeBranches;
			_references.Text = Resources.StrNoBranchesToMergeWith;
			_lblMergeWith.Text = Resources.StrMergeWith.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_lblMessage.Text = Resources.StrMessage.AddColon();
			_lnkAutoFormat.Text = Resources.StrAutoFormat;
			_chkNoFF.Text = Resources.StrsNoFastForward;
			_chkNoCommit.Text = Resources.StrsNoCommit;
			_chkSquash.Text = Resources.StrSquash;
		}

		private void SetupTooltips()
		{
			ToolTipService.Register(_chkNoFF, Resources.TipNoFF);
			ToolTipService.Register(_chkNoCommit, Resources.TipMergeNoCommit);
			ToolTipService.Register(_chkSquash, Resources.TipSquash);
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
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
					}
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
					catch(Exception exc)
					{
						if(exc.IsCritical())
						{
							throw;
						}
					}
				}
			}
		}

		#region Event Handlers

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

		private void OnAutoFormatLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AutoFormatMessage();
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrMerge; }
		}

		public string Message
		{
			get { return _txtMessage.Text; }
			set { _txtMessage.Text = value; }
		}

		public bool NoCommit
		{
			get { return _chkNoCommit.Checked; }
			set { _chkNoCommit.Checked = value; }
		}

		public bool Squash
		{
			get { return _chkSquash.Checked; }
			set { _chkSquash.Checked = value; }
		}

		public bool NoFastForward
		{
			get { return _chkNoFF.Checked; }
			set { _chkNoFF.Checked = value; }
		}

		#endregion

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
			bool noCommit		= NoCommit;
			bool noFastForward	= NoFastForward;
			bool squash			= Squash;
			string message		= Message;

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
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						_repository.Head.Merge(branches, noCommit, noFastForward, squash, message);
					}
				}
				catch(GitException exc)
				{
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
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						_repository.Head.Merge(MergeFrom, noCommit, noFastForward, squash, message);
					}
				}
				catch(AutomaticMergeFailedException exc)
				{
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

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

namespace gitter.Git.Gui
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	
	using gitter.Git.Gui.Dialogs;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class GitToolbar : ToolStrip
	{
		private readonly ToolStripSplitButton _fetchButton;
		private readonly ToolStripSplitButton _pullButton;
		private readonly ToolStripButton _pushButton;

		private readonly ToolStripButton _historyButton;

		private readonly ToolStripButton _commitButton;
		private readonly ToolStripButton _applyPatchButton;
		private readonly ToolStripSplitButton _stashButton;
		private readonly ToolStripButton _cleanButton;
		private readonly ToolStripMenuItem _stashPopItem;
		private readonly ToolStripMenuItem _stashApplyItem;

		private readonly ToolStripButton _checkoutButton;
		private readonly ToolStripButton _branchButton;
		private readonly ToolStripSplitButton _mergeButton;
		private readonly ToolStripMenuItem _mergeMultipleItem;

		private readonly ToolStripButton _tagButton;
		private readonly ToolStripButton _noteButton;

		private readonly GuiProvider _guiProvider;
		private Repository _repository;

		public GitToolbar(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, "guiProvider");

			_guiProvider = guiProvider;

			Text = Resources.StrGit;

			const TextImageRelation tir = TextImageRelation.ImageAboveText;
			const ToolStripItemDisplayStyle ds = ToolStripItemDisplayStyle.ImageAndText;

			Items.AddRange(new ToolStripItem[]
				{
					_fetchButton = new ToolStripSplitButton(Resources.StrFetch, CachedResources.Bitmaps["ImgFetch"])
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipFetch },
					_pullButton = new ToolStripSplitButton(Resources.StrPull, CachedResources.Bitmaps["ImgPull"])
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipPull },
					_pushButton = new ToolStripButton(Resources.StrPush, CachedResources.Bitmaps["ImgPush"])
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipPush },
					new ToolStripSeparator(),
					_historyButton = new ToolStripButton(Resources.StrHistory, CachedResources.Bitmaps["ImgHistory"], OnHistoryClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipHistory },
					new ToolStripSeparator(),
					_commitButton = new ToolStripButton(Resources.StrCommit, CachedResources.Bitmaps["ImgCommit"], OnCommitClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCommit },
					_applyPatchButton = new ToolStripButton(Resources.StrPatch, CachedResources.Bitmaps["ImgPatchApply"], OnApplyPatchClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipApplyPatches },
					_stashButton = new ToolStripSplitButton(Resources.StrStash, CachedResources.Bitmaps["ImgStash"])
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipStash },
					_cleanButton = new ToolStripButton(Resources.StrClean, CachedResources.Bitmaps["ImgClean"], OnCleanClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipClean },
					new ToolStripSeparator(),
					_checkoutButton = new ToolStripButton(Resources.StrCheckout, CachedResources.Bitmaps["ImgCheckout"], OnCheckoutClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCheckoutBranch },
					_branchButton = new ToolStripButton(Resources.StrBranch, CachedResources.Bitmaps["ImgBranch"], OnBranchClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCreateBranch },
					_mergeButton = new ToolStripSplitButton(Resources.StrMerge, CachedResources.Bitmaps["ImgMerge"])
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipMerge },
					new ToolStripSeparator(),
					_tagButton = new ToolStripButton(Resources.StrTag, CachedResources.Bitmaps["ImgTag"], OnTagClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCreateTag },
					_noteButton = new ToolStripButton(Resources.StrNote, CachedResources.Bitmaps["ImgNote"], OnNoteClick)
						{ TextImageRelation = tir, DisplayStyle = ds, Available = false /* GitFeatures.AdvancedNotesCommands.IsAvailable */ },
				});

			_fetchButton.ButtonClick += OnFetchClick;
			_pullButton.ButtonClick += OnPullClick;
			_pushButton.Click += OnPushClick;

			_mergeButton.ButtonClick += OnMergeClick;

			_mergeButton.DropDown.Items.Add(
				_mergeMultipleItem = new ToolStripMenuItem(Resources.StrMergeMultipleBranches, null, OnMultipleMergeClick));

			_stashButton.ButtonClick += OnStashClick;
			_stashButton.DropDown.Items.Add(
				_stashPopItem = new ToolStripMenuItem(Resources.StrSave, CachedResources.Bitmaps["ImgStashSave"], OnStashSaveClick));
			_stashButton.DropDown.Items.Add(
				_stashPopItem = new ToolStripMenuItem(Resources.StrPop, CachedResources.Bitmaps["ImgStashPop"], OnStashPopClick));
			_stashButton.DropDown.Items.Add(
				_stashApplyItem = new ToolStripMenuItem(Resources.StrApply, CachedResources.Bitmaps["ImgStashApply"], OnStashApplyClick));

			if(guiProvider.Repository != null)
			{
				AttachToRepository(guiProvider.Repository);
			}
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					if(_repository != null)
					{
						DetachFromRepository(_repository);
					}
					if(value != null)
					{
						AttachToRepository(value);
					}
				}
			}
		}

		private void AttachToRepository(Repository repository)
		{
			_repository = repository;
			_mergeButton.Enabled = !repository.Head.IsDetached;
			if(repository.Remotes.Count != 0)
			{
				foreach(var remote in repository.Remotes)
				{
					_fetchButton.DropDown.Items.Add(GuiItemFactory.GetFetchFromItem<ToolStripMenuItem>(remote, "{1}"));
					_pullButton.DropDown.Items.Add(GuiItemFactory.GetPullFromItem<ToolStripMenuItem>(remote, "{1}"));
				}
				_fetchButton.Enabled = true;
				_pullButton.Enabled = true;
				_pushButton.Enabled = true;
			}
			else
			{
				_fetchButton.Enabled = false;
				_pullButton.Enabled = false;
				_pushButton.Enabled = false;
			}

			if(repository.Stash.Count != 0)
			{
				_stashPopItem.Enabled = true;
				_stashApplyItem.Enabled = true;
			}
			else
			{
				_stashPopItem.Enabled = false;
				_stashApplyItem.Enabled = false;
			}

			_stashButton.Enabled = !repository.IsEmpty;

			repository.Head.PointerChanged += OnHeadChanged;
			repository.Remotes.ObjectAdded += OnRemoteAdded;
			repository.Remotes.ObjectRemoved += OnRemoteRemoved;
			repository.Remotes.Renamed += OnRemoteRenamed;

			repository.Stash.StashedStateCreated += OnStashCreated;
			repository.Stash.StashedStateDeleted += OnStashDeleted;

			Enabled = true;
		}

		private void DetachFromRepository(Repository repository)
		{
			_fetchButton.DropDown.Items.Clear();
			_pullButton.DropDown.Items.Clear();

			repository.Head.PointerChanged -= OnHeadChanged;
			repository.Remotes.ObjectAdded -= OnRemoteAdded;
			repository.Remotes.ObjectRemoved -= OnRemoteRemoved;
			repository.Remotes.Renamed -= OnRemoteRenamed;

			repository.Stash.StashedStateCreated -= OnStashCreated;
			repository.Stash.StashedStateDeleted -= OnStashDeleted;

			Enabled = false;
			_repository = null;
		}

		private void OnHeadChanged(object sender, RevisionPointerChangedEventArgs e)
		{
			var head = (Head)sender;
			_mergeButton.Enabled = !head.IsDetached;
			if(!_stashButton.Enabled)
			{
				_stashButton.Enabled = true;
			}
		}

		private void OnRemoteAdded(object sender, RemoteEventArgs e)
		{
			_fetchButton.DropDown.Items.Add(GuiItemFactory.GetFetchFromItem<ToolStripMenuItem>(e.Object, "{1}"));
			_pullButton.DropDown.Items.Add(GuiItemFactory.GetPullFromItem<ToolStripMenuItem>(e.Object, "{1}"));
			if(_repository.Remotes.Count == 1)
			{
				_fetchButton.Enabled = true;
				_pullButton.Enabled = true;
				_pushButton.Enabled = true;
			}
		}

		private void OnRemoteRenamed(object sender, RemoteEventArgs e)
		{
			foreach(ToolStripItem item in _fetchButton.DropDownItems)
			{
				if(item.Tag == e.Object)
				{
					item.Text = e.Object.Name;
					break;
				}
			}
			foreach(ToolStripItem item in _pullButton.DropDownItems)
			{
				if(item.Tag == e.Object)
				{
					item.Text = e.Object.Name;
					break;
				}
			}
		}

		private void OnRemoteRemoved(object sender, RemoteEventArgs e)
		{
			if(_repository.Remotes.Count == 0)
			{
				_fetchButton.Enabled = false;
				_pullButton.Enabled = false;
				_pushButton.Enabled = false;
			}
			int id = 0;
			foreach(ToolStripItem item in _fetchButton.DropDown.Items)
			{
				if(item.Tag == e.Object)
				{
					_fetchButton.DropDown.Items.RemoveAt(id);
					break;
				}
				++id;
			}
			id = 0;
			foreach(ToolStripItem item in _pullButton.DropDown.Items)
			{
				if(item.Tag == e.Object)
				{
					_pullButton.DropDown.Items.RemoveAt(id);
					break;
				}
				++id;
			}
		}

		private void OnStashCreated(object sender, StashedStateEventArgs e)
		{
			if(_repository.Stash.Count == 1)
			{
				_stashPopItem.Enabled = true;
				_stashApplyItem.Enabled = true;
			}
		}

		private void OnStashDeleted(object sender, StashedStateEventArgs e)
		{
			if(_repository.Stash.Count == 0)
			{
				_stashPopItem.Enabled = false;
				_stashApplyItem.Enabled = false;
			}
		}

		#region Button Event Handlers

		private void OnRefreshClick(object sender, EventArgs e)
		{
			var view = _guiProvider.Environment.ViewDockService.ActiveView;
			if(view != null)
			{
				var gitView = view as GitView;
				if(gitView != null)
				{
					gitView.RefreshContent();
				}
			}
		}

		private void OnFetchClick(object sender, EventArgs e)
		{
			GuiCommands.Fetch(_guiProvider.Environment.MainForm, Repository);
		}

		private void OnPullClick(object sender, EventArgs e)
		{
			GuiCommands.Pull(_guiProvider.Environment.MainForm, Repository);
		}

		private void OnPushClick(object sender, EventArgs e)
		{
			_guiProvider.StartPushDialog();
		}

		private void OnHistoryClick(object sender, EventArgs e)
		{
			_guiProvider.Environment.ViewDockService.ShowView(Guids.HistoryViewGuid);
		}

		private void OnCommitClick(object sender, EventArgs e)
		{
			_guiProvider.Environment.ViewDockService.ShowView(Guids.CommitViewGuid);
		}

		private void OnApplyPatchClick(object sender, EventArgs e)
		{
			_guiProvider.StartApplyPatchesDialog();
		}

		private void OnCleanClick(object sender, EventArgs e)
		{
			_guiProvider.StartCleanDialog();
		}

		private void OnStashClick(object sender, EventArgs e)
		{
			_guiProvider.Environment.ViewDockService.ShowView(Guids.StashViewGuid);
		}

		private void OnStashSaveClick(object sender, EventArgs e)
		{
			bool advanced = Control.ModifierKeys == Keys.Shift;
			if(advanced)
			{
				using(var dlg = new StashSaveDialog(_repository))
				{
					dlg.Run(this);
				}
			}
			else
			{
				GuiCommands.SaveStash(this, Repository.Stash, false, false, null);
			}
		}

		private void OnStashPopClick(object sender, EventArgs e)
		{
			bool restoreIndex = Control.ModifierKeys == Keys.Shift;

			GuiCommands.PopStashedState(this, _repository.Stash, restoreIndex);
		}

		private void OnStashApplyClick(object sender, EventArgs e)
		{
			bool restoreIndex = Control.ModifierKeys == Keys.Shift;

			GuiCommands.ApplyStashedState(this, _repository.Stash, restoreIndex);
		}

		private void OnCheckoutClick(object sender, EventArgs e)
		{
			_guiProvider.StartCheckoutDialog();
		}

		private void OnBranchClick(object sender, EventArgs e)
		{
			_guiProvider.StartCreateBranchDialog();
		}

		private void OnMergeClick(object sender, EventArgs e)
		{
			_guiProvider.StartMergeDialog(false);
		}

		private void OnMultipleMergeClick(object sender, EventArgs e)
		{
			_guiProvider.StartMergeDialog(true);
		}

		private void OnTagClick(object sender, EventArgs e)
		{
			_guiProvider.StartCreateTagDialog();
		}

		private void OnNoteClick(object sender, EventArgs e)
		{
			_guiProvider.StartAddNoteDialog();
		}

		#endregion

		#region Buttons

		public ToolStripItem FetchButton
		{
			get { return _fetchButton; }
		}

		public ToolStripItem PullButton
		{
			get { return _pullButton; }
		}

		public ToolStripItem PushButton
		{
			get { return _pushButton; }
		}

		public ToolStripItem HistoryButton
		{
			get { return _historyButton; }
		}

		public ToolStripItem CommitButton
		{
			get { return _commitButton; }
		}

		public ToolStripItem StashButton
		{
			get { return _stashButton; }
		}

		public ToolStripItem CleanButton
		{
			get { return _cleanButton; }
		}

		public ToolStripItem CheckoutButton
		{
			get { return _checkoutButton; }
		}

		public ToolStripItem BranchButton
		{
			get { return _branchButton; }
		}

		public ToolStripItem MergeButton
		{
			get { return _mergeButton; }
		}

		public ToolStripItem TagButton
		{
			get { return _tagButton; }
		}

		#endregion
	}
}

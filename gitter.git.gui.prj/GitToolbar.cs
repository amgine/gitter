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
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;

	using gitter.Git.Gui.Dialogs;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class GitToolbar : ToolStrip
	{
		static class Icons
		{
			const int Size = 16;

			public static readonly IDpiBoundValue<Bitmap> Fetch    = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"fetch",    Size);
			public static readonly IDpiBoundValue<Bitmap> Pull     = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"pull",     Size);
			public static readonly IDpiBoundValue<Bitmap> Push     = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"push",     Size);
			public static readonly IDpiBoundValue<Bitmap> Remote   = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"remote",   Size);

			public static readonly IDpiBoundValue<Bitmap> History  = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"history",  Size);

			public static readonly IDpiBoundValue<Bitmap> Commit   = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"commit",      Size);
			public static readonly IDpiBoundValue<Bitmap> Patch    = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"patch.apply", Size);
			public static readonly IDpiBoundValue<Bitmap> Stash    = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"stash",       Size);
			public static readonly IDpiBoundValue<Bitmap> StashSave  = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"stash.save",  Size);
			public static readonly IDpiBoundValue<Bitmap> StashPop   = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"stash.pop",   Size);
			public static readonly IDpiBoundValue<Bitmap> StashApply = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"stash.apply", Size);
			public static readonly IDpiBoundValue<Bitmap> Clean    = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"clean",       Size);

			public static readonly IDpiBoundValue<Bitmap> Checkout = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"checkout", Size);
			public static readonly IDpiBoundValue<Bitmap> Branch   = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"branch",   Size);
			public static readonly IDpiBoundValue<Bitmap> Merge    = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"merge",    Size);

			public static readonly IDpiBoundValue<Bitmap> Tag      = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"tag",      Size);

			public static readonly IDpiBoundValue<Bitmap> Note     = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"note",     Size);
		}

		private readonly ToolStripSplitButton _fetchButton;
		private readonly ToolStripSplitButton _pullButton;
		private readonly ToolStripButton _pushButton;

		private readonly ToolStripButton _historyButton;

		private readonly ToolStripButton _commitButton;
		private readonly ToolStripButton _applyPatchButton;
		private readonly ToolStripSplitButton _stashButton;
		private readonly ToolStripButton _cleanButton;
		private readonly ToolStripMenuItem _stashSaveItem;
		private readonly ToolStripMenuItem _stashPopItem;
		private readonly ToolStripMenuItem _stashApplyItem;

		private readonly ToolStripButton _checkoutButton;
		private readonly ToolStripButton _branchButton;
		private readonly ToolStripSplitButton _mergeButton;
		private readonly ToolStripMenuItem _mergeMultipleItem;

		private readonly ToolStripButton _tagButton;
		private readonly ToolStripButton _noteButton;

		private readonly GuiProvider _guiProvider;
		private readonly DpiBindings _bindings;
		private Repository _repository;

		public GitToolbar(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;

			Text = Resources.StrGit;

			const TextImageRelation tir = TextImageRelation.ImageAboveText;
			const ToolStripItemDisplayStyle ds = ToolStripItemDisplayStyle.ImageAndText;

			Items.AddRange(new ToolStripItem[]
				{
					_fetchButton = new ToolStripSplitButton(Resources.StrFetch)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipFetch },
					_pullButton = new ToolStripSplitButton(Resources.StrPull)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipPull },
					_pushButton = new ToolStripButton(Resources.StrPush)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipPush },
					new ToolStripSeparator(),
					_historyButton = new ToolStripButton(Resources.StrHistory, default, OnHistoryClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipHistory },
					new ToolStripSeparator(),
					_commitButton = new ToolStripButton(Resources.StrCommit, default, OnCommitClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCommit },
					_applyPatchButton = new ToolStripButton(Resources.StrPatch, default, OnApplyPatchClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipApplyPatches },
					_stashButton = new ToolStripSplitButton(Resources.StrStash)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipStash },
					_cleanButton = new ToolStripButton(Resources.StrClean, default, OnCleanClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipClean },
					new ToolStripSeparator(),
					_checkoutButton = new ToolStripButton(Resources.StrCheckout, default, OnCheckoutClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCheckoutBranch },
					_branchButton = new ToolStripButton(Resources.StrBranch, default, OnBranchClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCreateBranch },
					_mergeButton = new ToolStripSplitButton(Resources.StrMerge)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipMerge },
					new ToolStripSeparator(),
					_tagButton = new ToolStripButton(Resources.StrTag, default, OnTagClick)
						{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCreateTag },
					_noteButton = new ToolStripButton(Resources.StrNote, default, OnNoteClick)
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
				_stashSaveItem = new ToolStripMenuItem(Resources.StrSave, default, OnStashSaveClick));
			_stashButton.DropDown.Items.Add(
				_stashPopItem = new ToolStripMenuItem(Resources.StrPop, default, OnStashPopClick));
			_stashButton.DropDown.Items.Add(
				_stashApplyItem = new ToolStripMenuItem(Resources.StrApply, default, OnStashApplyClick));

			_bindings = new DpiBindings(this);
			_bindings.BindImage(_fetchButton,      Icons.Fetch);
			_bindings.BindImage(_pullButton,       Icons.Pull);
			_bindings.BindImage(_pushButton,       Icons.Push);
			_bindings.BindImage(_historyButton,    Icons.History);
			_bindings.BindImage(_commitButton,     Icons.Commit);
			_bindings.BindImage(_applyPatchButton, Icons.Patch);
			_bindings.BindImage(_stashButton,      Icons.Stash);
			_bindings.BindImage(_stashSaveItem,    Icons.StashSave);
			_bindings.BindImage(_stashPopItem,     Icons.StashPop);
			_bindings.BindImage(_stashApplyItem,   Icons.StashApply);
			_bindings.BindImage(_cleanButton,      Icons.Clean);
			_bindings.BindImage(_checkoutButton,   Icons.Checkout);
			_bindings.BindImage(_branchButton,     Icons.Branch);
			_bindings.BindImage(_mergeButton,      Icons.Merge);
			_bindings.BindImage(_tagButton,        Icons.Tag);
			_bindings.BindImage(_noteButton,       Icons.Note);

			if(guiProvider.Repository is not null)
			{
				AttachToRepository(guiProvider.Repository);
			}
		}

		public Repository Repository
		{
			get => _repository;
			set
			{
				if(_repository != value)
				{
					if(_repository is not null)
					{
						DetachFromRepository(_repository);
					}
					if(value is not null)
					{
						AttachToRepository(value);
					}
				}
			}
		}

		private void AttachToRepository(Repository repository)
		{
			Assert.IsNotNull(repository);

			_repository = repository;
			_mergeButton.Enabled = !repository.Head.IsDetached;
			lock(repository.Remotes.SyncRoot)
			{
				if(repository.Remotes.Count != 0)
				{
					foreach(var remote in repository.Remotes)
					{
						ToolStripMenuItem item;
						var factory = new GuiItemFactory(_bindings);
						item = factory.GetFetchFromItem<ToolStripMenuItem>(remote, "{1}");
						_fetchButton.DropDown.Items.Add(item);
						item = factory.GetPullFromItem<ToolStripMenuItem>(remote, "{1}");
						_pullButton.DropDown.Items.Add(item);
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

		private void RemoveAll(ToolStripItemCollection collection)
		{
			while(collection.Count > 0)
			{
				var index = collection.Count - 1;
				var item = collection[index];
				_bindings.BindImage(item, null);
				collection.RemoveAt(index);
				item.Dispose();
			}
		}

		private void DetachFromRepository(Repository repository)
		{
			Assert.IsNotNull(repository);

			RemoveAll(_fetchButton.DropDown.Items);
			RemoveAll(_pullButton.DropDown.Items);

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
			if(IsDisposed) return;
			if(InvokeRequired)
			{
				try
				{
					BeginInvoke(new EventHandler<RemoteEventArgs>(OnRemoteAdded), sender, e);
				}
				catch
				{
				}
				return;
			}

			var factory = new GuiItemFactory(_bindings);
			var fetch = factory.GetFetchFromItem<ToolStripMenuItem>(e.Object, "{1}");
			var pull  = factory.GetPullFromItem<ToolStripMenuItem>(e.Object, "{1}");

			_bindings.BindImage(fetch, Icons.Remote);
			_bindings.BindImage(pull, Icons.Remote);

			_fetchButton.DropDown.Items.Add(fetch);
			_pullButton.DropDown.Items.Add(pull);
			if(_repository.Remotes.Count >= 1)
			{
				_fetchButton.Enabled = true;
				_pullButton.Enabled  = true;
				_pushButton.Enabled  = true;
			}
		}

		private void OnRemoteRenamed(object sender, RemoteEventArgs e)
		{
			if(IsDisposed) return;
			if(InvokeRequired)
			{
				try
				{
					BeginInvoke(new EventHandler<RemoteEventArgs>(OnRemoteRenamed), sender, e);
				}
				catch
				{
				}
				return;
			}
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

		private void RemoveRemote(ToolStripItemCollection collection, Remote remote)
		{
			int id = 0;
			foreach(ToolStripItem item in collection)
			{
				if(item.Tag == remote)
				{
					collection.RemoveAt(id);
					_bindings.BindImage(item, null);
					item.Dispose();
					break;
				}
				++id;
			}
		}

		private void OnRemoteRemoved(object sender, RemoteEventArgs e)
		{
			if(IsDisposed) return;
			if(InvokeRequired)
			{
				try
				{
					BeginInvoke(new EventHandler<RemoteEventArgs>(OnRemoteRemoved), sender, e);
				}
				catch
				{
				}
				return;
			}
			if(_repository.Remotes.Count == 0)
			{
				_fetchButton.Enabled = false;
				_pullButton.Enabled  = false;
				_pushButton.Enabled  = false;
			}
			RemoveRemote(_fetchButton.DropDown.Items, e.Object);
			RemoveRemote(_pullButton.DropDown.Items, e.Object);
		}

		private void OnStashCreated(object sender, StashedStateEventArgs e)
		{
			if(IsDisposed) return;
			if(InvokeRequired)
			{
				try
				{
					BeginInvoke(new EventHandler<StashedStateEventArgs>(OnStashCreated), sender, e);
				}
				catch
				{
				}
				return;
			}
			if(_repository.Stash.Count == 1)
			{
				_stashPopItem.Enabled = true;
				_stashApplyItem.Enabled = true;
			}
		}

		private void OnStashDeleted(object sender, StashedStateEventArgs e)
		{
			if(IsDisposed) return;
			if(InvokeRequired)
			{
				try
				{
					BeginInvoke(new EventHandler<StashedStateEventArgs>(OnStashDeleted), sender, e);
				}
				catch
				{
				}
				return;
			}
			if(_repository.Stash.Count == 0)
			{
				_stashPopItem.Enabled   = false;
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
				using var dlg = new StashSaveDialog(_repository);
				dlg.Run(this);
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

		public ToolStripItem FetchButton => _fetchButton;

		public ToolStripItem PullButton => _pullButton;

		public ToolStripItem PushButton => _pushButton;

		public ToolStripItem HistoryButton => _historyButton;

		public ToolStripItem CommitButton => _commitButton;

		public ToolStripItem StashButton => _stashButton;

		public ToolStripItem CleanButton => _cleanButton;

		public ToolStripItem CheckoutButton => _checkoutButton;

		public ToolStripItem BranchButton => _branchButton;

		public ToolStripItem MergeButton => _mergeButton;

		public ToolStripItem TagButton => _tagButton;

		#endregion
	}
}

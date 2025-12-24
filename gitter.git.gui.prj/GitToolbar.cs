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

namespace gitter.Git.Gui;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using gitter.Git.Gui.Dialogs;
using gitter.Git.Gui.Views;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class GitToolbar : ToolStrip
{
	private readonly ToolStripMenuItem _mergeMultipleItem;
	private readonly ToolStripButton _applyPatchButton;
	private readonly ToolStripMenuItem _stashSaveItem;
	private readonly ToolStripMenuItem _stashPopItem;
	private readonly ToolStripMenuItem _stashApplyItem;
	private readonly ToolStripButton _noteButton;

	private readonly GuiProvider _guiProvider;
	private readonly DpiBindings _bindings;
	private Repository? _repository;

	public GitToolbar(GuiProvider guiProvider)
	{
		Verify.Argument.IsNotNull(guiProvider);

		_guiProvider = guiProvider;

		Text = Resources.StrGit;

		const TextImageRelation tir = TextImageRelation.ImageAboveText;
		const ToolStripItemDisplayStyle ds = ToolStripItemDisplayStyle.ImageAndText;

		Items.AddRange(
			[
				FetchButton = new ToolStripSplitButton(Resources.StrFetch)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipFetch },
				PullButton = new ToolStripSplitButton(Resources.StrPull)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipPull },
				PushButton = new ToolStripButton(Resources.StrPush)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipPush },
				new ToolStripSeparator(),
				HistoryButton = new ToolStripButton(Resources.StrHistory, default, OnHistoryClick)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipHistory },
				new ToolStripSeparator(),
				CommitButton = new ToolStripButton(Resources.StrCommit, default, OnCommitClick)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCommit },
				_applyPatchButton = new ToolStripButton(Resources.StrPatch, default, OnApplyPatchClick)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipApplyPatches },
				StashButton = new ToolStripSplitButton(Resources.StrStash)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipStash },
				CleanButton = new ToolStripButton(Resources.StrClean, default, OnCleanClick)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipClean },
				new ToolStripSeparator(),
				CheckoutButton = new ToolStripButton(Resources.StrCheckout, default, OnCheckoutClick)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCheckoutBranch },
				BranchButton = new ToolStripButton(Resources.StrBranch, default, OnBranchClick)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCreateBranch },
				MergeButton = new ToolStripSplitButton(Resources.StrMerge)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipMerge },
				new ToolStripSeparator(),
				TagButton = new ToolStripButton(Resources.StrTag, default, OnTagClick)
					{ TextImageRelation = tir, DisplayStyle = ds, ToolTipText = Resources.TipCreateTag },
				_noteButton = new ToolStripButton(Resources.StrNote, default, OnNoteClick)
					{ TextImageRelation = tir, DisplayStyle = ds, Available = false /* GitFeatures.AdvancedNotesCommands.IsAvailable */ },
			]);

		FetchButton.ButtonClick += OnFetchClick;
		PullButton.ButtonClick += OnPullClick;
		PushButton.Click += OnPushClick;

		MergeButton.ButtonClick += OnMergeClick;

		MergeButton.DropDown.Items.Add(
			_mergeMultipleItem = new ToolStripMenuItem(Resources.StrMergeMultipleBranches, null, OnMultipleMergeClick));

		StashButton.ButtonClick += OnStashClick;
		StashButton.DropDown.Items.Add(
			_stashSaveItem = new ToolStripMenuItem(Resources.StrSave, default, OnStashSaveClick));
		StashButton.DropDown.Items.Add(
			_stashPopItem = new ToolStripMenuItem(Resources.StrPop, default, OnStashPopClick));
		StashButton.DropDown.Items.Add(
			_stashApplyItem = new ToolStripMenuItem(Resources.StrApply, default, OnStashApplyClick));

		_bindings = new DpiBindings(this);
		_bindings.BindImage(FetchButton,       Icons.Fetch);
		_bindings.BindImage(PullButton,        Icons.Pull);
		_bindings.BindImage(PushButton,        Icons.Push);
		_bindings.BindImage(HistoryButton,     Icons.History);
		_bindings.BindImage(CommitButton,      Icons.Commit);
		_bindings.BindImage(_applyPatchButton, Icons.PatchApply);
		_bindings.BindImage(StashButton,       Icons.Stash);
		_bindings.BindImage(_stashSaveItem,    Icons.StashSave);
		_bindings.BindImage(_stashPopItem,     Icons.StashPop);
		_bindings.BindImage(_stashApplyItem,   Icons.StashApply);
		_bindings.BindImage(CleanButton,       Icons.Clean);
		_bindings.BindImage(CheckoutButton,    Icons.Checkout);
		_bindings.BindImage(BranchButton,      Icons.Branch);
		_bindings.BindImage(MergeButton,       Icons.Merge);
		_bindings.BindImage(TagButton,         Icons.Tag);
		_bindings.BindImage(_noteButton,       Icons.Note);

		if(guiProvider.Repository is not null)
		{
			AttachToRepository(guiProvider.Repository);
		}
	}

	public Repository? Repository
	{
		get => _repository;
		set
		{
			if(_repository == value) return;

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

	private void AttachToRepository(Repository repository)
	{
		Assert.IsNotNull(repository);

		_repository = repository;
		MergeButton.Enabled = !repository.Head.IsDetached;
		lock(repository.Remotes.SyncRoot)
		{
			if(repository.Remotes.Count != 0)
			{
				foreach(var remote in repository.Remotes)
				{
					ToolStripMenuItem item;
					var factory = new GuiItemFactory(_bindings);
					item = factory.GetFetchFromItem<ToolStripMenuItem>(remote, "{1}");
					FetchButton.DropDown.Items.Add(item);
					item = factory.GetPullFromItem<ToolStripMenuItem>(remote, "{1}");
					PullButton.DropDown.Items.Add(item);
				}
				FetchButton.Enabled = true;
				PullButton.Enabled = true;
				PushButton.Enabled = true;
			}
			else
			{
				FetchButton.Enabled = false;
				PullButton.Enabled = false;
				PushButton.Enabled = false;
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

		StashButton.Enabled = !repository.IsEmpty;

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
			_bindings.UnbindImage(item);
			collection.RemoveAt(index);
			item.Dispose();
		}
	}

	private void DetachFromRepository(Repository repository)
	{
		Assert.IsNotNull(repository);

		RemoveAll(FetchButton.DropDown.Items);
		RemoveAll(PullButton.DropDown.Items);

		repository.Head.PointerChanged -= OnHeadChanged;
		repository.Remotes.ObjectAdded -= OnRemoteAdded;
		repository.Remotes.ObjectRemoved -= OnRemoteRemoved;
		repository.Remotes.Renamed -= OnRemoteRenamed;

		repository.Stash.StashedStateCreated -= OnStashCreated;
		repository.Stash.StashedStateDeleted -= OnStashDeleted;

		Enabled = false;
		_repository = null;
	}

	private void OnHeadChanged(object? sender, RevisionPointerChangedEventArgs e)
	{
		if(sender is not Head head) return;
		MergeButton.Enabled = !head.IsDetached;
		if(!StashButton.Enabled)
		{
			StashButton.Enabled = true;
		}
	}

	private void OnRemoteAdded(object? sender, RemoteEventArgs e)
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

		FetchButton.DropDown.Items.Add(fetch);
		PullButton.DropDown.Items.Add(pull);
		if(_repository is { Remotes.Count: >= 1 })
		{
			FetchButton.Enabled = true;
			PullButton.Enabled  = true;
			PushButton.Enabled  = true;
		}
	}

	private void OnRemoteRenamed(object? sender, RemoteEventArgs e)
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
		foreach(ToolStripItem item in FetchButton.DropDownItems)
		{
			if(item.Tag == e.Object)
			{
				item.Text = e.Object.Name;
				break;
			}
		}
		foreach(ToolStripItem item in PullButton.DropDownItems)
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
				_bindings.UnbindImage(item);
				item.Dispose();
				break;
			}
			++id;
		}
	}

	private void OnRemoteRemoved(object? sender, RemoteEventArgs e)
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
		if(_repository is { Remotes.Count: 0 })
		{
			FetchButton.Enabled = false;
			PullButton.Enabled  = false;
			PushButton.Enabled  = false;
		}
		RemoveRemote(FetchButton.DropDown.Items, e.Object);
		RemoveRemote(PullButton.DropDown.Items, e.Object);
	}

	private void OnStashCreated(object? sender, StashedStateEventArgs e)
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
		if(_repository is { Stash.Count: 1 })
		{
			_stashPopItem.Enabled = true;
			_stashApplyItem.Enabled = true;
		}
	}

	private void OnStashDeleted(object? sender, StashedStateEventArgs e)
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
		if(_repository is { Stash.Count: 0 })
		{
			_stashPopItem.Enabled   = false;
			_stashApplyItem.Enabled = false;
		}
	}

	#region Button Event Handlers

	private void OnRefreshClick(object? sender, EventArgs e)
	{
		if(_guiProvider.Environment?.ViewDockService.ActiveView is GitView view)
		{
			view.RefreshContent();
		}
	}

	private void OnFetchClick(object? sender, EventArgs e)
	{
		if(Repository is null) return;

		GuiCommands.Fetch(_guiProvider.Environment?.MainForm, Repository);
	}

	private void OnPullClick(object? sender, EventArgs e)
	{
		if(Repository is null) return;

		GuiCommands.Pull(_guiProvider.Environment?.MainForm, Repository);
	}

	private void OnPushClick(object? sender, EventArgs e)
		=> _guiProvider.StartPushDialog();

	private void OnHistoryClick(object? sender, EventArgs e)
		=> _guiProvider.Environment?.ViewDockService.ShowView(Guids.HistoryViewGuid);

	private void OnCommitClick(object? sender, EventArgs e)
		=> _guiProvider.Environment?.ViewDockService.ShowView(Guids.CommitViewGuid);

	private void OnApplyPatchClick(object? sender, EventArgs e)
		=> _guiProvider.StartApplyPatchesDialog();

	private void OnCleanClick(object? sender, EventArgs e)
		=> _guiProvider.StartCleanDialog();

	private void OnStashClick(object? sender, EventArgs e)
		=> _guiProvider.Environment?.ViewDockService.ShowView(Guids.StashViewGuid);

	private void OnStashSaveClick(object? sender, EventArgs e)
	{
		if(Repository is null) return;

		bool advanced = Control.ModifierKeys == Keys.Shift;
		if(advanced)
		{
			using var dialog = new StashSaveDialog(Repository);
			dialog.Run(this);
		}
		else
		{
			GuiCommands.SaveStash(this, Repository.Stash, false, false, null);
		}
	}

	private void OnStashPopClick(object? sender, EventArgs e)
	{
		if(Repository is null) return;

		bool restoreIndex = Control.ModifierKeys == Keys.Shift;
		GuiCommands.PopStashedState(this, Repository.Stash, restoreIndex);
	}

	private void OnStashApplyClick(object? sender, EventArgs e)
	{
		if(Repository is null) return;

		bool restoreIndex = Control.ModifierKeys == Keys.Shift;
		GuiCommands.ApplyStashedState(this, Repository.Stash, restoreIndex);
	}

	private void OnCheckoutClick(object? sender, EventArgs e)
		=> _guiProvider.StartCheckoutDialog();

	private void OnBranchClick(object? sender, EventArgs e)
		=> _guiProvider.StartCreateBranchDialog();

	private void OnMergeClick(object? sender, EventArgs e)
		=> _guiProvider.StartMergeDialog(false);

	private void OnMultipleMergeClick(object? sender, EventArgs e)
		=> _guiProvider.StartMergeDialog(true);

	private void OnTagClick(object? sender, EventArgs e)
		=> _guiProvider.StartCreateTagDialog();

	private void OnNoteClick(object? sender, EventArgs e)
		=> _guiProvider.StartAddNoteDialog();

	#endregion

	#region Buttons

	public ToolStripSplitButton FetchButton { get; }

	public ToolStripSplitButton PullButton { get; }

	public ToolStripButton PushButton { get; }

	public ToolStripButton HistoryButton { get; }

	public ToolStripButton CommitButton { get; }

	public ToolStripSplitButton StashButton { get; }

	public ToolStripButton CleanButton { get; }

	public ToolStripButton CheckoutButton { get; }

	public ToolStripButton BranchButton { get; }

	public ToolStripSplitButton MergeButton { get; }

	public ToolStripButton TagButton { get; }

	#endregion
}

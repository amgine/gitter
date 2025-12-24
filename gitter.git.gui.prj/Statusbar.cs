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

namespace gitter.Git.Gui;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

internal sealed class Statusbar : IDisposable
{
	#region Static

	private static readonly IImageProvider ImgMergeInProcess      = new CombinedImageProvider(Icons.Merge,      Icons.Overlays.Conflict);
	private static readonly IImageProvider ImgCherryPickInProcess = new CombinedImageProvider(Icons.CherryPick, Icons.Overlays.Conflict);
	private static readonly IImageProvider ImgRevertInProcess     = new CombinedImageProvider(Icons.Revert,     Icons.Overlays.Conflict);
	private static readonly IImageProvider ImgRebaseInProcess     = new CombinedImageProvider(Icons.Rebase,     Icons.Overlays.Conflict);

	#endregion

	#region Data

	private Repository? _repository;

	private readonly ToolStripStatusLabel _headLabel;

	private readonly ToolStripStatusLabel _statusLabel;
	private readonly ToolStripStatusLabel _statusClean;
	private readonly ToolStripStatusLabel _statusUnmerged;
	private readonly ToolStripStatusLabel _statusStagedAdded;
	private readonly ToolStripStatusLabel _statusStagedModified;
	private readonly ToolStripStatusLabel _statusStagedRemoved;
	private readonly ToolStripStatusLabel _statusUnstagedUntracked;
	private readonly ToolStripStatusLabel _statusUnstagedModified;
	private readonly ToolStripStatusLabel _statusUnstagedRemoved;
	private readonly IImageController _repositoryStatusImageController;

	private readonly FileListToolTip _statusUnmergedToolTip;
	private readonly FileListToolTip _statusStagedAddedToolTip;
	private readonly FileListToolTip _statusStagedModifiedToolTip;
	private readonly FileListToolTip _statusStagedRemovedToolTip;
	private readonly FileListToolTip _statusUnstagedUntrackedToolTip;
	private readonly FileListToolTip _statusUnstagedModifiedToolTip;
	private readonly FileListToolTip _statusUnstagedRemovedToolTip;
	
	private readonly ToolStripStatusLabel _statusRepositoryState;

	private readonly ToolStripButton _rebaseContinue;
	private readonly ToolStripButton _rebaseSkip;
	private readonly ToolStripButton _rebaseAbort;

	private readonly ToolStripButton _cherryPickContinue;
	private readonly ToolStripButton _cherryPickQuit;
	private readonly ToolStripButton _cherryPickAbort;

	private readonly ToolStripButton _revertContinue;
	private readonly ToolStripButton _revertQuit;
	private readonly ToolStripButton _revertAbort;

	private readonly ToolStripStatusLabel _remoteLabel;
	private readonly ToolStripStatusLabel _userLabel;

	private readonly ToolStripItem[] _leftAlignedItems;
	private readonly ToolStripItem[] _rightAlignedItems;

	private readonly GuiProvider _guiProvider;

	private readonly StatusToolTip _statusToolTip;

	#endregion

	#region .ctor

	public Statusbar(GuiProvider guiProvider)
	{
		Verify.Argument.IsNotNull(guiProvider);

		SynchronizationContext = SynchronizationContext.Current;

		_guiProvider = guiProvider;
		var dpiBindings = guiProvider.MainFormDpiBindings;

		_leftAlignedItems =
		[
			new ToolStripStatusLabel(Resources.StrHead.AddColon()),
			_headLabel = new ToolStripStatusLabel(Resources.StrNoBranch),
			new ToolStripSeparator(),
			_statusLabel = new ToolStripStatusLabel(Resources.StrStatus.AddColon()),
			_statusClean = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false },
			_statusUnmerged = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false, DoubleClickEnabled = true },
			_statusStagedAdded = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false, DoubleClickEnabled = true },
			_statusStagedModified = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false, DoubleClickEnabled = true },
			_statusStagedRemoved = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false, DoubleClickEnabled = true },
			_statusUnstagedUntracked = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false, DoubleClickEnabled = true },
			_statusUnstagedModified = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false, DoubleClickEnabled = true },
			_statusUnstagedRemoved = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false, DoubleClickEnabled = true },

			_statusRepositoryState = new ToolStripStatusLabel(string.Empty, null)
				{ Available = false, DoubleClickEnabled = true },

			_rebaseContinue = new ToolStripButton(Resources.StrContinue, null, OnRebaseContinueClick)
				{ Available = false },
			_rebaseSkip = new ToolStripButton(Resources.StrSkip, null, OnRebaseSkipClick)
				{ Available = false },
			_rebaseAbort = new ToolStripButton(Resources.StrAbort, null, OnRebaseAbortClick)
				{ Available = false },

			_cherryPickContinue = new ToolStripButton(Resources.StrContinue, null, OnCherryPickContinueClick)
				{ Available = false, ToolTipText = Resources.TipCherryPickContinue },
			_cherryPickQuit = new ToolStripButton(Resources.StrQuit, null, OnCherryPickQuitClick)
				{ Available = false, ToolTipText = Resources.TipCherryPickQuit },
			_cherryPickAbort = new ToolStripButton(Resources.StrAbort, null, OnCherryPickAbortClick)
				{ Available = false, ToolTipText = Resources.TipCherryPickAbort },

			_revertContinue = new ToolStripButton(Resources.StrContinue, null, OnRevertContinueClick)
				{ Available = false, ToolTipText = Resources.TipRevertContinue },
			_revertQuit = new ToolStripButton(Resources.StrQuit, null, OnRevertQuitClick)
				{ Available = false, ToolTipText = Resources.TipRevertQuit },
			_revertAbort = new ToolStripButton(Resources.StrAbort, null, OnRevertAbortClick)
				{ Available = false, ToolTipText = Resources.TipRevertAbort },
		];

		dpiBindings.BindImage(_statusClean, Icons.StatusClean);
		dpiBindings.BindImage(_statusUnmerged,          FileStatusIcons.ImgUnmerged);
		dpiBindings.BindImage(_statusStagedAdded,       FileStatusIcons.ImgStagedAdded);
		dpiBindings.BindImage(_statusStagedModified,    FileStatusIcons.ImgStagedModified);
		dpiBindings.BindImage(_statusStagedRemoved,     FileStatusIcons.ImgStagedRemoved);
		dpiBindings.BindImage(_statusUnstagedUntracked, FileStatusIcons.ImgUnstagedUntracked);
		dpiBindings.BindImage(_statusUnstagedModified,  FileStatusIcons.ImgUnstagedModified);
		dpiBindings.BindImage(_statusUnstagedRemoved,   FileStatusIcons.ImgUnstagedRemoved);

		dpiBindings.BindImage(_rebaseContinue, Icons.RebaseContinue);
		dpiBindings.BindImage(_rebaseSkip,     Icons.RebaseSkip);
		dpiBindings.BindImage(_rebaseAbort,    Icons.RebaseAbort);

		dpiBindings.BindImage(_cherryPickContinue, Icons.CherryPickContinue);
		dpiBindings.BindImage(_cherryPickQuit,     Icons.CherryPickQuit);
		dpiBindings.BindImage(_cherryPickAbort,    Icons.CherryPickAbort);

		dpiBindings.BindImage(_revertContinue, Icons.RevertContinue);
		dpiBindings.BindImage(_revertQuit,     Icons.RevertQuit);
		dpiBindings.BindImage(_revertAbort,    Icons.RevertAbort);

		_repositoryStatusImageController = dpiBindings.BindImage(_statusRepositoryState, ImgMergeInProcess);

		_statusUnmerged.DoubleClick          += OnConflictsDoubleClick;
		_statusUnstagedModified.DoubleClick  += OnUnstagedDoubleClick;
		_statusUnstagedRemoved.DoubleClick   += OnUnstagedDoubleClick;
		_statusUnstagedUntracked.DoubleClick += OnUnstagedDoubleClick;

		_rightAlignedItems =
		[
			_userLabel   = new ToolStripStatusLabel(string.Empty, null),
			_remoteLabel = new ToolStripStatusLabel(string.Empty, null),
		];
		dpiBindings.BindImage(_remoteLabel, Icons.Remote);

		_userLabel.MouseDown   += OnUserLabelMouseDown;
		_remoteLabel.MouseDown += OnRemoteLabelMouseDown;

		if(guiProvider.Repository is not null)
		{
			AttachToRepository(guiProvider.Repository);
		}

		_statusToolTip                  = new StatusToolTip();
		_statusUnmergedToolTip          = new FileListToolTip();
		_statusStagedAddedToolTip       = new FileListToolTip();
		_statusStagedModifiedToolTip    = new FileListToolTip();
		_statusStagedRemovedToolTip	    = new FileListToolTip();
		_statusUnstagedUntrackedToolTip = new FileListToolTip();
		_statusUnstagedModifiedToolTip  = new FileListToolTip();
		_statusUnstagedRemovedToolTip   = new FileListToolTip();

		SetToolTips();

		_headLabel.DoubleClickEnabled = true;
		_headLabel.DoubleClick += OnHeadLabelDoubleClick;
		_headLabel.MouseDown   += OnHeadLabelMouseDown;

		_userLabel.DoubleClickEnabled = true;
		_userLabel.DoubleClick += OnUserDoubleClick;
	}

	#endregion

	private SynchronizationContext? SynchronizationContext { get; }

	private static Point GetToolTipPosition(CustomToolTip toolTip, ToolStripItem label)
	{
		if(label.Owner is null) return Point.Empty;

		var b = label.Bounds;
		var s = toolTip.Measure(GitterApplication.MainForm);
		var x = b.X + (b.Width - s.Width) / 2;
		var y = b.Y - s.Height - new DpiConverter(GitterApplication.MainForm).ConvertY(5);
		var clientCoords = new Point(x, y);
		var scr = label.Owner.PointToScreen(clientCoords);
		if(scr.X < 0)
		{
			scr.X = 0;
			clientCoords = label.Owner.PointToClient(scr);
		}
		return clientCoords;
	}

	private void ShowToolTip(FileListToolTip toolTip, ToolStripItem label, bool staged, FileStatus fileStatus)
	{
		if(Repository is not null && label.Owner is not null)
		{
			toolTip.Update(Repository.Status, staged, fileStatus);
			toolTip.Show(label.Owner, GetToolTipPosition(toolTip, label));
		}
	}

	private static void HideToolTip(FileListToolTip toolTip, ToolStripItem label)
	{
		if(label.Owner is null) return;
		toolTip.Hide(label.Owner);
	}

	private void SetToolTip(ToolStripItem item, FileListToolTip toolTip, bool staged, FileStatus fileStatus)
	{
		item.MouseEnter += (sender, _) => ShowToolTip(toolTip, (ToolStripItem)sender!, staged, fileStatus);
		item.MouseLeave += (sender, _) => HideToolTip(toolTip, (ToolStripItem)sender!);
	}

	private void SetToolTips()
	{
		_statusLabel.MouseEnter += OnStatusLabelMouseEnter;
		_statusLabel.MouseLeave += OnStatusLabelMouseLeave;

		SetToolTip(_statusUnmerged,          _statusUnmergedToolTip,          false, FileStatus.Unmerged);

		SetToolTip(_statusStagedAdded,       _statusStagedAddedToolTip,       true,  FileStatus.Added);
		SetToolTip(_statusStagedRemoved,     _statusStagedRemovedToolTip,     true,  FileStatus.Removed);
		SetToolTip(_statusStagedModified,    _statusStagedModifiedToolTip,    true,  FileStatus.Modified);

		SetToolTip(_statusUnstagedUntracked, _statusUnstagedUntrackedToolTip, false, FileStatus.Added);
		SetToolTip(_statusUnstagedRemoved,   _statusUnstagedRemovedToolTip,   false, FileStatus.Removed);
		SetToolTip(_statusUnstagedModified,  _statusUnstagedModifiedToolTip,  false, FileStatus.Modified);
	}

	private void OnStatusLabelMouseEnter(object? sender, EventArgs e)
	{
		if(Repository is not null && _statusLabel.Owner is not null)
		{
			_statusToolTip.Update(Repository.Status);
			_statusToolTip.Show(_statusLabel.Owner, GetToolTipPosition(_statusToolTip, _statusLabel));
		}
	}

	private void OnStatusLabelMouseLeave(object? sender, EventArgs e)
	{
		if(_statusLabel.Owner is null) return;
		_statusToolTip.Hide(_statusLabel.Owner);
	}

	private void OnUserLabelMouseDown(object? sender, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Right)
		{
			if(_repository is not null)
			{
				var item = (ToolStripItem)sender!;
				var parent = Utility.GetParentControl(item);
				if(parent is not null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.Add(new ToolStripMenuItem(
						Resources.StrChangeIdentity.AddEllipsis(), null,
						(s, _) => _guiProvider.StartUserIdentificationDialog()));
					Utility.MarkDropDownForAutoDispose(menu);
					var x = item.Bounds.X + e.X;
					var y = item.Bounds.Y + e.Y;
					menu.Show(parent, x, y);
				}
			}
		}
	}

	private static void OnRemoteLabelMouseDown(object? sender, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Right)
		{
			var item   = (ToolStripItem)sender!;
			var remote = (Remote)item.Tag!;
			if(remote is not null)
			{
				var parent = Utility.GetParentControl(item);
				if(parent is not null)
				{
					var menu = new RemoteMenu(remote);
					Utility.MarkDropDownForAutoDispose(menu);
					var x = item.Bounds.X + e.X;
					var y = item.Bounds.Y + e.Y;
					menu.Show(parent, x, y);
				}
			}
		}
	}

	private void InvokeRebaseControl(RebaseControl control)
	{
		if(Repository is null) return;
		var parent = _guiProvider.Environment?.MainForm;
		GuiCommands.Rebase(parent, Repository, control);
	}

	private void OnRebaseContinueClick(object? sender, EventArgs e)
	{
		InvokeRebaseControl(RebaseControl.Continue);
	}

	private void OnRebaseSkipClick(object? sender, EventArgs e)
	{
		InvokeRebaseControl(RebaseControl.Skip);
	}

	private void OnRebaseAbortClick(object? sender, EventArgs e)
	{
		InvokeRebaseControl(RebaseControl.Abort);
	}

	private void InvokeCherryPickControl(CherryPickControl control)
	{
		if(Repository is null) return;
		var parent = _guiProvider.Environment?.MainForm;
		GuiCommands.CherryPick(parent, Repository, control);
	}

	private void OnCherryPickContinueClick(object? sender, EventArgs e)
	{
		InvokeCherryPickControl(CherryPickControl.Continue);
	}

	private void OnCherryPickQuitClick(object? sender, EventArgs e)
	{
		InvokeCherryPickControl(CherryPickControl.Quit);
	}

	private void OnCherryPickAbortClick(object? sender, EventArgs e)
	{
		InvokeCherryPickControl(CherryPickControl.Abort);
	}

	private void InvokeRevertControl(RevertControl control)
	{
		if(Repository is null) return;
		var parent = _guiProvider.Environment?.MainForm;
		GuiCommands.Revert(parent, Repository, control);
	}

	private void OnRevertContinueClick(object? sender, EventArgs e)
	{
		InvokeRevertControl(RevertControl.Continue);
	}

	private void OnRevertQuitClick(object? sender, EventArgs e)
	{
		InvokeRevertControl(RevertControl.Quit);
	}

	private void OnRevertAbortClick(object? sender, EventArgs e)
	{
		InvokeRevertControl(RevertControl.Abort);
	}

	private void OnHeadLabelMouseDown(object? sender, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Right)
		{
			if(Repository is null) return;

			var item = (ToolStripItem)sender!;
			var parent = Utility.GetParentControl(item);
			if(parent is null) return;

			var menu = new ContextMenuStrip
			{
				Renderer = GitterApplication.Style.ToolStripRenderer,
			};

			var dpiBindings = new DpiBindings(menu);
			var factory     = new GuiItemFactory(dpiBindings);

			var checkout = new ToolStripMenuItem(
				Resources.StrSwitchBranch.AddEllipsis(), null,
				(_, _) => _guiProvider.StartCheckoutDialog());

			dpiBindings.BindImage(checkout, Icons.Checkout);

			menu.Items.Add(checkout);
			menu.Items.Add(factory.GetViewReflogItem<ToolStripMenuItem>(Repository.Head));

			Utility.MarkDropDownForAutoDispose(menu);
			var x = item.Bounds.X + e.X;
			var y = item.Bounds.Y + e.Y;
			menu.Show(parent, x, y);
		}
	}

	private void OnHeadLabelDoubleClick(object? sender, EventArgs e)
	{
		if(Repository is not null)
		{
			_guiProvider.StartCheckoutDialog();
		}
	}

	private void OnUnstagedDoubleClick(object? sender, EventArgs e)
	{
		if(Repository is not null)
		{
			_guiProvider.StartStageFilesDialog();
		}
	}

	private void OnConflictsDoubleClick(object? sender, EventArgs e)
	{
		if(Repository is not null)
		{
			_guiProvider.StartResolveConflictsDialog();
		}
	}

	private void OnUserDoubleClick(object? sender, EventArgs e)
	{
		if(Repository is not null)
		{
			_guiProvider.StartUserIdentificationDialog();
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
		_repository = repository;
		UpdateCurrentBranchLabel();
		UpdateRemoteLabel();
		UpdateStatus();
		UpdateUserIdentityLabel();
		repository.Head.PointerChanged		+= OnHeadChanged;
		repository.Refs.Heads.BranchRenamed	+= OnBranchRenamed;
		repository.Status.Changed			+= OnStatusChanged;
		repository.Remotes.ObjectAdded		+= OnRemoteAdded;
		repository.Remotes.ObjectRemoved	+= OnRemoteRemoved;
		repository.StateChanged				+= OnStateChanged;
		repository.UserIdentityChanged		+= OnUserIdentityChanged;
	}

	private void DetachFromRepository(Repository repository)
	{
		repository.Head.PointerChanged		-= OnHeadChanged;
		repository.Refs.Heads.BranchRenamed	-= OnBranchRenamed;
		repository.Status.Changed			-= OnStatusChanged;
		repository.Remotes.ObjectAdded		-= OnRemoteAdded;
		repository.Remotes.ObjectRemoved	-= OnRemoteRemoved;
		repository.StateChanged				-= OnStateChanged;
		repository.UserIdentityChanged		-= OnUserIdentityChanged;
		GitterApplication.MainForm.RemoveTaskbarOverlayIcon();
		_repository = null;
	}

	private void PostUpdate(SendOrPostCallback callback)
	{
		if(SynchronizationContext is not null)
		{
			SynchronizationContext.Post(callback, this);
		}
		else
		{
			callback(this);
		}
	}

	private void OnHeadChanged(object? sender, RevisionPointerChangedEventArgs e)
		=> PostUpdate(static state => ((Statusbar)state!).UpdateCurrentBranchLabel());

	private void OnUserIdentityChanged(object? sender, EventArgs e)
		=> PostUpdate(static state => ((Statusbar)state!).UpdateUserIdentityLabel());

	private void OnBranchRenamed(object? sender, BranchRenamedEventArgs e)
	{
		if(e.Object.IsCurrent)
		{
			PostUpdate(static state => ((Statusbar)state!).UpdateCurrentBranchLabel());
		}
	}

	private void OnStatusChanged(object? sender, EventArgs e)
		=> PostUpdate(static state => ((Statusbar)state!).UpdateStatus());

	private void OnStateChanged(object? sender, EventArgs e)
		=> PostUpdate(static state => ((Statusbar)state!).UpdateState());

	private void OnRemoteAdded(object? sender, RemoteEventArgs e)
		=> PostUpdate(state => ((Statusbar)state!).OnRemoteAdded(e.Object));

	private void OnRemoteRemoved(object? sender, RemoteEventArgs e)
		=> PostUpdate(state => ((Statusbar)state!).OnRemoteRemoved(e.Object));

	private void OnRemoteAdded(Remote remote)
	{
		if(IsDisposed) return;

		if(_remoteLabel.Tag is null)
		{
			UpdateRemoteLabel();
		}
	}

	private void OnRemoteRemoved(Remote remote)
	{
		if(IsDisposed) return;

		if(_remoteLabel.Tag == remote)
		{
			UpdateRemoteLabel();
		}
	}

	private void UpdateRemoteLabel()
	{
		if(Repository is null) return;

		lock(Repository.Remotes.SyncRoot)
		{
			if(Repository.Remotes.Count != 0)
			{
				var remote = Repository.Remotes.TryGetItem(GitConstants.DefaultRemoteName);
				if(remote is null)
				{
					foreach(var item in Repository.Remotes)
					{
						remote = item;
						break;
					}
				}
				_remoteLabel.Text = remote!.FetchUrl;
				_remoteLabel.Tag = remote;
				_remoteLabel.Available = true;
			}
			else
			{
				_remoteLabel.Available = false;
				_remoteLabel.Text = string.Empty;
				_remoteLabel.Tag = null;
			}
		}
	}

	private static void SetItemText(ToolStripItem item, int count)
	{
		if(count != 0)
		{
			item.Text = count.ToString(CultureInfo.CurrentCulture);
			item.Available = true;
		}
		else
		{
			item.Available = false;
		}
	}

	private void UpdateStatus()
	{
		if(Repository is null) return;

		var status = Repository.Status;
		lock(status.SyncRoot)
		{
			if(status.StagedFiles.Count == 0 && status.UnstagedFiles.Count == 0)
			{
				_statusClean.Available = true;

				_statusUnmerged.Available = false;
				_statusStagedAdded.Available = false;
				_statusStagedModified.Available = false;
				_statusStagedRemoved.Available = false;
				_statusUnstagedUntracked.Available = false;
				_statusUnstagedModified.Available = false;
				_statusUnstagedRemoved.Available = false;

				GitterApplication.MainForm.RemoveTaskbarOverlayIcon();
			}
			else
			{
				_statusClean.Available = false;

				SetItemText(_statusUnmerged,          status.UnmergedCount);
				SetItemText(_statusStagedAdded,       status.StagedAddedCount);
				SetItemText(_statusStagedModified,    status.StagedModifiedCount);
				SetItemText(_statusStagedRemoved,     status.StagedRemovedCount);
				SetItemText(_statusUnstagedUntracked, status.UnstagedUntrackedCount);
				SetItemText(_statusUnstagedModified,  status.UnstagedModifiedCount);
				SetItemText(_statusUnstagedRemoved,   status.UnstagedRemovedCount);

				int count =
					status.StagedAddedCount +
					status.StagedModifiedCount +
					status.StagedRemovedCount +
					status.UnmergedCount +
					status.UnstagedModifiedCount +
					status.UnstagedRemovedCount +
					status.UnstagedUntrackedCount;
				var resName = count switch
				{
					< 1 => null,
					> 9 => "9p",
					_   => count.ToString(CultureInfo.InvariantCulture),
				};
				if(resName is not null)
				{
					GitterApplication.MainForm.SetTaskbarOverlayIcon(
						CachedResources.Icons[resName],
						$"{count} modifications");
				}
			}
		}
		UpdateState();
	}

	private void ShowRebaseControls(bool show)
	{
		_rebaseAbort.Available = show;
		_rebaseSkip.Available = show;
		_rebaseContinue.Available = show;
	}

	private void ShowCherryPickControls(bool show)
	{
		_cherryPickAbort.Available = show;
		_cherryPickQuit.Available = show;
		_cherryPickContinue.Available = show;
	}

	private void ShowRevertControls(bool show)
	{
		_revertAbort.Available = show;
		_revertQuit.Available = show;
		_revertContinue.Available = show;
	}

	private static string GetHeadString(IRevisionPointer? revision)
	{
		if(revision is null) return string.Empty;

		return Sha1Hash.IsValidString(revision.Pointer)
			? revision.Pointer.Substring(0, 7)
			: revision.Pointer;
	}

	private void UpdateState()
	{
		if(IsDisposed) return;
		if(_repository is null) return;

		switch(_repository.State)
		{
			case RepositoryState.Merging:
				{
					_repositoryStatusImageController.Image = ImgMergeInProcess;
					var mergeHead = GetHeadString(_repository.MergeHead);
					_statusRepositoryState.Text = string.IsNullOrWhiteSpace(mergeHead) ?
						Resources.StrsMergeIsInProcess :
						string.Format("{0} ({1})", Resources.StrsMergeIsInProcess, mergeHead);
					_statusRepositoryState.Available = true;
					ShowRebaseControls(false);
					ShowCherryPickControls(false);
					ShowRevertControls(false);
				}
				break;
			case RepositoryState.CherryPicking:
				{
					_repositoryStatusImageController.Image = ImgCherryPickInProcess;
					var cherryPickHead = GetHeadString(_repository.CherryPickHead);
					_statusRepositoryState.Text = string.IsNullOrWhiteSpace(cherryPickHead) ?
						Resources.StrsCherryPickIsInProcess :
						string.Format("{0} ({1})", Resources.StrsCherryPickIsInProcess, cherryPickHead);
					_statusRepositoryState.Available = true;
					ShowRebaseControls(false);
					ShowRevertControls(false);
					ShowCherryPickControls(true);
				}
				break;
			case RepositoryState.Reverting:
				{
					_repositoryStatusImageController.Image = ImgRevertInProcess;
					var revertHead =  GetHeadString(_repository.RevertHead);
					_statusRepositoryState.Text = string.IsNullOrEmpty(revertHead) ?
						Resources.StrsRevertIsInProcess :
						string.Format("{0} ({1})", Resources.StrsRevertIsInProcess, revertHead);
					_statusRepositoryState.Available = true;
					ShowRebaseControls(false);
					ShowCherryPickControls(false);
					ShowRevertControls(true);
				}
				break;
			case RepositoryState.Rebasing:
				{
					_repositoryStatusImageController.Image = ImgRebaseInProcess;
					var rebaseHead = GetHeadString(_repository.RebaseHead);
					_statusRepositoryState.Text = string.IsNullOrWhiteSpace(rebaseHead) ?
						Resources.StrsRebaseIsInProcess :
						string.Format("{0} ({1})", Resources.StrsRebaseIsInProcess, rebaseHead);
					_statusRepositoryState.Available = true;
					ShowCherryPickControls(false);
					ShowRevertControls(false);
					ShowRebaseControls(true);
				}
				break;
			default:
				{
					_statusRepositoryState.Available = false;
					ShowRebaseControls(false);
					ShowCherryPickControls(false);
					ShowRevertControls(false);
				}
				break;
		}
	}

	public void UpdateCurrentBranchLabel()
	{
		if(IsDisposed) return;
		if(_repository is null) return;

		if(_repository.Head.IsEmpty)
		{
			_headLabel.Text = _repository.Head.Pointer.Pointer;
			_guiProvider.MainFormDpiBindings.BindImage(_headLabel, Icons.Branch);
		}
		else if(_repository.Head.Pointer is Branch currentBranch)
		{
			_guiProvider.MainFormDpiBindings.BindImage(_headLabel, Icons.Branch);
			_headLabel.Text = currentBranch.Name;
		}
		else
		{
			_guiProvider.MainFormDpiBindings.UnbindImage(_headLabel);
			_headLabel.Text = Resources.StrNoBranch;
		}
	}

	public void UpdateUserIdentityLabel()
	{
		if(IsDisposed) return;
		if(_repository is null) return;

		var user = _repository.UserIdentity;
		if(user is null)
		{
			_guiProvider.MainFormDpiBindings.BindImage(_userLabel, Icons.UserUnknown);
			_userLabel.Text = Resources.StrlUserIdentityNotConfigured.SurroundWith('<', '>');
		}
		else
		{
			_guiProvider.MainFormDpiBindings.BindImage(_userLabel, Icons.User);
			_userLabel.Text = user.Name + " <" + user.Email + ">";
		}
	}

	public GuiProvider Gui => _guiProvider;

	public ToolStripItem[] LeftAlignedItems => _leftAlignedItems;

	public ToolStripItem[] RightAlignedItems => _rightAlignedItems;

	public ToolStripItem HeadLabel => _headLabel;

	public ToolStripItem RemoteLabel => _remoteLabel;

	#region IDisposable

	public bool IsDisposed { get; private set; }

	public void Dispose()
	{
		if(IsDisposed) return;

		_statusUnmergedToolTip.Dispose();
		_statusStagedAddedToolTip.Dispose();
		_statusStagedModifiedToolTip.Dispose();
		_statusStagedRemovedToolTip.Dispose();
		_statusUnstagedUntrackedToolTip.Dispose();
		_statusUnstagedModifiedToolTip.Dispose();
		_statusUnstagedRemovedToolTip.Dispose();
		IsDisposed = true;
	}

	#endregion
}

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

namespace gitter.Git.Gui
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Gui.Properties.Resources;

	internal sealed class Statusbar : IDisposable
	{
		#region Static

		private static readonly Bitmap ImgClean =
			CachedResources.Bitmaps["ImgStatusClean"];
		private static readonly Bitmap ImgMergeInProcess =
			CachedResources.Bitmaps.CombineBitmaps("ImgMerge", "ImgOverlayConflict");
		private static readonly Bitmap ImgCherryPickInProcess =
			CachedResources.Bitmaps.CombineBitmaps("ImgCherryPick", "ImgOverlayConflict");
		private static readonly Bitmap ImgRevertInProcess =
			CachedResources.Bitmaps.CombineBitmaps("ImgRevert", "ImgOverlayConflict");
		private static readonly Bitmap ImgRebaseInProcess =
			CachedResources.Bitmaps.CombineBitmaps("ImgRebase", "ImgOverlayConflict");

		#endregion

		#region Data

		private Repository _repository;

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

		private readonly ToolStripStatusLabel _remoteLabel;
		private readonly ToolStripStatusLabel _userLabel;

		private readonly ToolStripItem[] _leftAlignedItems;
		private readonly ToolStripItem[] _rightAlignedItems;

		private readonly GuiProvider _guiProvider;

		private readonly StatusToolTip _statusToolTip;

		private bool _isDisposed;

		#endregion

		#region .ctor

		public Statusbar(GuiProvider guiProvider)
		{
			Verify.Argument.IsNotNull(guiProvider, "guiProvider");

			_guiProvider = guiProvider;
			_leftAlignedItems = new ToolStripItem[]
			{
				new ToolStripStatusLabel(Resources.StrHead.AddColon()),
				_headLabel = new ToolStripStatusLabel(Resources.StrNoBranch),
				new ToolStripSeparator(),
				_statusLabel = new ToolStripStatusLabel(Resources.StrStatus.AddColon()),
				_statusClean = new ToolStripStatusLabel(string.Empty, ImgClean)
					{ Available = false },
				_statusUnmerged = new ToolStripStatusLabel(string.Empty, FileStatusIcons.ImgUnmerged)
					{ Available = false, DoubleClickEnabled = true },
				_statusStagedAdded = new ToolStripStatusLabel(string.Empty, FileStatusIcons.ImgStagedAdded)
					{ Available = false, DoubleClickEnabled = true },
				_statusStagedModified = new ToolStripStatusLabel(string.Empty, FileStatusIcons.ImgStagedModified)
					{ Available = false, DoubleClickEnabled = true },
				_statusStagedRemoved = new ToolStripStatusLabel(string.Empty, FileStatusIcons.ImgStagedRemoved)
					{ Available = false, DoubleClickEnabled = true },
				_statusUnstagedUntracked = new ToolStripStatusLabel(string.Empty, FileStatusIcons.ImgUnstagedUntracked)
					{ Available = false, DoubleClickEnabled = true },
				_statusUnstagedModified = new ToolStripStatusLabel(string.Empty, FileStatusIcons.ImgUnstagedModified)
					{ Available = false, DoubleClickEnabled = true },
				_statusUnstagedRemoved = new ToolStripStatusLabel(string.Empty, FileStatusIcons.ImgUnstagedRemoved)
					{ Available = false, DoubleClickEnabled = true },

				_statusRepositoryState = new ToolStripStatusLabel(string.Empty, ImgMergeInProcess)
					{ Available = false, DoubleClickEnabled = true },

				_rebaseContinue = new ToolStripButton(Resources.StrContinue, CachedResources.Bitmaps["ImgRebaseContinue"], OnRebaseContinueClick)
					{ Available = false },
				_rebaseSkip = new ToolStripButton(Resources.StrSkip, CachedResources.Bitmaps["ImgRebaseSkip"], OnRebaseSkipClick)
					{ Available = false },
				_rebaseAbort = new ToolStripButton(Resources.StrAbort, CachedResources.Bitmaps["ImgRebaseAbort"], OnRebaseAbortClick)
					{ Available = false },
			};

			_statusUnmerged.DoubleClick				+= OnConflictsDoubleClick;
			_statusUnstagedModified.DoubleClick		+= OnUnstagedDoubleClick;
			_statusUnstagedRemoved.DoubleClick		+= OnUnstagedDoubleClick;
			_statusUnstagedUntracked.DoubleClick	+= OnUnstagedDoubleClick;

			_rightAlignedItems = new[]
			{
				_userLabel = new ToolStripStatusLabel(string.Empty, null),
				_remoteLabel = new ToolStripStatusLabel(string.Empty, CachedResources.Bitmaps["ImgRemote"]),
			};

			_userLabel.MouseDown += OnUserLabelMouseDown;
			_remoteLabel.MouseDown += OnRemoteLabelMouseDown;

			if(guiProvider.Repository != null)
			{
				AttachToRepository(guiProvider.Repository);
			}

			_statusToolTip					= new StatusToolTip();
			_statusUnmergedToolTip			= new FileListToolTip();
			_statusStagedAddedToolTip		= new FileListToolTip();
			_statusStagedModifiedToolTip	= new FileListToolTip();
			_statusStagedRemovedToolTip		= new FileListToolTip();
			_statusUnstagedUntrackedToolTip	= new FileListToolTip();
			_statusUnstagedModifiedToolTip	= new FileListToolTip();
			_statusUnstagedRemovedToolTip	= new FileListToolTip();

			SetToolTips();

			_headLabel.DoubleClickEnabled = true;
			_headLabel.DoubleClick += OnHeadLabelDoubleClick;
			_headLabel.MouseDown += OnHeadLabelMouseDown;

			_userLabel.DoubleClickEnabled = true;
			_userLabel.DoubleClick += OnUserDoubleClick;
		}

		#endregion

		private ISynchronizeInvoke SynchronizeInvoke
		{
			get { return _guiProvider.Environment; }
		}

		private static Point GetToolTipPosition(CustomToolTip toolTip, ToolStripItem label)
		{
			var b = label.Bounds;
			var s = toolTip.Size;
			var x = b.X + (b.Width - s.Width) / 2;
			var y = b.Y - s.Height - 5;
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
			if(Repository != null)
			{
				toolTip.Update(_repository.Status, staged, fileStatus);
				toolTip.Show(label.Owner, GetToolTipPosition(toolTip, label));
			}
		}

		private void SetToolTip(ToolStripItem item, FileListToolTip toolTip, bool staged, FileStatus fileStatus)
		{
			item.MouseEnter += (sender, e) =>
				ShowToolTip(toolTip, (ToolStripItem)sender, staged, fileStatus);
			item.MouseLeave += (sender, e) =>
				toolTip.Hide(((ToolStripItem)sender).Owner);
		}

		private void SetToolTips()
		{
			_statusLabel.MouseEnter += OnStatusLabelMouseEnter;
			_statusLabel.MouseLeave += OnstatusLabelMouseLeave;

			SetToolTip(_statusUnmerged,				_statusUnmergedToolTip,				false,	FileStatus.Unmerged);

			SetToolTip(_statusStagedAdded,			_statusStagedAddedToolTip,			true,	FileStatus.Added);
			SetToolTip(_statusStagedRemoved,		_statusStagedRemovedToolTip,		true,	FileStatus.Removed);
			SetToolTip(_statusStagedModified,		_statusStagedModifiedToolTip,		true,	FileStatus.Modified);

			SetToolTip(_statusUnstagedUntracked,	_statusUnstagedUntrackedToolTip,	false,	FileStatus.Added);
			SetToolTip(_statusUnstagedRemoved,		_statusUnstagedRemovedToolTip,		false,	FileStatus.Removed);
			SetToolTip(_statusUnstagedModified,		_statusUnstagedModifiedToolTip,		false,	FileStatus.Modified);
		}

		private void OnStatusLabelMouseEnter(object sender, EventArgs e)
		{
			if(Repository != null)
			{
				_statusToolTip.Update(Repository.Status);
				_statusToolTip.Show(_statusLabel.Owner, GetToolTipPosition(_statusToolTip, _statusLabel));
			}
		}

		private void OnstatusLabelMouseLeave(object sender, EventArgs e)
		{
			_statusToolTip.Hide(_statusLabel.Owner);
		}

		private void OnUserLabelMouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
			{
				if(_repository != null)
				{
					var item = (ToolStripItem)sender;
					var menu = new ContextMenuStrip();
					menu.Items.Add(new ToolStripMenuItem(
						Resources.StrChangeIdentity.AddEllipsis(), null,
						(s, eargs) => _guiProvider.StartUserIdentificationDialog()));
					Utility.MarkDropDownForAutoDispose(menu);
					var parent = Utility.GetParentControl(item);
					var x = item.Bounds.X + e.X;
					var y = item.Bounds.Y + e.Y;
					menu.Show(parent, x, y);
				}
			}
		}

		private static void OnRemoteLabelMouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
			{
				var item = (ToolStripItem)sender;
				var remote = (Remote)item.Tag;
				if(remote != null)
				{
					var menu = new RemoteMenu(remote);
					Utility.MarkDropDownForAutoDispose(menu);
					var parent = Utility.GetParentControl(item);
					var x = item.Bounds.X + e.X;
					var y = item.Bounds.Y + e.Y;
					menu.Show(parent, x, y);
				}
			}
		}

		private void InvokeRebaseControl(RebaseControl control)
		{
			var parent = _guiProvider.Environment.MainForm;

			GuiCommands.Rebase(parent, Repository, control);
		}

		private void OnRebaseContinueClick(object sender, EventArgs e)
		{
			InvokeRebaseControl(RebaseControl.Continue);
		}

		private void OnRebaseSkipClick(object sender, EventArgs e)
		{
			InvokeRebaseControl(RebaseControl.Skip);
		}

		private void OnRebaseAbortClick(object sender, EventArgs e)
		{
			InvokeRebaseControl(RebaseControl.Abort);
		}

		private void OnHeadLabelMouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
			{
				var item = (ToolStripItem)sender;
				var menu = new ContextMenuStrip();
				menu.Items.Add(new ToolStripMenuItem(
					Resources.StrSwitchBranch.AddEllipsis(), CachedResources.Bitmaps["ImgCheckout"],
					(s, eargs) => _guiProvider.StartCheckoutDialog()));
				menu.Items.Add(GuiItemFactory.GetViewReflogItem<ToolStripMenuItem>(Repository.Head));
				Utility.MarkDropDownForAutoDispose(menu);
				var parent = Utility.GetParentControl(item);
				var x = item.Bounds.X + e.X;
				var y = item.Bounds.Y + e.Y;
				menu.Show(parent, x, y);
			}
		}

		private void OnHeadLabelDoubleClick(object sender, EventArgs e)
		{
			if(Repository != null)
			{
				_guiProvider.StartCheckoutDialog();
			}
		}

		private void OnUnstagedDoubleClick(object sender, EventArgs e)
		{
			if(Repository != null)
			{
				_guiProvider.StartStageFilesDialog();
			}
		}

		private void OnConflictsDoubleClick(object sender, EventArgs e)
		{
			if(Repository != null)
			{
				_guiProvider.StartResolveConflictsDialog();
			}
		}

		private void OnUserDoubleClick(object sender, EventArgs e)
		{
			if(Repository != null)
			{
				_guiProvider.StartUserIdentificationDialog();
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

		private void OnHeadChanged(object sender, RevisionPointerChangedEventArgs e)
		{
			if(SynchronizeInvoke.InvokeRequired)
			{
				try
				{
					SynchronizeInvoke.BeginInvoke(new MethodInvoker(UpdateCurrentBranchLabel), null);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				UpdateCurrentBranchLabel();
			}
		}

		private void OnUserIdentityChanged(object sender, EventArgs e)
		{
			if(SynchronizeInvoke.InvokeRequired)
			{
				try
				{
					SynchronizeInvoke.BeginInvoke(new MethodInvoker(UpdateUserIdentityLabel), null);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				UpdateUserIdentityLabel();
			}
		}

		private void OnBranchRenamed(object sender, BranchRenamedEventArgs e)
		{
			if(e.Object.IsCurrent)
			{
				if(SynchronizeInvoke.InvokeRequired)
				{
					try
					{
						SynchronizeInvoke.BeginInvoke(new MethodInvoker(UpdateCurrentBranchLabel), null);
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					UpdateCurrentBranchLabel();
				}
			}
		}

		private void OnStatusChanged(object sender, EventArgs e)
		{
			if(SynchronizeInvoke.InvokeRequired)
			{
				try
				{
					SynchronizeInvoke.BeginInvoke(new MethodInvoker(UpdateStatus), null);
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				UpdateStatus();
			}
		}

		private void OnStateChanged(object sender, EventArgs e)
		{
			if(SynchronizeInvoke.InvokeRequired)
			{
				SynchronizeInvoke.BeginInvoke(new MethodInvoker(UpdateState), null);
			}
			else
			{
				UpdateState();
			}
		}

		private void OnRemoteAdded(object sender, RemoteEventArgs e)
		{
			if(SynchronizeInvoke.InvokeRequired)
			{
				try
				{
					SynchronizeInvoke.BeginInvoke(new Action<Remote>(OnRemoteAdded), new object[] { e.Object });
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				OnRemoteAdded(e.Object);
			}
		}

		private void OnRemoteRemoved(object sender, RemoteEventArgs e)
		{
			if(SynchronizeInvoke.InvokeRequired)
			{
				try
				{
					SynchronizeInvoke.BeginInvoke(new Action<Remote>(OnRemoteRemoved), new object[] { e.Object });
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				OnRemoteRemoved(e.Object);
			}
		}

		private void OnRemoteAdded(Remote remote)
		{
			if(_remoteLabel.Tag == null)
			{
				UpdateRemoteLabel();
			}
		}

		private void OnRemoteRemoved(Remote remote)
		{
			if(_remoteLabel.Tag == remote)
			{
				UpdateRemoteLabel();
			}
		}

		private string GetUserName()
		{
			if(Repository != null)
			{
				var username = Repository.Configuration.TryGetParameter(GitConstants.UserNameParameter);
				if(username != null)
				{
					var useremail = Repository.Configuration.TryGetParameter(GitConstants.UserEmailParameter);
					if(useremail != null)
					{
						return string.Format("{0} <{1}>", username.Value, useremail.Value);
					}
					else
					{
						return username.Value;
					}
				}
			}
			return null;
		}

		private void UpdateRemoteLabel()
		{
			lock(Repository.Remotes.SyncRoot)
			{
				if(Repository.Remotes.Count != 0)
				{
					var remote = Repository.Remotes.TryGetItem(GitConstants.DefaultRemoteName);
					if(remote == null)
					{
						foreach(var item in Repository.Remotes)
						{
							remote = item;
							break;
						}
					}
					_remoteLabel.Text = remote.FetchUrl;
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

		private void UpdateStatus()
		{
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
					_statusUnstagedRemoved.Available = false;
					_statusUnstagedModified.Available = false;

					GitterApplication.MainForm.RemoveTaskbarOverlayIcon();
				}
				else
				{
					_statusClean.Available = false;

					if(status.UnmergedCount != 0)
					{
						_statusUnmerged.Text = status.UnmergedCount.ToString(CultureInfo.CurrentCulture);
						_statusUnmerged.Available = true;
					}
					else
					{
						_statusUnmerged.Available = false;
					}
					if(status.StagedAddedCount != 0)
					{
						_statusStagedAdded.Text = status.StagedAddedCount.ToString(CultureInfo.CurrentCulture);
						_statusStagedAdded.Available = true;
					}
					else
					{
						_statusStagedAdded.Available = false;
					}
					if(status.StagedModifiedCount != 0)
					{
						_statusStagedModified.Text = status.StagedModifiedCount.ToString(CultureInfo.CurrentCulture);
						_statusStagedModified.Available = true;
					}
					else
					{
						_statusStagedModified.Available = false;
					}
					if(status.StagedRemovedCount != 0)
					{
						_statusStagedRemoved.Text = status.StagedRemovedCount.ToString(CultureInfo.CurrentCulture);
						_statusStagedRemoved.Available = true;
					}
					else
					{
						_statusStagedRemoved.Available = false;
					}
					if(status.UnstagedUntrackedCount != 0)
					{
						_statusUnstagedUntracked.Text = status.UnstagedUntrackedCount.ToString(CultureInfo.CurrentCulture);
						_statusUnstagedUntracked.Available = true;
					}
					else
					{
						_statusUnstagedUntracked.Available = false;
					}
					if(status.UnstagedModifiedCount != 0)
					{
						_statusUnstagedModified.Text = status.UnstagedModifiedCount.ToString(CultureInfo.CurrentCulture);
						_statusUnstagedModified.Available = true;
					}
					else
					{
						_statusUnstagedModified.Available = false;
					}
					if(status.UnstagedRemovedCount != 0)
					{
						_statusUnstagedRemoved.Text = status.UnstagedRemovedCount.ToString(CultureInfo.CurrentCulture);
						_statusUnstagedRemoved.Available = true;
					}
					else
					{
						_statusUnstagedRemoved.Available = false;
					}

					int count =
						status.StagedAddedCount +
						status.StagedModifiedCount +
						status.StagedRemovedCount +
						status.UnmergedCount +
						status.UnstagedModifiedCount +
						status.UnstagedRemovedCount +
						status.UnstagedUntrackedCount;
					string resName;
					if(count < 1)
					{
						resName = null;
					}
					else if(count > 9)
					{
						resName = "9p";
					}
					else
					{
						resName = count.ToString(CultureInfo.InvariantCulture);
					}
					if(resName != null)
					{
						GitterApplication.MainForm.SetTaskbarOverlayIcon(
							CachedResources.Icons["IcoStatusGreen" + resName],
							string.Format("{0} modifications", count));
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

		private static string GetHeadString(IRevisionPointer revision)
		{
			string headString;
			if(revision != null)
			{
				if(GitUtils.IsValidSHA1(revision.Pointer))
				{
					headString = revision.Pointer.Substring(0, 7);
				}
				else
				{
					headString = revision.Pointer;
				}
			}
			else
			{
				headString = string.Empty;
			}
			return headString;
		}

		private void UpdateState()
		{
			switch(_repository.State)
			{
				case RepositoryState.Merging:
					{
						_statusRepositoryState.Image = ImgMergeInProcess;
						var mergeHead = GetHeadString(_repository.MergeHead);
						_statusRepositoryState.Text = string.IsNullOrWhiteSpace(mergeHead) ?
							Resources.StrsMergeIsInProcess :
							string.Format("{0} ({1})", Resources.StrsMergeIsInProcess, mergeHead);
						_statusRepositoryState.Available = true;
						ShowRebaseControls(false);
					}
					break;
				case RepositoryState.CherryPicking:
					{
						_statusRepositoryState.Image = ImgCherryPickInProcess;
						var cherryPickHead = GetHeadString(_repository.CherryPickHead);
						_statusRepositoryState.Text = string.IsNullOrWhiteSpace(cherryPickHead) ?
							Resources.StrsCherryPickIsInProcess :
							string.Format("{0} ({1})", Resources.StrsCherryPickIsInProcess, cherryPickHead);
						_statusRepositoryState.Available = true;
						ShowRebaseControls(false);
					}
					break;
				case RepositoryState.Reverting:
					{
						_statusRepositoryState.Image = ImgRevertInProcess;
						var revertHead =  GetHeadString(_repository.RevertHead);
						_statusRepositoryState.Text = string.IsNullOrEmpty(revertHead) ?
							Resources.StrsRevertIsInProcess :
							string.Format("{0} ({1})", Resources.StrsRevertIsInProcess, revertHead);
						_statusRepositoryState.Available = true;
						ShowRebaseControls(false);
					}
					break;
				case RepositoryState.Rebasing:
					{
						_statusRepositoryState.Image = ImgRebaseInProcess;
						var rebaseHead = GetHeadString(_repository.RebaseHead);
						_statusRepositoryState.Text = string.IsNullOrWhiteSpace(rebaseHead) ?
							Resources.StrsRebaseIsInProcess :
							string.Format("{0} ({1})", Resources.StrsRebaseIsInProcess, rebaseHead);
						_statusRepositoryState.Available = true;
						ShowRebaseControls(true);
					}
					break;
				default:
					{
						_statusRepositoryState.Available = false;
						ShowRebaseControls(false);
					}
					break;
			}
		}

		public void UpdateCurrentBranchLabel()
		{
			if(_repository.Head.IsEmpty)
			{
				_headLabel.Image = CachedResources.Bitmaps["ImgBranch"];
				_headLabel.Text = _repository.Head.Pointer.Pointer;
			}
			else
			{
				var currentBranch = _repository.Head.Pointer as Branch;
				if(currentBranch != null)
				{
					_headLabel.Image = CachedResources.Bitmaps["ImgBranch"];
					_headLabel.Text = currentBranch.Name;
				}
				else
				{
					_headLabel.Image = null;
					_headLabel.Text = Resources.StrNoBranch;
				}
			}
		}

		public void UpdateUserIdentityLabel()
		{
			var user = _repository.UserIdentity;
			if(user == null)
			{
				_userLabel.Image = CachedResources.Bitmaps["ImgUserUnknown"];
				_userLabel.Text = Resources.StrlUserIdentityNotConfigured.SurroundWith('<', '>');
			}
			else
			{
				_userLabel.Image = CachedResources.Bitmaps["ImgUser"];
				_userLabel.Text = user.Name + " <" + user.Email + ">";
			}
		}

		public GuiProvider Gui
		{
			get { return _guiProvider; }
		}

		public ToolStripItem[] LeftAlignedItems
		{
			get { return _leftAlignedItems; }
		}

		public ToolStripItem[] RightAlignedItems
		{
			get { return _rightAlignedItems; }
		}

		public ToolStripItem HeadLabel
		{
			get { return _headLabel; }
		}

		public ToolStripItem RemoteLabel
		{
			get { return _remoteLabel; }
		}

		#region IDisposable

		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		public void Dispose()
		{
			if(!IsDisposed)
			{
				_statusUnmergedToolTip.Dispose();
				_statusStagedAddedToolTip.Dispose();
				_statusStagedModifiedToolTip.Dispose();
				_statusStagedRemovedToolTip.Dispose();
				_statusUnstagedUntrackedToolTip.Dispose();
				_statusUnstagedModifiedToolTip.Dispose();
				_statusUnstagedRemovedToolTip.Dispose();
				IsDisposed = true;
			}
		}

		#endregion
	}
}

namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	internal sealed class Statusbar
	{
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

		private readonly GuiProvider _gui;
		private Repository _repository;

		private readonly StatusToolTip _statusToolTip;

		private static readonly Bitmap ImgClean =
			CachedResources.Bitmaps["ImgStatusClean"];
		private static readonly Bitmap ImgMergeInProcess =
			CachedResources.Bitmaps.CombineBitmaps("ImgMerge", "ImgOverlayConflict");
		private static readonly Bitmap ImgCherryPickInProcess =
			CachedResources.Bitmaps.CombineBitmaps("ImgCherryPick", "ImgOverlayConflict");
		private static readonly Bitmap ImgRebaseInProcess =
			CachedResources.Bitmaps.CombineBitmaps("ImgRebase", "ImgOverlayConflict");

		public Statusbar(GuiProvider gui)
		{
			if(gui == null) throw new ArgumentNullException("gui");
			_gui = gui;

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


			_statusUnmerged.DoubleClick += OnConflictsDoubleClick;
			_statusUnstagedModified.DoubleClick += OnUnstagedDoubleClick;
			_statusUnstagedRemoved.DoubleClick += OnUnstagedDoubleClick;
			_statusUnstagedUntracked.DoubleClick += OnUnstagedDoubleClick;

			_rightAlignedItems = new[]
			{
				_userLabel = new ToolStripStatusLabel(string.Empty, null),
				_remoteLabel = new ToolStripStatusLabel(string.Empty, CachedResources.Bitmaps["ImgRemote"]),
			};

			_userLabel.MouseDown += OnUserLabelMouseDown;
			_remoteLabel.MouseDown += OnRemoteLabelMouseDown;

			if(gui.Repository != null)
			{
				AttachToRepository(gui.Repository);
			}

			_statusToolTip = new StatusToolTip();

			_statusUnmergedToolTip = new FileListToolTip();
			_statusStagedAddedToolTip = new FileListToolTip();
			_statusStagedModifiedToolTip = new FileListToolTip();
			_statusStagedRemovedToolTip = new FileListToolTip();
			_statusUnstagedUntrackedToolTip = new FileListToolTip();
			_statusUnstagedModifiedToolTip = new FileListToolTip();
			_statusUnstagedRemovedToolTip = new FileListToolTip();

			SetToolTips();

			_headLabel.DoubleClickEnabled = true;
			_headLabel.DoubleClick += OnHeadLabelDoubleClick;
			_headLabel.MouseDown += OnHeadLabelMouseDown;

			_userLabel.DoubleClickEnabled = true;
			_userLabel.DoubleClick += OnUserDoubleClick;
		}

		private static Point GetToolTipPosition(CustomToolTip toolTip, ToolStripStatusLabel label)
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

		private void ShowToolTip(FileListToolTip toolTip, ToolStripStatusLabel label, bool staged, FileStatus fileStatus)
		{
			if(_repository != null)
			{
				toolTip.Update(_repository.Status, staged, fileStatus);
				toolTip.Show(label.Owner, GetToolTipPosition(toolTip, label));
			}
		}

		private void SetToolTips()
		{
			_statusLabel.MouseEnter += (sender, e) =>
			{
				if(_repository != null)
				{
					_statusToolTip.Update(_repository.Status);
					_statusToolTip.Show(_statusLabel.Owner, GetToolTipPosition(_statusToolTip, _statusLabel));
				}
			};
			_statusLabel.MouseLeave += (sender, e) =>
			{
				_statusToolTip.Hide(_statusLabel.Owner);
			};

			_statusUnmerged.MouseEnter += (sender, e) =>
				ShowToolTip(_statusUnmergedToolTip, (ToolStripStatusLabel)sender, false, FileStatus.Unmerged);
			_statusUnmerged.MouseLeave += (sender, e) =>
				_statusUnmergedToolTip.Hide(((ToolStripStatusLabel)sender).Owner);

			_statusStagedAdded.MouseEnter += (sender, e) =>
				ShowToolTip(_statusStagedAddedToolTip, (ToolStripStatusLabel)sender, true, FileStatus.Added);
			_statusStagedAdded.MouseLeave += (sender, e) =>
				_statusStagedAddedToolTip.Hide(((ToolStripStatusLabel)sender).Owner);

			_statusStagedRemoved.MouseEnter += (sender, e) =>
				ShowToolTip(_statusStagedRemovedToolTip, (ToolStripStatusLabel)sender, true, FileStatus.Removed);
			_statusStagedRemoved.MouseLeave += (sender, e) =>
				_statusStagedRemovedToolTip.Hide(((ToolStripStatusLabel)sender).Owner);

			_statusStagedModified.MouseEnter += (sender, e) =>
				ShowToolTip(_statusStagedModifiedToolTip, (ToolStripStatusLabel)sender, true, FileStatus.Modified);
			_statusStagedModified.MouseLeave += (sender, e) =>
				_statusStagedModifiedToolTip.Hide(((ToolStripStatusLabel)sender).Owner);

			_statusUnstagedUntracked.MouseEnter += (sender, e) =>
				ShowToolTip(_statusUnstagedUntrackedToolTip, (ToolStripStatusLabel)sender, false, FileStatus.Added);
			_statusUnstagedUntracked.MouseLeave += (sender, e) =>
				_statusUnstagedUntrackedToolTip.Hide(((ToolStripStatusLabel)sender).Owner);

			_statusUnstagedRemoved.MouseEnter += (sender, e) =>
				ShowToolTip(_statusUnstagedRemovedToolTip, (ToolStripStatusLabel)sender, false, FileStatus.Removed);
			_statusUnstagedRemoved.MouseLeave += (sender, e) =>
				_statusUnstagedRemovedToolTip.Hide(((ToolStripStatusLabel)sender).Owner);

			_statusUnstagedModified.MouseEnter += (sender, e) =>
				ShowToolTip(_statusUnstagedModifiedToolTip, (ToolStripStatusLabel)sender, false, FileStatus.Modified);
			_statusUnstagedModified.MouseLeave += (sender, e) =>
				_statusUnstagedModifiedToolTip.Hide(((ToolStripStatusLabel)sender).Owner);
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
						(s, eargs) => _gui.StartUserIdentificationDialog()));
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
			var parent = _gui.Environment.MainForm;
			try
			{
				_repository.RebaseAsync(control).Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToRebase,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
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
					(s, eargs) => _gui.StartCheckoutDialog()));
				Utility.MarkDropDownForAutoDispose(menu);
				var parent = Utility.GetParentControl(item);
				var x = item.Bounds.X + e.X;
				var y = item.Bounds.Y + e.Y;
				menu.Show(parent, x, y);
			}
		}

		private void OnHeadLabelDoubleClick(object sender, EventArgs e)
		{
			if(_repository != null)
			{
				_gui.StartCheckoutDialog();
			}
		}

		private void OnUnstagedDoubleClick(object sender, EventArgs e)
		{
			_gui.StartStageFilesDialog();
		}

		private void OnConflictsDoubleClick(object sender, EventArgs e)
		{
			_gui.StartResolveConflictsDialog();
		}

		private void OnUserDoubleClick(object sender, EventArgs e)
		{
			if(_repository != null)
			{
				_gui.StartUserIdentificationDialog();
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
			if(repository.Remotes.Count != 0)
			{
				foreach(var remote in repository.Remotes)
				{
					_remoteLabel.Text = remote.FetchUrl;
					_remoteLabel.Tag = remote;
					break;
				}
				_remoteLabel.Available = true;
			}
			else
			{
				_remoteLabel.Available = false;
				_remoteLabel.Tag = null;
			}
			UpdateStatus();
			UpdateUserIdentityLabel();
			repository.Head.PointerChanged += OnHeadChanged;
			repository.Refs.Heads.BranchRenamed += OnBranchRenamed;
			repository.Status.Changed += OnStatusChanged;
			repository.StateChanged += OnStateChanged;
			repository.UserIdentityChanged += OnUserIdentityChanged;
		}

		private void DetachFromRepository(Repository repository)
		{
			repository.Head.PointerChanged -= OnHeadChanged;
			repository.Refs.Heads.BranchRenamed -= OnBranchRenamed;
			repository.Status.Changed -= OnStatusChanged;
			repository.StateChanged -= OnStateChanged;
			repository.UserIdentityChanged -= OnUserIdentityChanged;
			GitterApplication.MainForm.RemoveTaskbarOverlayIcon();
			_repository = null;
		}

		private void OnHeadChanged(object sender, RevisionPointerChangedEventArgs e)
		{
			if(_gui.Environment.InvokeRequired)
			{
				_gui.Environment.BeginInvoke(new MethodInvoker(UpdateCurrentBranchLabel), null);
			}
			else
			{
				UpdateCurrentBranchLabel();
			}
		}

		private void OnUserIdentityChanged(object sender, EventArgs e)
		{
			if(_gui.Environment.InvokeRequired)
			{
				_gui.Environment.BeginInvoke(new MethodInvoker(UpdateUserIdentityLabel), null);
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
				if(_gui.Environment.InvokeRequired)
				{
					_gui.Environment.BeginInvoke(new MethodInvoker(UpdateCurrentBranchLabel), null);
				}
				else
				{
					UpdateCurrentBranchLabel();
				}
			}
		}

		private void OnStatusChanged(object sender, EventArgs e)
		{
			if(_gui.Environment.InvokeRequired)
			{
				_gui.Environment.BeginInvoke(new MethodInvoker(UpdateStatus), null);
			}
			else
			{
				UpdateStatus();
			}
		}

		private void OnStateChanged(object sender, EventArgs e)
		{
			if(_gui.Environment.InvokeRequired)
			{
				_gui.Environment.BeginInvoke(new MethodInvoker(UpdateState), null);
			}
			else
			{
				UpdateState();
			}
		}

		private string GetUserName()
		{
			var username = _repository.Configuration.TryGetParameter(GitConstants.UserNameParameter);
			if(username != null)
			{
				var useremail = _repository.Configuration.TryGetParameter(GitConstants.UserEmailParameter);
				if(useremail != null)
				{
					return string.Format("{0} <{1}>", username.Value, useremail.Value);
				}
				else
				{
					return username.Value;
				}
			}
			return null;
		}

		private void UpdateStatus()
		{
			var status = _repository.Status;
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
			UpdateState();
		}

		private void ShowRebaseControls(bool show)
		{
			_rebaseAbort.Available = show;
			_rebaseSkip.Available = show;
			_rebaseContinue.Available = show;
		}

		private void UpdateState()
		{
			switch(_repository.State)
			{
				case RepositoryState.Merging:
					{
						_statusRepositoryState.Image = ImgMergeInProcess;
						var mergeHead = _repository.GetMergeHead();
						if(GitUtils.IsValidSHA1(mergeHead))
						{
							mergeHead = mergeHead.Substring(0, 7);
						}
						_statusRepositoryState.Text = string.IsNullOrEmpty(mergeHead) ?
							Resources.StrsMergeIsInProcess :
							string.Format("{0} ({1})", Resources.StrsMergeIsInProcess, mergeHead);
						_statusRepositoryState.Available = true;
						ShowRebaseControls(false);
					}
					break;
				case RepositoryState.CherryPicking:
					{
						_statusRepositoryState.Image = ImgCherryPickInProcess;
						var cherryPickHead = _repository.GetCherryPickHead();
						if(GitUtils.IsValidSHA1(cherryPickHead))
						{
							cherryPickHead = cherryPickHead.Substring(0, 7);
						}
						_statusRepositoryState.Text = string.IsNullOrEmpty(cherryPickHead) ?
							Resources.StrsCherryPickIsInProcess :
							string.Format("{0} ({1})", Resources.StrsCherryPickIsInProcess, cherryPickHead);
						_statusRepositoryState.Available = true;
						ShowRebaseControls(false);
					}
					break;
				case RepositoryState.Rebasing:
					{
						_statusRepositoryState.Image = ImgRebaseInProcess;
						var rebaseHead = _repository.GetRebaseHead();
						if(GitUtils.IsValidSHA1(rebaseHead))
						{
							rebaseHead = rebaseHead.Substring(0, 7);
						}
						if(rebaseHead.Length == 0)
						{
							_statusRepositoryState.Text = Resources.StrsRebaseIsInProcess;
						}
						else
						{
							_statusRepositoryState.Text = string.Format("{0} ({1})",
								Resources.StrsRebaseIsInProcess, rebaseHead);
						}
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
			get { return _gui; }
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
	}
}

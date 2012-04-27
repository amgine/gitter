namespace gitter.Git.Gui
{
	using System;
	using System.IO;
	using System.Drawing;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;

	using gitter.Git.AccessLayer;
	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Factory for creating buttons or menu items.</summary>
	public static class GuiItemFactory
	{
		#region Universal Items

		public static T GetViewReflogItem<T>(Reference reference)
			where T : ToolStripItem, new()
		{
			if(reference == null) throw new ArgumentNullException("reference");
			if(reference.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Reference"), "reference");

			var item = new T()
			{
				Image = CachedResources.Bitmaps[reference.Type == ReferenceType.RemoteBranch?
					"ImgViewReflogRemote":"ImgViewReflog"],
				Text = Resources.StrViewReflog,
				Tag = reference,
			};
			item.Click += OnViewReflogClick;
			return item;
		}

		public static T GetCheckoutPathItem<T>(IRevisionPointer revision, TreeFile file)
			where T : ToolStripItem, new()
		{
			if(file == null) throw new ArgumentNullException("file");

			return GetCheckoutPathItem<T>(revision, file.RelativePath);
		}

		public static T GetCheckoutPathItem<T>(IRevisionPointer revision, TreeDirectory directory)
			where T : ToolStripItem, new()
		{
			if(directory == null) throw new ArgumentNullException("directory");

			return GetCheckoutPathItem<T>(revision, directory.RelativePath + "/");
		}

		public static T GetCheckoutPathItem<T>(IRevisionPointer revision, string path)
			where T : ToolStripItem, new()
		{
			if(revision == null) throw new ArgumentNullException("revision");
			if(path == null) throw new ArgumentNullException("path");
			if(path.Length == 0) throw new ArgumentException("path");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgCheckout"],
				Text = Resources.StrCheckout,
				Tag = Tuple.Create(revision, path),
			};
			item.Click += OnCheckoutPathClick;
			return item;
		}

		public static T GetCheckoutRevisionItem<T>(Repository repository, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgCheckout"],
				Text = string.Format(nameFormat, Resources.StrCheckout),
				Tag = repository,
			};
			item.Click += OnCheckoutClick;
			return item;
		}

		public static T GetCheckoutRevisionItem<T>(IRevisionPointer revision, string nameFormat)
			where T : ToolStripItem, new()
		{
			#region validate args

			if(revision == null) throw new ArgumentNullException("revision");
			if(revision.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectIsDeleted, revision.GetType().Name), "revision1");
			}

			#endregion

			bool enabled = true;
			var head = revision.Repository.Head.Pointer;
			if(head == revision)
			{
				enabled = false;
			}
			else
			{
				if((head != null) &&
					(head.Type != ReferenceType.LocalBranch) &&
					(revision.Type != ReferenceType.LocalBranch) &&
					(revision.Dereference() == head))
				{
					enabled = false;
				}
			}

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgCheckout"],
				Text = string.Format(nameFormat, Resources.StrCheckout, revision==null?string.Empty:revision.Pointer),
				Tag = revision,
				Enabled = enabled,
			};
			item.Click += OnCheckoutRevisionClick;
			return item;
		}

		public static T GetRevertItem<T>(IRevisionPointer revision)
			where T : ToolStripItem, new()
		{
			#region validate args

			if(revision == null) throw new ArgumentNullException("revision");
			if(revision.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectIsDeleted, revision.GetType().Name), "revision1");
			}

			#endregion

			bool enabled = true;
			var rev = revision as Revision;
			if(rev != null)
				enabled = rev.Parents.Count <= 1;

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRevert"],
				Text = Resources.StrRevert,
				Enabled = enabled,
				Tag = revision,
			};
			item.Click += OnRevertClick;
			return item;
		}

		public static T GetRevertItem<T>(IEnumerable<IRevisionPointer> revisions)
			where T : ToolStripItem, new()
		{
			if(revisions == null) throw new ArgumentNullException("revisions");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRevert"],
				Text = Resources.StrRevert,
				Tag = revisions,
			};
			item.Click += OnMultipleRevertClick;
			return item;
		}

		public static T GetResetItem<T>(Repository repository, ResetMode resetModes = ResetMode.Mixed | ResetMode.Hard)
			where T : ToolStripItem, new()
		{
			#region validate args

			if(repository == null) throw new ArgumentNullException("repository");

			#endregion

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgDelete"],
				Text = Resources.StrReset.AddEllipsis(),
				Tag = Tuple.Create(repository, resetModes),
			};
			item.Click += OnResetClick;
			return item;
		}

		public static T GetResetHeadHereItem<T>(IRevisionPointer revision)
			where T : ToolStripItem, new()
		{
			#region validate args

			if(revision == null) throw new ArgumentNullException("revision");
			if(revision.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectIsDeleted, revision.GetType().Name), "revision1");
			}

			#endregion

			var currentBranch = revision.Repository.Head.CurrentBranch;
			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgReset"],
				Text = currentBranch != null ?
					string.Format(Resources.StrResetBranchHere, currentBranch.Name).AddEllipsis():
					Resources.StrResetHere.AddEllipsis(),
				Tag = revision,
			};
			item.Click += OnResetHeadClick;
			return item;
		}

		public static T GetRebaseHeadHereItem<T>(IRevisionPointer revision)
			where T : ToolStripItem, new()
		{
			#region validate args

			if(revision == null) throw new ArgumentNullException("revision");
			if(revision.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectIsDeleted, revision.GetType().Name), "revision1");
			}

			#endregion

			var currentBranch = revision.Repository.Head.CurrentBranch;
			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRebase"],
				Text = currentBranch != null ?
					string.Format(Resources.StrRebaseBranchHere, currentBranch.Name) : Resources.StrRebaseHere,
				Enabled = revision.Dereference() != revision.Repository.Head.Revision,
				Tag = revision,
			};
			item.Click += OnRebaseHeadHereClick;
			return item;
		}

		public static T GetCherryPickItem<T>(IRevisionPointer revision, string nameFormat)
			where T : ToolStripItem, new()
		{
			#region validate args

			if(revision == null) throw new ArgumentNullException("revision");
			if(revision.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectIsDeleted, revision.GetType().Name), "revision1");
			}

			#endregion

			bool enabled = !revision.Repository.IsEmpty;

			if(enabled)
			{
				var rev = revision as Revision;
				if(rev != null)
				{
					enabled = rev.Parents.Count <= 1;
				}
			}

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgCherryPick"],
				Text = string.Format(nameFormat, Resources.StrCherryPick),
				Tag = revision,
				Enabled = enabled,
			};
			item.Click += OnCherryPickClick;
			return item;
		}

		public static T GetCherryPickItem<T>(IEnumerable<IRevisionPointer> revisions)
			where T : ToolStripItem, new()
		{
			if(revisions == null) throw new ArgumentNullException("revisions");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgCherryPick"],
				Text = Resources.StrCherryPick,
				Tag = revisions,
			};
			item.Click += OnMultipleCherryPickClick;
			return item;
		}

		public static T GetSavePatchItem<T>(IRevisionPointer revision)
			where T : ToolStripItem, new()
		{
			#region validate args

			if(revision == null) throw new ArgumentNullException("revision");
			if(revision.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectIsDeleted, revision.GetType().Name), "revision1");
			}

			#endregion

			var item = new T()
			{
				Image	= CachedResources.Bitmaps["ImgPatchSave"],
				Text	= Resources.StrSavePatch.AddEllipsis(),
				Tag		= revision,
			};
			item.Click += OnSaveRevisionPatchClick;
			return item;
		}

		public static T GetCompareWithItem<T>(IRevisionPointer revision1, IRevisionPointer revision2)
			where T : ToolStripItem, new()
		{
			#region validate args

			if(revision1 == null) throw new ArgumentNullException("revision1");
			if(revision2 == null) throw new ArgumentNullException("revision2");
			if(revision1.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectIsDeleted, revision1.GetType().Name), "revision1");
			}
			if(revision2.IsDeleted)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcObjectIsDeleted, revision2.GetType().Name), "revision2");
			}

			#endregion

			var item = new T()
			{
				Image	= CachedResources.Bitmaps["ImgDiff"],
				Text	= Resources.StrCompare.AddEllipsis(),
				Tag		= Tuple.Create(revision1, revision2),
			};
			item.Click += OnCompareWithClick;
			return item;
		}

		private static void OnCompareWithClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var items = (Tuple<IRevisionPointer, IRevisionPointer>)item.Tag;
			var parent = Utility.GetParentControl(item);

			var rev1 = items.Item1;
			var rev2 = items.Item2;

			RepositoryProvider.Environment.ViewDockService.ShowView(
				Views.Guids.DiffViewGuid,
				new Dictionary<string, object>()
				{
					{ "source", new RevisionCompareDiffSource(rev1, rev2) }
				},
				true);
		}

		private static void OnViewReflogClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var reference = (Reference)item.Tag;
			RepositoryProvider.Environment.ViewDockService.ShowView(
				Views.Guids.ReflogViewGuid,
				new Dictionary<string, object>()
				{
					{ "reflog", reference.Reflog }
				},
				true);
		}

		private static void SavePatch(Control parent, string defaultFileName, Func<string> getPatch)
		{
			if(getPatch == null) throw new ArgumentNullException("getPatch");

			const string patchExt = ".patch";

			string fileName = null;
			using(var dlg = new SaveFileDialog()
				{
					Filter = Resources.StrPatches + "|" + patchExt,
					FileName = defaultFileName + patchExt,
					DefaultExt = patchExt,
					OverwritePrompt = true,
					Title = Resources.StrSavePatch,
				})
			{
				if(dlg.ShowDialog(parent) == DialogResult.OK)
				{
					fileName = dlg.FileName;
				}
			}
			if(fileName != null)
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				string patch = null;
				try
				{
					patch = getPatch();
				}
				catch(GitException exc)
				{
					if(parent != null) parent.Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						parent,
						exc.Message,
						Resources.ErrFailedToFormatPatch,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
				if(parent != null) parent.Cursor = Cursors.Default;
				if(patch != null)
				{
					try
					{
						File.WriteAllText(fileName, patch);
					}
					catch(IOException exc)
					{
						GitterApplication.MessageBoxService.Show(
							parent,
							exc.Message,
							Resources.ErrFailedToSavePatch,
							MessageBoxButton.Close,
							MessageBoxIcon.Error);
					}
				}
			}
		}

		private static void OnSaveRevisionPatchClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			var repository = revision.Repository;
			var parent = Utility.GetParentControl(item);
			string fileName = revision.Dereference().Name;

			SavePatch(parent, fileName, revision.FormatPatch);
		}

		private static void OnShowViewItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var toolGuid = (Guid)item.Tag;
			RepositoryProvider.Environment.ViewDockService.ShowView(toolGuid, true);
		}

		private static void OnCheckoutClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			if(repository != null && !repository.IsEmpty)
			{
				var parent = Utility.GetParentControl(item);
				using(var dlg = new CheckoutDialog(repository))
				{
					dlg.Run(parent);
				}
			}
		}

		private static void OnCheckoutPathClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var parent = Utility.GetParentControl(item);
			var data = (Tuple<IRevisionPointer, string>)item.Tag;
			var rev = data.Item1;
			var path = data.Item2;
			var repo = rev.Repository;

			lock(repo.Status.SyncRoot)
			{
				foreach(var f in repo.Status.UnstagedFiles)
				{
					if(f.RelativePath == path)
					{
						if(GitterApplication.MessageBoxService.Show(
							parent,
							Resources.StrsCheckoutPathWarning.UseAsFormat(path),
							Resources.StrCheckout,
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Warning) != DialogResult.Yes)
						{
							return;
						}
						break;
					}
				}
			}

			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				rev.CheckoutPath(path);
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToCheckout, path),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnCheckoutRevisionClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			var parent = Utility.GetParentControl(item);
			bool force = Control.ModifierKeys == Keys.Shift;
			if(GlobalBehavior.AskOnCommitCheckouts)
			{
				bool revIsLocalBranch = (revision is Branch) && !((Branch)revision).IsRemote;
				if(!revIsLocalBranch)
				{
					var rev = revision.Dereference();
					var branches = rev.GetBranches();
					for(int i = branches.Count - 1; i >= 0; --i)
					{
						if(branches[i].IsRemote || branches[i].IsCurrent)
							branches.RemoveAt(i);
					}
					if(branches.Count != 0)
					{
						using(var dlg = new ResolveCheckoutDialog())
						{
							dlg.SetAvailableBranches(branches);
							if(dlg.Run(parent) == DialogResult.Cancel) return;
							if(!dlg.CheckoutCommit)
							{
								revision = dlg.SelectedBranch;
							}
							if(!force)
							{
								force = Control.ModifierKeys == Keys.Shift;
							}
						}
					}
				}
			}
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				revision.Checkout(force);
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(UntrackedFileWouldBeOverwrittenException)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				if(GitterApplication.MessageBoxService.Show(
					parent,
					string.Format(Resources.AskOverwriteUntracked, revision.Pointer),
					Resources.StrCheckout,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					ProceedCheckout(parent, revision);
				}
			}
			catch(HaveLocalChangesException)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				if(GitterApplication.MessageBoxService.Show(
					parent,
					string.Format(Resources.AskThrowAwayLocalChanges, revision.Pointer),
					Resources.StrCheckout,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					ProceedCheckout(parent, revision);
				}
			}
			catch(HaveConflictsException)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				if(GitterApplication.MessageBoxService.Show(
					parent,
					string.Format(Resources.AskThrowAwayConflictedChanges, revision.Pointer),
					Resources.StrCheckout,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					ProceedCheckout(parent, revision);
				}
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToCheckout, revision.Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void ProceedCheckout(Control parent, IRevisionPointer revision)
		{
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				revision.Checkout(true);
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToCheckout, revision.Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnRevertClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			var parent = Utility.GetParentControl(item);
			if(Control.ModifierKeys == Keys.Shift)
			{
				using(var dlg = new RevertDialog(revision))
				{
					dlg.Run(parent);
				}
			}
			else
			{
				try
				{
					if(parent != null) parent.Cursor = Cursors.WaitCursor;
					revision.Revert();
					if(parent != null) parent.Cursor = Cursors.Default;
				}
				catch(GitException exc)
				{
					if(parent != null) parent.Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						parent,
						exc.Message,
						Resources.ErrFailedToRevert,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		private static void OnMultipleRevertClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revisions = (IEnumerable<IRevisionPointer>)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				revisions.Revert();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToRevert,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnResetClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Tuple<Repository, ResetMode>)item.Tag;
			var parent = Utility.GetParentControl(item);
			using(var dlg = new SelectResetModeDialog(data.Item2))
			{
				if(dlg.Run(parent) == DialogResult.OK)
				{
					try
					{
						if(parent != null) parent.Cursor = Cursors.WaitCursor;
						data.Item1.Status.Reset(dlg.ResetMode);
						if(parent != null) parent.Cursor = Cursors.Default;
					}
					catch(GitException exc)
					{
						if(parent != null) parent.Cursor = Cursors.Default;
						GitterApplication.MessageBoxService.Show(
							parent,
							exc.Message,
							Resources.ErrFailedToReset,
							MessageBoxButton.Close,
							MessageBoxIcon.Error);
					}
				}
			}
		}

		private static void OnResetHeadClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			var parent = Utility.GetParentControl(item);
			using(var dlg = new SelectResetModeDialog()
			{
				ResetMode = ResetMode.Mixed
			})
			{
				if(dlg.Run(parent) == DialogResult.OK)
				{
					try
					{
						if(parent != null) parent.Cursor = Cursors.WaitCursor;
						revision.ResetHeadHere(dlg.ResetMode);
						if(parent != null) parent.Cursor = Cursors.Default;
					}
					catch(GitException exc)
					{
						if(parent != null) parent.Cursor = Cursors.Default;
						GitterApplication.MessageBoxService.Show(
							parent,
							exc.Message,
							Resources.ErrFailedToReset,
							MessageBoxButton.Close,
							MessageBoxIcon.Error);
					}
				}
			}
		}

		private static void OnRebaseHeadHereClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				revision.RebaseHeadHereAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToCherryPick, revision.Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnCherryPickClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				revision.CherryPick();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(HaveConflictsException)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					Resources.ErrCherryPickIsNotPossibleWithConflicts,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			catch(HaveLocalChangesException)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					Resources.ErrCherryPickIsNotPossibleWithLocalChnges,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			catch(AutomaticCherryPickFailedException)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				using(var dlg = new ConflictsDialog(revision.Repository))
				{
					dlg.Run(parent);
				}
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToCherryPick, revision.Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnMultipleCherryPickClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revisions = (IEnumerable<IRevisionPointer>)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				revisions.CherryPick();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(HaveConflictsException)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					Resources.ErrCherryPickIsNotPossibleWithConflicts,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			catch(HaveLocalChangesException)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					Resources.ErrCherryPickIsNotPossibleWithLocalChnges,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			catch(AutomaticCherryPickFailedException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Warning);
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		#endregion

		#region Stash Items

		public static T GetShowStashViewItem<T>()
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = Resources.StrManage,
				Tag = Views.Guids.StashViewGuid,
			};
			item.Click += OnShowViewItemClick;
			return item;
		}

		public static T GetRefreshStashItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Text = Resources.StrRefresh,
				Image = CachedResources.Bitmaps["ImgRefresh"],
				Tag = repository,
			};
			item.Click += OnRefreshStashClick;
			return item;
		}

		public static T GetStashClearItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Text = Resources.StrClear,
				Image = CachedResources.Bitmaps["ImgStashClear"],
				Tag = repository,
			};
			item.Click += OnStashClearClick;
			return item;
		}

		public static T GetStashPopItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStashPop"],
				Text = Resources.StrPop,
				Tag = repository,
			};

			item.Click += OnStashPopClick;
			return item;
		}

		public static T GetStashPopItem<T>(StashedState stashedState)
			where T : ToolStripItem, new()
		{
			if(stashedState == null) throw new ArgumentNullException("stashedState");
			if(stashedState.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "StashedState"), "stashedState");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStashPop"],
				Text = Resources.StrPop,
				Tag = stashedState,
			};

			item.Click += OnStashPopStateClick;
			return item;
		}

		public static T GetStashApplyItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStashApply"],
				Text = Resources.StrApply,
				Tag = repository,
			};

			item.Click += OnStashApplyClick;
			return item;
		}

		public static T GetStashApplyItem<T>(StashedState stashedState)
			where T : ToolStripItem, new()
		{
			if(stashedState == null) throw new ArgumentNullException("stashedState");
			if(stashedState.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "StashedState"), "stashedState");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStashApply"],
				Text = Resources.StrApply,
				Tag = stashedState,
			};

			item.Click += OnStashApplyStateClick;
			return item;
		}

		public static T GetStashDropItem<T>(StashedState stashedState)
			where T : ToolStripItem, new()
		{
			if(stashedState == null) throw new ArgumentNullException("stashedState");
			if(stashedState.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "StashedState"), "stashedState");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStashDel"],
				Text = Resources.StrDrop,
				Tag = stashedState,
			};

			item.Click += OnStashDropStateClick;
			return item;
		}

		public static T GetStashDropItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStashDel"],
				Text = Resources.StrDrop,
				Tag = repository,
			};

			item.Click += OnStashDropClick;
			return item;
		}

		public static T GetStashToBranchItem<T>(StashedState stashedState)
			where T : ToolStripItem, new()
		{
			if(stashedState == null) throw new ArgumentNullException("stashedState");
			if(stashedState.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "StashedState"), "stashedState");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgBranch"],
				Text = Resources.StrToBranch.AddEllipsis(),
				Tag = stashedState,
			};

			item.Click += OnStashToBranchClick;
			return item;
		}

		public static T GetStashSaveKeepIndexItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStashSave"],
				Text = Resources.StrStash.AddEllipsis(),
				Tag = repository,
				Enabled = !repository.IsEmpty && repository.Status.UnmergedCount == 0,
			};

			item.Click += OnStashSaveKeepIndexItemClick;
			return item;
		}

		public static T GetStashSaveItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStashSave"],
				Text = Resources.StrStash.AddEllipsis(),
				Tag = repository,
				Enabled = !repository.IsEmpty && repository.Status.UnmergedCount == 0,
			};

			item.Click += OnStashSaveItemClick;
			return item;
		}

		private static void OnRefreshStashClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			repository.Stash.Refresh();
		}

		private static void OnStashClearClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				repository.Stash.ClearAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStashClear,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStashSaveItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			using(var dlg = new StashSaveDialog(repository))
			{
				dlg.Run(parent);
			}
			//try
			//{
			//    repository.Stash.SaveAsync(false, string.Empty).Invoke<ProgressForm>(parent);
			//}
			//catch(GitException exc)
			//{
			//    GitterApplication.MessageBoxService.Show(
			//        parent,
			//        exc.Message,
			//        Resources.ErrFailedToStash,
			//        MessageBoxButton.Close,
			//        MessageBoxIcon.Error);
			//}
		}

		private static void OnStashSaveKeepIndexItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			using(var dlg = new StashSaveDialog(repository)
				{
					KeepIndex = true,
				})
			{
				dlg.Run(parent);
			}
			//try
			//{
			//    repository.Stash.SaveAsync(true, string.Empty).Invoke<ProgressForm>(parent);
			//}
			//catch(GitException exc)
			//{
			//    GitterApplication.MessageBoxService.Show(
			//        parent,
			//        exc.Message,
			//        Resources.ErrFailedToStash,
			//        MessageBoxButton.Close,
			//        MessageBoxIcon.Error);
			//}
		}

		private static void OnStashPopClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			bool restoreIndex = Control.ModifierKeys == Keys.Shift;
			try
			{
				repository.Stash.PopAsync(restoreIndex).Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStashPop,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStashPopStateClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var stashedState = (StashedState)item.Tag;
			var parent = Utility.GetParentControl(item);
			bool restoreIndex = Control.ModifierKeys == Keys.Shift;
			try
			{
				stashedState.PopAsync(restoreIndex).Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToStashPopState, ((IRevisionPointer)stashedState).Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStashApplyClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			bool restoreIndex = Control.ModifierKeys == Keys.Shift;
			try
			{
				repository.Stash.ApplyAsync(restoreIndex).Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStashApply,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStashApplyStateClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var stashedState = (StashedState)item.Tag;
			var parent = Utility.GetParentControl(item);
			bool restoreIndex = Control.ModifierKeys == Keys.Shift;
			try
			{
				stashedState.ApplyAsync(restoreIndex).Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToStashApplyState, ((IRevisionPointer)stashedState).Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStashDropStateClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var stashedState = (StashedState)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				stashedState.DropAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToStashDropState, ((IRevisionPointer)stashedState).Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStashDropClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				repository.Stash.DropAsync().Invoke<ProgressForm>(parent);
			}
			catch(InvalidOperationException)
			{
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStashDrop,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}
		private static void OnStashToBranchClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var stashedState = (StashedState)item.Tag;
			using(var dlg = new StashToBranchDialog(stashedState))
			{
				dlg.Run(Utility.GetParentControl(item));
			}
		}

		#endregion

		#region Note Items

		public static T GetAddNoteItem<T>(IRevisionPointer revision)
			where T : ToolStripItem, new()
		{
			if(revision == null) throw new ArgumentNullException("revision");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgNoteAdd"],
				Text = Resources.StrAddNote.AddEllipsis(),
				Tag = revision,
			};
			item.Click += OnAddNoteClick;
			return item;
		}

		private static void OnAddNoteClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			var parent = Utility.GetParentControl(item);

			using(var dlg = new AddNoteDialog(revision.Repository)
				{
					Revision = revision.Pointer,
					AllowChangeRevision = false,
				})
			{
				dlg.Run(parent);
			}
		}

		#endregion

		#region Branch Items

		public static T GetCreateBranchItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null)
				throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgBranchAdd"],
				Text = Resources.StrCreateBranch.AddEllipsis(),
				Tag = repository,
			};
			item.Click += OnCreateBranchClick;
			return item;
		}

		public static T GetCreateBranchItem<T>(IRevisionPointer revision)
			where T : ToolStripItem, new()
		{
			if(revision != null && revision.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Revision pointer"), "revision");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgBranchAdd"],
				Text = Resources.StrCreateBranch.AddEllipsis(),
				Tag = revision,
			};
			item.Click += OnCreateBranchAtClick;
			return item;
		}

		public static T GetRemoveBranchItem<T>(BranchBase branch)
			where T : ToolStripItem, new()
		{
			return GetRemoveBranchItem<T>(branch, branch.IsRemote ? Resources.StrDelete.AddEllipsis() : Resources.StrDelete);
		}

		public static T GetRemoveBranchItem<T>(BranchBase branch, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(branch == null) throw new ArgumentNullException("branch");
			if(branch.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Branch"), "branch");

			var item = new T()
			{
				Image = CachedResources.Bitmaps[branch.IsRemote?"ImgBranchRemoteDel":"ImgBranchDel"],
				Text = string.Format(nameFormat, Resources.StrRemoveBranch, branch.Name),
				Tag = branch,
				Enabled = !branch.IsCurrent,
			};
			item.Click += OnRemoveBranchClick;
			return item;
		}

		public static T GetRenameBranchItem<T>(Branch branch, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(branch == null) throw new ArgumentNullException("branch");
			if(branch.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Branch"), "branch");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgBranchRename"],
				Text = string.Format(nameFormat, Resources.StrRename.AddEllipsis(), branch.Name),
				Tag = branch,
			};
			item.Click += OnRenameBranchClick;
			return item;
		}

		public static T GetMergeBranchItem<T>(IRevisionPointer revision)
			where T : ToolStripItem, new()
		{
			if(revision == null) throw new ArgumentNullException("branch");
			if(revision.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Branch"), "branch");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgMerge"],
				Text = Resources.StrMerge,
				Tag = revision,
				Enabled = revision != revision.Repository.Head.Pointer,
			};
			item.Click += OnMergeBranchClick;
			return item;
		}

		public static T GetPushBranchToRemoteItem<T>(Branch branch, Remote remote)
			where T : ToolStripItem, new()
		{
			if(branch == null) throw new ArgumentNullException("branch");
			if(remote == null) throw new ArgumentNullException("remote");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRemote"],
				Text = remote.Name,
				Tag = Tuple.Create(branch, remote),
			};
			item.Click += new EventHandler(OnPushBranchToRemoteClick);
			return item;
		}

		private static void OnPushBranchToRemoteClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var parent = Utility.GetParentControl(item);
			var data = (Tuple<Branch, Remote>)item.Tag;
			var branch = data.Item1;
			var remote = data.Item2;

			try
			{
				remote.PushAsync(new Branch[] { branch }, false, true, false).Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrPushFailed, remote.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnCreateBranchClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			using(var dlg = new CreateBranchDialog(repository)
			{
				StartingRevision = GitConstants.HEAD,
			})
			{
				dlg.Run(Utility.GetParentControl(item));
			}
		}

		private static void OnCreateBranchAtClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			string startingRevision;
			string defaultName;
			if(revision != null)
			{
				startingRevision = revision.Pointer;
				var branch = revision as Branch;
				if(branch != null && branch.IsRemote)
				{
					defaultName = branch.Name.Substring(branch.Name.LastIndexOf('/') + 1);
				}
				else
				{
					defaultName = string.Empty;
				}
			}
			else
			{
				startingRevision = string.Empty;
				defaultName = string.Empty;
			}
			using(var dlg = new CreateBranchDialog(revision.Repository)
			{
				StartingRevision = startingRevision,
			})
			{
				if(defaultName != string.Empty)
					dlg.BranchName = defaultName;
				dlg.Run(Utility.GetParentControl(item));
			}
		}

		private static void OnRemoveBranchClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var branch = (BranchBase)item.Tag;
			var parent = Utility.GetParentControl(item);
			if(branch != null)
			{
				if(branch.IsRemote)
				{
					using(var dlg = new RemoveRemoteBranchDialog((RemoteBranch)branch))
					{
						dlg.Run(parent);
					}
				}
				else
				{
					try
					{
						bool force = Control.ModifierKeys == Keys.Shift;
						if(parent != null) parent.Cursor = Cursors.WaitCursor;
						branch.Delete(force);
						if(parent != null) parent.Cursor = Cursors.Default;
					}
					catch(BranchIsNotFullyMergedException)
					{
						if(parent != null) parent.Cursor = Cursors.Default;
						if(GitterApplication.MessageBoxService.Show(
							parent,
							string.Format(Resources.StrAskBranchIsNotFullyMerged, branch.Name),
							Resources.StrDeleteBranch,
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Question) == DialogResult.Yes)
						{
							try
							{
								if(parent != null) parent.Cursor = Cursors.WaitCursor;
								branch.Delete(true);
								if(parent != null) parent.Cursor = Cursors.Default;
							}
							catch(GitException exc)
							{
								if(parent != null) parent.Cursor = Cursors.Default;
								GitterApplication.MessageBoxService.Show(
									parent,
									exc.Message,
									string.Format(Resources.ErrFailedToRemoveBranch, branch.Name),
									MessageBoxButton.Close,
									MessageBoxIcon.Error);
							}
						}
					}
					catch(GitException exc)
					{
						if(parent != null) parent.Cursor = Cursors.Default;
						GitterApplication.MessageBoxService.Show(
							parent,
							exc.Message,
							string.Format(Resources.ErrFailedToRemoveBranch, branch.Name),
							MessageBoxButton.Close,
							MessageBoxIcon.Error);
					}
				}
			}
		}

		private static void OnRenameBranchClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var branch = (Branch)item.Tag;
			if(branch != null)
			{
				using(var dlg = new RenameBranchDialog(branch))
				{
					dlg.Run(Utility.GetParentControl(item));
				}
			}
		}

		private static void OnMergeBranchClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			var parent = Utility.GetParentControl(item);
			if(revision != null)
			{
				if(Control.ModifierKeys == Keys.Shift)
				{
					using(var dlg = new MergeDialog(revision.Repository)
						{
							Branch = revision.Pointer,
							AllowChangingBranch = false,
						})
					{
						dlg.Run(parent);
					}
				}
				else
				{
					try
					{
						if(parent != null) parent.Cursor = Cursors.WaitCursor;
						revision.Repository.Head.Merge(revision);
						if(parent != null) parent.Cursor = Cursors.Default;
					}
					catch(AutomaticMergeFailedException)
					{
						if(parent != null) parent.Cursor = Cursors.Default;
						using(var dlg = new ConflictsDialog(revision.Repository))
						{
							dlg.Run(parent);
						}
					}
					catch(GitException exc)
					{
						if(parent != null) parent.Cursor = Cursors.Default;
						GitterApplication.MessageBoxService.Show(
							parent,
							exc.Message,
							string.Format(Resources.ErrFailedToMergeWith, revision.Pointer),
							MessageBoxButton.Close,
							MessageBoxIcon.Error);
					}
				}
			}
		}

		#endregion

		#region Tag Items

		public static T GetCreateTagItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgTagAdd"],
				Text = Resources.StrCreateTag.AddEllipsis(),
				Tag = repository,
			};
			item.Click += OnCreateTagClick;
			return item;
		}

		public static T GetCreateTagItem<T>(IRevisionPointer revision)
			where T : ToolStripItem, new()
		{
			if(revision != null && revision.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Revision pointer"), "revision");
			
			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgTagAdd"],
				Text = Resources.StrCreateTag.AddEllipsis(),
				Tag = revision,
			};
			item.Click += OnCreateTagAtClick;
			return item;
		}

		public static T GetRemoveTagItem<T>(Tag tag, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(tag == null) throw new ArgumentNullException("tag");
			if(tag.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Tag"), "tag");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgTagDel"],
				Text = string.Format(nameFormat, Resources.StrRemoveTag, tag.Name),
				Tag = tag,
			};
			item.Click += OnRemoveTagClick;
			return item;
		}

		private static void OnCreateTagClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			using(var dlg = new CreateTagDialog(repository)
			{
				Revision = GitConstants.HEAD,
			})
			{
				dlg.Run(Utility.GetParentControl(item));
			}
		}

		private static void OnCreateTagAtClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revision = (IRevisionPointer)item.Tag;
			using(var dlg = new CreateTagDialog(revision.Repository)
			{
				Revision = (revision != null) ? revision.Pointer : GitConstants.HEAD,
			})
			{
				dlg.Run(Utility.GetParentControl(item));
			}
		}

		private static void OnRemoveTagClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var tag = (Tag)item.Tag;
			var parent = Utility.GetParentControl(item);
			if(tag != null)
			{
				try
				{
					if(parent != null) parent.Cursor = Cursors.WaitCursor;
					tag.Delete();
					if(parent != null) parent.Cursor = Cursors.Default;
				}
				catch(GitException exc)
				{
					if(parent != null) parent.Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						parent,
						exc.Message,
						string.Format(Resources.ErrFailedToRemoveTag, tag.Name),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		#endregion

		#region Remote Items

		public static T GetEditRemotePropertiesItem<T>(Remote remote)
			where T : ToolStripItem, new()
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Remote"), "remote");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRemoteProperties"],
				Text = Resources.StrProperties.AddEllipsis(),
				Tag = remote,
			};
			item.Click += OnEditRemotePropertiesClick;
			return item;
		}

		public static T GetBrowseRemoteItem<T>(Remote remote)
			where T : ToolStripItem, new()
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Remote"), "remote");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgSearch"],
				Text = Resources.StrBrowse.AddEllipsis(),
				Tag = remote,
			};
			item.Click += OnBrowseRemoteClick;
			return item;
		}

		public static T GetFetchItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			return GetFetchItem<T>(repository, "{0}");
		}

		public static T GetFetchItem<T>(Repository repository, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");
			
			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgFetch"],
				Text = string.Format(nameFormat, Resources.StrFetch),
				Tag = repository,
				Enabled = repository.Remotes.Count != 0,
			};
			item.Click += OnFetchClick;
			return item;
		}

		public static T GetFetchFromItem<T>(Remote remote, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Remote"), "remote");
			
			var item = new T()
			{
				Image = CachedResources.Bitmaps[nameFormat=="{1}"?"ImgRemote":"ImgFetch"],
				Text = string.Format(nameFormat, Resources.StrFetch, remote.Name),
				Tag = remote,
			};
			item.Click += OnFetchFromClick;
			return item;
		}

		public static T GetPullItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			return GetPullItem<T>(repository, "{0}");
		}

		public static T GetPullItem<T>(Repository repository, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgPull"],
				Text = string.Format(nameFormat, Resources.StrPull),
				Tag = repository,
				Enabled = repository.Remotes.Count != 0,
			};
			item.Click += OnPullClick;
			return item;
		}

		public static T GetPullFromItem<T>(Remote remote, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Remote"), "remote");
			if(nameFormat == null) throw new ArgumentNullException("nameFormat");

			var item = new T()
			{
				Image = CachedResources.Bitmaps[nameFormat=="{1}"?"ImgRemote":"ImgPull"],
				Text = string.Format(nameFormat, Resources.StrPull, remote.Name),
				Tag = remote,
			};
			item.Click += OnPullFromClick;
			return item;
		}

		public static T GetRefreshRemotesItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRefresh"],
				Text = Resources.StrRefresh,
				Tag = repository,
			};
			item.Click += OnRefreshRemotesClick;
			return item;
		}

		public static T GetShowRemotesViewItem<T>()
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				//Image = CachedResources.Bitmaps["ImgRemotes"],
				Text = Resources.StrManage,
				Tag = Views.Guids.RemotesViewGuid,
			};
			item.Click += OnShowViewItemClick;
			return item;
		}

		public static T GetAddRemoteItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRemoteAdd"],
				Text = Resources.StrAddRemote.AddEllipsis(),
				Tag = repository,
			};
			item.Click += OnAddRemoteClick;
			return item;
		}

		public static T GetRemoveRemoteItem<T>(Remote remote, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Remote"), "remote");
			if(nameFormat == null) throw new ArgumentNullException("nameFormat");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRemoteRemove"],
				Text = string.Format(nameFormat, Resources.StrRemove, remote.Name),
				Tag = remote,
			};
			item.Click += OnRemoveRemoteClick;
			return item;
		}

		public static T GetRenameRemoteItem<T>(Remote remote, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "remote"), "remote");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRemoteRename"],
				Text = string.Format(nameFormat, Resources.StrRename.AddEllipsis(), remote.Name),
				Tag = remote,
			};
			item.Click += OnRenameRemoteClick;
			return item;
		}

		public static T GetPruneRemoteItem<T>(Remote remote, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(nameFormat == null) throw new ArgumentNullException("nameFormat");
			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgClean"],
				Text = string.Format(nameFormat, Resources.StrPruneRemote, remote.Name),
				Tag = remote,
			};
			item.Click += OnPruneRemoteClick;
			return item;
		}

		public static T GetRemoveRemoteBranchItem<T>(RemoteRepositoryBranch remoteBranch, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(remoteBranch == null) throw new ArgumentNullException("remoteBranch");
			if(remoteBranch.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "RemoteBranch"), "remoteBranch");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgBranchDel"],
				Text = string.Format(nameFormat, Resources.StrRemoveBranch, remoteBranch.Name),
				Tag = remoteBranch,
			};
			item.Click += OnRemoveRemoteReferenceClick;
			return item;
		}

		public static T GetRemoveRemoteTagItem<T>(RemoteRepositoryTag remoteTag, string nameFormat)
			where T : ToolStripItem, new()
		{
			if(remoteTag == null) throw new ArgumentNullException("remoteTag");
			if(remoteTag.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "RemoteTag"), "remoteTag");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgTagDel"],
				Text = string.Format(nameFormat, Resources.StrRemoveTag, remoteTag.Name),
				Tag = remoteTag,
			};
			item.Click += OnRemoveRemoteReferenceClick;
			return item;
		}

		private static void OnEditRemotePropertiesClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var remote = (Remote)item.Tag;
			var parent = Utility.GetParentControl(item);
			using(var d = new RemotePropertiesDialog(remote))
			{
				d.Run(parent);
			}
		}

		private static void OnBrowseRemoteClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var remote = (Remote)item.Tag;
			var parent = Utility.GetParentControl(item);
			using(var d = new RemoteReferencesDialog(remote))
			{
				d.Run(parent);
			}
		}

		private static void OnRemoveRemoteReferenceClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var reference = (IRemoteReference)item.Tag;
			var parent = Utility.GetParentControl(item);
			if(GitterApplication.MessageBoxService.Show(
				parent,
				Resources.AskRemoveRemoteReference,
				Resources.StrRemoveRemoteReference,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				try
				{
					parent.Cursor = Cursors.WaitCursor;
					reference.Delete();
					parent.Cursor = Cursors.Default;
				}
				catch(GitException exc)
				{
					parent.Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						parent,
						exc.Message,
						string.Format(reference.ReferenceType == ReferenceType.LocalBranch ?
							Resources.ErrFailedToRemoveBranch : Resources.ErrFailedToRemoveTag, reference.Name),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		private static void OnPruneRemoteClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var remote = (Remote)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				parent.Cursor = Cursors.WaitCursor;
				remote.PruneAsync().Invoke<ProgressForm>(parent);
				parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToPrune, remote.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnFetchClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				repository.Remotes.FetchAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToFetch,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}


		private static void OnPullClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				repository.Remotes.PullAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToPull,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}
		private static void OnFetchFromClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var remote = (Remote)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				remote.FetchAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToFetchFrom, remote.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnPullFromClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var remote = (Remote)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				remote.PullAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToPullFrom, remote.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnRefreshRemotesClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			repository.Remotes.Refresh();
		}

		private static void OnAddRemoteClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			using(var dlg = new AddRemoteDialog(repository))
			{
				dlg.Run(Utility.GetParentControl(item));
			}
		}

		private static void OnRenameRemoteClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var remote = (Remote)item.Tag;
			using(var dlg = new RenameRemoteDialog(remote))
			{
				dlg.Run(Utility.GetParentControl(item));
			}
		}

		private static void OnRemoveRemoteClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var remote = (Remote)item.Tag;
			var parent = Utility.GetParentControl(item);
			if(remote != null)
			{
				try
				{
					if(parent != null) parent.Cursor = Cursors.WaitCursor;
					remote.Delete();
					if(parent != null) parent.Cursor = Cursors.Default;
				}
				catch(GitException exc)
				{
					if(parent != null) parent.Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						parent,
						exc.Message,
						string.Format(Resources.ErrFailedToRemoveRemote, remote.Name),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		#endregion

		#region Submodule Items

		public static T GetShowSubmodulesViewItem<T>()
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = Resources.StrManage,
				Tag = Views.Guids.SubmodulesViewGuid,
			};
			item.Click += OnShowViewItemClick;
			return item;
		}

		public static T GetRefreshSubmodulesItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Text = Resources.StrRefresh,
				Image = CachedResources.Bitmaps["ImgRefresh"],
				Tag = repository,
			};
			item.Click += OnRefreshSubmodulesClick;
			return item;
		}

		public static T GetAddSubmoduleItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Text = Resources.StrAddSubmodule.AddEllipsis(),
				Image = CachedResources.Bitmaps["ImgSubmoduleAdd"],
				Tag = repository,
			};
			item.Click += OnAddSubmoduleClick;
			return item;
		}

		public static T GetUpdateSubmoduleItem<T>(Submodule submodule)
			where T : ToolStripItem, new()
		{
			if(submodule == null) throw new ArgumentNullException("submodule");
			if(submodule.IsDeleted) throw new ArgumentNullException(Resources.ExcObjectIsDeleted.UseAsFormat("Submodule"), "submodule");

			var item = new T()
			{
				Text = Resources.StrUpdate,
				Image = CachedResources.Bitmaps["ImgPull"],
				Tag = submodule,
			};
			item.Click += OnUpdateSubmoduleClick;
			return item;
		}

		public static T GetUpdateSubmodulesItem<T>(SubmodulesCollection submodules)
			where T : ToolStripItem, new()
		{
			if(submodules == null) throw new ArgumentNullException("submodules");

			var item = new T()
			{
				Text = Resources.StrUpdate,
				Image = CachedResources.Bitmaps["ImgPull"],
				Tag = submodules,
				Enabled = submodules.Count != 0,
			};
			item.Click += OnUpdateSubmodulesClick;
			return item;
		}

		static void OnRefreshSubmodulesClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;

			repository.Submodules.Refresh();
		}

		static void OnAddSubmoduleClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);

			using(var dlg = new AddSubmoduleDialog(repository))
			{
				dlg.Run(parent);
			}
		}

		static void OnUpdateSubmoduleClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var submodule = (Submodule)item.Tag;
			var parent = Utility.GetParentControl(item);

			try
			{
				submodule.UpdateAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					string.Format(Resources.ErrFailedToUpdateSubmodule, submodule.Name),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		static void OnUpdateSubmodulesClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var submodules = (SubmodulesCollection)item.Tag;
			var parent = Utility.GetParentControl(item);

			try
			{
				submodules.UpdateAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToUpdateSubmodule,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		#endregion

		#region Working Tree Items

		public static T GetMarkAsResolvedItem<T>(TreeItem treeItem)
			where T : ToolStripItem, new()
		{
			if(treeItem == null) throw new ArgumentNullException("treeItem");
			if(treeItem.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Working tree item"), "treeItem");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgMarkAsResolved"],
				Text = Resources.StrMarkAsResolved,
				Tag = treeItem,
			};
			item.Click += OnStageClick;
			return item;
		}

		public static T GetStageItem<T>(TreeItem treeItem)
			where T : ToolStripItem, new()
		{
			if(treeItem == null) throw new ArgumentNullException("treeItem");
			if(treeItem.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Working tree item"), "treeItem");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStage"],
				Text = Resources.StrStage,
				Tag = treeItem,
			};
			item.Click += OnStageClick;
			return item;
		}

		public static T GetStageItem<T>(Repository repository, TreeItem[] treeItems)
			where T : ToolStripItem, new()
		{
			if(treeItems == null) throw new ArgumentNullException("treeItems");
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStage"],
				Text = Resources.StrStage,
				Tag = Tuple.Create(repository, treeItems),
			};
			item.Click += OnStageSeveralClick;
			return item;
		}

		public static T GetManualStageItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			return GetManualStageItem<T>(repository, Resources.StrManual.AddEllipsis());
		}

		public static T GetManualStageItem<T>(Repository repository, string name)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = null,
				Text = name,
				Tag = repository,
			};
			item.Click += OnManualStageClick;
			return item;
		}

		public static T GetStageAllItem<T>(Repository repository, string name)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgStage"],
				Text = name,
				Tag = repository,
			};
			item.Click += OnStageAllClick;
			return item;
		}

		public static T GetUpdateItem<T>(Repository repository, string name)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = null,
				Text = name,
				Tag = repository,
			};
			item.Click += OnUpdateClick;
			return item;
		}

		public static T GetCommitItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgCommit"],
				Text = Resources.StrCommit.AddEllipsis(),
				Tag = repository,
			};
			item.Click += OnCommitClick;
			return item;
		}

		public static T GetUnstageItem<T>(TreeItem treeItem)
			where T : ToolStripItem, new()
		{
			if(treeItem == null) throw new ArgumentNullException("treeItem");
			if(treeItem.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Working tree item"), "treeItem");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgUnstage"],
				Text = Resources.StrUnstage,
				Tag = treeItem,
			};
			item.Click += OnUnstageClick;
			return item;
		}

		public static T GetUnstageItem<T>(Repository repository, TreeItem[] treeItems)
			where T : ToolStripItem, new()
		{
			if(treeItems == null) throw new ArgumentNullException("treeItems");
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgUnstage"],
				Text = Resources.StrUnstage,
				Tag = Tuple.Create(repository, treeItems),
			};
			item.Click += OnUnstageSeveralClick;
			return item;
		}

		public static T GetUnstageAllItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			return GetUnstageAllItem<T>(repository, Resources.StrUnstageAll);
		}

		public static T GetUnstageAllItem<T>(Repository repository, string name)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgUnstage"],
				Text = name,
				Tag = repository,
			};
			item.Click += OnUnstageAllClick;
			return item;
		}

		public static T GetMergeToolItem<T>(TreeFile file, MergeTool mergeTool)
			where T : ToolStripItem, new()
		{
			if(file == null) throw new ArgumentNullException("file");
			if(file.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "TreeFile"), "file");

			string text;
			switch(file.ConflictType)
			{
				case ConflictType.DeletedByThem:
				case ConflictType.DeletedByUs:
				case ConflictType.AddedByThem:
				case ConflictType.AddedByUs:
					text = Resources.StrResolveConflict.AddEllipsis();
					break;
				default:
					text = mergeTool == null ?
						Resources.StrRunMergeTool.AddEllipsis() :
						mergeTool.Name.AddEllipsis();
					break;
			}

			var item = new T()
			{
				Image = null,
				Text = text,
				Tag = Tuple.Create(file, mergeTool),
			};
			item.Click += OnMergeToolClick;
			return item;
		}

		public static T GetResolveConflictItem<T>(TreeFile file, ConflictResolution resolution)
			where T : ToolStripItem, new()
		{
			if(file == null) throw new ArgumentNullException("file");
			if(file.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "TreeFile"), "file");

			string text;
			switch(resolution)
			{
				case ConflictResolution.DeleteFile:
					text = Resources.StrDeleteFile;
					break;
				case ConflictResolution.KeepModifiedFile:
					text = Resources.StrKeepFile;
					break;
				case ConflictResolution.UseOurs:
					text = Resources.StrUseOurs;
					break;
				case ConflictResolution.UseTheirs:
					text = Resources.StrUseTheirs;
					break;
				default:
					throw new ArgumentException("resolution");
			}

			var item = new T()
			{
				Image = null,
				Text = text,
				Tag = Tuple.Create(file, resolution),
			};
			item.Click += OnResolveConflictItemClick;
			return item;
		}

		public static T GetMergeToolItem<T>(TreeFile file)
			where T : ToolStripItem, new()
		{
			return GetMergeToolItem<T>(file, null);
		}

		public static T GetRevertPathItem<T>(TreeItem treeItem)
			where T : ToolStripItem, new()
		{
			if(treeItem == null) throw new ArgumentNullException("treeItem");
			if(treeItem.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Working tree item"), "treeItem");

			var item = new T()
			{
				Text = Resources.StrRevert,
				Image = CachedResources.Bitmaps["ImgRevert"],
				Tag = treeItem,
			};
			item.Click += OnRevertPathClick;
			return item;
		}

		public static T GetRevertPathsItem<T>(IEnumerable<TreeItem> treeItems)
			where T : ToolStripItem, new()
		{
			if(treeItems == null) throw new ArgumentNullException("treeItem");

			var item = new T()
			{
				Text = Resources.StrRevert,
				Image = CachedResources.Bitmaps["ImgRevert"],
				Tag = treeItems,
			};
			item.Click += OnRevertPathsClick;
			return item;
		}

		public static T GetRemovePathItem<T>(TreeItem treeItem)
			where T : ToolStripItem, new()
		{
			if(treeItem == null) throw new ArgumentNullException("treeItem");
			if(treeItem.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "Working tree item"), "treeItem");

			var item = new T()
			{
				Text = Resources.StrDelete,
				Image = CachedResources.Bitmaps["ImgDelete"],
				Tag = treeItem,
			};
			item.Click += OnDeletePathClick;
			return item;
		}

		public static T GetBlameItem<T>(IRevisionPointer revision, TreeFile file)
			where T : ToolStripItem, new()
		{
			if(file == null) throw new ArgumentNullException("file");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgBlame"],
				Text = Resources.StrBlame,
				Tag = new RevisionFileBlameSource(revision, file.RelativePath),
			};
			item.Click += OnBlameClick;
			return item;
		}

		public static T GetBlameItem<T>(IRevisionPointer revision, string fileName)
			where T : ToolStripItem, new()
		{
			if(fileName == null) throw new ArgumentNullException("fileName");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgBlame"],
				Text = Resources.StrBlame,
				Tag = new RevisionFileBlameSource(revision, fileName),
			};
			item.Click += OnBlameClick;
			return item;
		}

		public static T GetPathHistoryItem<T>(IRevisionPointer revision, TreeFile file)
			where T : ToolStripItem, new()
		{
			if(file == null) throw new ArgumentNullException("file");

			return GetPathHistoryItem<T>(revision, file.RelativePath);
		}

		public static T GetPathHistoryItem<T>(IRevisionPointer revision, TreeDirectory directory)
			where T : ToolStripItem, new()
		{
			if(directory == null) throw new ArgumentNullException("directory");

			return GetPathHistoryItem<T>(revision, directory.RelativePath + "/");
		}

		public static T GetPathHistoryItem<T>(IRevisionPointer revision, string path)
			where T : ToolStripItem, new()
		{
			if(revision == null) throw new ArgumentNullException("revision");
			if(path == null) throw new ArgumentNullException("path");

			var item = new T()
			{
				Image = CachedResources.Bitmaps[path.EndsWith('/') ? "ImgFolderHistory" : "ImgFileHistory"],
				Text = Resources.StrHistory,
				Tag = new PathLogSource(revision, path),
			};
			item.Click += OnPathHistoryClick;
			return item;
		}

		private static void OnBlameClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (IBlameSource)item.Tag;

			RepositoryProvider.Environment.ViewDockService.ShowView(
				Views.Guids.BlameViewGuid,
				new Dictionary<string, object>()
				{
					{ "blame", data }
				});
		}

		private static void OnPathHistoryClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (ILogSource)item.Tag;

			RepositoryProvider.Environment.ViewDockService.ShowView(
				Views.Guids.PathHistoryViewGuid,
				new Dictionary<string, object>()
				{
					{ "source", data }
				});
		}

		private static void OnResolveConflictItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var parent = Utility.GetParentControl(item);
			var data = (Tuple<TreeFile, ConflictResolution>)item.Tag;
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				data.Item1.ResolveConflict(data.Item2);
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToResolve,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnCommitClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);

			using(var dlg = new CommitDialog(repository))
			{
				dlg.Run(parent);
			}
		}

		private static void OnUnstageAllClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				repository.Status.UnstageAll();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToUnstage,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnDeletePathClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var treeItem = (TreeItem)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(GitterApplication.MessageBoxService.Show(
					parent,
					Resources.StrAskDeletePath.UseAsFormat(treeItem.RelativePath),
					Resources.StrDelete,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning) == DialogResult.No)
				{
					return;
				}
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				treeItem.RemoveFromWorkingTree();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToDeletePath.UseAsFormat(treeItem.RelativePath),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnRevertPathsClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var treeItems = (IEnumerable<TreeItem>)item.Tag;
			var parent = Utility.GetParentControl(item);

			Repository repository = null;
			foreach(var treeItem in treeItems)
			{
				repository = treeItem.Repository;
				break;
			}

			if(repository != null)
			{
				try
				{
					if(GitterApplication.MessageBoxService.Show(
						parent,
						Resources.StrAskRevertPaths,
						Resources.StrRevert,
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Warning) == DialogResult.No)
					{
						return;
					}

					var paths = new List<string>();
					foreach(var treeItem in treeItems)
					{
						paths.Add(treeItem.RelativePath);
					}

					if(parent != null) parent.Cursor = Cursors.WaitCursor;
					try
					{
						using(repository.Monitor.BlockNotifications(
							RepositoryNotifications.WorktreeUpdated))
						{
								repository.Accessor.CheckoutFiles(new CheckoutFilesParameters(paths)
									{
										Mode = CheckoutFileMode.IgnoreUnmergedEntries,
									});
						}
					}
					finally
					{
						repository.Status.Refresh();
					}
					if(parent != null) parent.Cursor = Cursors.Default;
				}
				catch(GitException exc)
				{
					if(parent != null) parent.Cursor = Cursors.Default;
					GitterApplication.MessageBoxService.Show(
						parent,
						exc.Message,
						Resources.ErrFailedToRevertPaths,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		private static void OnRevertPathClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var treeItem = (TreeItem)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(treeItem.Type == TreeItemType.Tree || treeItem.Status == FileStatus.Modified)
				{
					if(GitterApplication.MessageBoxService.Show(
						parent,
						Resources.StrAskRevertPath.UseAsFormat(treeItem.RelativePath),
						Resources.StrRevert,
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Warning) == DialogResult.No)
					{
						return;
					}
				}
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				treeItem.Revert();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToRevertPath.UseAsFormat(treeItem.RelativePath),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnMergeToolClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Tuple<TreeFile, MergeTool>)item.Tag;
			var file = data.Item1;
			var tool = data.Item2;
			var parent = Utility.GetParentControl(item);
			try
			{
				switch(file.ConflictType)
				{
					case ConflictType.DeletedByThem:
						using(var dlg = new ConflictResolutionDialog(file.RelativePath, FileStatus.Modified, FileStatus.Removed,
							ConflictResolution.KeepModifiedFile, ConflictResolution.DeleteFile))
						{
							if(dlg.Run(parent) == DialogResult.OK)
							{
								file.ResolveConflict(dlg.ConflictResolution);
							}
						}
						break;
					case ConflictType.DeletedByUs:
						using(var dlg = new ConflictResolutionDialog(file.RelativePath, FileStatus.Removed, FileStatus.Modified,
							ConflictResolution.KeepModifiedFile, ConflictResolution.DeleteFile))
						{
							if(dlg.Run(parent) == DialogResult.OK)
							{
								file.ResolveConflict(dlg.ConflictResolution);
							}
						}
						break;
					case ConflictType.AddedByThem:
						using(var dlg = new ConflictResolutionDialog(file.RelativePath, FileStatus.Removed, FileStatus.Added,
							ConflictResolution.KeepModifiedFile, ConflictResolution.DeleteFile))
						{
							if(dlg.Run(parent) == DialogResult.OK)
							{
								file.ResolveConflict(dlg.ConflictResolution);
							}
						}
						break;
					case ConflictType.AddedByUs:
						using(var dlg = new ConflictResolutionDialog(file.RelativePath, FileStatus.Added, FileStatus.Removed,
							ConflictResolution.KeepModifiedFile, ConflictResolution.DeleteFile))
						{
							if(dlg.Run(parent) == DialogResult.OK)
							{
								file.ResolveConflict(dlg.ConflictResolution);
							}
						}
						break;
					default:
						file.RunMergeToolAsync(tool).Invoke<ProgressForm>(parent);
						break;
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToRunMergeTool,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStageClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var treeItem = (TreeItem)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				treeItem.Stage();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStage,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStageSeveralClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Tuple<Repository, TreeItem[]>)item.Tag;
			var repository = data.Item1;
			var treeItems = data.Item2;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				repository.Status.Stage(treeItems);
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStage,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnStageAllClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				repository.Status.StageAll();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStage,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnUpdateClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				repository.Status.StageUpdated();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToStage,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnManualStageClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			using(var dlg = new StageDialog(repository))
			{
				dlg.Run(Utility.GetParentControl(item));
			}
		}

		private static void OnUnstageClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var treeItem = (TreeItem)item.Tag;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				treeItem.Unstage();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToUnstage,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnUnstageSeveralClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Tuple<Repository, TreeItem[]>)item.Tag;
			var repository = data.Item1;
			var treeItems = data.Item2;
			var parent = Utility.GetParentControl(item);
			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				repository.Status.Unstage(treeItems);
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToUnstage,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		#endregion

		#region ConfigParameter Items

		public static T GetShowConfigurationViewItem<T>()
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = Resources.StrManage,
				Tag = Views.Guids.ConfigViewGuid,
			};
			item.Click += OnShowViewItemClick;
			return item;
		}

		public static T GetRefreshConfigurationItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Text = Resources.StrRefresh,
				Image = CachedResources.Bitmaps["ImgRefresh"],
				Tag = repository,
			};
			item.Click += OnRefreshConfigurationClick;
			return item;
		}

		public static T GetUnsetParameterItem<T>(ConfigParameter configParameter)
			where T : ToolStripItem, new()
		{
			if(configParameter == null) throw new ArgumentNullException("configParameter");
			var item = new T()
			{
				Text = Resources.StrUnset,
				Image = CachedResources.Bitmaps["ImgConfigRemove"],
				Tag = configParameter,
			};
			item.Click += OnUnsetParameterClick;
			return item;
		}

		private static void OnRefreshConfigurationClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			repository.Configuration.Refresh();
		}

		private static void OnUnsetParameterClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var parameter = (ConfigParameter)item.Tag;
			var parent = Utility.GetParentControl(item);

			try
			{
				if(parent != null) parent.Cursor = Cursors.WaitCursor;
				parameter.Unset();
				if(parent != null) parent.Cursor = Cursors.Default;
			}
			catch(ConfigParameterDoesNotExistException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				if(parameter.ConfigFile == ConfigFile.Repository)
				{
					if(GitterApplication.MessageBoxService.Show(
						parent,
						Resources.AskRemoveConfigParameterFromAllFiles,
						Resources.AskRemoveConfigParameter.UseAsFormat(parameter.Name),
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question) == DialogResult.Yes)
					{
						try
						{
							RepositoryProvider.Git.UnsetConfigValue(
								new UnsetConfigValueParameters()
								{
									ConfigFile = Git.ConfigFile.User,
									ParameterName = parameter.Name,
								});
						}
						catch { }
						try
						{
							RepositoryProvider.Git.UnsetConfigValue(
								new UnsetConfigValueParameters()
								{
									ConfigFile = Git.ConfigFile.System,
									ParameterName = parameter.Name,
								});
						}
						catch { }
						parameter.Refresh();
					}
				}
				else
				{
					GitterApplication.MessageBoxService.Show(
						parent,
						exc.Message,
						Resources.ErrFailedToUnsetParameter,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
			catch(GitException exc)
			{
				if(parent != null) parent.Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToUnsetParameter,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		#endregion

		#region Diff Items

		public static T GetCopyDiffLinesItem<T>(IEnumerable<DiffLine> lines, string text, bool copyAsPatch, DiffLineState state)
			where T : ToolStripItem, new()
		{
			if(lines == null)
				throw new ArgumentNullException("lines");

			bool enabled = false;
			foreach(var line in lines)
			{
				if(line == null) throw new ArgumentException("lines");
				if((line.State & state) != DiffLineState.Invalid)
				{
					enabled = true;
					break;
				}
			}

			var item = new T()
			{
				Text = text,
				Image = CachedResources.Bitmaps["ImgCopyToClipboard"],
				Tag = Tuple.Create(lines, copyAsPatch, state),
				Enabled = enabled,
			};
			item.Click += OnCopyDiffLinesCick;
			return item;
		}

		public static T GetCopyDiffLinesItem<T>(IEnumerable<DiffLine> lines, bool copyAsPatch)
			where T : ToolStripItem, new()
		{
			return GetCopyDiffLinesItem<T>(lines, copyAsPatch ? Resources.StrCopyAsPatch : Resources.StrCopy, copyAsPatch,
				DiffLineState.Added | DiffLineState.Context | DiffLineState.Header | DiffLineState.NotPresent | DiffLineState.Removed);
		}

		private static void OnCopyDiffLinesCick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Tuple<IEnumerable<DiffLine>, bool, DiffLineState>)item.Tag;
			var lines = data.Item1;
			var copyAsPatch = data.Item2;
			var state = data.Item3;
			var sb = new StringBuilder();
			foreach(var line in lines)
			{
				if((line.State & state) != DiffLineState.Invalid)
				{
					if(copyAsPatch)
					{
						switch(line.State)
						{
							case DiffLineState.Added:
								sb.Append('+');
								break;
							case DiffLineState.Removed:
								sb.Append('-');
								break;
							case DiffLineState.Context:
								sb.Append(' ');
								break;
						}
					}
					sb.Append(line.Text);
					sb.Append(line.Ending);
				}
			}
			var text = sb.ToString();
			if(string.IsNullOrEmpty(text))
			{
				Clipboard.Clear();
			}
			else
			{
				Clipboard.SetText(text);
			}
		}

		#endregion

		#region Misc Items

		public static T GetExtractAndOpenFileItem<T>(Tree tree, string fileName)
			where T : ToolStripItem, new()
		{
			if(tree == null) throw new ArgumentNullException("tree");
			if(fileName == null) throw new ArgumentNullException("fileName");

			var item = new T()
			{
				Text = Resources.StrOpen,
				Tag = Tuple.Create(tree, fileName, false),
			};
			item.Click += OnExtractFileItemClick;
			return item;
		}

		public static T GetExtractAndOpenFileWithItem<T>(Tree tree, string fileName)
			where T : ToolStripItem, new()
		{
			if(tree == null) throw new ArgumentNullException("tree");
			if(fileName == null) throw new ArgumentNullException("fileName");

			var item = new T()
			{
				Text = Resources.StrOpenWith.AddEllipsis(),
				Tag = Tuple.Create(tree, fileName, true),
			};
			item.Click += OnExtractFileItemClick;
			return item;
		}

		public static T GetOpenUrlItem<T>(string name, Image image, string url)
			where T : ToolStripItem, new()
		{
			if(url == null) throw new ArgumentNullException("url");

			var item = new T()
			{
				Image = image,
				Text = name != null ? name : url,
				Tag = url,
			};
			item.Click += OnOpenUrlItemClick;
			return item;
		}

		public static T GetOpenAppItem<T>(string name, Image image, string app, string cmdLine)
			where T : ToolStripItem, new()
		{
			if(app == null) throw new ArgumentNullException("url");

			var item = new T()
			{
				Image = image,
				Text = name != null ? name : app,
				Tag = Tuple.Create(app, cmdLine),
			};
			item.Click += OnOpenAppItemClick;
			return item;
		}

		public static T GetOpenUrlWithItem<T>(string name, Image image, string url)
			where T : ToolStripItem, new()
		{
			if(url == null) throw new ArgumentNullException("url");

			var item = new T()
			{
				Image = image,
				Text = name != null ? name : url,
				Tag = url,
			};
			item.Click += OnOpenUrlWithItemClick;
			return item;
		}

		public static T GetOpenCmdAtItem<T>(string name, Image image, string path)
			where T : ToolStripItem, new()
		{
			if(path == null) throw new ArgumentNullException("path");

			var item = new T()
			{
				Image = image,
				Text = name != null ? name : path,
				Tag = path,
			};
			item.Click += OnOpenCmdAtItemClick;
			return item;
		}

		public static T GetCleanItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgClean"],
				Text = Resources.StrClean.AddEllipsis(),
				Tag = repository,
			};
			item.Click += OnCleanClick;
			return item;
		}

		public static T GetViewDiffItem<T>(IDiffSource diffSource)
			where T : ToolStripItem, new()
		{
			if(diffSource == null)
				throw new ArgumentNullException("diffSource");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgDiff"],
				Text = Resources.StrViewDiff,
				Tag = diffSource,
			};
			item.Click += OnViewDiffItemClick;
			return item;
		}

		public static T GetViewTreeItem<T>(IRevisionPointer revisionPointer)
			where T : ToolStripItem, new()
		{
			if(revisionPointer == null)
				throw new ArgumentNullException("revisionPointer");

			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgFolderTree"],
				Text = Resources.StrViewTree,
				Tag = revisionPointer,
			};
			item.Click += OnViewTreeItemClick;
			return item;
		}

		public static T GetShowReferencesViewItem<T>()
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = Resources.StrManage,
				Tag = Views.Guids.ReferencesViewGuid,
			};
			item.Click += OnShowViewItemClick;
			return item;
		}

		public static T GetRefreshAllReferencesListItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Image = CachedResources.Bitmaps["ImgRefresh"],
				Text = Resources.StrRefresh,
				Tag = repository,
			};
			item.Click += OnRefreshAllReferencesClick;
			return item;
		}

		public static T GetCopyToClipboardItem<T>(string name, Func<string> text)
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = name,
				Image = CachedResources.Bitmaps["ImgCopyToClipboard"],
				Tag = text,
			};
			item.Click += OnCopyToClipboardClick;
			return item;
		}

		public static T GetCopyToClipboardItem<T>(string name, string text)
			where T : ToolStripItem, new()
		{
			return GetCopyToClipboardItem<T>(name, text, true);
		}

		public static T GetCopyHashToClipboardItem<T>(string name, string text)
			where T : ToolStripItem, new()
		{
			return GetCopyHashToClipboardItem<T>(name, text, true);
		}

		public static T GetCopyToClipboardItem<T>(string name, string text, bool enableToolTip)
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = name,
				Image = CachedResources.Bitmaps["ImgCopyToClipboard"],
				Tag = text,
			};
			if(enableToolTip && name != text) item.ToolTipText = text;
			item.Click += OnCopyToClipboardClick;
			return item;
		}

		public static T GetCopyHashToClipboardItem<T>(string name, string text, bool enableToolTip)
			where T : ToolStripItem, new()
		{
			var item = new T()
			{
				Text = name,
				Image = CachedResources.Bitmaps["ImgCopyToClipboard"],
				Tag = text,
			};
			if(enableToolTip && name != text) item.ToolTipText = text;
			item.Click += OnCopyHashToClipboardClick;
			return item;
		}

		public static T GetRefreshReferencesItem<T>(Repository repository, ReferenceType referenceTypes, string name)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Text = name,
				Image = CachedResources.Bitmaps["ImgRefresh"],
				Tag = Tuple.Create(repository, referenceTypes),
			};
			item.Click += OnRefreshReferencesClick;
			return item;
		}

		public static T GetResolveConflictsItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Text = Resources.StrResolveConflicts.AddEllipsis(),
				Image = null,
				Tag = repository,
			};
			item.Click += OnResolveConflictsClick;
			return item;
		}

		public static T GetExpandAllItem<T>(CustomListBoxItem treeItem)
			where T : ToolStripItem, new()
		{
			if(treeItem == null) throw new ArgumentNullException("treeItem");

			var item = new T()
			{
				Text = Resources.StrExpandAll,
				Image = null,
				Enabled = treeItem.Items.Count != 0,
				Tag = treeItem,
			};
			item.Click += OnExpandAllClick;
			return item;
		}

		public static T GetCollapseAllItem<T>(CustomListBoxItem treeItem)
			where T : ToolStripItem, new()
		{
			if(treeItem == null) throw new ArgumentNullException("treeItem");

			var item = new T()
			{
				Text = Resources.StrCollapseAll,
				Image = null,
				Enabled = treeItem.Items.Count != 0,
				Tag = treeItem,
			};
			item.Click += OnCollapseAllClick;
			return item;
		}

		public static T GetSendEmailItem<T>(string email)
			where T : ToolStripItem, new()
		{
			if(email == null) throw new ArgumentNullException("email");

			var item = new T()
			{
				Text = Resources.StrSendEmail,
				Image = CachedResources.Bitmaps["ImgMailSend"],
				Tag = email,
				ToolTipText = email,
				Enabled = email.Length != 0,
			};
			item.Click += OnSendEmailClick;
			return item;
		}

		public static T GetCompressRepositoryItem<T>(Repository repository)
			where T : ToolStripItem, new()
		{
			if(repository == null) throw new ArgumentNullException("repository");

			var item = new T()
			{
				Text = Resources.StrCompressRepository,
				Image = CachedResources.Bitmaps["ImgGC"],
				Tag = repository,
			};
			item.Click += OnCompressRepositoryClick;
			return item;
		}

		private static void OnExtractFileItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Tuple<Tree, string, bool>)item.Tag;
			var tree = data.Item1;
			var path = data.Item2;
			var openas = data.Item3;
			if(openas)
			{
				tree.ShowOpenFileWithDialog(path);
			}
			else
			{
				tree.OpenFile(path);
			}
		}

		private static void OnCompressRepositoryClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var parent = Utility.GetParentControl(item);
			var repo = (Repository)item.Tag;

			try
			{
				repo.GarbageCollectAsync().Invoke<ProgressForm>(parent);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToCompressRepository,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		private static void OnOpenCmdAtItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var path = (string)item.Tag;

			var psi = new ProcessStartInfo("cmd")
			{
				WorkingDirectory = path,
			};
			using(var p = new Process())
			{
				p.StartInfo = psi;
				p.Start();
			}
		}

		private static void OnSendEmailClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var email = (string)item.Tag;
			if(!email.StartsWith(@"mailto://"))
			{
				email = @"mailto://" + email;
			}
			Utility.OpenUrl(email);
		}

		private static void OnViewDiffItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var diffSource = (IDiffSource)item.Tag;

			RepositoryProvider.Environment.ViewDockService.ShowView(
				Views.Guids.DiffViewGuid,
				new Dictionary<string, object>()
				{
					{ "source", diffSource }
				});
		}

		private static void OnViewTreeItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var revPtr = (IRevisionPointer)item.Tag;

			RepositoryProvider.Environment.ViewDockService.ShowView(
				Views.Guids.TreeViewGuid,
				new Dictionary<string, object>()
				{
					{ "tree", new RevisionTreeSource(revPtr) }
				});
		}

		private static void OnResolveConflictsClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);

			using(var dlg = new ConflictsDialog(repository))
			{
				dlg.Run(parent);
			}
		}

		private static void OnCleanClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var repository = (Repository)item.Tag;
			var parent = Utility.GetParentControl(item);

			using(var dlg = new CleanDialog(repository))
			{
				dlg.Run(parent);
			}
		}

		private static void OnOpenUrlItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var url = (string)item.Tag;

			Utility.OpenUrl(url);
		}

		private static void OnOpenAppItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Tuple<string, string>)item.Tag;

			if(data.Item2 != null)
			{
				System.Diagnostics.Process.Start(data.Item1, data.Item2);
			}
			else
			{
				System.Diagnostics.Process.Start(data.Item1);
			}
		}

		private static void OnOpenUrlWithItemClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var url = (string)item.Tag;

			Utility.ShowOpenWithDialog(url);
		}

		private static void OnCopyToClipboardClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var text = item.Tag as string;
			if(text == null)
			{
				text = ((Func<string>)item.Tag)();
			}
			Clipboard.SetText(text);
		}

		private static void OnCopyHashToClipboardClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var text = item.Tag as string;
			if(text == null)
			{
				text = ((Func<string>)item.Tag)();
			}
			if(Control.ModifierKeys == Keys.Shift)
			{
				text = text.Substring(0, 7);
			}
			Clipboard.SetText(text);
		}

		private static void OnRefreshReferencesClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Tuple<Repository, ReferenceType>)item.Tag;
			var repository = data.Item1;
			var type = data.Item2;

			if((type | ReferenceType.Remote) == ReferenceType.Remote)
				repository.Remotes.Refresh();
			if((type | ReferenceType.Branch) == ReferenceType.Branch)
				repository.Refs.RefreshBranches();
			else
				if((type | ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
					repository.Refs.Heads.Refresh();
				else if((type | ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch)
					repository.Refs.Remotes.Refresh();
			if((type | ReferenceType.Tag) == ReferenceType.Tag)
				repository.Refs.Tags.Refresh();
			if((type | ReferenceType.Stash) == ReferenceType.Stash)
				repository.Stash.Refresh();
		}

		private static void OnRefreshAllReferencesClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var data = (Repository)item.Tag;
			data.Refs.Refresh();
		}

		private static void OnExpandAllClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var treeItem = (CustomListBoxItem)item.Tag;
			treeItem.ExpandAll();
		}

		private static void OnCollapseAllClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var treeItem = (CustomListBoxItem)item.Tag;
			treeItem.CollapseAll();
		}

		#endregion
	}
}

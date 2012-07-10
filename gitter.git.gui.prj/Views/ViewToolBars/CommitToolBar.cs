namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class CommitToolbar : ToolStrip
	{
		#region Data

		private readonly CommitView _view;
		private readonly ToolStripButton _btnRefresh;
		private readonly ToolStripButton _btnTreeMode;
		private readonly ToolStripSplitButton _stageAll;
		private readonly ToolStripItem _unstageAll;
		private readonly ToolStripSplitButton _btnReset;

		#endregion

		public CommitToolbar(CommitView view)
		{
			if(view == null) throw new ArgumentNullException("view");
			_view = view;

			Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) =>
				{
					_view.RefreshContent();
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(_btnTreeMode = new ToolStripButton(Resources.StrShowDirectoryTree, CachedResources.Bitmaps["ImgFolderTree"],
				(sender, e) =>
				{
					var button = (ToolStripButton)sender;
					_view.TreeMode = button.Checked = !button.Checked;
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				Checked = view.TreeMode,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(_stageAll = new ToolStripSplitButton(Resources.StrStageAll, CachedResources.Bitmaps["ImgStageAll"])
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
					DropDownDirection = ToolStripDropDownDirection.BelowRight
				});
			Items.Add(_unstageAll = new ToolStripButton(Resources.StrUnstageAll, CachedResources.Bitmaps["ImgUnstageAll"])
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnReset = new ToolStripSplitButton(Resources.StrReset, CachedResources.Bitmaps["ImgDelete"])
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				});

			_stageAll.ButtonClick += (sender, e) =>
				{
					_view.Repository.Status.StageAll();
				};

			_stageAll.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrUpdate, null, (s, e) => _view.Repository.Status.StageUpdated()));
			_stageAll.DropDown.Items.Add(new ToolStripSeparator());
			_stageAll.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrManual.AddEllipsis(), null,
				(sender, e) =>
				{
					using(var dlg = new StageDialog(_view.Repository))
					{
						dlg.Run(_view);
					}
				}));

			_unstageAll.Click += (sender, e) =>
				{
					_view.Repository.Status.UnstageAll();
				};

			_btnReset.ButtonClick += (s, e) =>
				{
					using(var dlg = new SelectResetModeDialog(ResetMode.Mixed | ResetMode.Hard)
						{
							ResetMode = ResetMode.Mixed,
						})
					{
						if(dlg.Run(this) == DialogResult.OK)
						{
							Reset(dlg.ResetMode);
						}
					}
				};

			_btnReset.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrMixed, null, (s, e) => AskAndReset(ResetMode.Mixed)));
			_btnReset.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrHard, null, (s, e) => AskAndReset(ResetMode.Hard)));
		}

		private void AskAndReset(ResetMode mode)
		{
			if(mode == ResetMode.Hard)
			{
				if(GitterApplication.MessageBoxService.Show(
					this,
					Resources.StrAskHardReset,
					Resources.StrHardReset,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
			}
			Reset(mode);
		}

		private void Reset(ResetMode mode)
		{
			Cursor = Cursors.WaitCursor;
			try
			{
				_view.Repository.Status.Reset(mode);
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToReset,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		public ToolStripButton TreeModeButton
		{
			get { return _btnTreeMode; }
		}
	}
}

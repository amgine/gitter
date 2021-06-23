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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class CommitToolbar : ToolStrip
	{
		static class Icons
		{
			const int Size = 16;

			public static readonly IDpiBoundValue<Bitmap> Refresh    = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"refresh",     Size);
			public static readonly IDpiBoundValue<Bitmap> TreeMode   = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"folder.tree", Size);
			public static readonly IDpiBoundValue<Bitmap> StageAll   = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"stage.all",   Size);
			public static readonly IDpiBoundValue<Bitmap> UnstageAll = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"unstage.all", Size);
			public static readonly IDpiBoundValue<Bitmap> Reset      = DpiBoundValue.Icon(CachedResources.ScaledBitmaps, @"delete",      Size);
		}

		#region Data

		private readonly CommitView _commitView;
		private readonly ToolStripButton _btnRefresh;
		private readonly ToolStripButton _btnTreeMode;
		private readonly ToolStripSplitButton _stageAll;
		private readonly ToolStripItem _unstageAll;
		private readonly ToolStripSplitButton _btnReset;
		private readonly DpiBindings _bindings;

		#endregion

		public CommitToolbar(CommitView commitView)
		{
			Verify.Argument.IsNotNull(commitView, nameof(commitView));

			_commitView = commitView;
			Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, default,
				(_, _) => _commitView.RefreshContent())
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(_btnTreeMode = new ToolStripButton(Resources.StrShowDirectoryTree, default,
				(sender, _) =>
				{
					var button = (ToolStripButton)sender;
					_commitView.TreeMode = button.Checked = !button.Checked;
				})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
				Checked = commitView.TreeMode,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(_stageAll = new ToolStripSplitButton(Resources.StrStageAll, default)
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
					DropDownDirection = ToolStripDropDownDirection.BelowRight
				});
			Items.Add(_unstageAll = new ToolStripButton(Resources.StrUnstageAll, default)
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				});
			Items.Add(new ToolStripSeparator());
			Items.Add(_btnReset = new ToolStripSplitButton(Resources.StrReset, default)
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
				});

			_stageAll.ButtonClick += (_, _) => _commitView.Repository.Status.StageAll();

			_stageAll.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrUpdate, null, (_, _) => _commitView.Repository.Status.StageUpdated()));
			_stageAll.DropDown.Items.Add(new ToolStripSeparator());
			_stageAll.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrManual.AddEllipsis(), null,
				(_, _) =>
				{
					using var dlg = new StageDialog(_commitView.Repository);
					dlg.Run(_commitView);
				}));

			_unstageAll.Click += (_, _) => _commitView.Repository.Status.UnstageAll();

			_btnReset.ButtonClick += (_, _) =>
				{
					using var dlg = new SelectResetModeDialog(ResetMode.Mixed | ResetMode.Hard)
					{
						ResetMode = ResetMode.Mixed,
					};
					if(dlg.Run(this) == DialogResult.OK)
					{
						Reset(dlg.ResetMode);
					}
				};

			_btnReset.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrMixed, null, (_, _) => AskAndReset(ResetMode.Mixed)));
			_btnReset.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrHard,  null, (_, _) => AskAndReset(ResetMode.Hard)));

			_bindings = new DpiBindings(this);
			_bindings.BindImage(_btnRefresh,  Icons.Refresh);
			_bindings.BindImage(_btnTreeMode, Icons.TreeMode);
			_bindings.BindImage(_stageAll,    Icons.StageAll);
			_bindings.BindImage(_unstageAll,  Icons.UnstageAll);
			_bindings.BindImage(_btnReset,    Icons.Reset);
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
			try
			{
				using(_commitView.ChangeCursor(Cursors.WaitCursor))
				{
					_commitView.Repository.Status.Reset(mode);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToReset,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		public ToolStripButton TreeModeButton => _btnTreeMode;
	}
}

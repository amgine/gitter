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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;

using gitter.Git.Gui.Dialogs;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class CommitToolbar : ToolStrip
{
	#region Data

	private readonly CommitView _commitView;
	private readonly ToolStripSplitButton _stageAll;
	private readonly ToolStripItem _unstageAll;
	private readonly ToolStripSplitButton _btnReset;
	private readonly DpiBindings _dpiBindings;

	#endregion

	public CommitToolbar(CommitView commitView)
	{
		Verify.Argument.IsNotNull(commitView);

		_dpiBindings = new DpiBindings(this);
		var factory = new GuiItemFactory(_dpiBindings);

		_commitView = commitView;
		Items.Add(factory.CreateRefreshContentButton(commitView));
		Items.Add(TreeModeButton = new ToolStripButton(Resources.StrShowDirectoryTree, default,
			(sender, _) =>
			{
				var button = (ToolStripButton)sender!;
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

		_stageAll.ButtonClick += (_, _) =>
		{
			if(_commitView.Repository is null) return;

			_commitView.Repository.Status.StageAll();
		};

		_stageAll.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrUpdate, null, (_, _) =>
		{
			if(_commitView.Repository is null) return;

			_commitView.Repository.Status.StageUpdated();
		}));
		_stageAll.DropDown.Items.Add(new ToolStripSeparator());
		_stageAll.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrManual.AddEllipsis(), null,
			(_, _) =>
			{
				if(_commitView.Repository is null) return;

				using var dialog = new StageDialog(_commitView.Repository);
				dialog.Run(_commitView);
			}));

		_unstageAll.Click += (_, _) =>
		{
			if(_commitView.Repository is null) return;

			_commitView.Repository.Status.UnstageAll();
		};

		_btnReset.ButtonClick +=
			(_, _) =>
			{
				using var dialog = new SelectResetModeDialog(ResetMode.Mixed | ResetMode.Hard)
				{
					ResetMode = ResetMode.Mixed,
				};
				if(dialog.Run(this) == DialogResult.OK)
				{
					Reset(dialog.ResetMode);
				}
			};

		_btnReset.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrMixed, null, (_, _) => AskAndReset(ResetMode.Mixed)));
		_btnReset.DropDown.Items.Add(new ToolStripMenuItem(Resources.StrHard,  null, (_, _) => AskAndReset(ResetMode.Hard)));

		_dpiBindings.BindImage(TreeModeButton, Icons.FolderTree);
		_dpiBindings.BindImage(_stageAll,      Icons.StageAll);
		_dpiBindings.BindImage(_unstageAll,    Icons.UnstageAll);
		_dpiBindings.BindImage(_btnReset,      Icons.Delete);
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
		if(_commitView.Repository is null) return;

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

	public ToolStripButton TreeModeButton { get; }
}

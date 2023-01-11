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
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Services;
using gitter.Framework.Controls;

using gitter.Git.AccessLayer;
using gitter.Git.Gui.Dialogs;
using gitter.Git.Gui.Views;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Factory for creating buttons or menu items.</summary>
public class GuiItemFactory
{
	private readonly DpiBindings _dpiBindings;

	public GuiItemFactory(DpiBindings dpiBindings)
	{
		Verify.Argument.IsNotNull(dpiBindings);

		_dpiBindings = dpiBindings;
	}

	#region Universal Items

	internal ToolStripButton CreateRefreshContentButton(GitViewBase view)
	{
		Verify.Argument.IsNotNull(view);

		var button = new ToolStripButton(Resources.StrRefresh, default, (_, _) => view.RefreshContent())
		{
			DisplayStyle = ToolStripItemDisplayStyle.Image,
		};
		_dpiBindings.BindImage(button, Icons.Refresh);
		return button;
	}

	public T GetViewReflogItem<T>(Reference reference)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(reference, nameof(reference));

		var item = new T()
		{
			Text = Resources.StrViewReflog,
			Tag  = reference,
		};
		_dpiBindings.BindImage(item, reference.Type == ReferenceType.RemoteBranch
			? Icons.RemoteBranchReflog
			: Icons.BranchReflog);
		item.Click += OnViewReflogClick;
		return item;
	}

	public T GetCheckoutPathItem<T>(IRevisionPointer revision, TreeFile file)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(file, nameof(file));

		return GetCheckoutPathItem<T>(revision, file.RelativePath);
	}

	public T GetCheckoutPathItem<T>(IRevisionPointer revision, TreeCommit commit)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(commit, nameof(commit));

		return GetCheckoutPathItem<T>(revision, commit.RelativePath);
	}

	public T GetCheckoutPathItem<T>(IRevisionPointer revision, TreeDirectory directory)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(directory, nameof(directory));

		return GetCheckoutPathItem<T>(revision, directory.RelativePath + "/");
	}

	public T GetCheckoutPathItem<T>(IRevisionPointer revision, string path)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);
		Verify.Argument.IsNeitherNullNorWhitespace(path);

		var item = new T()
		{
			Text = Resources.StrCheckout,
			Tag  = Tuple.Create(revision, path),
		};
		_dpiBindings.BindImage(item, Icons.Checkout);
		item.Click += OnCheckoutPathClick;
		return item;
	}

	public T GetCheckoutRevisionItem<T>(Repository repository, string nameFormat = "{0}")
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var text = nameFormat == "{0}"
			? Resources.StrCheckout
			: string.Format(nameFormat, Resources.StrCheckout);
		var item = new T()
		{
			Text = text,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.Checkout);
		item.Click += OnCheckoutClick;
		return item;
	}

	public T GetCheckoutRevisionItem<T>(IRevisionPointer revision, string nameFormat = "{0}")
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		bool enabled = true;
		var head = revision.Repository.Head.Pointer;
		if(head == revision)
		{
			enabled = false;
		}
		else
		{
			if((head is not null) &&
				(head.Type != ReferenceType.LocalBranch) &&
				(revision.Type != ReferenceType.LocalBranch) &&
				(revision.Dereference() == head))
			{
				enabled = false;
			}
		}

		var text = nameFormat == "{0}"
			? Resources.StrCheckout
			: string.Format(nameFormat, Resources.StrCheckout, revision is null ? string.Empty : revision.Pointer);
		var item = new T()
		{
			Text    = text,
			Tag     = revision,
			Enabled = enabled,
		};
		_dpiBindings.BindImage(item, Icons.Checkout);
		item.Click += OnCheckoutRevisionClick;
		return item;
	}

	public T GetRevertItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		bool isEnabled	= !revision.Repository.IsEmpty;
		bool isMerge	= revision.Dereference().Parents.Count > 1;

		var item = new T()
		{
			Text	= isMerge ? Resources.StrRevert.AddEllipsis() : Resources.StrRevert,
			Enabled	= isEnabled,
			Tag		= revision,
		};
		_dpiBindings.BindImage(item, Icons.Revert);
		item.Click += isMerge ? OnRevertMergeClick : OnRevertClick;
		return item;
	}

	public T GetRevertItem<T>(IEnumerable<IRevisionPointer> revisions)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointerSequence(revisions, nameof(revisions));

		var item = new T()
		{
			Text = Resources.StrRevert,
			Tag  = revisions,
		};
		_dpiBindings.BindImage(item, Icons.Revert);
		item.Click += OnMultipleRevertClick;
		return item;
	}

	public T GetResetItem<T>(Repository repository, ResetMode resetModes = ResetMode.Mixed | ResetMode.Hard)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text  = Resources.StrReset.AddEllipsis(),
			Tag   = Tuple.Create(repository, resetModes),
		};
		_dpiBindings.BindImage(item, Icons.Delete);
		item.Click += OnResetClick;
		return item;
	}

	public T GetResetHeadHereItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		var currentBranch = revision.Repository.Head.CurrentBranch;
		var item = new T()
		{
			Text = currentBranch is not null ?
				string.Format(Resources.StrResetBranchHere, currentBranch.Name).AddEllipsis():
				Resources.StrResetHere.AddEllipsis(),
			Tag = revision,
		};
		_dpiBindings.BindImage(item, Icons.Reset);
		item.Click += OnResetHeadClick;
		return item;
	}

	public T GetRebaseHeadHereItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		var currentBranch = revision.Repository.Head.CurrentBranch;
		var item = new T()
		{
			Text = currentBranch is not null ?
				string.Format(Resources.StrRebaseBranchHere, currentBranch.Name) : Resources.StrRebaseHere,
			Enabled = revision.Dereference() != revision.Repository.Head.Revision,
			Tag = revision,
		};
		_dpiBindings.BindImage(item, Icons.Rebase);
		item.Click += OnRebaseHeadHereClick;
		return item;
	}

	public T GetCherryPickItem<T>(IRevisionPointer revision, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		bool isEnabled	= !revision.Repository.IsEmpty;
		bool isMerge	= revision.Dereference().Parents.Count > 1;

		var item = new T()
		{
			Text	= string.Format(nameFormat, isMerge ? Resources.StrCherryPick.AddEllipsis() : Resources.StrCherryPick),
			Tag		= revision,
			Enabled	= isEnabled,
		};
		_dpiBindings.BindImage(item, Icons.CherryPick);
		item.Click += isMerge ? OnCherryPickMergeClick : OnCherryPickClick;
		return item;
	}

	public T GetCherryPickItem<T>(IEnumerable<IRevisionPointer> revisions)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointerSequence(revisions, nameof(revisions));

		var item = new T()
		{
			Text = Resources.StrCherryPick,
			Tag = revisions,
		};
		_dpiBindings.BindImage(item, Icons.CherryPick);
		item.Click += OnMultipleCherryPickClick;
		return item;
	}

	public T GetSavePatchItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		var item = new T()
		{
			Text	= Resources.StrSavePatch.AddEllipsis(),
			Tag		= revision,
		};
		_dpiBindings.BindImage(item, Icons.PatchSave);
		item.Click += OnSaveRevisionPatchClick;
		return item;
	}

	public T GetArchiveItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		var item = new T()
		{
			Text = Resources.StrArchive.AddEllipsis(),
			Tag  = revision,
		};
		_dpiBindings.BindImage(item, Icons.Archive);
		item.Click += OnArchiveClick;
		return item;
	}

	public T GetCompareWithItem<T>(IRevisionPointer revision1, IRevisionPointer revision2)
		where T : ToolStripItem, new()
	{
		Verify.Argument.AreValidRevisionPointers(revision1, revision2);

		var item = new T()
		{
			Text = Resources.StrCompare.AddEllipsis(),
			Tag  = Tuple.Create(revision1, revision2),
		};
		_dpiBindings.BindImage(item, Icons.Diff);
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

		GitterApplication.WorkingEnvironment.ViewDockService.ShowView(
			Views.Guids.DiffViewGuid,
			new Views.DiffViewModel(rev1.GetCompareDiffSource(rev2), null));
	}

	private static void OnViewReflogClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var reference = (Reference)item.Tag;
		GitterApplication.WorkingEnvironment.ViewDockService.ShowView(
			Views.Guids.ReflogViewGuid,
			new Views.ReflogViewModel(reference.Reflog));
	}
	
	private static void OnArchiveClick(object sender, EventArgs e)
	{
		var item     = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;
		var parent   = Utility.GetParentControl(item);

		GuiCommands.Archive(parent, revision, null, null);
	}

	private static void OnSaveRevisionPatchClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var revision   = (IRevisionPointer)item.Tag;
		var repository = revision.Repository;
		var parent     = Utility.GetParentControl(item);
		var fileName   = revision.Dereference().Hash;

		GuiCommands.FormatPatch(parent, revision);
	}

	private static void OnShowViewItemClick(object sender, EventArgs e)
	{
		var item     = (ToolStripItem)sender;
		var viewGuid = (Guid)item.Tag;

		GitterApplication.WorkingEnvironment
			                .ViewDockService
			                .ShowView(viewGuid, true);
	}

	private static void OnCheckoutClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		if(repository is { IsEmpty: false })
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
		var item		= (ToolStripItem)sender;
		var parent		= Utility.GetParentControl(item);
		var data		= (Tuple<IRevisionPointer, string>)item.Tag;
		var revision	= data.Item1;
		var path		= data.Item2;
		var repository	= revision.Repository;

		lock(repository.Status.SyncRoot)
		{
			foreach(var f in repository.Status.UnstagedFiles)
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				revision.CheckoutPath(path);
			}
		}
		catch(GitException exc)
		{
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
		var item     = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;
		var parent   = Utility.GetParentControl(item);
		var force    = Control.ModifierKeys == Keys.Shift;

		if(GlobalBehavior.AskOnCommitCheckouts)
		{
			bool revIsLocalBranch = (revision is Branch branch) && !branch.IsRemote;
			if(!revIsLocalBranch)
			{
				var rev = revision.Dereference();
				var branches = rev.References.GetBranches();
				if(branches.Count != 0 && branches.Any(b => !b.IsCurrent))
				{
					using var dlg = new ResolveCheckoutDialog();
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
		try
		{
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				revision.Checkout(force);
			}
		}
		catch(UntrackedFileWouldBeOverwrittenException)
		{
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				revision.Checkout(true);
			}
		}
		catch(GitException exc)
		{
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
			using var dlg = new RevertDialog(revision);
			dlg.Run(parent);
		}
		else
		{
			try
			{
				using(parent.ChangeCursor(Cursors.WaitCursor))
				{
					revision.Revert();
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToRevert,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}
	}

	private static void OnRevertMergeClick(object sender, EventArgs e)
	{
		var item     = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;
		var parent   = Utility.GetParentControl(item);

		using var dlg = new RevertDialog(revision);
		dlg.Run(parent);
	}

	private static void OnMultipleRevertClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var revisions = (IEnumerable<IRevisionPointer>)item.Tag;
		var parent = Utility.GetParentControl(item);
		try
		{
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				revisions.Revert();
			}
		}
		catch(GitException exc)
		{
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
		var item   = (ToolStripItem)sender;
		var data   = (Tuple<Repository, ResetMode>)item.Tag;
		var parent = Utility.GetParentControl(item);

		using var dlg = new SelectResetModeDialog(data.Item2);
		if(dlg.Run(parent) == DialogResult.OK)
		{
			try
			{
				using(parent.ChangeCursor(Cursors.WaitCursor))
				{
					data.Item1.Status.Reset(dlg.ResetMode);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					exc.Message,
					Resources.ErrFailedToReset,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
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
					using(parent.ChangeCursor(Cursors.WaitCursor))
					{
						revision.ResetHeadHere(dlg.ResetMode);
					}
				}
				catch(GitException exc)
				{
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
		var item     = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;
		var parent   = Utility.GetParentControl(item);

		GuiCommands.RebaseHeadTo(parent, revision);
	}

	private static void OnCherryPickClick(object sender, EventArgs e)
	{
		var item     = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;
		var parent   = Utility.GetParentControl(item);

		if(Control.ModifierKeys == Keys.Shift)
		{
			using var dlg = new CherryPickDialog(revision);
			dlg.Run(parent);
		}
		else
		{
			try
			{
				using(parent.ChangeCursor(Cursors.WaitCursor))
				{
					revision.CherryPick();
				}
			}
			catch(HaveConflictsException)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					Resources.ErrCherryPickIsNotPossibleWithConflicts,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			catch(HaveLocalChangesException)
			{
				GitterApplication.MessageBoxService.Show(
					parent,
					Resources.ErrCherryPickIsNotPossibleWithLocalChnges,
					Resources.StrCherryPick,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
			catch(AutomaticCherryPickFailedException)
			{
				using var dlg = new ConflictsDialog(revision.Repository);
				dlg.Run(parent);
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
	}

	private static void OnCherryPickMergeClick(object sender, EventArgs e)
	{
		var item     = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;
		var parent   = Utility.GetParentControl(item);

		using var dlg = new CherryPickDialog(revision);
		dlg.Run(parent);
	}

	private static void OnMultipleCherryPickClick(object sender, EventArgs e)
	{
		var item      = (ToolStripItem)sender;
		var revisions = (IEnumerable<IRevisionPointer>)item.Tag;
		var parent    = Utility.GetParentControl(item);
		try
		{
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				revisions.CherryPick();
			}
		}
		catch(HaveConflictsException)
		{
			GitterApplication.MessageBoxService.Show(
				parent,
				Resources.ErrCherryPickIsNotPossibleWithConflicts,
				Resources.StrCherryPick,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		catch(HaveLocalChangesException)
		{
			GitterApplication.MessageBoxService.Show(
				parent,
				Resources.ErrCherryPickIsNotPossibleWithLocalChnges,
				Resources.StrCherryPick,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		catch(AutomaticCherryPickFailedException exc)
		{
			GitterApplication.MessageBoxService.Show(
				parent,
				exc.Message,
				Resources.StrCherryPick,
				MessageBoxButton.Close,
				MessageBoxIcon.Warning);
		}
		catch(GitException exc)
		{
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

	public T GetRefreshStashItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrRefresh,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.Refresh);
		item.Click += OnRefreshStashClick;
		return item;
	}

	public T GetStashClearItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrClear,
			Tag = repository,
		};
		_dpiBindings.BindImage(item, Icons.StashClear);
		item.Click += OnStashClearClick;
		return item;
	}

	public T GetStashPopItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrPop,
			Tag = repository,
		};
		_dpiBindings.BindImage(item, Icons.StashPop);
		item.Click += OnStashPopClick;
		return item;
	}

	public T GetStashPopItem<T>(StashedState stashedState)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(stashedState, nameof(stashedState));

		var item = new T()
		{
			Text = Resources.StrPop,
			Tag = stashedState,
		};
		_dpiBindings.BindImage(item, Icons.StashPop);
		item.Click += OnStashPopStateClick;
		return item;
	}

	public T GetStashApplyItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrApply,
			Tag = repository,
		};
		_dpiBindings.BindImage(item, Icons.StashApply);
		item.Click += OnStashApplyClick;
		return item;
	}

	public T GetStashApplyItem<T>(StashedState stashedState)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(stashedState);

		var item = new T()
		{
			Text = Resources.StrApply,
			Tag = stashedState,
		};
		_dpiBindings.BindImage(item, Icons.StashApply);
		item.Click += OnStashApplyStateClick;
		return item;
	}

	public T GetStashDropItem<T>(StashedState stashedState)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(stashedState);

		var item = new T()
		{
			Text = Resources.StrDrop,
			Tag = stashedState,
		};
		_dpiBindings.BindImage(item, Icons.StashDrop);
		item.Click += OnStashDropStateClick;
		return item;
	}

	public T GetStashDropItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrDrop,
			Tag = repository,
		};
		_dpiBindings.BindImage(item, Icons.StashDrop);
		item.Click += OnStashDropClick;
		return item;
	}

	public T GetStashToBranchItem<T>(StashedState stashedState)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(stashedState);

		var item = new T()
		{
			Text = Resources.StrToBranch.AddEllipsis(),
			Tag = stashedState,
		};
		_dpiBindings.BindImage(item, Icons.Branch);
		item.Click += OnStashToBranchClick;
		return item;
	}

	public T GetStashSaveKeepIndexItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrStash.AddEllipsis(),
			Tag  = repository,
			Enabled = !repository.IsEmpty && repository.Status.UnmergedCount == 0,
		};
		_dpiBindings.BindImage(item, Icons.StashSave);
		item.Click += OnStashSaveKeepIndexItemClick;
		return item;
	}

	public T GetStashSaveItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text    = Resources.StrStash.AddEllipsis(),
			Tag     = repository,
			Enabled = !repository.IsEmpty && repository.Status.UnmergedCount == 0,
		};
		_dpiBindings.BindImage(item, Icons.StashSave);
		item.Click += OnStashSaveItemClick;
		return item;
	}

	private static void OnRefreshStashClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;

		repository.Stash.Refresh();
	}

	private static void OnStashClearClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent     = Utility.GetParentControl(item);

		GuiCommands.ClearStash(parent, repository.Stash);
	}

	private static void OnStashSaveItemClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent     = Utility.GetParentControl(item);

		using var dlg = new StashSaveDialog(repository);
		dlg.Run(parent);
	}

	private static void OnStashSaveKeepIndexItemClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent     = Utility.GetParentControl(item);

		using var dlg = new StashSaveDialog(repository)
		{
			KeepIndex = true,
		};
		dlg.Run(parent);
	}

	private static void OnStashPopClick(object sender, EventArgs e)
	{
		var item         = (ToolStripItem)sender;
		var repository   = (Repository)item.Tag;
		var parent       = Utility.GetParentControl(item);
		var restoreIndex = Control.ModifierKeys == Keys.Shift;

		GuiCommands.PopStashedState(parent, repository.Stash, restoreIndex);
	}

	private static void OnStashPopStateClick(object sender, EventArgs e)
	{
		var item         = (ToolStripItem)sender;
		var stashedState = (StashedState)item.Tag;
		var parent       = Utility.GetParentControl(item);
		var restoreIndex = Control.ModifierKeys == Keys.Shift;

		GuiCommands.PopStashedState(parent, stashedState, restoreIndex);
	}

	private static void OnStashApplyClick(object sender, EventArgs e)
	{
		var item         = (ToolStripItem)sender;
		var repository   = (Repository)item.Tag;
		var parent       = Utility.GetParentControl(item);
		var restoreIndex = Control.ModifierKeys == Keys.Shift;

		GuiCommands.ApplyStashedState(parent, repository.Stash, restoreIndex);
	}

	private static void OnStashApplyStateClick(object sender, EventArgs e)
	{
		var item         = (ToolStripItem)sender;
		var stashedState = (StashedState)item.Tag;
		var parent       = Utility.GetParentControl(item);
		var restoreIndex = Control.ModifierKeys == Keys.Shift;

		GuiCommands.ApplyStashedState(parent, stashedState, restoreIndex);
	}

	private static void OnStashDropStateClick(object sender, EventArgs e)
	{
		var item         = (ToolStripItem)sender;
		var stashedState = (StashedState)item.Tag;
		var parent       = Utility.GetParentControl(item);

		GuiCommands.DropStashedState(parent, stashedState);
	}

	private static void OnStashDropClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent     = Utility.GetParentControl(item);

		GuiCommands.DropStashedState(parent, repository.Stash);
	}

	private static void OnStashToBranchClick(object sender, EventArgs e)
	{
		var item         = (ToolStripItem)sender;
		var stashedState = (StashedState)item.Tag;

		using var dlg = new StashToBranchDialog(stashedState);
		dlg.Run(Utility.GetParentControl(item));
	}

	#endregion

	#region Note Items

	public T GetAddNoteItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		var item = new T()
		{
			Text = Resources.StrAddNote.AddEllipsis(),
			Tag  = revision,
		};
		_dpiBindings.BindImage(item, Icons.NoteAdd);
		item.Click += OnAddNoteClick;
		return item;
	}

	private static void OnAddNoteClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;
		var parent = Utility.GetParentControl(item);

		using var dlg = new AddNoteDialog(revision.Repository);
		dlg.Revision.Value = revision.Pointer;
		dlg.Run(parent);
	}

	#endregion

	#region Branch Items

	public T GetCreateBranchItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrCreateBranch.AddEllipsis(),
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.BranchAdd);
		item.Click += OnCreateBranchClick;
		return item;
	}

	public T GetCreateBranchItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		if(revision != null)
		{
			Verify.Argument.IsValidRevisionPointer(revision);
		}

		var item = new T()
		{
			Text = Resources.StrCreateBranch.AddEllipsis(),
			Tag  = revision,
		};
		_dpiBindings.BindImage(item, Icons.BranchAdd);
		item.Click += OnCreateBranchAtClick;
		return item;
	}

	public T GetRemoveBranchItem<T>(BranchBase branch)
		where T : ToolStripItem, new()
	{
		return GetRemoveBranchItem<T>(branch, branch.IsRemote ? Resources.StrDelete.AddEllipsis() : Resources.StrDelete);
	}

	public T GetRemoveBranchItem<T>(BranchBase branch, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(branch);

		var item = new T()
		{
			Text    = string.Format(nameFormat, Resources.StrRemoveBranch, branch.Name),
			Tag     = branch,
			Enabled = !branch.IsCurrent,
		};
		_dpiBindings.BindImage(item, branch.IsRemote ? Icons.RemoteBranchDelete : Icons.BranchDelete);
		item.Click += OnRemoveBranchClick;
		return item;
	}

	public T GetRenameBranchItem<T>(Branch branch, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(branch);

		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrRename.AddEllipsis(), branch.Name),
			Tag  = branch,
		};
		_dpiBindings.BindImage(item, Icons.BranchRename);
		item.Click += OnRenameBranchClick;
		return item;
	}

	public T GetMergeItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		var item = new T()
		{
			Text	= Resources.StrMerge,
			Tag		= revision,
			Enabled	= revision != revision.Repository.Head.Pointer,
		};
		_dpiBindings.BindImage(item, Icons.Merge);
		item.Click += OnMergeBranchClick;
		return item;
	}

	public T GetPushBranchToRemoteItem<T>(Branch branch, Remote remote)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(branch);
		Verify.Argument.IsValidGitObject(remote);

		var item = new T()
		{
			Text = remote.Name,
			Tag  = Tuple.Create(branch, remote),
		};
		_dpiBindings.BindImage(item, Icons.Remote);
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

		GuiCommands.Push(parent, remote, new Branch[] { branch }, false, true, false);
	}

	private static void OnCreateBranchClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;

		using var dlg = new CreateBranchDialog(repository);
		dlg.StartingRevision.Value = GitConstants.HEAD;
		dlg.Run(Utility.GetParentControl(item));
	}

	private static void OnCreateBranchAtClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;
		string startingRevision;
		string defaultBranchName;
		if(revision != null)
		{
			startingRevision  = revision.Pointer;
			defaultBranchName = BranchHelper.TryFormatDefaultLocalBranchName(revision);
		}
		else
		{
			startingRevision  = string.Empty;
			defaultBranchName = string.Empty;
		}

		using var dlg = new CreateBranchDialog(revision.Repository);
		dlg.StartingRevision.Value = startingRevision;
		if(!string.IsNullOrWhiteSpace(defaultBranchName))
		{
			dlg.BranchName.Value = defaultBranchName;
		}
		dlg.Run(Utility.GetParentControl(item));
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
				var remoteBranch = (RemoteBranch)branch;
				if(remoteBranch.Remote != null)
				{
					using(var dlg = new RemoveRemoteBranchDialog(remoteBranch))
					{
						dlg.Run(parent);
					}
				}
				else
				{
					if(GitterApplication.MessageBoxService.Show(
						parent,
						Resources.StrAskRemoteBranchRemove.UseAsFormat(remoteBranch.Name),
						Resources.StrRemoveBranch,
						MessageBoxButton.YesNo,
						MessageBoxIcon.Question) == DialogResult.Yes)
					{
						try
						{
							using(parent.ChangeCursor(Cursors.WaitCursor))
							{
								remoteBranch.Delete();
							}
						}
						catch(GitException exc)
						{
							GitterApplication.MessageBoxService.Show(
								parent,
								exc.Message,
								Resources.ErrFailedToRemoveBranch.UseAsFormat(branch.Name),
								MessageBoxButton.Close,
								MessageBoxIcon.Error);
						}
					}
				}
			}
			else
			{
				try
				{
					bool force = Control.ModifierKeys == Keys.Shift;
					using(parent.ChangeCursor(Cursors.WaitCursor))
					{
						branch.Delete(force);
					}
				}
				catch(BranchIsNotFullyMergedException)
				{
					if(GitterApplication.MessageBoxService.Show(
						parent,
						Resources.StrAskBranchIsNotFullyMerged.UseAsFormat(branch.Name),
						Resources.StrDeleteBranch,
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question) == DialogResult.Yes)
					{
						try
						{
							using(parent.ChangeCursor(Cursors.WaitCursor))
							{
								branch.Delete(true);
							}
						}
						catch(GitException exc)
						{
							GitterApplication.MessageBoxService.Show(
								parent,
								exc.Message,
								Resources.ErrFailedToRemoveBranch.UseAsFormat(branch.Name),
								MessageBoxButton.Close,
								MessageBoxIcon.Error);
						}
					}
				}
				catch(GitException exc)
				{
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
				using(var dlg = new MergeDialog(revision.Repository))
				{
					dlg.Revisions.Value = new[] { revision };
					dlg.Run(parent);
				}
			}
			else
			{
				try
				{
					using(parent.ChangeCursor(Cursors.WaitCursor))
					{
						revision.Repository.Head.Merge(revision);
					}
				}
				catch(AutomaticMergeFailedException)
				{
					using(var dlg = new ConflictsDialog(revision.Repository))
					{
						dlg.Run(parent);
					}
				}
				catch(GitException exc)
				{
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

	public T GetCreateTagItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrCreateTag.AddEllipsis(),
			Tag = repository,
		};
		_dpiBindings.BindImage(item, Icons.TagAdd);
		item.Click += OnCreateTagClick;
		return item;
	}

	public T GetCreateTagItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		if(revision is not null)
		{
			Verify.Argument.IsValidRevisionPointer(revision);
		}
			
		var item = new T()
		{
			Text = Resources.StrCreateTag.AddEllipsis(),
			Tag  = revision,
		};
		_dpiBindings.BindImage(item, Icons.TagAdd);
		item.Click += OnCreateTagAtClick;
		return item;
	}

	public T GetRemoveTagItem<T>(Tag tag, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(tag, nameof(tag));

		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrRemoveTag, tag.Name),
			Tag  = tag,
		};
		_dpiBindings.BindImage(item, Icons.TagDelete);
		item.Click += OnRemoveTagClick;
		return item;
	}

	private static void OnCreateTagClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;

		using var dlg = new CreateTagDialog(repository);
		dlg.Revision.Value = GitConstants.HEAD;
		dlg.Run(Utility.GetParentControl(item));
	}

	private static void OnCreateTagAtClick(object sender, EventArgs e)
	{
		var item     = (ToolStripItem)sender;
		var revision = (IRevisionPointer)item.Tag;

		using var dlg = new CreateTagDialog(revision.Repository);
		dlg.Revision.Value = (revision != null) ? revision.Pointer : GitConstants.HEAD;
		dlg.Run(Utility.GetParentControl(item));
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
				using(parent.ChangeCursor(Cursors.WaitCursor))
				{
					tag.Delete();
				}
			}
			catch(GitException exc)
			{
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

	public T GetEditRemotePropertiesItem<T>(Remote remote)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(remote, nameof(remote));

		var item = new T()
		{
			Text = Resources.StrProperties.AddEllipsis(),
			Tag = remote,
		};
		_dpiBindings.BindImage(item, Icons.RemoteProperties);
		item.Click += OnEditRemotePropertiesClick;
		return item;
	}

	public T GetShowRemoteItem<T>(Remote remote)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(remote, nameof(remote));

		var item = new T()
		{
			Text = Resources.StrBrowse,
			Tag = remote,
		};
		_dpiBindings.BindImage(item, Icons.Search);
		item.Click += OnShowRemoteClick;
		return item;
	}

	public T GetFetchItem<T>(Repository repository, string nameFormat = "{0}")
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);
			
		var item = new T()
		{
			Text    = string.Format(nameFormat, Resources.StrFetch),
			Tag     = repository,
			Enabled = repository.Remotes.Count != 0,
		};
		_dpiBindings.BindImage(item, Icons.Fetch);
		item.Click += OnFetchClick;
		return item;
	}

	public T GetFetchFromItem<T>(Remote remote, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(remote, nameof(remote));
			
		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrFetch, remote.Name),
			Tag  = remote,
		};
		_dpiBindings.BindImage(item, nameFormat == "{1}"
			? Icons.Remote
			: Icons.Fetch);
		item.Click += OnFetchFromClick;
		return item;
	}

	public T GetPullItem<T>(Repository repository, string nameFormat = "{0}")
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text    = string.Format(nameFormat, Resources.StrPull),
			Tag     = repository,
			Enabled = repository.Remotes.Count != 0,
		};
		_dpiBindings.BindImage(item, Icons.Pull);
		item.Click += OnPullClick;
		return item;
	}

	public T GetPullFromItem<T>(Remote remote, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(remote, nameof(remote));
		Verify.Argument.IsNotNull(nameFormat);

		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrPull, remote.Name),
			Tag  = remote,
		};
		_dpiBindings.BindImage(item, nameFormat == "{1}"
			? Icons.Remote
			: Icons.Pull);
		item.Click += OnPullFromClick;
		return item;
	}

	public T GetRefreshRemotesItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrRefresh,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.Refresh);
		item.Click += OnRefreshRemotesClick;
		return item;
	}

	public T GetShowRemotesViewItem<T>()
		where T : ToolStripItem, new()
	{
		var item = new T()
		{
			Text = Resources.StrManage,
			Tag  = Views.Guids.RemotesViewGuid,
		};
		item.Click += OnShowViewItemClick;
		return item;
	}

	public T GetAddRemoteItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrAddRemote.AddEllipsis(),
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.RemoteAdd);
		item.Click += OnAddRemoteClick;
		return item;
	}

	public T GetRemoveRemoteItem<T>(Remote remote, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(remote, nameof(remote));
		Verify.Argument.IsNotNull(nameFormat);

		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrRemove, remote.Name),
			Tag  = remote,
		};
		_dpiBindings.BindImage(item, Icons.RemoteDelete);
		item.Click += OnRemoveRemoteClick;
		return item;
	}

	public T GetRenameRemoteItem<T>(Remote remote, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(remote, nameof(remote));

		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrRename.AddEllipsis(), remote.Name),
			Tag  = remote,
		};
		_dpiBindings.BindImage(item, Icons.RemoteRename);
		item.Click += OnRenameRemoteClick;
		return item;
	}

	public T GetPruneRemoteItem<T>(Remote remote, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(remote, nameof(remote));
		Verify.Argument.IsNotNull(nameFormat);

		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrPruneRemote, remote.Name),
			Tag  = remote,
		};
		_dpiBindings.BindImage(item, Icons.Prune);
		item.Click += OnPruneRemoteClick;
		return item;
	}

	public T GetRemoveRemoteBranchItem<T>(RemoteRepositoryBranch remoteBranch, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(remoteBranch);
		Verify.Argument.IsFalse(remoteBranch.IsDeleted, nameof(remoteBranch),
			Resources.ExcObjectIsDeleted.UseAsFormat("RemoteBranch"));

		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrRemoveBranch, remoteBranch.Name),
			Tag = remoteBranch,
		};
		_dpiBindings.BindImage(item, Icons.RemoteBranchDelete);
		item.Click += OnRemoveRemoteReferenceClick;
		return item;
	}

	public T GetRemoveRemoteTagItem<T>(RemoteRepositoryTag remoteTag, string nameFormat)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(remoteTag);
		Verify.Argument.IsFalse(remoteTag.IsDeleted, nameof(remoteTag),
			Resources.ExcObjectIsDeleted.UseAsFormat("RemoteTag"));

		var item = new T()
		{
			Text = string.Format(nameFormat, Resources.StrRemoveTag, remoteTag.Name),
			Tag = remoteTag,
		};
		_dpiBindings.BindImage(item, Icons.TagDelete);
		item.Click += OnRemoveRemoteReferenceClick;
		return item;
	}

	private static void OnEditRemotePropertiesClick(object sender, EventArgs e)
	{
		var item   = (ToolStripItem)sender;
		var remote = (Remote)item.Tag;
		var parent = Utility.GetParentControl(item);

		using var d = new RemotePropertiesDialog(remote);
		d.Run(parent);
	}

	private static void OnShowRemoteClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var remote = (Remote)item.Tag;
		var viewModel = new Views.RemoteViewModel(remote);
		GitterApplication
			.WorkingEnvironment
			.ViewDockService
			.ShowView(Views.Guids.RemoteViewGuid, viewModel);
	}

	private static void OnRemoveRemoteReferenceClick(object sender, EventArgs e)
	{
		var item      = (ToolStripItem)sender;
		var reference = (IRemoteReference)item.Tag;
		var parent    = Utility.GetParentControl(item);

		if(GitterApplication.MessageBoxService.Show(
			parent,
			Resources.AskRemoveRemoteReference,
			Resources.StrRemoveRemoteReference,
			MessageBoxButtons.YesNo,
			MessageBoxIcon.Warning) == DialogResult.Yes)
		{
			try
			{
				using(parent.ChangeCursor(Cursors.WaitCursor))
				{
					reference.Delete();
				}
			}
			catch(GitException exc)
			{
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
		var item   = (ToolStripItem)sender;
		var remote = (Remote)item.Tag;
		var parent = Utility.GetParentControl(item);

		GuiCommands.Prune(parent, remote);
	}

	private static void OnFetchClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent     = Utility.GetParentControl(item);

		GuiCommands.Fetch(parent, repository);
	}

	private static void OnPullClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent     = Utility.GetParentControl(item);

		GuiCommands.Pull(parent, repository);
	}

	private static void OnFetchFromClick(object sender, EventArgs e)
	{
		var item   = (ToolStripItem)sender;
		var remote = (Remote)item.Tag;
		var parent = Utility.GetParentControl(item);

		if(!remote.IsDeleted)
		{
			GuiCommands.Fetch(parent, remote);
		}
	}

	private static void OnPullFromClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var remote     = (Remote)item.Tag;
		var parent     = Utility.GetParentControl(item);

		if(!remote.IsDeleted)
		{
			GuiCommands.Pull(parent, remote);
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
		using var dlg = new AddRemoteDialog(repository);
		dlg.Run(Utility.GetParentControl(item));
	}

	private static void OnRenameRemoteClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var remote = (Remote)item.Tag;
		using var dlg = new RenameRemoteDialog(remote);
		dlg.Run(Utility.GetParentControl(item));
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
				using(parent.ChangeCursor(Cursors.WaitCursor))
				{
					remote.Delete();
				}
			}
			catch(GitException exc)
			{
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

	public T GetRefreshSubmodulesItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrRefresh,
			Tag = repository,
		};
		_dpiBindings.BindImage(item, Icons.Refresh);
		item.Click += OnRefreshSubmodulesClick;
		return item;
	}

	public T GetAddSubmoduleItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrAddSubmodule.AddEllipsis(),
			Tag = repository,
		};
		_dpiBindings.BindImage(item, Icons.SubmoduleAdd);
		item.Click += OnAddSubmoduleClick;
		return item;
	}

	public T GetUpdateSubmoduleItem<T>(Submodule submodule)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(submodule, nameof(submodule));

		var item = new T()
		{
			Text = Resources.StrUpdate,
			Tag  = submodule,
		};
		_dpiBindings.BindImage(item, Icons.Pull);
		item.Click += OnUpdateSubmoduleClick;
		return item;
	}

	public T GetSyncSubmoduleItem<T>(Submodule submodule)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(submodule, nameof(submodule));

		var item = new T()
		{
			Text  = Resources.StrSync,
			Tag   = submodule,
		};
		_dpiBindings.BindImage(item, Icons.SubmoduleSync);
		item.Click += OnSyncSubmoduleClick;
		return item;
	}

	public T GetSyncSubmodulesItem<T>(SubmodulesCollection submodules)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(submodules);

		var item = new T()
		{
			Text    = Resources.StrSync,
			Tag     = submodules,
			Enabled = submodules.Count != 0,
		};
		_dpiBindings.BindImage(item, Icons.SubmoduleSync);
		item.Click += OnSyncSubmodulesClick;
		return item;
	}

	public T GetUpdateSubmodulesItem<T>(SubmodulesCollection submodules)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(submodules);

		var item = new T()
		{
			Text = Resources.StrUpdate,
			Tag = submodules,
			Enabled = submodules.Count != 0,
		};
		_dpiBindings.BindImage(item, Icons.Pull);
		item.Click += OnUpdateSubmodulesClick;
		return item;
	}

	static void OnRefreshSubmodulesClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;

		repository.Submodules.Refresh();
	}

	static void OnAddSubmoduleClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent = Utility.GetParentControl(item);

		using var dlg = new AddSubmoduleDialog(repository);
		dlg.Run(parent);
	}

	static void OnUpdateSubmoduleClick(object sender, EventArgs e)
	{
		var item      = (ToolStripItem)sender;
		var submodule = (Submodule)item.Tag;
		var parent    = Utility.GetParentControl(item);

		GuiCommands.UpdateSubmodule(parent, submodule);
	}

	static void OnSyncSubmoduleClick(object sender, EventArgs e)
	{
		var item      = (ToolStripItem)sender;
		var submodule = (Submodule)item.Tag;
		var parent    = Utility.GetParentControl(item);

		GuiCommands.SyncSubmodule(parent, submodule);
	}

	static void OnUpdateSubmodulesClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var submodules = (SubmodulesCollection)item.Tag;
		var parent     = Utility.GetParentControl(item);

		GuiCommands.UpdateSubmodules(parent, submodules);
	}

	static void OnSyncSubmodulesClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var submodules = (SubmodulesCollection)item.Tag;
		var parent     = Utility.GetParentControl(item);

		GuiCommands.SyncSubmodules(parent, submodules);
	}

	#endregion

	#region Working Tree Items

	public T GetMarkAsResolvedItem<T>(TreeItem treeItem)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(treeItem, nameof(treeItem));

		var item = new T()
		{
			Text = Resources.StrMarkAsResolved,
			Tag  = treeItem,
		};
		_dpiBindings.BindImage(item, Icons.MarkResolved);
		item.Click += OnStageClick;
		return item;
	}

	public T GetStageItem<T>(TreeItem treeItem)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(treeItem, nameof(treeItem));

		var item = new T()
		{
			Text = Resources.StrStage,
			Tag  = treeItem,
		};
		_dpiBindings.BindImage(item, Icons.Stage);
		item.Click += OnStageClick;
		return item;
	}

	public T GetStageItem<T>(Repository repository, TreeItem[] treeItems)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(treeItems);
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrStage,
			Tag  = Tuple.Create(repository, treeItems),
		};
		_dpiBindings.BindImage(item, Icons.Stage);
		item.Click += OnStageSeveralClick;
		return item;
	}

	public T GetManualStageItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		return GetManualStageItem<T>(repository, Resources.StrManual.AddEllipsis());
	}

	public T GetManualStageItem<T>(Repository repository, string name)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = name,
			Tag  = repository,
		};
		item.Click += OnManualStageClick;
		return item;
	}

	public T GetStageAllItem<T>(Repository repository, string name)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = name,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.StageAll);
		item.Click += OnStageAllClick;
		return item;
	}

	public T GetUpdateItem<T>(Repository repository, string name)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = name,
			Tag  = repository,
		};
		item.Click += OnUpdateClick;
		return item;
	}

	public T GetCommitItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrCommit.AddEllipsis(),
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.Commit);
		item.Click += OnCommitClick;
		return item;
	}

	public T GetUnstageItem<T>(TreeItem treeItem)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(treeItem, nameof(treeItem));

		var item = new T()
		{
			Text = Resources.StrUnstage,
			Tag  = treeItem,
		};
		_dpiBindings.BindImage(item, Icons.Unstage);
		item.Click += OnUnstageClick;
		return item;
	}

	public T GetUnstageItem<T>(Repository repository, TreeItem[] treeItems)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(treeItems);
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrUnstage,
			Tag  = Tuple.Create(repository, treeItems),
		};
		_dpiBindings.BindImage(item, Icons.Unstage);
		item.Click += OnUnstageSeveralClick;
		return item;
	}

	public T GetUnstageAllItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		return GetUnstageAllItem<T>(repository, Resources.StrUnstageAll);
	}

	public T GetUnstageAllItem<T>(Repository repository, string name)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = name,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.UnstageAll);
		item.Click += OnUnstageAllClick;
		return item;
	}

	public T GetMergeToolItem<T>(TreeFile file, MergeTool mergeTool = null)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(file, nameof(file));

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
			Text  = text,
			Tag   = Tuple.Create(file, mergeTool),
		};
		item.Click += OnMergeToolClick;
		return item;
	}

	public T GetResolveConflictItem<T>(TreeFile file, ConflictResolution resolution)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(file, nameof(file));

		var text = resolution switch
		{
			ConflictResolution.DeleteFile       => Resources.StrDeleteFile,
			ConflictResolution.KeepModifiedFile => Resources.StrKeepFile,
			ConflictResolution.UseOurs          => Resources.StrUseOurs,
			ConflictResolution.UseTheirs        => Resources.StrUseTheirs,
			_ => throw new ArgumentException("Unknown conflict resolution.", nameof(resolution)),
		};
		var item = new T()
		{
			Text = text,
			Tag  = Tuple.Create(file, resolution),
		};
		item.Click += OnResolveConflictItemClick;
		return item;
	}

	public T GetRevertPathItem<T>(TreeItem treeItem)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(treeItem, nameof(treeItem));

		var item = new T()
		{
			Text = Resources.StrRevert,
			Tag  = treeItem,
		};
		_dpiBindings.BindImage(item, Icons.Revert);
		item.Click += OnRevertPathClick;
		return item;
	}

	public T GetRevertPathsItem<T>(IEnumerable<TreeItem> treeItems)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(treeItems);

		var item = new T()
		{
			Text = Resources.StrRevert,
			Tag  = treeItems,
		};
		_dpiBindings.BindImage(item, Icons.Revert);
		item.Click += OnRevertPathsClick;
		return item;
	}

	public T GetRemovePathItem<T>(TreeItem treeItem)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(treeItem, nameof(treeItem));

		var item = new T()
		{
			Text = Resources.StrDelete,
			Tag  = treeItem,
		};
		_dpiBindings.BindImage(item, Icons.Delete);
		item.Click += OnDeletePathClick;
		return item;
	}

	public T GetBlameItem<T>(IRevisionPointer revision, TreeFile file)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);
		Verify.Argument.IsValidGitObject(file, nameof(file));

		var item = new T()
		{
			Text = Resources.StrBlame,
			Tag  = revision.GetBlameSource(file.RelativePath),
		};
		_dpiBindings.BindImage(item, Icons.Blame);
		item.Click += OnBlameClick;
		return item;
	}

	public T GetBlameItem<T>(IRevisionPointer revision, string fileName)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);
		Verify.Argument.IsNeitherNullNorWhitespace(fileName);

		var item = new T()
		{
			Text = Resources.StrBlame,
			Tag  = revision.GetBlameSource(fileName),
		};
		_dpiBindings.BindImage(item, Icons.Blame);
		item.Click += OnBlameClick;
		return item;
	}

	public T GetPathHistoryItem<T>(IRevisionPointer revision, TreeFile file)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(file, nameof(file));

		return GetPathHistoryItem<T>(revision, file.RelativePath);
	}

	public T GetPathHistoryItem<T>(IRevisionPointer revision, TreeCommit commit)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(commit, nameof(commit));

		return GetPathHistoryItem<T>(revision, commit.RelativePath);
	}

	public T GetPathHistoryItem<T>(IRevisionPointer revision, TreeDirectory directory)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(directory, nameof(directory));

		return GetPathHistoryItem<T>(revision, directory.RelativePath + "/");
	}

	public T GetPathHistoryItem<T>(IRevisionPointer revision, string path)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);
		Verify.Argument.IsNotNull(path);

		var item = new T()
		{
			Text = Resources.StrHistory,
			Tag  = new PathLogSource(revision, path),
		};
		_dpiBindings.BindImage(item, path.EndsWith('/') ? Icons.FolderHistory : Icons.FileHistory);
		item.Click += OnPathHistoryClick;
		return item;
	}

	private static void OnBlameClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var data = (IBlameSource)item.Tag;

		GitterApplication.WorkingEnvironment.ViewDockService.ShowView(
			Views.Guids.BlameViewGuid,
			new Views.BlameViewModel(data));
	}

	private static void OnPathHistoryClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var data = (ILogSource)item.Tag;

		GitterApplication.WorkingEnvironment.ViewDockService.ShowView(
			Views.Guids.PathHistoryViewGuid,
			new Views.HistoryViewModel(data));
	}

	private static void OnResolveConflictItemClick(object sender, EventArgs e)
	{
		var item   = (ToolStripItem)sender;
		var parent = Utility.GetParentControl(item);
		var data   = (Tuple<TreeFile, ConflictResolution>)item.Tag;
		try
		{
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				data.Item1.ResolveConflict(data.Item2);
			}
		}
		catch(GitException exc)
		{
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
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent     = Utility.GetParentControl(item);
		try
		{
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				repository.Status.UnstageAll();
			}
		}
		catch(GitException exc)
		{
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
		var item     = (ToolStripItem)sender;
		var treeItem = (TreeItem)item.Tag;
		var parent   = Utility.GetParentControl(item);
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				treeItem.RemoveFromWorkingTree();
			}
		}
		catch(GitException exc)
		{
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

				using(parent.ChangeCursor(Cursors.WaitCursor))
				{
					repository.Status.RevertPaths(paths);
				}
			}
			catch(GitException exc)
			{
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
			if(treeItem.ItemType == TreeItemType.Tree || treeItem.Status == FileStatus.Modified)
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				treeItem.Revert();
			}
		}
		catch(GitException exc)
		{
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
					ProgressForm.MonitorTaskAsModalWindow(parent, Resources.StrRunningMergeTool,
						(p, c) => file.RunMergeToolAsync(tool, p, c));
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				treeItem.Stage();
			}
		}
		catch(GitException exc)
		{
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				repository.Status.Stage(treeItems);
			}
		}
		catch(GitException exc)
		{
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				repository.Status.StageAll();
			}
		}
		catch(GitException exc)
		{
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				repository.Status.StageUpdated();
			}
		}
		catch(GitException exc)
		{
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				treeItem.Unstage();
			}
		}
		catch(GitException exc)
		{
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				repository.Status.Unstage(treeItems);
			}
		}
		catch(GitException exc)
		{
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

	public T GetShowConfigurationViewItem<T>()
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

	public T GetRefreshConfigurationItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrRefresh,
			Tag = repository,
		};
		_dpiBindings.BindImage(item, Icons.Refresh);
		item.Click += OnRefreshConfigurationClick;
		return item;
	}

	public T GetUnsetParameterItem<T>(ConfigParameter configParameter)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidGitObject(configParameter, nameof(configParameter));

		var item = new T()
		{
			Text = Resources.StrUnset,
			Tag = configParameter,
		};
		_dpiBindings.BindImage(item, Icons.ConfigUnset);
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
			using(parent.ChangeCursor(Cursors.WaitCursor))
			{
				parameter.Unset();
			}
		}
		catch(ConfigParameterDoesNotExistException exc)
		{
			if(parameter.ConfigFile == ConfigFile.Repository)
			{
				if(GitterApplication.MessageBoxService.Show(
					parent,
					Resources.AskRemoveConfigParameterFromAllFiles,
					Resources.AskRemoveConfigParameter.UseAsFormat(parameter.Name),
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question) == DialogResult.Yes)
				{
					var gitAccessor = ((IGitRepository)parameter.Repository).Accessor.GitAccessor;
					try
					{
						gitAccessor.UnsetConfigValue.Invoke(
							new UnsetConfigValueParameters()
							{
								ConfigFile = Git.ConfigFile.User,
								ParameterName = parameter.Name,
							});
					}
					catch(Exception exception) when(!exception.IsCritical())
					{
					}
					try
					{
						gitAccessor.UnsetConfigValue.Invoke(
							new UnsetConfigValueParameters()
							{
								ConfigFile = Git.ConfigFile.System,
								ParameterName = parameter.Name,
							});
					}
					catch(Exception exception) when(!exception.IsCritical())
					{
					}
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

	public T GetCopyDiffLinesItem<T>(IEnumerable<DiffLine> lines, string text, bool copyAsPatch, DiffLineState state)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(lines);
		Verify.Argument.HasNoNullItems(lines);

		bool enabled = false;
		foreach(var line in lines)
		{
			if(line is null) throw new ArgumentException("Cannot contain null items.", nameof(lines));
			if((line.State & state) != DiffLineState.Invalid)
			{
				enabled = true;
				break;
			}
		}

		var item = new T()
		{
			Text = text,
			Tag = Tuple.Create(lines, copyAsPatch, state),
			Enabled = enabled,
		};
		_dpiBindings.BindImage(item, CommonIcons.ClipboardCopy);
		item.Click += OnCopyDiffLinesCick;
		return item;
	}

	public T GetCopyDiffLinesItem<T>(IEnumerable<DiffLine> lines, bool copyAsPatch)
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
		ClipboardEx.TrySetTextSafe(text);
	}

	#endregion

	#region Misc Items

	public T GetShowContributorsViewItem<T>()
		where T : ToolStripItem, new()
	{
		var item = new T()
		{
			Text = Resources.StrManage,
			Tag  = Views.Guids.ContributorsViewGuid,
		};
		item.Click += OnShowViewItemClick;
		return item;
	}

	public T GetRefreshContributorsItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrRefresh,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.Refresh);
		item.Click += OnRefreshContributorsClick;
		return item;
	}

	public static T GetSaveAsItem<T>(Tree tree, string fileName)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(tree);
		Verify.Argument.IsNeitherNullNorWhitespace(fileName);

		var item = new T()
		{
			Text = Resources.StrSaveAs.AddEllipsis(),
			Tag = Tuple.Create(tree, fileName),
		};
		item.Click += OnSaveAsItemClick;
		return item;
	}

	public static T GetExtractAndOpenFileItem<T>(Tree tree, string fileName)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(tree);
		Verify.Argument.IsNeitherNullNorWhitespace(fileName);

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
		Verify.Argument.IsNotNull(tree);
		Verify.Argument.IsNeitherNullNorWhitespace(fileName);

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
		Verify.Argument.IsNeitherNullNorWhitespace(url);

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
		Verify.Argument.IsNeitherNullNorWhitespace(app);

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
		Verify.Argument.IsNeitherNullNorWhitespace(url);

		var item = new T()
		{
			Image = image,
			Text  = name ?? url,
			Tag   = url,
		};
		item.Click += OnOpenUrlWithItemClick;
		return item;
	}

	public static T GetOpenCmdAtItem<T>(string name, Image image, string path)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(path);

		var item = new T()
		{
			Image = image,
			Text  = name ?? path,
			Tag   = path,
		};
		item.Click += OnOpenCmdAtItemClick;
		return item;
	}

	public T GetCleanItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrClean.AddEllipsis(),
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.Clean);
		item.Click += OnCleanClick;
		return item;
	}

	public T GetViewDiffItem<T>(IDiffSource diffSource)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(diffSource);

		var item = new T()
		{
			Text = Resources.StrViewDiff,
			Tag  = diffSource,
		};
		_dpiBindings.BindImage(item, Icons.Diff);
		item.Click += OnViewDiffItemClick;
		return item;
	}

	public T GetViewTreeItem<T>(IRevisionPointer revision)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsValidRevisionPointer(revision);

		var item = new T()
		{
			Text = Resources.StrViewTree,
			Tag  = revision,
		};
		_dpiBindings.BindImage(item, Icons.FolderTree);
		item.Click += OnViewTreeItemClick;
		return item;
	}

	public T GetShowReferencesViewItem<T>()
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

	public T GetRefreshAllReferencesListItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrRefresh,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.Refresh);
		item.Click += OnRefreshAllReferencesClick;
		return item;
	}

	public T GetCopyToClipboardItem<T>(string name, Func<string> text)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(text);

		var item = new T()
		{
			Text = name,
			Tag = text,
		};
		_dpiBindings.BindImage(item, Icons.ClipboardCopy);
		item.Click += OnCopyToClipboardClick;
		return item;
	}

	public T GetCopyToClipboardItem<T>(string name, string text, bool enableToolTip = true)
		where T : ToolStripItem, new()
	{
		var item = new T()
		{
			Text = name,
			Tag  = text,
		};
		if(enableToolTip && name != text) item.ToolTipText = text;
		_dpiBindings.BindImage(item, Icons.ClipboardCopy);
		item.Click += OnCopyToClipboardClick;
		return item;
	}

	public T GetCopyHashToClipboardItem<T>(string name, string text, bool enableToolTip = true)
		where T : ToolStripItem, new()
	{
		var item = new T()
		{
			Text = name,
			Tag  = text,
		};
		if(enableToolTip && name != text) item.ToolTipText = text;
		_dpiBindings.BindImage(item, Icons.ClipboardCopy);
		item.Click += OnCopyHashToClipboardClick;
		return item;
	}

	public T GetRefreshReferencesItem<T>(Repository repository, ReferenceType referenceTypes, string name)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = name,
			Tag  = Tuple.Create(repository, referenceTypes),
		};
		_dpiBindings.BindImage(item, Icons.Refresh);
		item.Click += OnRefreshReferencesClick;
		return item;
	}

	public T GetResolveConflictsItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrResolveConflicts.AddEllipsis(),
			Tag  = repository,
		};
		item.Click += OnResolveConflictsClick;
		return item;
	}

	public static T GetExpandAllItem<T>(CustomListBoxItem treeItem)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(treeItem);

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
		Verify.Argument.IsNotNull(treeItem);

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

	public T GetSendEmailItem<T>(string email)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNeitherNullNorWhitespace(email);

		var item = new T()
		{
			Text = Resources.StrSendEmail,
			Tag = email,
			ToolTipText = email,
			Enabled = email.Length != 0,
		};
		_dpiBindings.BindImage(item, Icons.MailSend);
		item.Click += OnSendEmailClick;
		return item;
	}

	public T GetCompressRepositoryItem<T>(Repository repository)
		where T : ToolStripItem, new()
	{
		Verify.Argument.IsNotNull(repository);

		var item = new T()
		{
			Text = Resources.StrCompressRepository,
			Tag  = repository,
		};
		_dpiBindings.BindImage(item, Icons.GarbageCollect);
		item.Click += OnCompressRepositoryClick;
		return item;
	}

	private static void OnRefreshContributorsClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		repository.Users.Refresh();
	}

	private static void OnSaveAsItemClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var data = (Tuple<Tree, string>)item.Tag;

		data.Item1.ExtractBlobToFile(data.Item2);
	}

	private static void OnExtractFileItemClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var data = (Tuple<Tree, string, bool>)item.Tag;
		var tree = data.Item1;
		var path = data.Item2;
		var openas = data.Item3;

		var fileName = tree.ExtractBlobToTemporaryFile(path);
		if(!string.IsNullOrWhiteSpace(fileName))
		{
			if(openas)
			{
				Utility.ShowOpenWithDialog(fileName);
			}
			else
			{
				var process = Utility.CreateProcessFor(fileName);
				process.EnableRaisingEvents = true;
				process.Exited += OnFileViewerProcessExited;
				process.Start();
			}
		}
	}

	private static void OnFileViewerProcessExited(object sender, EventArgs e)
	{
		var process = (Process)sender;
		var path = process.StartInfo.FileName;
		try
		{
			if(File.Exists(path))
			{
				var time = File.GetLastWriteTime(path);
				if(time > process.StartTime)
				{
					/*
					GitterApplication.MainForm.BeginInvoke(
						new Action(() =>
							{
								GitterApplication.MessageBoxService.Show(
									GitterApplication.MainForm,
									"File '{0}' was modified.\nDo you want to save it",
									"gitter",
									MessageBoxButton.YesNo,
									MessageBoxIcon.Question);
							}));
					*/
					File.Delete(path);
				}
				else
				{
					File.Delete(path);
				}
			}
		}
		catch(Exception exc) when(!exc.IsCritical())
		{
		}
		process.Dispose();
	}

	private static void OnCompressRepositoryClick(object sender, EventArgs e)
	{
		var item       = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent     = Utility.GetParentControl(item);

		GuiCommands.GarbageCollect(parent, repository);
	}

	private static void OnOpenCmdAtItemClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var path = (string)item.Tag;

		var psi = new ProcessStartInfo(@"cmd")
		{
			WorkingDirectory = path,
		};
		using var p = new Process { StartInfo = psi };
		p.Start();
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
		var item       = (ToolStripItem)sender;
		var diffSource = (IDiffSource)item.Tag;

		GitterApplication.WorkingEnvironment.ViewDockService.ShowView(
			Views.Guids.DiffViewGuid,
			new Views.DiffViewModel(diffSource, null));
	}

	private static void OnViewTreeItemClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var revPtr = (IRevisionPointer)item.Tag;

		GitterApplication.WorkingEnvironment.ViewDockService.ShowView(
			Views.Guids.TreeViewGuid,
			new Views.TreeViewModel(new RevisionTreeSource(revPtr)));
	}

	private static void OnResolveConflictsClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent = Utility.GetParentControl(item);

		using var dlg = new ConflictsDialog(repository);
		dlg.Run(parent);
	}

	private static void OnCleanClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var repository = (Repository)item.Tag;
		var parent = Utility.GetParentControl(item);

		using var dlg = new CleanDialog(repository);
		dlg.Run(parent);
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
			Process.Start(data.Item1, data.Item2)?.Dispose();
		}
		else
		{
			Process.Start(data.Item1)?.Dispose();
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
		var text = item.Tag switch
		{
			string       x => x,
			Func<string> f => f(),
			_ => throw new InvalidOperationException(),
		};
		ClipboardEx.TrySetTextSafe(text);
	}

	private static void OnCopyHashToClipboardClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var text = item.Tag switch
		{
			string       x => x,
			Func<string> f => f(),
			_ => throw new InvalidOperationException(),
		};
		if((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
		{
			text = text.Substring(0, 7);
		}
		ClipboardEx.TrySetTextSafe(text);
	}

	private static void OnRefreshReferencesClick(object sender, EventArgs e)
	{
		var item = (ToolStripItem)sender;
		var data = (Tuple<Repository, ReferenceType>)item.Tag;
		var repository = data.Item1;
		var type = data.Item2;

		if((type | ReferenceType.Remote) == ReferenceType.Remote)
		{
			repository.Remotes.Refresh();
		}
		if((type | ReferenceType.Branch) == ReferenceType.Branch)
		{
			repository.Refs.RefreshBranches();
		}
		else
		{
			if((type | ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
			{
				repository.Refs.Heads.Refresh();
			}
			else if((type | ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch)
			{
				repository.Refs.Remotes.Refresh();
			}
		}
		if((type | ReferenceType.Tag) == ReferenceType.Tag)
		{
			repository.Refs.Tags.Refresh();
		}
		if((type | ReferenceType.Stash) == ReferenceType.Stash)
		{
			repository.Stash.Refresh();
		}
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

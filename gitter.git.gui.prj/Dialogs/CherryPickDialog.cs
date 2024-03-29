﻿#region Copyright Notice
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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;

using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class CherryPickDialog : GitDialogBase, IExecutableDialog
{
	#region .ctor

	public CherryPickDialog(IRevisionPointer revisionPointer)
	{
		Verify.Argument.IsValidRevisionPointer(revisionPointer, nameof(revisionPointer));

		InitializeComponent();

		RevisionPointer = revisionPointer;

		Text = Resources.StrCherryPickCommit;

		_lblRevision.Text = Resources.StrRevision.AddColon();
		_grpMainlineParentCommit.Text = Resources.StrMainlineParentCommit;
		_grpOptions.Text = Resources.StrOptions;
		_chkNoCommit.Text = Resources.StrsNoCommit;
		ToolTipService.Register(_chkNoCommit, Resources.TipCherryPickNoCommit);

		_txtRevision.Text = revisionPointer.Pointer;

		GitterApplication.FontManager.InputFont.Apply(_txtRevision);

		var revision = revisionPointer.Dereference();
		if(!revision.IsLoaded)
		{
			revision.Load();
		}
		if(revision.Parents.Count > 1)
		{
			_lstCommits.Style = GitterApplication.DefaultStyle;
			bool first = true;
			foreach(var parent in revision.Parents)
			{
				if(!parent.IsLoaded)
				{
					parent.Load();
				}
				if(first)
				{
					first = false;
				}
				else
				{
					_lstCommits.Panels.Add(new FlowPanelSeparator() { Height = 10 });
				}
				_lstCommits.Panels.Add(
					new RevisionHeaderPanel()
					{
						Revision = parent,
						IsSelectable = true,
						IsSelected = _lstCommits.Panels.Count == 0,
					});
			}
		}
		else
		{
			_pnlMainlineParentCommit.Visible = false;
			Height -= _pnlMainlineParentCommit.Height + _pnlOptions.Margin.Top;
			Width = 385;
		}
	}

	#endregion

	#region Properties

	public IRevisionPointer RevisionPointer { get; }

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(480, 327));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCherryPick;

	public bool NoCommit
	{
		get => _chkNoCommit.Checked;
		set => _chkNoCommit.Checked = value;
	}

	public int MainlineParentCommit
	{
		get
		{
			int index = 0;
			foreach(var p in _lstCommits.Panels)
			{
				if(p is RevisionHeaderPanel rhp)
				{
					++index;
					if(rhp.IsSelected)
					{
						return index;
					}
				}
			}
			return 0;
		}
		set
		{
			Verify.Argument.IsNotNegative(value);

			if(value == 0)
			{
				foreach(var p in _lstCommits.Panels)
				{
					if(p is RevisionHeaderPanel rhp)
					{
						rhp.IsSelected = false;
					}
				}
			}
			else
			{
				int index = 0;
				foreach(var p in _lstCommits.Panels)
				{
					if(p is RevisionHeaderPanel rhp)
					{
						++index;
						if(index == value)
						{
							rhp.IsSelected = true;
							break;
						}
					}
				}
			}
		}
	}

	#endregion

	#region IExecutableDialog

	public bool Execute()
	{
		int mainline	= MainlineParentCommit;
		bool noCommit	= NoCommit;
		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				RevisionPointer.CherryPick(mainline, noCommit);
			}
		}
		catch(HaveConflictsException)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				Resources.ErrCherryPickIsNotPossibleWithConflicts,
				Resources.StrCherryPick,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		catch(HaveLocalChangesException)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				Resources.ErrCherryPickIsNotPossibleWithLocalChnges,
				Resources.StrCherryPick,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		catch(AutomaticCherryPickFailedException)
		{
			BeginInvoke(new Action(
				() =>
				{
					using var dlg = new ConflictsDialog(RevisionPointer.Repository);
					dlg.Run(GitterApplication.MainForm);
				}));
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				string.Format(Resources.ErrFailedToCherryPick, RevisionPointer.Pointer),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		return true;
	}

	#endregion
}

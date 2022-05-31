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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;

sealed class FilesToAddBinding : AsyncDataBinding<IList<TreeFile>>
{
	public FilesToAddBinding(Repository repository, TreeListBox treeListBox)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNotNull(treeListBox);

		Repository  = repository;
		TreeListBox = treeListBox;
		Progress    = treeListBox.ProgressMonitor;
	}

	public Repository Repository { get; }

	private TreeListBox TreeListBox { get; }

	public string Pattern { get; set; }

	public bool IncludeUntracked { get; set; }

	public bool IncludeIgnored { get; set; }

	protected override Task<IList<TreeFile>> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
	{
		Verify.State.IsFalse(IsDisposed, "FilesToAddBinding is disposed.");

		TreeListBox.Cursor = Cursors.WaitCursor;
		return Repository.Status.GetFilesToAddAsync(Pattern, IncludeUntracked, IncludeIgnored,
			progress, cancellationToken);
	}

	protected override void OnFetchCompleted(IList<TreeFile> data)
	{
		if(TreeListBox.IsDisposed)
		{
			return;
		}

		TreeListBox.BeginUpdate();
		TreeListBox.Items.Clear();
		foreach(var item in data)
		{
			TreeListBox.Items.Add(new TreeFileListItem(item, true));
		}
		TreeListBox.EndUpdate();
		TreeListBox.Cursor = Cursors.Default;
	}

	protected override void OnFetchFailed(Exception exception)
	{
		if(TreeListBox.IsDisposed)
		{
			return;
		}

		TreeListBox.BeginUpdate();
		TreeListBox.Items.Clear();
		TreeListBox.EndUpdate();
		TreeListBox.Cursor = Cursors.Default;
	}

	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(!TreeListBox.IsDisposed)
			{
				TreeListBox.Items.Clear();
			}
		}
		base.Dispose(disposing);
	}
}

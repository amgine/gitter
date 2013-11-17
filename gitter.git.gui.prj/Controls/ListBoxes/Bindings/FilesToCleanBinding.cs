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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;

	sealed class FilesToCleanBinding : AsyncDataBinding<IList<TreeItem>>
	{
		#region Data

		private readonly Repository _repository;
		private readonly TreeListBox _treeListBox;

		#endregion

		#region .ctor

		public FilesToCleanBinding(Repository repository, TreeListBox treeListBox)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(treeListBox, "treeListBox");

			_repository  = repository;
			_treeListBox = treeListBox;

			Progress = treeListBox.ProgressMonitor;
		}

		#endregion

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		private TreeListBox TreeListBox
		{
			get { return _treeListBox; }
		}

		public string IncludePattern
		{
			get;
			set;
		}

		public string ExcludePattern
		{
			get;
			set;
		}

		public CleanFilesMode CleanFilesMode
		{
			get;
			set;
		}

		public bool IncludeDirectories
		{
			get;
			set;
		}

		#endregion

		#region Methods

		protected override Task<IList<TreeItem>> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsFalse(IsDisposed, "FilesToCleanBinding is disposed.");

			TreeListBox.Cursor = Cursors.WaitCursor;
			return Repository.Status.GetFilesToCleanAsync(
				IncludePattern, ExcludePattern, CleanFilesMode, IncludeDirectories,
				progress, cancellationToken);
		}

		protected override void OnFetchCompleted(IList<TreeItem> data)
		{
			if(TreeListBox.IsDisposed)
			{
				return;
			}

			TreeListBox.BeginUpdate();
			TreeListBox.Items.Clear();
			foreach(var item in data)
			{
				if(item.ItemType == TreeItemType.Tree)
				{
					TreeListBox.Items.Add(new TreeDirectoryListItem(
						(TreeDirectory)item, TreeDirectoryListItemType.ShowNothing));
				}
				else
				{
					TreeListBox.Items.Add(new TreeFileListItem(
						(TreeFile)item, true));
				}
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

		#endregion
	}
}

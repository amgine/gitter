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

namespace gitter.Git.Gui.Controls.ListBoxes
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class TreeListsBinding : AsyncDataBinding<Tree>
	{
		#region Data

		private readonly ITreeSource _treeSource;
		private readonly TreeListBox _directoryTreeListBox;
		private readonly TreeListBox _directoryContentListBox;

		#endregion

		#region .ctor

		public TreeListsBinding(ITreeSource treeSource, TreeListBox directoryTreeListBox, TreeListBox directoryContentListBox)
		{
			Verify.Argument.IsNotNull(treeSource, "treeSource");
			Verify.Argument.IsNotNull(directoryTreeListBox, "directoryTreeListBox");
			Verify.Argument.IsNotNull(directoryContentListBox, "directoryContentListBox");

			_treeSource              = treeSource;
			_directoryTreeListBox    = directoryTreeListBox;
			_directoryContentListBox = directoryContentListBox;

			Progress = directoryTreeListBox.ProgressMonitor;
		}

		#endregion

		#region Properties

		public ITreeSource TreeSource
		{
			get { return _treeSource; }
		}

		public TreeListBox DirectoryTreeListBox
		{
			get { return _directoryTreeListBox; }
		}

		public TreeListBox DirectoryContentListBox
		{
			get { return _directoryContentListBox; }
		}

		#endregion

		#region Methods

		protected override Task<Tree> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			DirectoryContentListBox.Text = string.Empty;

			return TreeSource.GetTreeAsync(progress, cancellationToken);
		}

		protected override void OnFetchCompleted(Tree tree)
		{
			if(IsDisposed || (DirectoryTreeListBox.IsDisposed && DirectoryContentListBox.IsDisposed))
			{
				return;
			}

			DirectoryContentListBox.ProgressMonitor.Report(OperationProgress.Completed);
			DirectoryTreeListBox.SetTree(tree.Root, TreeListBoxMode.ShowDirectoryTree);
			DirectoryContentListBox.SetTree(tree.Root, TreeListBoxMode.ShowDirectoryContent);
		}

		protected override void OnFetchFailed(Exception exception)
		{
			if(IsDisposed || (DirectoryTreeListBox.IsDisposed && DirectoryContentListBox.IsDisposed))
			{
				return;
			}

			DirectoryContentListBox.ProgressMonitor.Report(OperationProgress.Completed);
			DirectoryContentListBox.Text = Resources.StrsFailedToFetchTree;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(!DirectoryTreeListBox.IsDisposed)
				{
					DirectoryTreeListBox.Clear();
				}
				if(!DirectoryContentListBox.IsDisposed)
				{
					DirectoryContentListBox.Clear();
				}
			}
			base.Dispose(disposing);
		}

		#endregion
	}
}

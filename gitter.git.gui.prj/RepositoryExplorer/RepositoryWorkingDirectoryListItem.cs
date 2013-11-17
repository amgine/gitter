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

namespace gitter.Git.Gui
{
	using System;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;
	
	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class RepositoryWorkingDirectoryListItem : RepositoryExplorerItemBase
	{
		private sealed class AsyncTreeDataSource : AsyncDataBinding<Tree>
		{
			#region Data

			private readonly IRevisionPointer _revision;
			private readonly CustomListBoxItemsCollection _items;
			private readonly EventHandler<BoundItemActivatedEventArgs<TreeItem>> _onItemActivated;
			private readonly EventHandler<ItemContextMenuRequestEventArgs> _onItemContextMenuRequested;
			private TreeBinding _binding;

			#endregion

			public AsyncTreeDataSource(IRevisionPointer revision, CustomListBoxItemsCollection items,
				EventHandler<BoundItemActivatedEventArgs<TreeItem>> onItemActivated,
				EventHandler<ItemContextMenuRequestEventArgs> onItemContextMenuRequested)
			{
				Verify.Argument.IsNotNull(revision, "revision");
				Verify.Argument.IsNotNull(items, "items");

				_revision = revision;
				_items    = items;

				_onItemActivated = onItemActivated;
				_onItemContextMenuRequested = onItemContextMenuRequested;
			}

			public IRevisionPointer Revision
			{
				get { return _revision; }
			}

			public CustomListBoxItemsCollection Items
			{
				get { return _items; }
			}

			protected override Task<Tree> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				if(_binding != null)
				{
					_binding.ItemActivated -= _onItemActivated;
					_binding.ItemContextMenuRequested -= _onItemContextMenuRequested;
					_binding.Dispose();
					_binding = null;
				}
				return Revision.GetTreeAsync(progress, cancellationToken);
			}

			protected override void OnFetchCompleted(Tree tree)
			{
				if(tree != null)
				{
					_binding = new TreeBinding(Items, tree.Root, false);
					_binding.ItemActivated += _onItemActivated;
					_binding.ItemContextMenuRequested += _onItemContextMenuRequested;
				}
			}

			protected override void OnFetchFailed(Exception exception)
			{
				LoggingService.Global.Warning(exception, "Failed to fetch HEAD tree: " + exception.Message);
			}

			protected override void Dispose(bool disposing)
			{
				if(disposing)
				{
					if(_binding != null)
					{
						_binding.ItemActivated -= _onItemActivated;
						_binding.ItemContextMenuRequested -= _onItemContextMenuRequested;
						_binding.Dispose();
						_binding = null;
					}
				}
				base.Dispose(disposing);
			}
		}

		private AsyncTreeDataSource _dataSource;

		public RepositoryWorkingDirectoryListItem()
			: base(CachedResources.Bitmaps["ImgFolder"], Resources.StrWorkingDirectory)
		{
		}

		private AsyncTreeDataSource DataSource
		{
			get { return _dataSource; }
			set
			{
				if(_dataSource != value)
				{
					if(_dataSource != null)
					{
						_dataSource.Dispose();
					}
					_dataSource = value;
					if(_dataSource != null)
					{
						_dataSource.ReloadData();
					}
				}
			}
		}

		/// <summary>Called when item is attached to listbox.</summary>
		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			if(Repository != null)
			{
				DataSource = new AsyncTreeDataSource(Repository.Head, Items, OnItemActivated, OnItemContextMenuRequested);
			}
		}

		/// <summary>Called when item is detached from listbox.</summary>
		protected override void OnListBoxDetached()
		{
			DataSource = null;
			base.OnListBoxDetached();
		}

		protected override void DetachFromRepository()
		{
			DataSource = null;
			Repository.Head.PositionChanged -= OnHeadPositionChanged;
		}

		protected override void AttachToRepository()
		{
			if(IsAttachedToListBox)
			{
				DataSource = new AsyncTreeDataSource(Repository.Head, Items, OnItemActivated, OnItemContextMenuRequested);
			}
			Repository.Head.PositionChanged += OnHeadPositionChanged;
		}

		private void Refresh()
		{
			if(DataSource != null)
			{
				DataSource.ReloadData();
			}
		}

		private void OnHeadPositionChanged(object sender, RevisionChangedEventArgs e)
		{
			var listBox = ListBox;
			if(listBox != null && listBox.InvokeRequired)
			{
				listBox.BeginInvoke(new MethodInvoker(Refresh), null);
			}
			else
			{
				Refresh();
			}
		}

		private static void OnItemActivated(object sender, BoundItemActivatedEventArgs<TreeItem> e)
		{
			var item = e.Object;
			if(item.ItemType == TreeItemType.Blob)
			{
				Utility.OpenUrl(item.FullPath);
			}
		}

		private void OnItemContextMenuRequested(object sender, ItemContextMenuRequestEventArgs e)
		{
			var item = e.Item as ITreeItemListItem;
			if(item != null)
			{
				var file = item.TreeItem as TreeFile;
				if(file != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpen, null, file.FullPath),
							GuiItemFactory.GetOpenUrlWithItem<ToolStripMenuItem>(Resources.StrOpenWith.AddEllipsis(), null, file.FullPath),
							GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenContainingFolder, null, Path.GetDirectoryName(file.FullPath)),
							new ToolStripSeparator(),
							new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
								new ToolStripItem[]
								{
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFileName, file.Name),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, file.RelativePath),
									GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, file.FullPath),
								}),
							new ToolStripSeparator(),
							GuiItemFactory.GetBlameItem<ToolStripMenuItem>(Repository.Head, file.RelativePath),
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(Repository.Head, file.RelativePath),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					return;
				}
				var directory = item.TreeItem as TreeDirectory;
				if(directory != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, directory.FullPath),
							GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, directory.FullPath),
						});
					if(e.Item.Items.Count != 0)
					{
						menu.Items.AddRange(
							new ToolStripItem[]
							{
								new ToolStripSeparator(),
								GuiItemFactory.GetExpandAllItem<ToolStripMenuItem>(e.Item),
								GuiItemFactory.GetCollapseAllItem<ToolStripMenuItem>(e.Item),
							});
					}
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							new ToolStripSeparator(),
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(Repository.Head, directory.RelativePath + "/"),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					return;
				}
				var commit = item.TreeItem as TreeCommit;
				if(commit != null)
				{
					var menu = new ContextMenuStrip();
					menu.Items.AddRange(
						new ToolStripItem[]
						{
							GuiItemFactory.GetOpenAppItem<ToolStripMenuItem>(
								Resources.StrOpenWithGitter, null, Application.ExecutablePath, commit.FullPath.SurroundWithDoubleQuotes()),
							GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, commit.FullPath),
							GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, commit.FullPath),
							new ToolStripSeparator(),
							GuiItemFactory.GetPathHistoryItem<ToolStripMenuItem>(Repository.Head, commit.RelativePath),
						});
					Utility.MarkDropDownForAutoDispose(menu);
					e.ContextMenu = menu;
					return;
				}
			}
		}

		/// <summary>Gets the context menu.</summary>
		/// <param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(Repository != null)
			{
				var menu = new ContextMenuStrip();
				menu.Items.AddRange(
					new ToolStripItem[]
					{
						GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, Repository.WorkingDirectory),
						GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, Repository.WorkingDirectory),
						new ToolStripSeparator(),
						GuiItemFactory.GetExpandAllItem<ToolStripMenuItem>(requestEventArgs.Item),
						GuiItemFactory.GetCollapseAllItem<ToolStripMenuItem>(requestEventArgs.Item),
					});
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}
			else
			{
				return null;
			}
		}
	}
}

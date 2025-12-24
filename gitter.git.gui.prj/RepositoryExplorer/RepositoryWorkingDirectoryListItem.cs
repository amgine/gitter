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
		private readonly EventHandler<BoundItemActivatedEventArgs<TreeItem>> _onItemActivated;
		private readonly EventHandler<ItemContextMenuRequestEventArgs> _onItemContextMenuRequested;
		private TreeBinding? _binding;

		public AsyncTreeDataSource(IRevisionPointer revision, CustomListBoxItemsCollection items,
			EventHandler<BoundItemActivatedEventArgs<TreeItem>> onItemActivated,
			EventHandler<ItemContextMenuRequestEventArgs> onItemContextMenuRequested)
		{
			Verify.Argument.IsNotNull(revision);
			Verify.Argument.IsNotNull(items);

			Revision = revision;
			Items    = items;

			_onItemActivated = onItemActivated;
			_onItemContextMenuRequested = onItemContextMenuRequested;
		}

		public IRevisionPointer Revision { get; }

		public CustomListBoxItemsCollection Items { get; }

		protected override Task<Tree> FetchDataAsync(IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
		{
			DisposeBinding();
			return Revision.GetTreeAsync(progress, cancellationToken);
		}

		protected override void OnFetchCompleted(Tree tree)
		{
			if(tree is not null)
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

		private void DisposeBinding()
		{
			if(_binding is null) return;

			_binding.ItemActivated -= _onItemActivated;
			_binding.ItemContextMenuRequested -= _onItemContextMenuRequested;
			_binding.Dispose();
			_binding = null;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				DisposeBinding();
			}
			base.Dispose(disposing);
		}
	}

	private AsyncTreeDataSource? _dataSource;

	public RepositoryWorkingDirectoryListItem()
		: base(Icons.Folder, Resources.StrWorkingDirectory)
	{
	}

	private AsyncTreeDataSource? DataSource
	{
		get => _dataSource;
		set
		{
			if(_dataSource == value) return;

			_dataSource?.Dispose();
			_dataSource = value;
			_dataSource?.ReloadData();
		}
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);
		if(Repository is not null)
		{
			DataSource = new AsyncTreeDataSource(Repository.Head, Items, OnItemActivated, OnItemContextMenuRequested);
		}
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		DataSource = null;
		base.OnListBoxDetached(listBox);
	}

	protected override void AttachToRepository(Repository repository)
	{
		if(IsAttachedToListBox)
		{
			DataSource = new AsyncTreeDataSource(repository.Head, Items, OnItemActivated, OnItemContextMenuRequested);
		}
		repository.Head.PositionChanged += OnHeadPositionChanged;
	}

	protected override void DetachFromRepository(Repository repository)
	{
		DataSource = null;
		repository.Head.PositionChanged -= OnHeadPositionChanged;
	}

	private void Refresh() => DataSource?.ReloadData();

	private void OnHeadPositionChanged(object? sender, RevisionChangedEventArgs e)
	{
		var listBox = ListBox;
		if(listBox is not null && listBox.InvokeRequired)
		{
			listBox.BeginInvoke(new MethodInvoker(Refresh), null);
		}
		else
		{
			Refresh();
		}
	}

	private static void OnItemActivated(object? sender, BoundItemActivatedEventArgs<TreeItem> e)
	{
		var item = e.Object;
		if(item.ItemType == TreeItemType.Blob)
		{
			Utility.OpenUrl(item.FullPath);
		}
	}

	private static ContextMenuStrip CreateFileContextMenu(TreeFile file)
	{
		Assert.IsNotNull(file);

		var menu = new ContextMenuStrip
		{
			Renderer = GitterApplication.Style.ToolStripRenderer,
		};
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			[
				GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpen, null, file.FullPath),
				GuiItemFactory.GetOpenUrlWithItem<ToolStripMenuItem>(Resources.StrOpenWith.AddEllipsis(), null, file.FullPath),
				GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenContainingFolder, null, Path.GetDirectoryName(file.FullPath)!),
				new ToolStripSeparator(),
				new ToolStripMenuItem(Resources.StrCopyToClipboard, null,
					[
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFileName, file.Name),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrRelativePath, file.RelativePath),
						factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrFullPath, file.FullPath),
					]),
				new ToolStripSeparator(),
				factory.GetBlameItem<ToolStripMenuItem>(file.Repository.Head, file.RelativePath),
				factory.GetPathHistoryItem<ToolStripMenuItem>(file.Repository.Head, file.RelativePath),
			]);
		return menu;
	}

	private static ContextMenuStrip CreateDirectoryContextMenu(CustomListBoxItem item, TreeDirectory directory)
	{
		Assert.IsNotNull(item);
		Assert.IsNotNull(directory);

		var menu = new ContextMenuStrip
		{
			Renderer = GitterApplication.Style.ToolStripRenderer,
		};
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			[
				GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, directory.FullPath),
				GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, directory.FullPath),
			]);
		if(item.Items.Count > 0)
		{
			menu.Items.AddRange(
				[
					new ToolStripSeparator(),
					GuiItemFactory.GetExpandAllItem<ToolStripMenuItem>(item),
					GuiItemFactory.GetCollapseAllItem<ToolStripMenuItem>(item),
				]);
		}
		menu.Items.AddRange(
			[
				new ToolStripSeparator(),
				factory.GetPathHistoryItem<ToolStripMenuItem>(directory.Repository.Head, directory.RelativePath + "/"),
			]);
		return menu;
	}

	private static ContextMenuStrip CreateCommitContextMenu(TreeCommit commit)
	{
		Assert.IsNotNull(commit);

		var menu = new ContextMenuStrip
		{
			Renderer = GitterApplication.Style.ToolStripRenderer,
		};
		var dpiBindings = new DpiBindings(menu);
		var factory     = new GuiItemFactory(dpiBindings);
		menu.Items.AddRange(
			[
				GuiItemFactory.GetOpenAppItem<ToolStripMenuItem>(
					Resources.StrOpenWithGitter, null, Application.ExecutablePath, commit.FullPath.SurroundWithDoubleQuotes()),
				GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, commit.FullPath),
				GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, commit.FullPath),
				new ToolStripSeparator(),
				factory.GetPathHistoryItem<ToolStripMenuItem>(commit.Repository.Head, commit.RelativePath),
			]);
		return menu;
	}

	private void OnItemContextMenuRequested(object? sender, ItemContextMenuRequestEventArgs e)
	{
		Assert.IsNotNull(e);

		if(e.Item is not ITreeItemListItem item) return;

		var menu = item.TreeItem switch
		{
			TreeFile      file      => CreateFileContextMenu(file),
			TreeDirectory directory => CreateDirectoryContextMenu(e.Item, directory),
			TreeCommit    commit    => CreateCommitContextMenu(commit),
			_ => default,
		};
		if(menu is not null)
		{
			Utility.MarkDropDownForAutoDispose(menu);
			e.ContextMenu = menu;
		}
	}

	/// <inheritdoc/>
	public override ContextMenuStrip? GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		if(Repository is null) return default;

		var menu = new ContextMenuStrip();
		menu.Items.AddRange(
			[
				GuiItemFactory.GetOpenUrlItem<ToolStripMenuItem>(Resources.StrOpenInWindowsExplorer, null, Repository.WorkingDirectory),
				GuiItemFactory.GetOpenCmdAtItem<ToolStripMenuItem>(Resources.StrOpenCommandLine, null, Repository.WorkingDirectory),
				new ToolStripSeparator(),
				GuiItemFactory.GetExpandAllItem<ToolStripMenuItem>(requestEventArgs.Item),
				GuiItemFactory.GetCollapseAllItem<ToolStripMenuItem>(requestEventArgs.Item),
			]);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}
}

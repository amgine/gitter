namespace gitter.Git.Gui.Views
{
	using System;
	using System.Drawing;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Collections.Generic;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class TreeToolbar : ToolStrip
	{
		private static readonly Bitmap ImgFolder = CachedResources.Bitmaps["ImgFolder"];

		#region Data

		private readonly TreeView _treeView;

		#endregion

		private void BuildPath()
		{
			this.DisableRedraw();
			while(Items.Count != 2)
			{
				var item = Items[2];
				Items.RemoveAt(2);
				item.Dispose();
			}
			var stack = new Stack<TreeDirectory>();
			var folder = _treeView.CurrentDirectory;
			if(folder != null)
			{
				while(folder != null)
				{
					stack.Push(folder);
					folder = folder.Parent;
				}
				TreeDirectory prev = null;
				while(stack.Count != 0)
				{
					folder = stack.Pop();
					var item = new ToolStripSplitButton(folder.Name, ImgFolder)
					{
						DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
						Tag = folder,
					};
					if(folder.Directories.Count != 0)
					{
						foreach(var subFolder in folder.Directories)
						{
							var subItem = new ToolStripMenuItem(subFolder.Name, ImgFolder)
							{
								Tag = subFolder,
								Checked = subFolder == prev,
							};
							subItem.Click += (sender, e) =>
							{
								_treeView.CurrentDirectory = (TreeDirectory)((ToolStripItem)sender).Tag;
							};
							item.DropDownItems.Add(subItem);
						}
					}
					else
					{
						item.DropDownItems.Add(new ToolStripMenuItem(Resources.StrlNoSubdirectories.SurroundWith('<', '>'))
							{
								Enabled = false,
							});
					}
					item.ButtonClick += (sender, e) =>
					{
						_treeView.CurrentDirectory = (TreeDirectory)((ToolStripItem)sender).Tag;
					};
					Items.Add(item);
					prev = folder;
				}
			}
			this.EnableRedraw();
			this.RedrawWindow();
		}

		public TreeToolbar(TreeView treeView)
		{
			Verify.Argument.IsNotNull(treeView, "treeView");

			_treeView = treeView;
			_treeView.CurrentDirectoryChanged += OnCurrentDirectoryChanged;

			Items.Add(new ToolStripButton(Resources.StrGoUpOneLevel, CachedResources.Bitmaps["ImgFolderUp"], (sender, e) =>
				{
					var cd = _treeView.CurrentDirectory;
					cd = cd.Parent;
					if(cd != null) _treeView.CurrentDirectory = cd;
				})
				{
					DisplayStyle = ToolStripItemDisplayStyle.Image,
				});
			Items.Add(new ToolStripSeparator());
		}

		private void OnCurrentDirectoryChanged(object sender, EventArgs e)
		{
			BuildPath();
		}
	}
}

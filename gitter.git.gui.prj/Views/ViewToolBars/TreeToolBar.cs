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
			if(folder is not null)
			{
				do
				{
					stack.Push(folder);
					folder = folder.Parent;
				}
				while(folder is not null);
				var prev = default(TreeDirectory);
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
							subItem.Click += (sender, _) =>
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
					item.ButtonClick += (sender, _) =>
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
			Verify.Argument.IsNotNull(treeView, nameof(treeView));

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

			UpdateIcons(DeviceDpi);
		}

		private void UpdateIcons(int dpi)
		{
			var iconSize = dpi * 16 / 96;

		}

		protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
		{
			base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
			UpdateIcons(deviceDpiNew);
		}

		private void OnCurrentDirectoryChanged(object sender, EventArgs e)
		{
			BuildPath();
		}
	}
}

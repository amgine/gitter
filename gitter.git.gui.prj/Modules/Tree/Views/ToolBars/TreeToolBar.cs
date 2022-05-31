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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class TreeToolbar : ToolStrip
{
	#region Data

	private readonly TreeView _treeView;
	private readonly DpiBindings _dpiBindings;

	#endregion

	private void BuildPath()
	{
		this.DisableRedraw();
		while(Items.Count != 2)
		{
			var item = Items[2];
			_dpiBindings.UnbindImage(item);
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
				var item = new ToolStripSplitButton(folder.Name, null)
				{
					DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
					Tag = folder,
				};
				_dpiBindings.BindImage(item, Icons.Folder);
				if(folder.Directories.Count != 0)
				{
					foreach(var subFolder in folder.Directories)
					{
						var subItem = new ToolStripMenuItem(subFolder.Name, null)
						{
							Tag = subFolder,
							Checked = subFolder == prev,
						};
						_dpiBindings.BindImage(subItem, Icons.Folder);
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
		Verify.Argument.IsNotNull(treeView);

		_treeView = treeView;
		_treeView.CurrentDirectoryChanged += OnCurrentDirectoryChanged;

		_dpiBindings = new DpiBindings(this);

		var up = new ToolStripButton(Resources.StrGoUpOneLevel, null, (_, _) =>
		{
			var cd = _treeView.CurrentDirectory;
			cd = cd.Parent;
			if(cd is not null) _treeView.CurrentDirectory = cd;
		})
		{
			DisplayStyle = ToolStripItemDisplayStyle.Image,
		};
		_dpiBindings.BindImage(up, Icons.FolderUp);
		Items.Add(up);
		Items.Add(new ToolStripSeparator());
	}

	private void OnCurrentDirectoryChanged(object sender, EventArgs e)
	{
		BuildPath();
	}
}

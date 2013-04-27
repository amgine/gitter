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
	using System.Drawing;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public class RemoteReferencesListBox : CustomListBox
	{
		private Remote _remote;
		private RemoteReferencesCollection _refs;

		private sealed class GroupItem : CustomListBoxItem<string>
		{
			private static readonly Bitmap ImgFolder = CachedResources.Bitmaps["ImgFolder"];

			public GroupItem(string name)
				: base(name)
			{
			}

			protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			{
				if(measureEventArgs.ColumnIndex == 0)
				{
					return measureEventArgs.MeasureImageAndText(ImgFolder, DataContext);
				}
				return Size.Empty;
			}

			protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
			{
				if(paintEventArgs.ColumnIndex == 0)
				{
					paintEventArgs.PaintImageAndText(ImgFolder, DataContext);
				}
			}
		}

		private readonly GroupItem _grpBranches;
		private readonly GroupItem _grpTags;

		public RemoteReferencesListBox()
		{
			Columns.Add(new NameColumn());
			Columns.Add(new HashColumn() { IsVisible = true, Abbreviate = true, Width = 60 });

			_grpBranches = new GroupItem(Resources.StrBranches) { IsExpanded = true };
			_grpTags = new GroupItem(Resources.StrTags) { IsExpanded = true };

			ShowTreeLines = true;
		}

		public void Load(Remote remote)
		{
			if(_remote == remote) return;
			if(_remote != null)
			{
				DetachFromRemote();
			}
			_remote = remote;
			if(_remote != null)
			{
				AttachToRemote();
			}
		}

		private void DetachFromRemote()
		{
			_refs.BranchCreated -= OnBranchCreated;
			_refs.TagCreated -= OnTagCreated;
			_refs = null;
			Items.ClearSafe();
		}

		private void AttachToRemote()
		{
			_refs = _remote.GetReferences();
			_refs.BranchCreated += OnBranchCreated;
			_refs.TagCreated += OnTagCreated;
		}

		public void FetchData()
		{
			_refs.Refresh();
			if(_grpBranches.ListBox == null)
			{
				Items.AddSafe(_grpBranches);
			}
			if(_grpTags.ListBox == null)
			{
				Items.AddSafe(_grpTags);
			}
		}

		private void OnBranchCreated(object sender, RemoteReferenceEventArgs e)
		{
			_grpBranches.Items.AddSafe(new RemoteReferenceListItem(e.Reference));
		}

		private void OnTagCreated(object sender, RemoteReferenceEventArgs e)
		{
			_grpTags.Items.AddSafe(new RemoteReferenceListItem(e.Reference));
		}
	}
}

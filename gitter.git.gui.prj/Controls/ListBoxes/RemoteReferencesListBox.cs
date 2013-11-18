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

		#region Data

		private RemoteReferencesCollection _remoteReferences;
		private readonly GroupItem _grpBranches;
		private readonly GroupItem _grpTags;

		#endregion

		#region .ctor

		public RemoteReferencesListBox()
		{
			Columns.Add(new NameColumn());
			Columns.Add(new HashColumn() { IsVisible = true, Abbreviate = true, Width = 60 });

			_grpBranches = new GroupItem(Resources.StrHeads) { IsExpanded = true };
			_grpTags     = new GroupItem(Resources.StrTags) { IsExpanded = true };

			ShowTreeLines = true;
		}

		#endregion

		#region Properties

		public RemoteReferencesCollection RemoteReferences
		{
			get { return _remoteReferences; }
			set
			{
				if(_remoteReferences != value)
				{
					if(_remoteReferences != null)
					{
						lock(_remoteReferences.SyncRoot)
						{
							_remoteReferences.BranchCreated -= OnBranchCreated;
							_remoteReferences.TagCreated -= OnTagCreated;
							if(_grpBranches != null)
							{
								_grpBranches.Items.ClearSafe();
							}
							if(_grpTags != null)
							{
								_grpTags.Items.ClearSafe();
							}
							Items.ClearSafe();
						}
					}
					_remoteReferences = value;
					if(_remoteReferences != null)
					{
						lock(_remoteReferences.SyncRoot)
						{
							foreach(var branch in _remoteReferences.Branches)
							{
								_grpBranches.Items.AddSafe(new RemoteReferenceListItem(branch));
							}
							foreach(var tag in _remoteReferences.Tags)
							{
								_grpTags.Items.AddSafe(new RemoteReferenceListItem(tag));
							}
							if(_grpBranches.ListBox == null)
							{
								Items.AddSafe(_grpBranches);
							}
							if(_grpTags.ListBox == null)
							{
								Items.AddSafe(_grpTags);
							}
							_remoteReferences.BranchCreated += OnBranchCreated;
							_remoteReferences.TagCreated += OnTagCreated;
						}
					}
				}
			}
		}

		#endregion

		#region Methods

		private void OnBranchCreated(object sender, RemoteReferenceEventArgs e)
		{
			_grpBranches.Items.AddSafe(new RemoteReferenceListItem(e.Reference));
		}

		private void OnTagCreated(object sender, RemoteReferenceEventArgs e)
		{
			_grpTags.Items.AddSafe(new RemoteReferenceListItem(e.Reference));
		}

		#endregion
	}
}

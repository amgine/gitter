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
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class PathHistoryToolbar : ToolStrip
	{
		#region Data

		private readonly PathHistoryView _view;
		private ToolStripButton _btnShowDetails;
		private ToolStripButton _btnRefresh;
		private ToolStripButton _btnDateOrder;
		private ToolStripButton _btnTopoOrder;
		private ToolStripDropDownButton _btnLimit;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="PathHistoryToolbar"/> class.</summary>
		/// <param name="view">Host history view.</param>
		public PathHistoryToolbar(PathHistoryView view)
		{
			_view = view;

			_view.LogOptionsChanged += OnLogOptionsChanged;

			Items.AddRange(
				new ToolStripItem[]
				{
					// left-aligned
					_btnRefresh = new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"], OnRefreshButtonClick)
						{
							DisplayStyle = ToolStripItemDisplayStyle.Image,
						},
					new ToolStripSeparator(),
					_btnDateOrder = new ToolStripButton(Resources.StrDateOrder, CachedResources.Bitmaps["ImgDateOrder"], OnDateOrderButtonClick)
						{
							Checked = _view.LogOptions.Order == RevisionQueryOrder.DateOrder,
							DisplayStyle = ToolStripItemDisplayStyle.Image,
						},
					_btnTopoOrder = new ToolStripButton(Resources.StrTopoOrder, CachedResources.Bitmaps["ImgTopoOrder"], OnTopoOrderButtonClick)
						{
							Checked = _view.LogOptions.Order == RevisionQueryOrder.TopoOrder,
							DisplayStyle = ToolStripItemDisplayStyle.Image,
						},
					new ToolStripSeparator(),
					_btnLimit = new ToolStripDropDownButton(string.Empty, null,
						new ToolStripItem[]
						{
							new ToolStripMenuItem(Resources.StrlUnlimited, null, OnLimitOptionClick) { Tag = 0 },
							new ToolStripMenuItem( "100 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 100 },
							new ToolStripMenuItem( "500 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 500 },
							new ToolStripMenuItem("1000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 1000 },
							new ToolStripMenuItem("2000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 2000 },
							new ToolStripMenuItem("5000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 5000 },
						}),
					// right-aligned
					_btnShowDetails = new ToolStripButton(Resources.StrAutoShowDiff, CachedResources.Bitmaps["ImgDiff"], OnShowDetailsButtonClick)
						{
							Checked = _view.ShowDetails,
							DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
							Alignment = ToolStripItemAlignment.Right,
						}
				});
			UpdateLimitButtonText();
		}

		private void OnRefreshButtonClick(object sender, EventArgs e)
		{
			_view.RefreshContent();
		}

		private void OnDateOrderButtonClick(object sender, EventArgs e)
		{
			_view.LogOptions.Order = RevisionQueryOrder.DateOrder;
		}

		private void OnTopoOrderButtonClick(object sender, EventArgs e)
		{
			_view.LogOptions.Order = RevisionQueryOrder.TopoOrder;
		}

		private void OnShowDetailsButtonClick(object sender, EventArgs e)
		{
			var button = (ToolStripButton)sender;
			button.Checked = !button.Checked;
			_view.ShowDetails = button.Checked;
		}

		private void OnLogOptionsChanged(object sender, EventArgs e)
		{
			_btnDateOrder.Checked = _view.LogOptions.Order == RevisionQueryOrder.DateOrder;
			_btnTopoOrder.Checked = _view.LogOptions.Order == RevisionQueryOrder.TopoOrder;
			UpdateLimitButtonText();
		}

		private void OnLimitOptionClick(object sender, EventArgs e)
		{
			_view.LogOptions.MaxCount = (int)((ToolStripItem)sender).Tag;
		}

		private void UpdateLimitButtonText()
		{
			foreach(ToolStripMenuItem item in _btnLimit.DropDownItems)
			{
				if(item.Tag is int && (int)item.Tag == _view.LogOptions.MaxCount)
				{
					_btnLimit.Text = item.Text;
					item.Checked = true;
				}
				else
				{
					item.Checked = false;
				}
			}
		}

		public ToolStripButton RefreshButton
		{
			get { return _btnRefresh; }
		}

		public ToolStripButton ShowDiffButton
		{
			get { return _btnShowDetails; }
		}
	}
}

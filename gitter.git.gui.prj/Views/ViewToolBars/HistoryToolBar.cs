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
	using System.Text;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	internal sealed class HistoryToolbar : ToolStrip
	{
		#region Data

		private readonly HistoryView _view;
		private ToolStripButton _btnShowDetails;
		private ToolStripButton _btnRefresh;
		private ToolStripButton _btnDateOrder;
		private ToolStripButton _btnTopoOrder;
		private ToolStripDropDownButton _btnLimit;
		private ToolStripDropDownButton _btnFilter;
		private HistoryFilterDropDown _filterDropDown;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="HistoryToolbar"/> class.</summary>
		/// <param name="view">Host history view.</param>
		public HistoryToolbar(HistoryView view)
		{
			Verify.Argument.IsNotNull(view, "view");

			_view = view;

			_view.LogOptionsChanged += OnLogOptionsChanged;
			_view.RepositoryChanged += OnRepositoryChanged;

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
					_btnFilter = new ToolStripDropDownButton(GetFilterButtonText(), CachedResources.Bitmaps["ImgFilter"])
						{
							DropDown = new Popup(
								_filterDropDown = new HistoryFilterDropDown()
								{
									LogOptions = _view.LogOptions,
									Repository = _view.Repository,
								})
								{
									Resizable = false,
								},
							ToolTipText = Resources.StrFilter,
						},
					_btnLimit = new ToolStripDropDownButton(string.Empty, null,
						new ToolStripItem[]
						{
							new ToolStripMenuItem(Resources.StrlUnlimited, null, OnLimitOptionClick) { Tag = 0 },
							new ToolStripMenuItem( "100 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 100 },
							new ToolStripMenuItem( "500 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 500 },
							new ToolStripMenuItem("1000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 1000 },
							new ToolStripMenuItem("2000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 2000 },
							new ToolStripMenuItem("5000 " + Resources.StrlCommits, null, OnLimitOptionClick) { Tag = 5000 },
						})
						{
							ToolTipText = Resources.StrsCommitLimit,
						},
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

		private void OnRepositoryChanged(object sender, EventArgs e)
		{
			_filterDropDown.Repository = _view.Repository;
		}

		private void OnLogOptionsChanged(object sender, EventArgs e)
		{
			_filterDropDown.LogOptions = _view.LogOptions;
			_btnDateOrder.Checked = _view.LogOptions.Order == RevisionQueryOrder.DateOrder;
			_btnTopoOrder.Checked = _view.LogOptions.Order == RevisionQueryOrder.TopoOrder;
			UpdateLimitButtonText();
			_btnFilter.Text = GetFilterButtonText();
		}

		private void OnLimitOptionClick(object sender, EventArgs e)
		{
			_view.LogOptions.MaxCount = (int)((ToolStripItem)sender).Tag;
		}

		private string GetFilterButtonText()
		{
			switch(_view.LogOptions.Filter)
			{
				case LogReferenceFilter.All:
					return Resources.StrAll;
				case LogReferenceFilter.HEAD:
					return GitConstants.HEAD;
				case LogReferenceFilter.Allowed:
					StringBuilder sb = null;
					if(_view.LogOptions.AllowedReferences != null)
					{
						int count = 0;
						foreach(var reference in _view.LogOptions.AllowedReferences)
						{
							if(sb == null)
							{
								sb = new StringBuilder(reference.Name);
							}
							else
							{
								if(count < 3)
								{
									sb.Append(", ");
									sb.Append(reference.Name);
								}
								else
								{
									sb.Append(", ...");
									break;
								}
							}
							++count;
						}
					}
					if(sb == null)
					{
						return GitConstants.HEAD;
					}
					else
					{
						return sb.ToString();
					}
				default:
					return Resources.StrFilter;
			}
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

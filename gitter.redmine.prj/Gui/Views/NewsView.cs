#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Globalization;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using gitter.Redmine.Gui.ListBoxes;

	using Resources = gitter.Redmine.Properties.Resources;

	partial class NewsView : RedmineViewBase, ISearchableView<NewsSearchOptions>
	{
		#region Data

		private readonly NewsToolbar _toolbar;
		private NewsSearchToolBar _searchToolbar;
		private NewsListBinding _dataSource;

		#endregion

		#region .ctor

		public NewsView(IWorkingEnvironment environment)
			: base(Guids.NewsViewGuid, environment)
		{
			InitializeComponent();

			Text = Resources.StrNews;

			AddTopToolStrip(_toolbar = new NewsToolbar(this));

			_lstNews.ItemActivated += OnItemActivated;
			_lstNews.PreviewKeyDown += OnKeyDown;
		}

		#endregion

		#region Properties

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgNews"]; }
		}

		private NewsListBinding DataSource
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

		#endregion

		#region Methods

		protected override void OnContextAttached()
		{
			DataSource = new NewsListBinding(ServiceContext, _lstNews);
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as NewsListItem;
			if(item != null)
			{
				ShowNewsDetails(item.DataContext);
			}
		}

		private void ShowNewsDetails(News news)
		{
			var url = ServiceContext.ServiceUri + "news/" + news.Id;
			RedmineServiceProvider.Environment.ViewDockService.ShowWebBrowserView(url);
		}

		protected override void SaveMoreViewTo(Section section)
		{
			var listNode = section.GetCreateSection("NewsList");
			_lstNews.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			var listNode = section.TryGetSection("NewsList");
			if(listNode != null)
			{
				_lstNews.LoadViewFrom(listNode);
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.F:
					if(e.Modifiers == Keys.Control)
					{
						ShowSearchToolBar();
						e.IsInputKey = true;
					}
					break;
				case Keys.F5:
					RefreshContent();
					break;
			}
		}

		public override void RefreshContent()
		{
			if(DataSource != null)
			{
				DataSource.ReloadData();
			}
		}

		#endregion

		#region ISearchableView

		private bool TestItem(NewsListItem item, NewsSearchOptions search)
		{
			var news = item.DataContext;
			if(news.Title.Contains(search.Text)) return true;
			int id;
			if(int.TryParse(search.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
			{
				if(news.Id == id) return true;
			}
			return false;
		}

		private bool Search(int start, NewsSearchOptions search, int direction)
		{
			if(search.Text.Length == 0) return true;
			int count = _lstNews.Items.Count;
			if(count == 0) return false;
			int end;
			if(direction == 1)
			{
				start = (start + 1) % count;
				end = start - 1;
				if(end < 0) end += count;
			}
			else
			{
				start = (start - 1);
				if(start < 0) start += count;
				end = (start + 1) % count;
			}
			while(start != end)
			{
				var item = _lstNews.Items[start] as NewsListItem;
				if(item != null)
				{
					if(TestItem(item, search))
					{
						item.FocusAndSelect();
						return true;
					}
				}
				if(direction == 1)
				{
					start = (start + 1) % count;
				}
				else
				{
					--start;
					if(start < 0) start = count - 1;
				}
			}
			return false;
		}

		public bool SearchFirst(NewsSearchOptions search)
		{
			Verify.Argument.IsNotNull(search, "search");

			return Search(-1, search, 1);
		}

		public bool SearchNext(NewsSearchOptions search)
		{
			Verify.Argument.IsNotNull(search, "search");

			if(search.Text.Length == 0) return true;
			if(_lstNews.SelectedItems.Count == 0)
			{
				return Search(-1, search, 1);
			}
			var start = _lstNews.Items.IndexOf(_lstNews.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool SearchPrevious(NewsSearchOptions search)
		{
			Verify.Argument.IsNotNull(search, "search");

			if(search.Text.Length == 0) return true;
			if(_lstNews.SelectedItems.Count == 0) return Search(-1, search, 1);
			var start = _lstNews.Items.IndexOf(_lstNews.SelectedItems[0]);
			return Search(start, search, -1);
		}

		public bool SearchToolBarVisible
		{
			get { return _searchToolbar != null && _searchToolbar.Visible; }
			set
			{
				if(value)
				{
					ShowSearchToolBar();
				}
				else
				{
					HideSearchToolBar();
				}
			}
		}

		private void ShowSearchToolBar()
		{
			if(_searchToolbar == null)
			{
				AddBottomToolStrip(_searchToolbar = new NewsSearchToolBar(this));
			}
			_searchToolbar.FocusSearchTextBox();
		}

		private void HideSearchToolBar()
		{
			if(_searchToolbar != null)
			{
				RemoveToolStrip(_searchToolbar);
				_searchToolbar.Dispose();
				_searchToolbar = null;
			}
		}

		#endregion
	}
}

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

	using Resources = gitter.Redmine.Properties.Resources;

	partial class NewsView : RedmineViewBase, ISearchableView<NewsSearchOptions>
	{
		private readonly NewsToolbar _toolbar;
		private NewsSearchToolBar _searchToolbar;
		private volatile AsyncFunc<RedmineServiceContext, LinkedList<News>> _currentLookup;

		public NewsView(IDictionary<string, object> parameters)
			: base(Guids.NewsViewGuid, parameters)
		{
			InitializeComponent();

			Text = Resources.StrNews;

			AddTopToolStrip(_toolbar = new NewsToolbar(this));

			_lstNews.ItemActivated += OnItemActivated;
			_lstNews.PreviewKeyDown += OnKeyDown;
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgNews"]; }
		}

		protected override void OnContextAttached()
		{
			RefreshContent();
		}

		private void OnFetchCompleted(IAsyncResult ar)
		{
			var af = (AsyncFunc<RedmineServiceContext, LinkedList<News>>)ar.AsyncState;
			if(_currentLookup == af)
			{
				_currentLookup = null;
				IEnumerable<News> news = null;
				try
				{
					news = af.EndInvoke(ar);
				}
				catch(Exception exc)
				{
					if(!IsDisposed)
					{
						try
						{
							BeginInvoke(new Action<Exception>(ShowErrorNotification), exc);
						}
						catch { }
					}
				}
				if(news != null)
				{
					if(!IsDisposed)
					{
						try
						{
							BeginInvoke(new Action<IEnumerable<News>>(ShowNews), news);
						}
						catch { }
					}
				}
			}
		}

		private void ShowErrorNotification(Exception exc)
		{
			if(IsDisposed) return;
			_lstNews.Text = Resources.StrsFailedToFetchNews;
		}

		private void ShowNews(IEnumerable<News> news)
		{
			if(IsDisposed) return;
			_lstNews.BeginUpdate();
			_lstNews.Items.Clear();
			foreach(var n in news)
			{
				var item = new NewsListItem(n);
				_lstNews.Items.Add(item);
			}
			if(_lstNews.Items.Count == 0)
			{
				_lstNews.Text = Resources.StrsNoNewsToDisplay;
			}
			_lstNews.EndUpdate();
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as NewsListItem;
			if(item != null)
			{
				ShowNewsDetails(item.Data);
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
			if(ServiceContext == null)
			{
				_lstNews.Items.Clear();
			}
			else
			{
				if(_currentLookup != null)
				{
					return;
				}
				var af = AsyncFunc.Create(
					ServiceContext,
					(context, mon) => context.News.Fetch(context.DefaultProjectId),
					Resources.StrsFetchingNews.AddEllipsis(),
					string.Empty);
				_currentLookup = af;
				af.BeginInvoke(this, _lstNews.ProgressMonitor, OnFetchCompleted, af);
			}
		}

		#region ISearchableView

		private bool TestItem(NewsListItem item, NewsSearchOptions search)
		{
			var news = item.Data;
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
			if(search == null) throw new ArgumentNullException("search");

			return Search(-1, search, 1);
		}

		public bool SearchNext(NewsSearchOptions search)
		{
			if(search == null) throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstNews.SelectedItems.Count == 0)
				return Search(-1, search, 1);
			var start = _lstNews.Items.IndexOf(_lstNews.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool SearchPrevious(NewsSearchOptions search)
		{
			if(search == null) throw new ArgumentNullException("search");

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

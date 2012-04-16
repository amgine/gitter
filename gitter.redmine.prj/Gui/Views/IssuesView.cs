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

	partial class IssuesView : RedmineViewBase, ISearchableView<IssuesSearchOptions>
	{
		private readonly IssuesToolbar _toolbar;
		private IssuesSearchToolBar _searchToolbar;
		private volatile AsyncFunc<RedmineServiceContext, LinkedList<Issue>> _currentLookup;

		public IssuesView(IWorkingEnvironment environment, IDictionary<string, object> parameters)
			: base(Guids.IssuesViewGuid, environment, parameters)
		{
			InitializeComponent();

			Text = Resources.StrIssues;

			AddTopToolStrip(_toolbar = new IssuesToolbar(this));

			//_lstIssues.ShowTreeLines = true;
			_lstIssues.ItemActivated += OnItemActivated;
			_lstIssues.PreviewKeyDown += OnKeyDown;
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgBug"]; }
		}

		protected override void OnContextAttached()
		{
			RefreshContent();
		}

		private void OnFetchCompleted(IAsyncResult ar)
		{
			var af = (AsyncFunc<RedmineServiceContext, LinkedList<Issue>>)ar.AsyncState;
			if(_currentLookup == af)
			{
				_currentLookup = null;
				LinkedList<Issue> issues = null;
				try
				{
					issues = af.EndInvoke(ar);
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
				if(issues != null)
				{
					if(!IsDisposed)
					{
						try
						{
							BeginInvoke(new Action<LinkedList<Issue>>(ShowIssues), issues);
						}
						catch { }
					}
				}
			}
		}

		private void ShowErrorNotification(Exception exc)
		{
			if(IsDisposed) return;
			_lstIssues.Text = Resources.StrsFailedToFetchIssues;
		}

		private void ShowIssues(LinkedList<Issue> issues)
		{
			if(IsDisposed) return;
			_lstIssues.BeginUpdate();
			_lstIssues.Items.Clear();
			if(issues.Count != 0)
			{
				/*
				var d = new Dictionary<Issue, IssueListItem>();
				foreach(var issue in issues)
				{
					var item = new IssueListItem(issue);
					d[issue] = item;
				}
				foreach(var kvp in d)
				{
					if(kvp.Key.Parent != null)
					{
						IssueListItem parent;
						if(d.TryGetValue(kvp.Key.Parent, out parent))
						{
							parent.Expand();
							parent.Items.Add(kvp.Value);
						}
					}
				}
				foreach(var issue in issues)
				{
					var item = d[issue];
					if(item.ListBox != null) continue;
					if(item.Parent == null)
					{
						_lstIssues.Items.Add(item);
					}
				}
				*/
				var cf = new Dictionary<int, CustomListBoxColumn>();
				foreach(var column in _lstIssues.Columns)
				{
					if(column.Id >= (int)ColumnId.CustomFieldOffset)
					{
						var id = column.Id - (int)ColumnId.CustomFieldOffset;
						cf.Add(id, column);
					}
				}
				foreach(var issue in issues)
				{
					foreach(var cfv in issue.CustomFields)
					{
						if(!cf.ContainsKey(cfv.Field.Id))
						{
							var column = new IssueCustomFieldColumn(cfv.Field);
							cf.Add(cfv.Field.Id, column);
							_lstIssues.Columns.Add(column);
						}
					}
					_lstIssues.Items.Add(new IssueListItem(issue));
				}
			}
			else
			{
				_lstIssues.Text = Resources.StrsNoIssuesToDisplay;
			}
			_lstIssues.EndUpdate();
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as IssueListItem;
			if(item != null)
			{
				ShowIssueDetails(item.Data);
			}
		}

		private void ShowIssueDetails(Issue issue)
		{
			var url = ServiceContext.ServiceUri + "issues/" + issue.Id;
			RedmineServiceProvider.Environment.ViewDockService.ShowWebBrowserView(url);
		}

		protected override void SaveMoreViewTo(Section section)
		{
			var listNode = section.GetCreateSection("IssuesList");
			_lstIssues.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			var listNode = section.TryGetSection("IssuesList");
			if(listNode != null)
			{
				_lstIssues.LoadViewFrom(listNode);
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
				_lstIssues.Items.Clear();
			}
			else
			{
				if(_currentLookup != null)
				{
					return;
				}
				var af = AsyncFunc.Create(
					ServiceContext,
					(context, mon) => context.Issues.FetchOpen(context.DefaultProjectId),
					Resources.StrsFetchingIssues.AddEllipsis(),
					string.Empty);
				_currentLookup = af;
				af.BeginInvoke(this, _lstIssues.ProgressMonitor, OnFetchCompleted, af);
			}
		}

		#region ISearchableView

		private bool TestItem(IssueListItem item, IssuesSearchOptions search)
		{
			var issue = item.Data;
			if(issue.Subject.Contains(search.Text)) return true;
			int id;
			if(int.TryParse(search.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
			{
				if(issue.Id == id) return true;
			}
			return false;
		}

		private bool Search(int start, IssuesSearchOptions search, int direction)
		{
			if(search.Text.Length == 0) return true;
			int count = _lstIssues.Items.Count;
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
				var item = _lstIssues.Items[start] as IssueListItem;
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

		public bool SearchFirst(IssuesSearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			return Search(-1, search, 1);
		}

		public bool SearchNext(IssuesSearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstIssues.SelectedItems.Count == 0)
				return Search(-1, search, 1);
			var start = _lstIssues.Items.IndexOf(_lstIssues.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool SearchPrevious(IssuesSearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstIssues.SelectedItems.Count == 0) return Search(-1, search, 1);
			var start = _lstIssues.Items.IndexOf(_lstIssues.SelectedItems[0]);
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
				AddBottomToolStrip(_searchToolbar = new IssuesSearchToolBar(this));
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

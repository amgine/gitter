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

namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Globalization;
	using System.Windows.Forms;
	
	using gitter.Framework;
	using gitter.Framework.Configuration;
	using gitter.Framework.Controls;

	using gitter.Redmine.Gui.ListBoxes;

	using Resources = gitter.Redmine.Properties.Resources;

	partial class IssuesView : RedmineViewBase, ISearchableView<IssuesSearchOptions>
	{
		#region Data

		private readonly IssuesToolbar _toolbar;
		private IssuesSearchToolBar _searchToolbar;
		private IssuesListBinding _dataSource;

		#endregion

		#region .ctor

		public IssuesView(IWorkingEnvironment environment)
			: base(Guids.IssuesViewGuid, environment)
		{
			InitializeComponent();

			Text = Resources.StrIssues;

			AddTopToolStrip(_toolbar = new IssuesToolbar(this));

			_lstIssues.ItemActivated += OnItemActivated;
			_lstIssues.PreviewKeyDown += OnKeyDown;
		}

		#endregion

		#region Properties

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgBug"]; }
		}

		private IssuesListBinding DataSource
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
			DataSource = new IssuesListBinding(ServiceContext, _lstIssues);
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as IssueListItem;
			if(item != null)
			{
				ShowIssueDetails(item.DataContext);
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
			if(DataSource != null)
			{
				DataSource.ReloadData();
			}
		}

		#endregion

		#region ISearchableView

		private bool TestItem(IssueListItem item, IssuesSearchOptions search)
		{
			var issue = item.DataContext;
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
			Verify.Argument.IsNotNull(search, "search");

			return Search(-1, search, 1);
		}

		public bool SearchNext(IssuesSearchOptions search)
		{
			Verify.Argument.IsNotNull(search, "search");

			if(search.Text.Length == 0) return true;
			if(_lstIssues.SelectedItems.Count == 0)
			{
				return Search(-1, search, 1);
			}
			var start = _lstIssues.Items.IndexOf(_lstIssues.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool SearchPrevious(IssuesSearchOptions search)
		{
			Verify.Argument.IsNotNull(search, "search");

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

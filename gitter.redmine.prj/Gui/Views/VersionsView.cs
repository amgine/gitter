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

	partial class VersionsView : RedmineViewBase, ISearchableView<VersionsSearchOptions>
	{
		private readonly VersionsToolbar _toolbar;
		private VersionsSearchToolBar _searchToolbar;
		private volatile AsyncFunc<RedmineServiceContext, LinkedList<ProjectVersion>> _currentLookup;

		public VersionsView(IWorkingEnvironment environment, IDictionary<string, object> parameters)
			: base(Guids.VersionsViewGuid, environment, parameters)
		{
			InitializeComponent();

			Text = Resources.StrVersions;

			AddTopToolStrip(_toolbar = new VersionsToolbar(this));

			_lstVersions.ItemActivated += OnItemActivated;
			_lstVersions.PreviewKeyDown += OnKeyDown;
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgVersion"]; }
		}

		protected override void OnContextAttached()
		{
			RefreshContent();
		}

		private void OnFetchCompleted(IAsyncResult ar)
		{
			var af = (AsyncFunc<RedmineServiceContext, LinkedList<ProjectVersion>>)ar.AsyncState;
			if(_currentLookup == af)
			{
				_currentLookup = null;
				IEnumerable<ProjectVersion> versions = null;
				try
				{
					versions = af.EndInvoke(ar);
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
				if(versions != null)
				{
					if(!IsDisposed)
					{
						try
						{
							BeginInvoke(new Action<IEnumerable<ProjectVersion>>(ShowVersions), versions);
						}
						catch { }
					}
				}
			}
		}

		private void ShowErrorNotification(Exception exc)
		{
			if(IsDisposed) return;
			_lstVersions.Text = Resources.StrsFailedToFetchVersions;
		}

		private void ShowVersions(IEnumerable<ProjectVersion> versions)
		{
			if(IsDisposed) return;
			_lstVersions.BeginUpdate();
			_lstVersions.Items.Clear();
			foreach(var version in versions)
			{
				var item = new VersionListItem(version);
				_lstVersions.Items.Add(item);
			}
			if(_lstVersions.Items.Count == 0)
			{
				_lstVersions.Text = Resources.StrsNoVersionsToDisplay;
			}
			_lstVersions.EndUpdate();
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as VersionListItem;
			if(item != null)
			{
				ShowVersionDetails(item.DataContext);
			}
		}

		private void ShowVersionDetails(ProjectVersion version)
		{
			var url = ServiceContext.ServiceUri + "versions/" + version.Id;
			RedmineServiceProvider.Environment.ViewDockService.ShowWebBrowserView(url);
		}

		protected override void SaveMoreViewTo(Section section)
		{
			var listNode = section.GetCreateSection("VersionsList");
			_lstVersions.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			var listNode = section.TryGetSection("VersionsList");
			if(listNode != null)
			{
				_lstVersions.LoadViewFrom(listNode);
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
				_lstVersions.Items.Clear();
			}
			else
			{
				if(_currentLookup != null)
				{
					return;
				}
				var af = AsyncFunc.Create(
					ServiceContext,
					(context, mon) => context.ProjectVersions.Fetch(context.DefaultProjectId),
					Resources.StrsFetchingVersions.AddEllipsis(),
					string.Empty);
				_currentLookup = af;
				af.BeginInvoke(this, _lstVersions.ProgressMonitor, OnFetchCompleted, af);
			}
		}

		#region ISearchableView

		private bool TestItem(VersionListItem item, VersionsSearchOptions search)
		{
			var version = item.DataContext;
			if(version.Name.Contains(search.Text)) return true;
			if(version.Description.Contains(search.Text)) return true;
			int id;
			if(int.TryParse(search.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
			{
				if(version.Id == id) return true;
			}
			return false;
		}

		private bool Search(int start, VersionsSearchOptions search, int direction)
		{
			if(search.Text.Length == 0) return true;
			int count = _lstVersions.Items.Count;
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
				var item = _lstVersions.Items[start] as VersionListItem;
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

		public bool SearchFirst(VersionsSearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			return Search(-1, search, 1);
		}

		public bool SearchNext(VersionsSearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstVersions.SelectedItems.Count == 0)
				return Search(-1, search, 1);
			var start = _lstVersions.Items.IndexOf(_lstVersions.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool SearchPrevious(VersionsSearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstVersions.SelectedItems.Count == 0) return Search(-1, search, 1);
			var start = _lstVersions.Items.IndexOf(_lstVersions.SelectedItems[0]);
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
				AddBottomToolStrip(_searchToolbar = new VersionsSearchToolBar(this));
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

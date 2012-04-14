namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	partial class ReferencesView : GitViewBase, ISearchableView<SearchOptions>
	{
		private ReferencesToolbar _toolbar;
		private ReferencesSearchToolBar _searchToolbar;

		public ReferencesView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.ReferencesViewGuid, parameters, gui)
		{
			InitializeComponent();
			_lstReferences.Columns.ShowAll((c) => c.Id != (int)ColumnId.TreeHash);
			_lstReferences.PreviewKeyDown += OnKeyDown;

			Text = Resources.StrReferences;

			AddTopToolStrip(_toolbar = new ReferencesToolbar(this));
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgBranch"]; }
		}

		protected override void AttachToRepository(Repository repository)
		{
			_lstReferences.LoadData(Repository);
			_lstReferences.Items[0].IsExpanded = true;
			_lstReferences.Items[1].ExpandAll();
			_lstReferences.Items[2].IsExpanded = true;
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_lstReferences.LoadData(null);
		}

		public IRevisionPointer SelectedReference
		{
			get
			{
				if(_lstReferences.SelectedItems.Count == 0) return null;
				var branchItem = _lstReferences.SelectedItems[0] as BranchListItem;
				if(branchItem != null) return branchItem.Data;
				var tagItem = _lstReferences.SelectedItems[0] as TagListItem;
				if(tagItem != null) return tagItem.Data;
				return null;
			}
		}

		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(RefreshContent));
			}
			else
			{
				if(Repository != null)
				{
					Cursor = Cursors.WaitCursor;
					_lstReferences.BeginUpdate();
					Repository.Refs.Refresh();
					_lstReferences.EndUpdate();
					Cursor = Cursors.Default;
				}
			}
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.F:
					if(e.Modifiers == Keys.Control)
					{
						/*
						ShowSearchToolBar();
						e.IsInputKey = true;
						*/
					}
					break;
				case Keys.F5:
					RefreshContent();
					e.IsInputKey = true;
					break;
			}
		}

		private bool TestItem(IRevisionPointerListItem item, SearchOptions search)
		{
			var rev = item.RevisionPointer;
			if(rev.FullName.Contains(search.Text)) return true;
			if(rev.Dereference().Name.Contains(search.Text)) return true;
			return false;
		}

		private bool Search(int start, SearchOptions search, int direction)
		{
			if(search.Text.Length == 0) return true;
			int count = _lstReferences.Items.Count;
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
				var item = _lstReferences.Items[start];
				var revItem = item as IRevisionPointerListItem;
				if(revItem != null)
				{
					if(TestItem(revItem, search))
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

		public bool SearchFirst(SearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			return Search(-1, search, 1);
		}

		public bool SearchNext(SearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstReferences.SelectedItems.Count == 0)
				return Search(-1, search, 1);
			var start = _lstReferences.Items.IndexOf(_lstReferences.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool SearchPrevious(SearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstReferences.SelectedItems.Count == 0) return Search(-1, search, 1);
			var start = _lstReferences.Items.IndexOf(_lstReferences.SelectedItems[0]);
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
				AddBottomToolStrip(_searchToolbar = new ReferencesSearchToolBar(this));
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

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var listNode = section.GetCreateSection("ReferenceList");
			_lstReferences.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("ReferenceList");
			if(listNode != null)
				_lstReferences.LoadViewFrom(listNode);
		}
	}
}

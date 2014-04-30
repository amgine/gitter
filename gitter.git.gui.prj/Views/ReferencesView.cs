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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class ReferencesView : GitViewBase //, ISearchableView<SearchOptions>
	{
		private ReferencesToolbar _toolbar;
		//private ReferencesSearchToolBar _searchToolbar;

		public ReferencesView(GuiProvider gui)
			: base(Guids.ReferencesViewGuid, gui)
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
				if(branchItem != null) return branchItem.DataContext;
				var tagItem = _lstReferences.SelectedItems[0] as TagListItem;
				if(tagItem != null) return tagItem.DataContext;
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
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						_lstReferences.BeginUpdate();
						Repository.Refs.Refresh();
						_lstReferences.EndUpdate();
					}
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
			if(rev.Dereference().HashString.Contains(search.Text)) return true;
			return false;
		}

		/*
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
			Verify.Argument.IsNotNull(search, "search");

			return Search(-1, search, 1);
		}

		public bool SearchNext(SearchOptions search)
		{
			Verify.Argument.IsNotNull(search, "search");

			if(search.Text.Length == 0) return true;
			if(_lstReferences.SelectedItems.Count == 0)
			{
				return Search(-1, search, 1);
			}
			var start = _lstReferences.Items.IndexOf(_lstReferences.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool SearchPrevious(SearchOptions search)
		{
			Verify.Argument.IsNotNull(search, "search");

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
		*/

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
			{
				_lstReferences.LoadViewFrom(listNode);
			}
		}
	}
}

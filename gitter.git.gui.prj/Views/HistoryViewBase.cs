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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	abstract class HistoryViewBase : GitViewBase, ISearchableView<HistorySearchOptions>
	{
		#region Data

		private RevisionListBox _lstRevisions;
		private ILogSource _logSource;
		private LogOptions _options;
		private RevisionLogBinding _dataSource;
		private HistorySearchToolBar<HistoryViewBase> _searchToolbar;
		private ISearch<HistorySearchOptions> _search;
		private bool _showDetails;

		#endregion

		#region Events

		private static readonly object LogOptionsChangedEvent = new object();

		public event EventHandler LogOptionsChanged
		{
			add { Events.AddHandler(LogOptionsChangedEvent, value); }
			remove { Events.RemoveHandler(LogOptionsChangedEvent, value); }
		}

		protected virtual void OnLogOptionsChanged()
		{
			var handler = (EventHandler)Events[LogOptionsChangedEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}

		private static readonly object ShowDetailsChangedEvent = new object();

		public event EventHandler ShowDetailsChanged
		{
			add { Events.AddHandler(ShowDetailsChangedEvent, value); }
			remove { Events.RemoveHandler(ShowDetailsChangedEvent, value); }
		}

		protected virtual void OnShowDetailsChanged()
		{
			var handler = (EventHandler)Events[ShowDetailsChangedEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		protected HistoryViewBase(Guid guid, GuiProvider gui)
			: base(guid, gui)
		{
			SuspendLayout();
			_lstRevisions = new RevisionListBox
			{
				BorderStyle		= BorderStyle.None,
				Dock			= DockStyle.Fill,
				Location		= Point.Empty,
				ShowStatusItems	= true,
				Multiselect		= true,
				Text			= Resources.StrsNoCommitsToDisplay,
			};
			_lstRevisions.SelectionChanged += OnSelectionChanged;
			_lstRevisions.ItemActivated += OnItemActivated;
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			Controls.Add(_lstRevisions);
			ResumeLayout(false);
			PerformLayout();
			_showDetails = true;
			_search = new HistorySearch<HistorySearchOptions>(_lstRevisions);
			_options = new LogOptions();
			_options.Changed += OnLogOptionsChanged;
		}

		#endregion

		#region Properties

		/// <summary>Returns a value indicating whether this instance is a document.</summary>
		/// <value><c>true</c> if this instance is a document; otherwise, <c>false</c>.</value>
		public override bool IsDocument
		{
			get { return true; }
		}

		protected RevisionListBox RevisionListBox
		{
			get { return _lstRevisions; }
		}

		protected ILogSource LogSource
		{
			get { return _logSource; }
			set
			{
				if(_logSource != value)
				{
					_logSource = value;
					if(value != null)
					{
						RevisionLogBinding = new RevisionLogBinding(value, RevisionListBox, LogOptions);
					}
					else
					{
						RevisionLogBinding = null;
					}
				}
			}
		}

		public LogOptions LogOptions
		{
			get { return _options; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				if(_options != value)
				{
					_options.Changed -= OnLogOptionsChanged;
					_options = value;
					_options.Changed += OnLogOptionsChanged;

					if(RevisionLogBinding != null)
					{
						RevisionLogBinding.LogOptions = value;
					}

					OnLogOptionsChanged();
					RefreshContent();
				}
			}
		}

		public bool ShowDetails
		{
			get { return _showDetails; }
			set
			{
				if(value != _showDetails)
				{
					_showDetails = value;
					if(value)
					{
						ShowSelectedCommitDetails();
					}
					OnShowDetailsChanged();
				}
			}
		}

		public Revision SelectedRevision
		{
			get
			{
				if(_lstRevisions.SelectedItems.Count == 0) return null;
				var item = _lstRevisions.SelectedItems[0] as RevisionListItem;
				if(item == null) return null;
				return item.DataContext;
			}
		}

		public IEnumerable<Revision> SelectedRevisions
		{
			get
			{
				foreach(var item in RevisionListBox.SelectedItems)
				{
					var rli = item as RevisionListItem;
					if(rli != null)
					{
						yield return rli.DataContext;
					}
				}
			}
		}

		private RevisionLogBinding RevisionLogBinding
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

		#region Event Handlers

		private void OnLogOptionsChanged(object sender, EventArgs e)
		{
			RefreshContent();
			OnLogOptionsChanged();
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			if(ShowDetails)
			{
				ShowSelectedCommitDetails();
			}
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var diffSource = GetDiffSourceFromItem(e.Item);
			if(diffSource != null)
			{
				ShowDiffView(diffSource);
			}
		}

		#endregion

		#region ISearchableView<HistorySearchOptions>

		public ISearch<HistorySearchOptions> Search
		{
			get { return _search; }
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

		protected void ShowSearchToolBar()
		{
			if(_searchToolbar == null)
			{
				AddBottomToolStrip(_searchToolbar = new HistorySearchToolBar<HistoryViewBase>(this));
			}
			_searchToolbar.FocusSearchTextBox();
		}

		protected void HideSearchToolBar()
		{
			if(_searchToolbar != null)
			{
				RemoveToolStrip(_searchToolbar);
				_searchToolbar.Dispose();
				_searchToolbar = null;
			}
		}

		#endregion

		protected void ReloadRevisionLog()
		{
			if(RevisionLogBinding != null)
			{
				RevisionLogBinding.ReloadData();
			}
		}

		protected void ShowSelectedCommitDetails()
		{
			switch(RevisionListBox.SelectedItems.Count)
			{
				case 1:
					{
						var item = RevisionListBox.SelectedItems[0];
						var diffSource = GetDiffSourceFromItem(item);
						if(diffSource != null)
						{
							ShowContextualDiffView(diffSource);
						}
					}
					break;
				case 2:
					{
						var item1 = RevisionListBox.SelectedItems[0];
						var item2 = RevisionListBox.SelectedItems[1];
						var diffSource = GetDiffSourceFromItems(item1, item2);
						if(diffSource != null)
						{
							ShowContextualDiffView(diffSource);
						}
					}
					break;
			}
		}

		protected virtual IEnumerable<string> GetPaths()
		{
			var pathLogSource = LogSource as PathLogSource;
			if(pathLogSource != null)
			{
				return new[] { pathLogSource.Path };
			}
			return null;
		}

		protected virtual IDiffSource GetDiffSourceFromItem(CustomListBoxItem item)
		{
			var revisionItem = item as RevisionListItem;
			if(revisionItem != null)
			{
				return revisionItem.DataContext.GetDiffSource(GetPaths());
			}
			var fakeItem = item as FakeRevisionListItem;
			if(fakeItem != null)
			{
				switch(fakeItem.Type)
				{
					case FakeRevisionItemType.StagedChanges:
						return Repository.Status.GetDiffSource(true, GetPaths());
					case FakeRevisionItemType.UnstagedChanges:
						return Repository.Status.GetDiffSource(false, GetPaths());
				}
			}
			return null;
		}

		protected virtual IDiffSource GetDiffSourceFromItems(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var revisionItem1 = item1 as RevisionListItem;
			if(revisionItem1 == null) return null;
			var revisionItem2 = item2 as RevisionListItem;
			if(revisionItem2 == null) return null;
			var rev1 = revisionItem1.DataContext;
			var rev2 = revisionItem2.DataContext;
			return rev1.GetCompareDiffSource(rev2, GetPaths());
		}

		public void SelectRevision(IRevisionPointer revision)
		{
			RevisionListBox.SelectRevision(revision);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				RevisionLogBinding = null;
			}
			base.Dispose(disposing);
		}
	}
}

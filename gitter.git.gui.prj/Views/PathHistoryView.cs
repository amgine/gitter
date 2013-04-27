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

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class PathHistoryView : GitViewBase, ISearchableView<HistorySearchOptions>
	{
		#region Data

		private PathLogSource _logSource;
		private RevisionLog _revisionLog;
		private LogOptions _options;
		private PathHistoryToolbar _toolBar;
		private HistorySearchToolBar<PathHistoryView> _searchToolbar;
		private ISearch<HistorySearchOptions> _search;
		private bool _autoShowDiff;
		private AsyncLogRequest _pendingRequest;

		#endregion

		#region Events

		public event EventHandler LogOptionsChanged;

		#endregion

		public PathHistoryView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.PathHistoryViewGuid, gui, parameters)
		{
			InitializeComponent();

			_autoShowDiff = true;

			for(int i = 0; i < _lstRevisions.Columns.Count; ++i)
			{
				if(_lstRevisions.Columns[i].Id == (int)ColumnId.Graph)
				{
					_lstRevisions.Columns.RemoveAt(i);
					break;
				}
			}
			_lstRevisions.Text = Resources.StrsNoCommitsToDisplay;
			_lstRevisions.Multiselect = true;
			_lstRevisions.SelectionChanged += (sender, e) =>
				{
					if(ShowDetails)
					{
						ShowSelectedCommitDetails();
					}
				};
			_lstRevisions.ItemActivated += OnItemActivated;
			_lstRevisions.PreviewKeyDown += OnKeyDown;
			_options = new LogOptions();
			_options.Filter = LogReferenceFilter.Allowed;
			_options.Changed += OnLogOptionsChanged;
			_search = new HistorySearch<HistorySearchOptions>(_lstRevisions);

			AddTopToolStrip(_toolBar = new PathHistoryToolbar(this));
			ApplyParameters(parameters);
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			base.ApplyParameters(parameters);
			_logSource = (PathLogSource)parameters["source"];
			if(_logSource != null)
			{
				Text = Resources.StrHistory + ": " + _logSource.ToString();
				var request = new AsyncLogRequest(Repository, _logSource.GetLogAsync(_options));
				_pendingRequest = request;
				request.Query.BeginInvoke(this, _lstRevisions.ProgressMonitor, OnHistoryLoaded, request);
			}
			else
			{
				Text = Resources.StrHistory;
				_lstRevisions.Clear();
			}
		}

		/// <summary>Returns a value indicating whether this instance is a document.</summary>
		/// <value><c>true</c> if this instance is a document; otherwise, <c>false</c>.</value>
		public override bool IsDocument
		{
			get { return true; }
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
					LogOptionsChanged.Raise(this);
					RefreshContent();
				}
			}
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var revItem = e.Item as RevisionListItem;
			if(revItem != null)
			{
				ShowDiffView(revItem.DataContext.GetDiffSource(new[] { _logSource.Path }));
				return;
			}
			var fakeItem = e.Item as FakeRevisionListItem;
			if(fakeItem != null)
			{
				switch(fakeItem.Type)
				{
					case FakeRevisionItemType.StagedChanges:
						ShowDiffView(Repository.Status.GetDiffSource(true, new[] { _logSource.Path }));
						break;
					case FakeRevisionItemType.UnstagedChanges:
						ShowDiffView(Repository.Status.GetDiffSource(false, new[] { _logSource.Path }));
						break;
				}
				return;
			}
		}

		public override Image Image
		{
			get
			{
				if(_logSource != null)
				{
					if(_logSource.Path.EndsWith('/'))
					{
						return CachedResources.Bitmaps["ImgFolderHistory"];
					}
				}
				return CachedResources.Bitmaps["ImgFileHistory"];
			}
		}

		public void SelectRevision(IRevisionPointer revision)
		{
			_lstRevisions.SelectRevision(revision);
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
				foreach(var item in _lstRevisions.SelectedItems)
				{
					var rli = item as RevisionListItem;
					if(rli != null) yield return rli.DataContext;
				}
			}
		}

		private sealed class AsyncLogRequest
		{
			private readonly Repository _repository;
			private readonly IAsyncFunc<RevisionLog> _query;

			public AsyncLogRequest(Repository repository, IAsyncFunc<RevisionLog> query)
			{
				_repository = repository;
				_query = query;
			}

			public IAsyncFunc<RevisionLog> Query
			{
				get { return _query; }
			}

			public Repository Repository
			{
				get { return _repository; }
			}
		}

		private void OnHistoryLoaded(IAsyncResult ar)
		{
			if(!IsDisposed)
			{
				var request = (AsyncLogRequest)ar.AsyncState;
				if(request == _pendingRequest)
				{
					var log = request.Query.EndInvoke(ar);
					BeginInvoke(
						new MethodInvoker(
						() =>
						{
							if(!IsDisposed)
							{
								if(_pendingRequest == request)
								{
									_pendingRequest = null;
									_lstRevisions.SetLog(log);
									if(_lstRevisions.UnstagedItem != null)
									{
										_lstRevisions.UnstagedItem.FocusAndSelect();
										return;
									}
									if(_lstRevisions.StagedItem != null)
									{
										_lstRevisions.StagedItem.FocusAndSelect();
										return;
									}
									if(_lstRevisions.HeadItem != null)
									{
										_lstRevisions.HeadItem.FocusAndSelect();
										return;
									}
								}
							}
						}));
				}
			}
		}

		protected override void DetachFromRepository(Repository repository)
		{
			base.DetachFromRepository(repository);

			_lstRevisions.Clear();
			_logSource = null;
			_revisionLog = null;
			_options.Changed -= OnLogOptionsChanged;
			_options.Reset();
			_options.Changed += OnLogOptionsChanged;
			_pendingRequest = null;
			LogOptionsChanged.Raise(this);
		}

		private void OnStashDeleted(object sender, StashedStateEventArgs e)
		{
			if(e.Object.Index == 0)
			{
				var item = _lstRevisions.TryGetItem(e.Object.Revision);
				if(item != null)
				{
					RefreshContent();
				}
			}
		}

		private void OnLogOptionsChanged(object sender, EventArgs e)
		{
			RefreshContent();
			LogOptionsChanged.Raise(this);
		}

		/// <summary>Refreshes the content.</summary>
		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(RefreshContent));
			}
			else
			{
				if(_pendingRequest != null) return;
				if(Repository != null && _logSource != null)
				{
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						_lstRevisions.BeginUpdate();
						_revisionLog = _logSource.GetLog(_options);
						var state = _lstRevisions.GetState();
						_lstRevisions.SetLog(_revisionLog);
						_lstRevisions.SetState(state);
						_lstRevisions.EndUpdate();
					}
				}
			}
		}

		private void ShowSelectedCommitDetails()
		{
			switch(_lstRevisions.SelectedItems.Count)
			{
				case 1:
					{
						var item = _lstRevisions.SelectedItems[0];
						var revisionItem = item as RevisionListItem;
						if(revisionItem != null)
						{
							ShowContextualDiffView(revisionItem.DataContext.GetDiffSource(new[] { _logSource.Path }));
							return;
						}
						var fakeItem = item as FakeRevisionListItem;
						if(fakeItem != null)
						{
							IDiffSource diff = null;
							switch(fakeItem.Type)
							{
								case FakeRevisionItemType.StagedChanges:
									diff = Repository.Status.GetDiffSource(true, new[] { _logSource.Path });
									break;
								case FakeRevisionItemType.UnstagedChanges:
									diff = Repository.Status.GetDiffSource(false, new[] { _logSource.Path });
									break;
							}
							if(diff != null)
							{
								ShowContextualDiffView(diff);
							}
						}
					}
					break;
				case 2:
					{
						var item1 = _lstRevisions.SelectedItems[0];
						var revisionItem1 = item1 as RevisionListItem;
						if(revisionItem1 == null) return;
						var item2 = _lstRevisions.SelectedItems[1];
						var revisionItem2 = item2 as RevisionListItem;
						if(revisionItem2 == null) return;
						var rev1 = revisionItem1.DataContext;
						var rev2 = revisionItem2.DataContext;
						ShowContextualDiffView(rev1.GetCompareDiffSource(rev2, new[] { _logSource.Path }));
					}
					break;
				default:
					break;
			}
		}

		public RevisionListBox RevisionListBox
		{
			get { return _lstRevisions; }
		}

		public bool ShowDetails
		{
			get { return _autoShowDiff; }
			set
			{
				if(value != _autoShowDiff)
				{
					_autoShowDiff = value;
					if(value)
					{
						ShowSelectedCommitDetails();
					}
				}
			}
		}

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

		private void ShowSearchToolBar()
		{
			if(_searchToolbar == null)
			{
				AddBottomToolStrip(_searchToolbar = new HistorySearchToolBar<PathHistoryView>(this));
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

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var layoutNode = section.GetCreateSection("Layout");
			layoutNode.SetValue("ShowDetails", ShowDetails);
			var listNode = section.GetCreateSection("RevisionList");
			_lstRevisions.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var layoutNode = section.TryGetSection("Layout");
			if(layoutNode != null)
			{
				//_toolbar.ShowDiffButton.Checked = ShowDetails = layoutNode.GetValue("ShowDetails", ShowDetails);
			}
			var listNode = section.TryGetSection("RevisionList");
			if(listNode != null)
			{
				_lstRevisions.LoadViewFrom(listNode);
			}
		}
	}
}

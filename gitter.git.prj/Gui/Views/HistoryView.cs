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

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Tool for displaying a sequence of revisions with graph support.</summary>
	[ToolboxItem(false)]
	partial class HistoryView : GitViewBase, ISearchableView<HistorySearchOptions>
	{
		#region Data

		private ILogSource _logSource;
		private IList<Revision> _log;
		private LogOptions _options;
		private readonly HistoryToolbar _toolbar;
		private HistorySearchToolBar _searchToolbar;
		private bool _autoShowDiff;
		private AsyncLogRequest _pendingRequest;

		#endregion

		#region Events

		public event EventHandler LogOptionsChanged;

		#endregion

		public HistoryView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.HistoryViewGuid, gui, parameters)
		{
			InitializeComponent();

			_autoShowDiff = true;

			Text = Resources.StrHistory;

			_lstRevisions.Text = Resources.StrNoCommitsToDisplay;
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
			_options.Changed += OnLogOptionsChanged;

			AddTopToolStrip(_toolbar = new HistoryToolbar(this));
		}

		/// <summary>
		/// Gets a value indicating whether this instance is document.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is document; otherwise, <c>false</c>.
		/// </value>
		public override bool IsDocument
		{
			get { return true; }
		}

		public LogOptions LogOptions
		{
			get { return _options; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
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
				ShowDiffTool(new RevisionChangesDiffSource(revItem.Data));
				return;
			}
			var fakeItem = e.Item as FakeRevisionListItem;
			if(fakeItem != null)
			{
				switch(fakeItem.Type)
				{
					case FakeRevisionItemType.StagedChanges:
						ShowDiffTool(new IndexChangesDiffSource(Repository, true));
						break;
					case FakeRevisionItemType.UnstagedChanges:
						ShowDiffTool(new IndexChangesDiffSource(Repository, false));
						break;
				}
				return;
			}
		}

		/// <summary>Gets view image.</summary>
		/// <value>This view image.</value>
		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgHistory"]; }
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
				return item.Data;
			}
		}

		public IEnumerable<Revision> SelectedRevisions
		{
			get
			{
				foreach(var item in _lstRevisions.SelectedItems)
				{
					var rli = item as RevisionListItem;
					if(rli != null) yield return rli.Data;
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

		protected override void AttachToRepository(Repository repository)
		{
			base.AttachToRepository(repository);
			_logSource = new RepositoryLogSource(repository);

			var request = new AsyncLogRequest(repository, _logSource.GetLogAsync(_options));
			_pendingRequest = request;
			request.Query.BeginInvoke(this, _lstRevisions.ProgressMonitor, OnHistoryLoaded, request);
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
									_lstRevisions.SetLog(request.Repository, log.Revisions);
									request.Repository.CommitCreated += OnCommitCreated;
									request.Repository.Updated += OnRepositoryUpdated;
									request.Repository.Stash.StashedStateDeleted += OnStashDeleted;
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

			repository.CommitCreated -= OnCommitCreated;
			repository.Updated -= OnRepositoryUpdated;
			repository.Stash.StashedStateDeleted -= OnStashDeleted;
	
			_lstRevisions.Clear();
			_logSource = null;
			_log = null;
			_options.Changed -= OnLogOptionsChanged;
			_options.Reset();
			_options.Changed += OnLogOptionsChanged;
			_pendingRequest = null;
			LogOptionsChanged.Raise(this);
		}

		protected override void LoadRepositoryConfig(Section section)
		{
			base.LoadRepositoryConfig(section);
			var logOptionsNode = section.TryGetSection("LogOptions");
			if(logOptionsNode != null)
			{
				_options.Changed -= OnLogOptionsChanged;
				_options.LoadFrom(logOptionsNode);
				_options.Changed += OnLogOptionsChanged;
				LogOptionsChanged.Raise(this);
			}
		}

		protected override void SaveRepositoryConfig(Section section)
		{
			base.SaveRepositoryConfig(section);
			var logOptionsNode = section.GetCreateSection("LogOptions");
			_options.SaveTo(logOptionsNode);
		}

		private void OnCommitCreated(object sender, RevisionEventArgs e)
		{
			RefreshContent();
		}

		private void OnRepositoryUpdated(object sender, EventArgs e)
		{
			RefreshContent();
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
					Cursor = Cursors.WaitCursor;
					_lstRevisions.BeginUpdate();
					Repository.Status.Refresh();
					_log = _logSource.GetLog(_options).Revisions;
					var state = _lstRevisions.GetState();
					_lstRevisions.SetLog(Repository, _log);
					_lstRevisions.SetState(state);
					_lstRevisions.EndUpdate();
					Cursor = Cursors.Default;
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
							ShowContextualDiffTool(new RevisionChangesDiffSource(revisionItem.Data));
							return;
						}
						var fakeItem = item as FakeRevisionListItem;
						if(fakeItem != null)
						{
							IDiffSource diff = null;
							switch(fakeItem.Type)
							{
								case FakeRevisionItemType.StagedChanges:
									diff = new IndexChangesDiffSource(Repository, true);
									break;
								case FakeRevisionItemType.UnstagedChanges:
									diff = new IndexChangesDiffSource(Repository, false);
									break;
							}
							if(diff != null)
							{
								ShowContextualDiffTool(diff);
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
						ShowContextualDiffTool(new RevisionCompareDiffSource(
							revisionItem1.Data, revisionItem2.Data));
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

		private bool TestItem(RevisionListItem item, HistorySearchOptions search)
		{
			var rev = item.Data;
			if(rev.Subject.Contains(search.Text)) return true;
			if(rev.Body.Contains(search.Text)) return true;
			if(rev.Author.Name.Contains(search.Text)) return true;
			if(rev.Committer.Name.Contains(search.Text)) return true;
			if(rev.SHA1.StartsWith(search.Text)) return true;
			if(rev.TreeHash.StartsWith(search.Text)) return true;
			lock(rev.RefsSyncRoot)
			{
				foreach(var reference in rev.Refs.Values)
				{
					if(reference.FullName.Contains(search.Text)) return true;
				}
			}
			return false;
		}

		private bool Search(int start, HistorySearchOptions search, int direction)
		{
			if(search.Text.Length == 0) return true;
			int count = _lstRevisions.Items.Count;
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
				var item = _lstRevisions.Items[start] as RevisionListItem;
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

		public bool SearchFirst(HistorySearchOptions search)
		{
			if(search == null)
			{
				throw new ArgumentNullException("search");
			}

			return Search(-1, search, 1);
		}

		public bool SearchNext(HistorySearchOptions search)
		{
			if(search == null)
				throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstRevisions.SelectedItems.Count == 0)
				return Search(-1, search, 1);
			var start = _lstRevisions.Items.IndexOf(_lstRevisions.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool SearchPrevious(HistorySearchOptions search)
		{
			if(search == null) throw new ArgumentNullException("search");

			if(search.Text.Length == 0) return true;
			if(_lstRevisions.SelectedItems.Count == 0) return Search(-1, search, 1);
			var start = _lstRevisions.Items.IndexOf(_lstRevisions.SelectedItems[0]);
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
				AddBottomToolStrip(_searchToolbar = new HistorySearchToolBar(this));
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
				_toolbar.ShowDiffButton.Checked = ShowDetails = layoutNode.GetValue("ShowDetails", ShowDetails);
			}
			var listNode = section.TryGetSection("RevisionList");
			if(listNode != null)
			{
				_lstRevisions.LoadViewFrom(listNode);
			}
		}
	}
}

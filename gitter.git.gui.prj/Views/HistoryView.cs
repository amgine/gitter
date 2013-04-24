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

	/// <summary>Tool for displaying a sequence of revisions with graph support.</summary>
	[ToolboxItem(false)]
	partial class HistoryView : GitViewBase, ISearchableView<HistorySearchOptions>
	{
		#region Data

		private ILogSource _logSource;
		private RevisionLog _log;
		private LogOptions _options;
		private readonly HistoryToolbar _toolbar;
		private HistorySearchToolBar<HistoryView> _searchToolbar;
		private ISearch<HistorySearchOptions> _search;
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
			_options.Changed += OnLogOptionsChanged;
			_search = new HistorySearch<HistorySearchOptions>(_lstRevisions);

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
				ShowDiffView(revItem.DataContext.GetDiffSource());
				return;
			}
			var fakeItem = e.Item as FakeRevisionListItem;
			if(fakeItem != null)
			{
				switch(fakeItem.Type)
				{
					case FakeRevisionItemType.StagedChanges:
						ShowDiffView(Repository.Status.GetDiffSource(true));
						break;
					case FakeRevisionItemType.UnstagedChanges:
						ShowDiffView(Repository.Status.GetDiffSource(false));
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
									_lstRevisions.SetLog(log);
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
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						_lstRevisions.BeginUpdate();
						Repository.Status.Refresh();
						_log = _logSource.GetLog(_options);
						var state = _lstRevisions.GetState();
						_lstRevisions.SetLog(_log);
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
							ShowContextualDiffView(revisionItem.DataContext.GetDiffSource());
							return;
						}
						var fakeItem = item as FakeRevisionListItem;
						if(fakeItem != null)
						{
							IDiffSource diff = null;
							switch(fakeItem.Type)
							{
								case FakeRevisionItemType.StagedChanges:
									diff = Repository.Status.GetDiffSource(true);
									break;
								case FakeRevisionItemType.UnstagedChanges:
									diff = Repository.Status.GetDiffSource(false);
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
						ShowContextualDiffView(rev1.GetCompareDiffSource(rev2));
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
				AddBottomToolStrip(_searchToolbar = new HistorySearchToolBar<HistoryView>(this));
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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	internal partial class ReflogView : GitViewBase, ISearchableView<ReflogSearchOptions>
	{
		#region Data

		private readonly ReflogToolbar _toolbar;
		private ReflogSearchToolBar<ReflogView> _searchToolbar;
		private ISearch<ReflogSearchOptions> _search;

		#endregion

		public ReflogView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.ReflogViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrReflog;

			_lstReflog.SelectionChanged += OnReflogSelectionChanged;
			_lstReflog.ItemActivated += OnReflogItemActivated;
			_lstReflog.PreviewKeyDown += OnKeyDown;

			_search = new ReflogSearch<ReflogSearchOptions>(_lstReflog);

			ApplyParameters(parameters);
			AddTopToolStrip(_toolbar = new ReflogToolbar(this));
		}

		public override bool IsDocument
		{
			get { return true; }
		}

		public override Image Image
		{
			get
			{
				if(_lstReflog.Reflog != null)
				{
					if(_lstReflog.Reflog.Reference.Type == ReferenceType.RemoteBranch)
						return CachedResources.Bitmaps["ImgViewReflogRemote"];
				}
				return CachedResources.Bitmaps["ImgViewReflog"];
			}
		}

		private void OnReflogSelectionChanged(object sender, EventArgs e)
		{
			ShowSelectedCommitDetails();
		}

		private void OnReflogItemActivated(object sender, ItemEventArgs e)
		{
			ShowDiffTool(((ReflogRecordListItem)e.Item).DataContext.Revision.GetDiffSource());
		}

		private void ShowSelectedCommitDetails()
		{
			switch(_lstReflog.SelectedItems.Count)
			{
				case 1:
					{
						var item = _lstReflog.SelectedItems[0] as ReflogRecordListItem;
						if(item != null)
						{
							ShowContextualDiffTool(item.DataContext.Revision.GetDiffSource());
						}
					}
					break;
			}
		}

		public override void RefreshContent()
		{
			var reflog = _lstReflog.Reflog;
			if(reflog != null)
			{
				_lstReflog.Cursor = Cursors.WaitCursor;
				try
				{
					reflog.Refresh();
				}
				finally
				{
					_lstReflog.Cursor = Cursors.Default;
				}
			}
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			base.ApplyParameters(parameters);

			var reflog = (Reflog)parameters["reflog"];
			_lstReflog.Load(reflog);
			Text = Resources.StrReflog + ": " + reflog.Reference.Name;
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		public ISearch<ReflogSearchOptions> Search
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
				AddBottomToolStrip(_searchToolbar = new ReflogSearchToolBar<ReflogView>(this));
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
					e.IsInputKey = true;
					break;
			}
		}
	}
}

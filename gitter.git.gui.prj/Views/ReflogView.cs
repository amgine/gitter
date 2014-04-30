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
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	internal partial class ReflogView : GitViewBase, ISearchableView<ReflogSearchOptions>
	{
		#region Data

		private readonly ReflogToolbar _toolbar;
		private ReflogSearchToolBar<ReflogView> _searchToolbar;
		private ISearch<ReflogSearchOptions> _search;
		private Reflog _reflog;
		private Reference _reference;

		#endregion

		#region .ctor

		public ReflogView(GuiProvider gui)
			: base(Guids.ReflogViewGuid, gui)
		{
			InitializeComponent();

			_lstReflog.SelectionChanged += OnReflogSelectionChanged;
			_lstReflog.ItemActivated += OnReflogItemActivated;
			_lstReflog.PreviewKeyDown += OnKeyDown;

			_search = new ReflogSearch<ReflogSearchOptions>(_lstReflog);

			AddTopToolStrip(_toolbar = new ReflogToolbar(this));
		}

		#endregion

		#region Properties

		public override bool IsDocument
		{
			get { return true; }
		}

		public override Image Image
		{
			get
			{
				if(Reflog != null && Reflog.Reference.Type == ReferenceType.RemoteBranch)
				{
					return CachedResources.Bitmaps["ImgViewReflogRemote"];
				}
				else
				{
					return CachedResources.Bitmaps["ImgViewReflog"];
				}
			}
		}

		public Reflog Reflog
		{
			get { return _reflog; }
			private set
			{
				if(_reflog != value)
				{
					_reflog = value;
					_lstReflog.Load(value);
					Reference = value != null ? value.Reference : null;
				}
			}
		}

		public Reference Reference
		{
			get { return _reference; }
			private set
			{
				if(_reference != value)
				{
					if(_reference != null)
					{
						var branch = _reference as Branch;
						if(branch != null)
						{
							branch.Renamed -= OnBranchRenamed;
						}
					}
					_reference = value;
					if(_reference != null)
					{
						var branch = _reference as Branch;
						if(branch != null)
						{
							branch.Renamed += OnBranchRenamed;
						}
					}
					UpdateText();
				}
			}
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

		#endregion

		private void ShowSelectedCommitDetails()
		{
			switch(_lstReflog.SelectedItems.Count)
			{
				case 1:
					{
						var item = _lstReflog.SelectedItems[0] as ReflogRecordListItem;
						if(item != null)
						{
							ShowContextualDiffView(item.DataContext.Revision.GetDiffSource());
						}
					}
					break;
			}
		}

		public override void RefreshContent()
		{
			if(Reflog != null)
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Reflog.Refresh();
				}
			}
		}

		protected override void AttachViewModel(object viewModel)
		{
			base.AttachViewModel(viewModel);

			var vm = viewModel as ReflogViewModel;
			if(vm != null)
			{
				Reflog = vm.Reflog;
			}
		}

		protected override void DetachViewModel(object viewModel)
		{
			base.DetachViewModel(viewModel);

			var vm = viewModel as ReflogViewModel;
			if(vm != null)
			{
				Reflog = null;
			}
		}

		private void UpdateText()
		{
			if(Reference != null)
			{
				Text = Resources.StrReflog + ": " + Reference.Name;
			}
			else
			{
				Text = Resources.StrReflog;
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
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

		#region Event Handlers

		private void OnReflogSelectionChanged(object sender, EventArgs e)
		{
			ShowSelectedCommitDetails();
		}

		private void OnReflogItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as ReflogRecordListItem;
			if(item != null)
			{
				var reflogRecord = item.DataContext;
				ShowDiffView(reflogRecord.Revision.GetDiffSource());
			}
		}

		private void OnBranchRenamed(object sender, NameChangeEventArgs e)
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new MethodInvoker(UpdateText));
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					UpdateText();
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

		#endregion
	}
}

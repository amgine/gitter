#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

		private ILogSource _logSource;
		private LogOptions _options;
		private RevisionLogBinding _dataSource;
		protected readonly ISearchToolBarController _searchToolbar;
		private bool _showDetails;

		#endregion

		#region Events

		private static readonly object LogOptionsChangedEvent = new();

		public event EventHandler LogOptionsChanged
		{
			add    => Events.AddHandler    (LogOptionsChangedEvent, value);
			remove => Events.RemoveHandler (LogOptionsChangedEvent, value);
		}

		protected virtual void OnLogOptionsChanged()
			=> ((EventHandler)Events[LogOptionsChangedEvent])?.Invoke(this, EventArgs.Empty);

		private static readonly object ShowDetailsChangedEvent = new();

		public event EventHandler ShowDetailsChanged
		{
			add    => Events.AddHandler    (ShowDetailsChangedEvent, value);
			remove => Events.RemoveHandler (ShowDetailsChangedEvent, value);
		}

		protected virtual void OnShowDetailsChanged()
			=> ((EventHandler)Events[ShowDetailsChangedEvent])?.Invoke(this, EventArgs.Empty);

		#endregion

		#region .ctor

		protected HistoryViewBase(Guid guid, GuiProvider gui)
			: base(guid, gui)
		{
			SuspendLayout();
			RevisionListBox = new RevisionListBox
			{
				BorderStyle		= BorderStyle.None,
				Dock			= DockStyle.Fill,
				Location		= Point.Empty,
				ShowStatusItems	= true,
				Multiselect		= true,
				Text			= Resources.StrsNoCommitsToDisplay,
			};
			RevisionListBox.SelectionChanged += OnSelectionChanged;
			RevisionListBox.ItemActivated += OnItemActivated;
			AutoScaleDimensions = new SizeF(96F, 96F);
			Controls.Add(RevisionListBox);
			ResumeLayout(false);
			PerformLayout();
			_showDetails = true;
			Search = new HistorySearch<HistorySearchOptions>(RevisionListBox);
			_searchToolbar = CreateSearchToolbarController<HistoryViewBase, HistorySearchToolBar, HistorySearchOptions>(this);
			_options = new LogOptions();
			_options.Changed += OnLogOptionsChanged;
		}

		#endregion

		#region Properties

		/// <summary>Returns a value indicating whether this instance is a document.</summary>
		/// <value><c>true</c> if this instance is a document; otherwise, <c>false</c>.</value>
		public override bool IsDocument => true;

		protected RevisionListBox RevisionListBox { get; }

		protected ILogSource LogSource
		{
			get => _logSource;
			set
			{
				if(_logSource != value)
				{
					_logSource = value;
					RevisionLogBinding = value != null
						? new RevisionLogBinding(value, RevisionListBox, LogOptions)
						: default;
				}
			}
		}

		public LogOptions LogOptions
		{
			get => _options;
			set
			{
				Verify.Argument.IsNotNull(value, nameof(value));

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
			get => _showDetails;
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
				if(RevisionListBox.SelectedItems.Count == 0) return null;
				if(RevisionListBox.SelectedItems[0] is not RevisionListItem item) return null;
				return item.DataContext;
			}
		}

		public IEnumerable<Revision> SelectedRevisions
		{
			get
			{
				foreach(var item in RevisionListBox.SelectedItems)
				{
					if(item is RevisionListItem rli)
					{
						yield return rli.DataContext;
					}
				}
			}
		}

		private RevisionLogBinding RevisionLogBinding
		{
			get => _dataSource;
			set
			{
				if(_dataSource != value)
				{
					_dataSource?.Dispose();
					_dataSource = value;
					_dataSource?.ReloadData();
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

		public ISearch<HistorySearchOptions> Search { get; }

		public bool SearchToolBarVisible
		{
			get => _searchToolbar.IsVisible;
			set => _searchToolbar.IsVisible = value;
		}

		#endregion

		protected void ReloadRevisionLog()
		{
			if(IsDisposed) return;
			RevisionLogBinding?.ReloadData();
		}

		protected void ShowSelectedCommitDetails()
		{
			var diffSource = RevisionListBox.SelectedItems.Count switch
			{
				1 => GetDiffSourceFromItem(RevisionListBox.SelectedItems[0]),
				2 => GetDiffSourceFromItems(RevisionListBox.SelectedItems[0], RevisionListBox.SelectedItems[1]),
				_ => default,
			};
			if(diffSource != null)
			{
				ShowContextualDiffView(diffSource);
			}
		}

		protected virtual IEnumerable<string> GetPaths()
			=> LogSource switch
			{
				PathLogSource pathLogSource => new[] { pathLogSource.Path },
				_ => default,
			};

		protected virtual IDiffSource GetDiffSourceFromItem(CustomListBoxItem item)
			=> item switch
			{
				RevisionListItem revisionItem
					=> revisionItem.DataContext.GetDiffSource(GetPaths()),
				FakeRevisionListItem fakeItem when fakeItem.Type == FakeRevisionItemType.StagedChanges
					=> Repository.Status.GetDiffSource(true, GetPaths()),
				FakeRevisionListItem fakeItem when fakeItem.Type == FakeRevisionItemType.UnstagedChanges
					=> Repository.Status.GetDiffSource(false, GetPaths()),
				_ => default,
			};

		protected virtual IDiffSource GetDiffSourceFromItems(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			if(item1 is not RevisionListItem revisionItem1) return null;
			if(item2 is not RevisionListItem revisionItem2) return null;
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

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

namespace gitter.Git.Gui.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;

using gitter.Git.Gui.Dialogs;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary><see cref="CustomListBox"/> for displaying <see cref="RevisionListItem"/>.</summary>
public class RevisionListBox : CustomListBox
{
	private static Action<RevisionListBox> _onCreated;

	public static void OnCreated(Action<RevisionListBox> onCreated)
	{
		_onCreated += onCreated;
	}

	private Repository _repository;
	private RevisionListItem _currentRevisionItem;
	private RevisionLog _revisionLog;
	private Dictionary<Revision, RevisionListItem> _itemLookupTable;
	private int _currentIndex;
	private Branch _currentBranch;

	private bool _showStatusItems;

	private FakeRevisionListItem _unstagedItem;
	private FakeRevisionListItem _stagedItem;

	public sealed class State
	{
		public bool UnstagedSeleted { get; set; }

		public bool StagedSelected { get; set; }

		public IEnumerable<Revision> SelectedRevisions { get; set; }

		public int VScrollPos { get; set; }
	}

	/// <summary>Create <see cref="RevisionListBox"/>.</summary>
	/// <param name="graphBuilderFactory">Graph builder factory.</param>
	public RevisionListBox(IGraphBuilderFactory graphBuilderFactory)
	{
		Verify.Argument.IsNotNull(graphBuilderFactory);

		GraphBuilderFactory = graphBuilderFactory;

		AllowDrop        = true;
		_currentIndex    = -1;
		_showStatusItems = true;

		_onCreated?.Invoke(this);
	}

	private IGraphBuilderFactory GraphBuilderFactory { get; }

	private CustomListBoxColumn GraphColumn => Columns.GetById((int)ColumnId.Graph);

	private void DetachFromRepository()
	{
		_repository.Head.PointerChanged -= OnHeadChanged;
		_repository.Status.Changed -= OnStatusUpdated;
		if(_currentBranch is not null)
		{
			_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
		}
		_currentBranch = null;
		_currentRevisionItem = null;
		_currentIndex = -1;
	}

	private void AttachToRepository()
	{
		_repository.Head.PointerChanged += OnHeadChanged;
		_repository.Status.Changed += OnStatusUpdated;
		_currentBranch = _repository.Head.CurrentBranch;
		if(_currentBranch is not null)
		{
			_currentBranch.PositionChanged += OnCurrentBranchPositionChanged;
		}
	}

	public State GetState()
	{
		List<Revision> revs;
		if(SelectedItems.Count == 0)
		{
			revs = null;
		}
		else
		{
			revs = new List<Revision>(SelectedItems.Count);
			for(int i = 0; i < SelectedItems.Count; ++i)
			{
				if(SelectedItems[i] is RevisionListItem revItem)
				{
					revs.Add(revItem.DataContext);
				}
			}
		}
		var state = new State
		{
			StagedSelected    = _stagedItem   is { IsSelected: true },
			UnstagedSeleted   = _unstagedItem is { IsSelected: true },
			SelectedRevisions = revs,
			VScrollPos        = VScrollPos,
		};
		return state;
	}

	public void SetState(State state)
	{
		if(state is null)
		{
			SelectedItems.Clear();
		}
		else
		{
			if(state.UnstagedSeleted && _unstagedItem is not null)
			{
				_unstagedItem.IsSelected = true;
			}
			if(state.StagedSelected && _stagedItem is not null)
			{
				_stagedItem.IsSelected = true;
			}
			if(state.SelectedRevisions is not null)
			{
				foreach(var item in state.SelectedRevisions)
				{
					if(_itemLookupTable.TryGetValue(item, out var revItem))
					{
						revItem.IsSelected = true;
					}
				}
			}
			VScrollBar.Value = Math.Min(state.VScrollPos, MaxVScrollPos);
		}
	}

	private void UpdateRevisionLog(RevisionLog value)
	{
		var state = GetState();

		_currentIndex = -1;
		_currentRevisionItem = null;
		_repository = value.Repository;
		_revisionLog = value;
		var oldLookupTable = _itemLookupTable;
		_itemLookupTable = new Dictionary<Revision, RevisionListItem>(capacity: value.RevisionsCount);

		var head = _repository.Head.Revision;

		BeginUpdate();
		Items.Clear();
		var graphColumn = GraphColumn;
		if(graphColumn is not null)
		{
			var builder = GraphBuilderFactory.CreateGraphBuilder<Revision>();
			var graph   = builder.BuildGraph(value.Revisions, value.GetParents);

			int graphSize = 0;
			int currentIndex = -1;
			var currentRevisionItem = default(RevisionListItem);
			_stagedItem = null;
			_unstagedItem = null;
			for(int i = 0; i < value.RevisionsCount; ++i)
			{
				var revision = value.Revisions[i];
				if(oldLookupTable is null || !oldLookupTable.TryGetValue(revision, out var revisionListItem))
				{
					revisionListItem = new RevisionListItem(revision);
				}
				_itemLookupTable.Add(revision, revisionListItem);

				if(graph[i].Length > graphSize)
				{
					graphSize = graph[i].Length;
				}
				revisionListItem.Graph = graph[i];
				if(revision == head)
				{
					currentRevisionItem = revisionListItem;
					currentIndex = i;
				}
				Items.Add(revisionListItem);
			}
			_currentIndex = currentIndex;
			_currentRevisionItem = currentRevisionItem;
			graphColumn.Width = 21 * graphSize;
			lock(value.Repository.Status.SyncRoot)
			{
				CheckNeedOfFakeItems();
				ReinsertFakeItems(builder);
				AttachToRepository();
			}
		}
		else
		{
			int currentIndex = -1;
			var currentRevisionItem = default(RevisionListItem);
			_stagedItem = null;
			_unstagedItem = null;
			for(int i = 0; i < value.RevisionsCount; ++i)
			{
				var revision = value.Revisions[i];
				if(oldLookupTable is null || !oldLookupTable.TryGetValue(revision, out var revisionListItem))
				{
					revisionListItem = new RevisionListItem(revision);
				}
				_itemLookupTable.Add(revision, revisionListItem);
				if(revision == head)
				{
					currentRevisionItem = revisionListItem;
					currentIndex = i;
				}
				Items.Add(revisionListItem);
			}
			_currentIndex = currentIndex;
			_currentRevisionItem = currentRevisionItem;
		}
		RecomputeHeaderSizes();
		SetState(state);
		EndUpdate();
	}

	public RevisionLog RevisionLog
	{
		get => _revisionLog;
		set
		{
			if(_repository is not null)
			{
				DetachFromRepository();
			}

			if(_currentBranch is not null)
			{
				_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
				_currentBranch = null;
			}

			if(value is null)
			{
				_repository = null;
				_revisionLog = null;
				_itemLookupTable = null;
				_stagedItem = null;
				_unstagedItem = null;
				_currentIndex = -1;
				_currentRevisionItem = null;
				Items.Clear();
				return;
			}

			UpdateRevisionLog(value);
		}
	}

	public RevisionListItem HeadItem => _currentRevisionItem;

	public FakeRevisionListItem StagedItem => _stagedItem;

	public FakeRevisionListItem UnstagedItem => _unstagedItem;

	private void RefreshCurrentRevisionItem(Revision currentRevision)
	{
		int id = 0;
		var currentIndex = -1;
		var currentRevisionItem = default(RevisionListItem);
		foreach(CustomListBoxItem<Revision> item in Items)
		{
			if(item.DataContext == currentRevision && item.DataContext is not null)
			{
				currentRevisionItem = (RevisionListItem)item;
				currentIndex = id;
				break;
			}
			++id;
		}
		_currentIndex = currentIndex;
		_currentRevisionItem = currentRevisionItem;
	}

	private void RemoveFakeItems(IGraphBuilder<Revision> builder)
	{
		if(_showStatusItems)
		{
			if(_stagedItem is not null)
			{
				_stagedItem.Remove();
				if(_currentIndex != -1)
				{
					--_currentIndex;
				}
			}
			if(_unstagedItem is not null)
			{
				_unstagedItem.Remove();
				if(_currentIndex != -1)
				{
					--_currentIndex;
				}
			}
			if(_currentRevisionItem is not null)
			{
				if(_currentIndex == 0)
				{
					builder.CleanGraph(_currentRevisionItem.Graph);
				}
				else
				{
					var prev = ((RevisionListItem)Items[_currentIndex - 1]).Graph;
					var next = ((RevisionListItem)Items[_currentIndex]).Graph;
					builder.CleanGraph(prev, next);
				}
			}
		}
	}

	private int CheckNeedOfFakeItems()
	{
		bool needStaged = false;
		bool needUnstaged = false;
		if(_repository is not null && _showStatusItems)
		{
			if(_currentRevisionItem is not null || _repository.IsEmpty)
			{
				needStaged   = _repository.Status.StagedFiles.Count   != 0;
				needUnstaged = _repository.Status.UnstagedFiles.Count != 0;
			}
		}
		int count = 0;
		if(needStaged)
		{
			_stagedItem ??= new FakeRevisionListItem(_repository, FakeRevisionItemType.StagedChanges);
			++count;
		}
		else
		{
			_stagedItem = null;
		}
		if(needUnstaged)
		{
			_unstagedItem ??= new FakeRevisionListItem(_repository, FakeRevisionItemType.UnstagedChanges);
			++count;
		}
		else
		{
			_unstagedItem = null;
		}
		return count;
	}

	private void ReinsertFakeItems(IGraphBuilder<Revision> builder)
	{
		int fakeItems = 0;
		int graphLength = 0;
		if(_stagedItem is not null)
		{
			if(_currentIndex != -1)
			{
				Items.Insert(_currentIndex, _stagedItem);
				++_currentIndex;
				_stagedItem.Graph = builder.AddGraphLineToTop(_currentRevisionItem.Graph);
				graphLength = _stagedItem.Graph.Length;
				++fakeItems;
			}
			else if(_repository.IsEmpty)
			{
				Items.Insert(0, _stagedItem);
				_stagedItem.Graph = builder.AddGraphLineToTop(null);
				graphLength = _stagedItem.Graph.Length;
				++fakeItems;
			}
			else
			{
				_stagedItem = null;
			}
		}
		if(_unstagedItem is not null)
		{
			if(_currentIndex != -1)
			{
				if(_stagedItem is not null)
				{
					Items.Insert(_currentIndex - 1, _unstagedItem);
				}
				else
				{
					Items.Insert(_currentIndex, _unstagedItem);
				}
				++_currentIndex;
				if(_unstagedItem is not null && _stagedItem is not null)
				{
					_unstagedItem.Graph = builder.AddGraphLineToTop(_stagedItem.Graph);
				}
				else
				{
					_unstagedItem.Graph = _currentRevisionItem is not null
						? builder.AddGraphLineToTop(_currentRevisionItem.Graph)
						: builder.AddGraphLineToTop(null);
				}
				if(_unstagedItem.Graph.Length > graphLength)
				{
					graphLength = _unstagedItem.Graph.Length;
				}
				++fakeItems;
			}
			else if(_repository.IsEmpty)
			{
				Items.Insert(0, _unstagedItem);
				if(_unstagedItem is not null && _stagedItem is not null)
				{
					_unstagedItem.Graph = builder.AddGraphLineToTop(_stagedItem.Graph);
				}
				else
				{
					_unstagedItem.Graph = _currentRevisionItem is not null
						? builder.AddGraphLineToTop(_currentRevisionItem.Graph)
						: builder.AddGraphLineToTop(null);
				}
				if(_unstagedItem.Graph.Length > graphLength)
				{
					graphLength = _unstagedItem.Graph.Length;
				}
				++fakeItems;
			}
			else
			{
				_unstagedItem = null;
			}
		}
		if(fakeItems != 0)
		{
			graphLength *= 21;
			var graphColumn = GraphColumn;
			if(graphColumn is not null)
			{
				var d = graphColumn.Width.Value * 96 / graphColumn.Width.Dpi.X;
				if(d < graphLength)
				{
					graphColumn.Width = new(graphLength, Dpi.Default);
				}
				else
				{
					graphColumn.AutoSize();
				}
			}
		}
	}

	public void Clear()
	{
		if(_repository is not null)
		{
			_repository.Head.PointerChanged -= OnHeadChanged;
			_repository.Status.Changed -= OnStatusUpdated;
			if(_currentBranch != null)
			{
				_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
				_currentBranch = null;
			}
			_currentIndex = -1;
			_itemLookupTable = null;
			_revisionLog = null;
			_currentRevisionItem = null;
			_repository = null;
			Items.Clear();
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			if(_repository is not null)
			{
				_repository.Head.PointerChanged -= OnHeadChanged;
				_repository.Status.Changed -= OnStatusUpdated;
				if(_currentBranch is not null)
				{
					_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
					_currentBranch = null;
				}
				_currentIndex = -1;
				_itemLookupTable = null;
				_revisionLog = null;
				_currentRevisionItem = null;
				_repository = null;
				Items.Clear();
			}
		}
		base.Dispose(disposing);
	}

	#region Event Handlers

	private void OnHeadChanged(object sender, RevisionPointerChangedEventArgs e)
	{
		if(InvokeRequired)
		{
			BeginInvoke(new Action<Repository>(RelocateFakeItemsAfterCheckout), _repository);
		}
		else
		{
			RelocateFakeItemsAfterCheckout(_repository);
		}
	}

	private void OnCurrentBranchPositionChanged(object sender, RevisionChangedEventArgs e)
	{
		if(InvokeRequired)
		{
			BeginInvoke(new Action<Revision>(RelocateFakeItemsAfterHeadReset), e.NewValue);
		}
		else
		{
			RelocateFakeItemsAfterHeadReset(e.NewValue);
		}
	}

	private void OnStatusUpdated(object sender, EventArgs e)
	{
		var status = (Status)sender;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new Action<Status>(VerifyFakeItemsAfterStatusUpdate), status);
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			if(IsDisposed) return;
			VerifyFakeItemsAfterStatusUpdate(status);
		}
	}

	#endregion

	private void RelocateFakeItemsAfterHeadReset(Revision revision)
	{
		var builder = GraphBuilderFactory.CreateGraphBuilder<Revision>();
		BeginUpdate();
		RemoveFakeItems(builder);
		RefreshCurrentRevisionItem(revision);
		ReinsertFakeItems(builder);
		if(_currentRevisionItem != null)
		{
			_currentRevisionItem.InvalidateSafe();
		}
		EndUpdate();
	}

	private void RelocateFakeItemsAfterCheckout(Repository repository)
	{
		var builder = GraphBuilderFactory.CreateGraphBuilder<Revision>();
		BeginUpdate();
		RemoveFakeItems(builder);
		if(!repository.Head.IsEmpty)
		{
			var headRev = repository.Head.Revision;
			if(_currentRevisionItem is not null)
			{
				_currentRevisionItem.InvalidateSafe();
			}
			if(_currentBranch is not null)
			{
				_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
			}
			_currentBranch = _repository.Head.Pointer as Branch;
			if(_currentBranch is not null)
			{
				_currentBranch.PositionChanged += OnCurrentBranchPositionChanged;
			}
			RefreshCurrentRevisionItem(headRev);
			ReinsertFakeItems(builder);
		}
		else
		{
			if(_currentRevisionItem is not null)
			{
				_currentRevisionItem.InvalidateSafe();
			}
			if(_currentBranch is not null)
			{
				_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
			}
			_currentBranch = null;
			_currentRevisionItem = null;
			_currentIndex = -1;
			ReinsertFakeItems(builder);
		}
		EndUpdate();
	}

	private void VerifyFakeItemsAfterStatusUpdate(Status status)
	{
		if(_showStatusItems && (_currentIndex != -1 || status.Repository.IsEmpty))
		{
			var builder = GraphBuilderFactory.CreateGraphBuilder<Revision>();
			bool showStaged = status.StagedFiles.Count != 0;
			bool showUnstaged = status.UnstagedFiles.Count != 0;
			int changes = 0;
			if(showUnstaged)
			{
				if(_unstagedItem is null) ++changes;
			}
			else
			{
				if(_unstagedItem is not null) ++changes;
			}
			if(showStaged)
			{
				if(_stagedItem is null) ++changes;
			}
			else
			{
				if(_stagedItem is not null) ++changes;
			}
			if(changes != 0)
			{
				BeginUpdate();
				if(showStaged)
				{
					if(_stagedItem is null)
					{
						_stagedItem = new FakeRevisionListItem(status.Repository, FakeRevisionItemType.StagedChanges);
						if(_currentIndex >= 0)
						{
							Items.Insert(_currentIndex, _stagedItem);
							++_currentIndex;
						}
						else
						{
							if(showUnstaged && _unstagedItem is not null)
							{
								Items.Insert(1, _stagedItem);
							}
							else
							{
								Items.Insert(0, _stagedItem);
							}
						}
					}
					if(showUnstaged)
					{
						if(_unstagedItem is null)
						{
							_unstagedItem = new FakeRevisionListItem(status.Repository, FakeRevisionItemType.UnstagedChanges);
							if(_currentIndex >= 0)
							{
								Items.Insert(_currentIndex - 1, _unstagedItem);
								++_currentIndex;
							}
							else
							{
								Items.Insert(0, _unstagedItem);
							}
						}
					}
					else
					{
						if(_unstagedItem is not null)
						{
							_unstagedItem.Remove();
							_unstagedItem = null;
							if(_currentIndex != -1)
							{
								--_currentIndex;
							}
						}
					}
				}
				else
				{
					if(_stagedItem is not null)
					{
						_stagedItem.Remove();
						_stagedItem = null;
						if(_currentIndex != -1)
						{
							--_currentIndex;
						}
					}
					if(showUnstaged)
					{
						if(_unstagedItem is null)
						{
							_unstagedItem = new FakeRevisionListItem(status.Repository, FakeRevisionItemType.UnstagedChanges);
							if(_currentIndex >= 0)
							{
								Items.Insert(_currentIndex, _unstagedItem);
								++_currentIndex;
							}
							else
							{
								Items.Insert(0, _unstagedItem);
							}
						}
					}
					else
					{
						if(_unstagedItem is not null)
						{
							_unstagedItem.Remove();
							_unstagedItem = null;
							if(_currentIndex != -1)
							{
								--_currentIndex;
							}
						}
					}
				}
				int fakeitems = 0;
				if(_stagedItem is not null) ++fakeitems;
				if(_unstagedItem is not null) ++fakeitems;
				if(_currentIndex != -1)
				{
					if(_currentIndex == fakeitems)
					{
						builder.CleanGraph(_currentRevisionItem.Graph);
					}
					else
					{
						var prev = ((RevisionListItem)Items[_currentIndex - fakeitems - 1]).Graph;
						var next = ((RevisionListItem)Items[_currentIndex]).Graph;
						builder.CleanGraph(prev, next);
					}
				}
				if(_stagedItem is not null)
				{
					_stagedItem.Graph = builder.AddGraphLineToTop(_currentRevisionItem?.Graph);
					if(_unstagedItem is not null)
					{
						_unstagedItem.Graph = builder.AddGraphLineToTop(_stagedItem.Graph);
					}
				}
				else
				{
					if(_unstagedItem is not null)
					{
						_unstagedItem.Graph = builder.AddGraphLineToTop(_currentRevisionItem?.Graph);
					}
				}
				GraphColumn?.AutoSize();
				EndUpdate();
			}
		}
	}

	protected override ContextMenuStrip GetMultiselectContextMenu(ItemsContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		if(requestEventArgs.Items.Count != 2) return null;
		var revisions = new List<Revision>(requestEventArgs.Items.Count);
		foreach(var item in requestEventArgs.Items)
		{
			if(item is RevisionListItem revItem)
			{
				revisions.Add(revItem.DataContext);
			}
		}
		if(revisions.Count == 2)
		{
			var menu = new RevisionsMenu(revisions);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
		return default;
	}

	#region drag'n'drop support

	private void UpdateDragEffect(DragEventArgs drgevent)
	{
		if((drgevent.AllowedEffect & DragDropEffects.Move) != DragDropEffects.Move)
		{
			drgevent.Effect = DragDropEffects.None;
			return;
		}
		var data = drgevent.Data;
		if(data.GetDataPresent<Branch>())
		{
			var p = PointToClient(new Point(drgevent.X, drgevent.Y));
			var htr = HitTest(p.X, p.Y);
			if(htr.Area == HitTestArea.Item)
			{
				if(Items[htr.ItemIndex] is RevisionListItem item)
				{
					var branch = data.GetData<Branch>();
					if(branch.Repository == item.DataContext.Repository)
					{
						drgevent.Effect = DragDropEffects.Move;
						return;
					}
				}
			}
		}
		drgevent.Effect = DragDropEffects.None;
	}

	protected override void OnDragOver(DragEventArgs drgevent)
	{
		UpdateDragEffect(drgevent);
		base.OnDragOver(drgevent);
	}

	protected override void OnDragEnter(DragEventArgs drgevent)
	{
		UpdateDragEffect(drgevent);
		base.OnDragEnter(drgevent);
	}

	protected override void OnDragLeave(EventArgs e)
	{
		base.OnDragLeave(e);
	}

	protected override void OnDragDrop(DragEventArgs drgevent)
	{
		base.OnDragDrop(drgevent);
		var data = drgevent.Data;
		if(data.GetDataPresent<Branch>())
		{
			var p = PointToClient(new Point(drgevent.X, drgevent.Y));
			var htr = HitTest(p.X, p.Y);
			if(htr.Area == HitTestArea.Item)
			{
				if(Items[htr.ItemIndex] is RevisionListItem revItem)
				{
					var branch = data.GetData<Branch>();
					if(branch.Revision != revItem.DataContext)
					{
						BeginInvoke(new Action<Branch, Revision>(CompleteBranchDragAndDrop),
							branch, revItem.DataContext);
					}
				}
			}
		}
	}

	private void CompleteBranchDragAndDrop(Branch branch, Revision revision)
	{
		Assert.IsNotNull(branch);
		Assert.IsNotNull(revision);

		if(branch.IsCurrent)
		{
			using var dlg = new SelectResetModeDialog()
			{
				ResetMode = ResetMode.Mixed
			};
			if(dlg.Run(this) != DialogResult.OK) return;
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					revision.ResetHeadHere(dlg.ResetMode);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToReset,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}
		else
		{
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					branch.Reset(revision);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToReset,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}
	}

	#endregion

	#region Properties

	public bool HeadPresented => _currentRevisionItem is not null;

	public bool ShowStatusItems
	{
		get => _showStatusItems;
		set => _showStatusItems = value;
	}

	public RevisionListItem this[Revision revision]
		=> _itemLookupTable[revision];

	#endregion

	public bool SelectRevision(IRevisionPointer revision)
	{
		Verify.Argument.IsNotNull(revision);

		var rev = revision.Dereference();
		if(_itemLookupTable.TryGetValue(rev, out var item))
		{
			item.FocusAndSelect();
			return true;
		}
		return false;
	}

	public RevisionListItem TryGetItem(Revision revision)
	{
		Verify.Argument.IsNotNull(revision);

		if(_itemLookupTable is null) return null;
		if(_itemLookupTable.TryGetValue(revision, out var item))
		{
			return item;
		}
		return null;
	}

	public void RebuildGraph()
	{
		var graphColumn = GraphColumn;
		if(graphColumn is null) return;

		var builder  = GraphBuilderFactory.CreateGraphBuilder<Revision>();
		int graphLen = 0;
		if(_itemLookupTable.Count != 0)
		{
			var graph = builder.BuildGraph(_revisionLog.Revisions, rev => rev.Parents);
			int id = 0;
			foreach(IRevisionGraphListItem item in Items)
			{
				if(graph[id].Length > graphLen)
				{
					graphLen = graph[id].Length;
				}
				item.Graph = graph[id++];
			}
		}
		if(_showStatusItems)
		{
			if(_stagedItem is not null)
			{
				_stagedItem.Graph = builder.AddGraphLineToTop(_currentRevisionItem?.Graph);
				if(_stagedItem.Graph.Length > graphLen)
				{
					graphLen = _stagedItem.Graph.Length;
				}
			}
			if(_unstagedItem is not null)
			{
				_unstagedItem.Graph = _stagedItem is not null
					? builder.AddGraphLineToTop(_stagedItem.Graph)
					: builder.AddGraphLineToTop(_currentRevisionItem?.Graph);
				if(_unstagedItem.Graph.Length > graphLen)
				{
					graphLen = _unstagedItem.Graph.Length;
				}
			}
		}
		graphColumn.Width = 21 * graphLen;
		Invalidate();
	}
}

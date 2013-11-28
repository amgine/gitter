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

namespace gitter.Git.Gui.Controls
{
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
		public RevisionListBox()
		{
			Columns.AddRange(new CustomListBoxColumn[]
				{
					new HashColumn(),
					new TreeHashColumn(),
					new CommitDateColumn(),
					new GraphColumn(),
					new SubjectColumn(),
					new CommitterColumn(),
					new CommitterEmailColumn(),
					new AuthorDateColumn(),
					new AuthorColumn(),
					new AuthorEmailColumn(),
				});
			AllowDrop = true;
			_currentIndex = -1;
			_showStatusItems = true;
		}

		private CustomListBoxColumn GraphColumn
		{
			get { return Columns.GetById((int)ColumnId.Graph); }
		}

		private void DetachFromRepository()
		{
			_repository.Head.PointerChanged -= OnHeadChanged;
			_repository.Status.Changed -= OnStatusUpdated;
			if(_currentBranch != null)
			{
				_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
			}
			_currentBranch = null;
			_currentRevisionItem = null;
			_itemLookupTable = null;
			_currentIndex = -1;
		}

		private void AttachToRepository()
		{
			_repository.Head.PointerChanged += OnHeadChanged;
			_repository.Status.Changed += OnStatusUpdated;
			_currentBranch = _repository.Head.CurrentBranch;
			if(_currentBranch != null)
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
					var revItem = SelectedItems[i] as RevisionListItem;
					if(revItem != null)
						revs.Add(revItem.DataContext);
				}
			}
			var state = new State
			{
				StagedSelected = _stagedItem != null && _stagedItem.IsSelected,
				UnstagedSeleted = _unstagedItem != null && _unstagedItem.IsSelected,
				SelectedRevisions = revs,
				VScrollPos = VScrollPos,
			};
			return state;
		}

		public void SetState(State state)
		{
			if(state == null)
			{
				SelectedItems.Clear();
			}
			else
			{
				if(state.UnstagedSeleted && _unstagedItem != null)
				{
					_unstagedItem.IsSelected = true;
				}
				if(state.StagedSelected && _stagedItem != null)
				{
					_stagedItem.IsSelected = true;
				}
				if(state.SelectedRevisions != null)
				{
					foreach(var item in state.SelectedRevisions)
					{
						RevisionListItem revItem;
						if(_itemLookupTable.TryGetValue(item, out revItem))
						{
							revItem.IsSelected = true;
						}
					}
				}
				var scrollPos = state.VScrollPos;
				if(scrollPos > MaxVScrollPos)
				{
					scrollPos = MaxVScrollPos;
				}
				VScrollBar.Value = scrollPos;
			}
		}

		public RevisionLog RevisionLog
		{
			get { return _revisionLog; }
			set
			{
				if(_repository != null)
				{
					DetachFromRepository();
				}

				if(_currentBranch != null)
				{
					_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
					_currentBranch = null;
				}

				if(value == null)
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

				var state = GetState();

				_currentIndex = -1;
				_currentRevisionItem = null;
				_repository = value.Repository;
				_revisionLog = value;
				var oldLookupTable = _itemLookupTable;
				_itemLookupTable = new Dictionary<Revision, RevisionListItem>(value.RevisionsCount);

				var head = _repository.Head.Revision;

				BeginUpdate();
				Items.Clear();
				var graphColumn = GraphColumn;
				if(graphColumn != null)
				{
					var builder = GlobalBehavior.GraphBuilderFactory.CreateGraphBuilder<Revision>();
					var graph   = builder.BuildGraph(value.Revisions, value.GetParents);

					int graphSize = 0;
					int currentIndex = -1;
					RevisionListItem currentRevisionItem = null;
					_stagedItem = null;
					_unstagedItem = null;
					for(int i = 0; i < value.RevisionsCount; ++i)
					{
						var revision = value.Revisions[i];
						RevisionListItem revisionListItem;
						if(oldLookupTable == null || !oldLookupTable.TryGetValue(revision, out revisionListItem))
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
					RevisionListItem currentRevisionItem = null;
					_stagedItem = null;
					_unstagedItem = null;
					for(int i = 0; i < value.RevisionsCount; ++i)
					{
						var revision = value.Revisions[i];
						RevisionListItem revisionListItem;
						if(oldLookupTable == null || !oldLookupTable.TryGetValue(revision, out revisionListItem))
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
		}

		public RevisionListItem HeadItem
		{
			get { return _currentRevisionItem; }
		}

		public FakeRevisionListItem StagedItem
		{
			get { return _stagedItem; }
		}

		public FakeRevisionListItem UnstagedItem
		{
			get { return _unstagedItem; }
		}

		private void RefreshCurrentRevisionItem(Revision currentRevision)
		{
			int id = 0;
			var currentIndex = -1;
			RevisionListItem currentRevisionItem = null;
			foreach(CustomListBoxItem<Revision> item in Items)
			{
				if(item.DataContext == currentRevision && item.DataContext != null)
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
				if(_stagedItem != null)
				{
					_stagedItem.Remove();
					if(_currentIndex != -1)
					{
						--_currentIndex;
					}
				}
				if(_unstagedItem != null)
				{
					_unstagedItem.Remove();
					if(_currentIndex != -1)
					{
						--_currentIndex;
					}
				}
				if(_currentRevisionItem != null)
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
			if(_repository != null && _showStatusItems)
			{
				if(_currentRevisionItem != null || _repository.IsEmpty)
				{
					if(_repository.Status.StagedFiles.Count != 0)
					{
						needStaged = true;
					}
					if(_repository.Status.UnstagedFiles.Count != 0)
					{
						needUnstaged = true;
					}
				}
			}
			int count = 0;
			if(needStaged)
			{
				if(_stagedItem == null)
				{
					_stagedItem = new FakeRevisionListItem(_repository, FakeRevisionItemType.StagedChanges);
				}
				++count;
			}
			else
			{
				_stagedItem = null;
			}
			if(needUnstaged)
			{
				if(_unstagedItem == null)
				{
					_unstagedItem = new FakeRevisionListItem(_repository, FakeRevisionItemType.UnstagedChanges);
				}
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
			if(_stagedItem != null)
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
			if(_unstagedItem != null)
			{
				if(_currentIndex != -1)
				{
					if(_stagedItem != null)
					{
						Items.Insert(_currentIndex - 1, _unstagedItem);
					}
					else
					{
						Items.Insert(_currentIndex, _unstagedItem);
					}
					++_currentIndex;
					if(_unstagedItem != null && _stagedItem != null)
					{
						_unstagedItem.Graph = builder.AddGraphLineToTop(_stagedItem.Graph);
					}
					else
					{
						if(_currentRevisionItem != null)
						{
							_unstagedItem.Graph = builder.AddGraphLineToTop(_currentRevisionItem.Graph);
						}
						else
						{
							_unstagedItem.Graph = builder.AddGraphLineToTop(null);
						}
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
					if(_unstagedItem != null && _stagedItem != null)
					{
						_unstagedItem.Graph = builder.AddGraphLineToTop(_stagedItem.Graph);
					}
					else
					{
						if(_currentRevisionItem != null)
						{
							_unstagedItem.Graph = builder.AddGraphLineToTop(_currentRevisionItem.Graph);
						}
						else
						{
							_unstagedItem.Graph = builder.AddGraphLineToTop(null);
						}
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
				if(graphColumn != null)
				{
					if(graphColumn.Width < graphLength)
					{
						graphColumn.Width = graphLength;
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
			if(_repository != null)
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

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_repository != null)
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
				BeginInvoke(new Action<Status>(VerifyFakeItemsAfterStatusUpdate), status);
			}
			else
			{
				VerifyFakeItemsAfterStatusUpdate(status);
			}
		}

		#endregion

		private void RelocateFakeItemsAfterHeadReset(Revision revision)
		{
			var builder = GlobalBehavior.GraphBuilderFactory.CreateGraphBuilder<Revision>();
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
			var builder = GlobalBehavior.GraphBuilderFactory.CreateGraphBuilder<Revision>();
			BeginUpdate();
			RemoveFakeItems(builder);
			if(!repository.Head.IsEmpty)
			{
				var headRev = repository.Head.Revision;
				if(_currentRevisionItem != null)
				{
					_currentRevisionItem.InvalidateSafe();
				}
				if(_currentBranch != null)
				{
					_currentBranch.PositionChanged -= OnCurrentBranchPositionChanged;
				}
				_currentBranch = _repository.Head.Pointer as Branch;
				if(_currentBranch != null)
				{
					_currentBranch.PositionChanged += OnCurrentBranchPositionChanged;
				}
				RefreshCurrentRevisionItem(headRev);
				ReinsertFakeItems(builder);
			}
			else
			{
				if(_currentRevisionItem != null)
				{
					_currentRevisionItem.InvalidateSafe();
				}
				if(_currentBranch != null)
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
				var builder = GlobalBehavior.GraphBuilderFactory.CreateGraphBuilder<Revision>();
				bool showStaged = status.StagedFiles.Count != 0;
				bool showUnstaged = status.UnstagedFiles.Count != 0;
				int changes = 0;
				if(showUnstaged)
				{
					if(_unstagedItem == null) ++changes;
				}
				else
				{
					if(_unstagedItem != null) ++changes;
				}
				if(showStaged)
				{
					if(_stagedItem == null) ++changes;
				}
				else
				{
					if(_stagedItem != null) ++changes;
				}
				if(changes != 0)
				{
					BeginUpdate();
					if(showStaged)
					{
						if(_stagedItem == null)
						{
							_stagedItem = new FakeRevisionListItem(status.Repository, FakeRevisionItemType.StagedChanges);
							if(_currentIndex >= 0)
							{
								Items.Insert(_currentIndex, _stagedItem);
								++_currentIndex;
							}
							else
							{
								if(showUnstaged && _unstagedItem != null)
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
							if(_unstagedItem == null)
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
							if(_unstagedItem != null)
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
						if(_stagedItem != null)
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
							if(_unstagedItem == null)
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
							if(_unstagedItem != null)
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
					if(_stagedItem != null) ++fakeitems;
					if(_unstagedItem != null) ++fakeitems;
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
					if(_stagedItem != null)
					{
						_stagedItem.Graph = builder.AddGraphLineToTop(
							_currentRevisionItem == null ? null : _currentRevisionItem.Graph);
						if(_unstagedItem != null)
						{
							_unstagedItem.Graph = builder.AddGraphLineToTop(_stagedItem.Graph);
						}
					}
					else
					{
						if(_unstagedItem != null)
						{
							_unstagedItem.Graph = builder.AddGraphLineToTop(
								_currentRevisionItem == null ? null : _currentRevisionItem.Graph);
						}
					}
					var graphColumn = GraphColumn;
					if(graphColumn != null)
					{
						graphColumn.AutoSize();
					}
					EndUpdate();
				}
			}
		}

		protected override ContextMenuStrip GetMultiselectContextMenu(ItemsContextMenuRequestEventArgs requestEventArgs)
		{
			if(requestEventArgs.Items.Count != 2) return null;
			var revisions = new List<Revision>(requestEventArgs.Items.Count);
			foreach(var item in requestEventArgs.Items)
			{
				var revItem = item as RevisionListItem;
				if(revItem != null)
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
			else
			{
				return null;
			}
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
					var item = Items[htr.ItemIndex] as RevisionListItem;
					if(item != null)
					{
						var branch = data.GetData<Branch>();
						if(branch.Repository == item.DataContext.Repository)
						{
							drgevent.Effect = DragDropEffects.Move;
						}
						else
						{
							drgevent.Effect = DragDropEffects.None;
						}
					}
					else
					{
						drgevent.Effect = DragDropEffects.None;
					}
				}
				else
				{
					drgevent.Effect = DragDropEffects.None;
				}
			}
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
					var revItem = Items[htr.ItemIndex] as RevisionListItem;
					if(revItem != null)
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
			if(branch.IsCurrent)
			{
				using(var dlg = new SelectResetModeDialog()
				{
					ResetMode = ResetMode.Mixed
				})
				{
					if(dlg.Run(this) == DialogResult.OK)
					{
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

		public bool HeadPresented
		{
			get { return _currentRevisionItem != null; }
		}

		public bool ShowStatusItems
		{
			get { return _showStatusItems; }
			set { _showStatusItems = value; }
		}

		public RevisionListItem this[Revision revision]
		{
			get { return _itemLookupTable[revision]; }
		}

		#endregion

		public void SelectRevision(IRevisionPointer revision)
		{
			Verify.Argument.IsNotNull(revision, "revision");

			var rev = revision.Dereference();
			RevisionListItem item;
			if(_itemLookupTable.TryGetValue(rev, out item))
			{
				item.FocusAndSelect();
			}
		}

		public RevisionListItem TryGetItem(Revision revision)
		{
			Verify.Argument.IsNotNull(revision, "revision");

			if(_itemLookupTable == null) return null;
			RevisionListItem item;
			if(_itemLookupTable.TryGetValue(revision, out item))
			{
				return item;
			}
			return null;
		}

		public void RebuildGraph()
		{
			var graphColumn = GraphColumn;
			if(graphColumn != null)
			{
				var builder = GlobalBehavior.GraphBuilderFactory.CreateGraphBuilder<Revision>();
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
					if(_stagedItem != null)
					{
						if(_currentRevisionItem != null)
						{
							_stagedItem.Graph = builder.AddGraphLineToTop(_currentRevisionItem.Graph);
						}
						else
						{
							_stagedItem.Graph = builder.AddGraphLineToTop(null);
						}
						if(_stagedItem.Graph.Length > graphLen)
						{
							graphLen = _stagedItem.Graph.Length;
						}
					}
					if(_unstagedItem != null)
					{
						if(_stagedItem != null)
						{
							_unstagedItem.Graph = builder.AddGraphLineToTop(_stagedItem.Graph);
						}
						else
						{
							if(_currentRevisionItem != null)
							{
								_unstagedItem.Graph = builder.AddGraphLineToTop(_currentRevisionItem.Graph);
							}
							else
							{
								_unstagedItem.Graph = builder.AddGraphLineToTop(null);
							}
						}
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
	}
}

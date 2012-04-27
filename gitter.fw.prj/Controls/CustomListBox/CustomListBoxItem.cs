namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>List item which can be hosted in <see cref="CustomListBox"/>.</summary>
	public abstract class CustomListBoxItem : CustomListBoxHostedItem
	{
		#region Events

		/// <summary>Item is activated.</summary>
		public event EventHandler Activated;
		/// <summary>Item's <see cref="CheckedState"/> changed.</summary>
		public event EventHandler CheckedStateChanged;
		/// <summary>Context menu requested.</summary>
		public event EventHandler<ItemContextMenuRequestEventArgs> ContextMenuRequested;

		#endregion

		#region Data

		private readonly CustomListBoxItemsCollection _items;
		private CustomListBoxItem _parent;
		private bool _selected;
		private bool _expanded;
		private CheckedState _checkedState;
		private bool _threeStateCheckboxMode;
		private int _cachedLevel;

		#endregion

		#region Static Data

		private static readonly Bitmap ImgCollapse = Resources.ImgMinus;
		private static readonly Bitmap ImgCollapseHovered = Resources.ImgMinusHovered;
		private static readonly Bitmap ImgExpand = Resources.ImgPlus;
		private static readonly Bitmap ImgExpandHovered = Resources.ImgPlusHovered;

		private static readonly Dictionary<CheckedState, Bitmap> ImgCheckedState =
			new Dictionary<CheckedState, Bitmap>()
			{
				{ CheckedState.Checked,			Resources.ImgChecked		},
				{ CheckedState.Unchecked,		Resources.ImgUnchecked		},
				{ CheckedState.Intermediate,	Resources.ImgIntermediate	},
			};

		private static readonly Dictionary<CheckedState, Bitmap> ImgCheckedStateHovered =
			new Dictionary<CheckedState, Bitmap>()
			{
				{ CheckedState.Checked,			Resources.ImgCheckedHover		},
				{ CheckedState.Unchecked,		Resources.ImgUncheckedHover		},
				{ CheckedState.Intermediate,	Resources.ImgIntermediateHover	},
			};

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="CustomListBoxItem"/> class.</summary>
		public CustomListBoxItem()
		{
			_cachedLevel = -1;

			_items = new CustomListBoxItemsCollection(null, this);

			_checkedState = CheckedState.Unchecked;

			_items.Changing += OnItemsChanging;
			_items.Changed += OnItemsChanged;
		}

		#endregion

		#region Event Handlers

		private void OnItemsChanging(object sender, NotifyCollectionEventArgs e)
		{
			if(IsAttachedToListBox && IsPresented)
			{
				ListBox.OnItemsChanging(sender, e);
			}
		}

		private void OnItemsChanged(object sender, NotifyCollectionEventArgs e)
		{
			if(IsAttachedToListBox && IsPresented)
			{
				ListBox.OnItemsChanged(sender, e);
			}
		}

		#endregion

		#region Properties

		/// <summary>Parent item. Null if item's root is listbox itself.</summary>
		public CustomListBoxItem Parent
		{
			get { return _parent; }
			internal set
			{
				if(_parent != value)
				{
					_parent = value;
					_cachedLevel = -1;
					OnParentChanged();
				}
			}
		}

		/// <summary>Gets the collection of items, containing this item.</summary>
		/// <value>Collection of items, containing this item.</value>
		private CustomListBoxItemsCollection ParentItems
		{
			get
			{
				if(_parent == null)
				{
					return ListBox != null ? ListBox.Items : null;
				}
				else
				{
					return _parent.Items;
				}
			}
		}

		/// <summary>Child items.</summary>
		public CustomListBoxItemsCollection Items
		{
			get { return _items; }
		}

		/// <summary>Tooltip text for an item.</summary>
		public string ToolTipText
		{
			get { return GetToolTipText(); }
		}

		/// <summary>Current <see cref="CheckedState"/> of this item.</summary>
		public CheckedState CheckedState
		{
			get { return _checkedState; }
			set
			{
				if(_checkedState != value)
				{
					_checkedState = value;
					OnCheckedStateChanged();
					Invalidate();
				}
			}
		}

		/// <summary>Returns true if item is CheckedState == <see cref="CheckedState.Checked"/>.</summary>
		public bool IsChecked
		{
			get { return _checkedState == CheckedState.Checked; }
			set { CheckedState = value ? CheckedState.Checked : CheckedState.Unchecked; }
		}

		/// <summary>Item should operate as a three-state checkbox.</summary>
		public bool ThreeStateCheckboxMode
		{
			get { return _threeStateCheckboxMode; }
			set { _threeStateCheckboxMode = value; }
		}

		/// <summary>Item level.</summary>
		public int Level
		{
			get
			{
				if(_cachedLevel == -1)
				{
					_cachedLevel = (_parent == null) ? (0) : (_parent.Level + 1);
				}
				return _cachedLevel;
			}
		}

		/// <summary>Checks if item is selected.</summary>
		public bool IsSelected
		{
			get { return _selected; }
			set
			{
				if(_selected != value)
				{
					if(IsAttachedToListBox && IsPresented)
					{
						if(value)
						{
							ListBox.SelectedItems.Add(this);
						}
						else
						{
							ListBox.SelectedItems.Remove(this);
						}
						Invalidate();
					}
					else
					{
						_selected = value;
					}
				}
			}
		}

		/// <summary>Returns true if this item is currently focused.</summary>
		public bool IsFocused
		{
			get { return IsAttachedToListBox && ListBox.FocusedItem == this; }
		}

		/// <summary>Gets/sets item expanded state.</summary>
		public bool IsExpanded
		{
			get { return _expanded; }
			set
			{
				if(_expanded != value)
				{
					if(value)
					{
						Expand();
					}
					else
					{
						Collapse();
					}
				}
			}
		}

		/// <summary>Gets/sets item collapsed state.</summary>
		public bool IsCollapsed
		{
			get { return !_expanded; }
			set
			{
				if(_expanded == value)
				{
					if(value)
					{
						Collapse();
					}
					else
					{
						Expand();
					}
				}
			}
		}

		/// <summary>Returns true if item is not hidden by collapsed parent item.</summary>
		public bool IsPresented
		{
			get
			{
				if(_parent == null) return true;
				return _parent.IsExpanded && _parent.IsPresented;
			}
		}

		#endregion

		/// <summary>Returns tooltip text for an item.</summary>
		/// <returns>Tooltip text for an item.</returns>
		protected virtual string GetToolTipText()
		{
			return null;
		}

		public CustomListBoxTextEditor StartTextEditor(CustomListBoxColumn column, Font font, string text)
		{
			if(!IsAttachedToListBox) throw new InvalidOperationException();
			return ListBox.StartTextEditor(this, column, font, text);
		}

		public CustomListBoxTextEditor StartTextEditor(CustomListBoxColumn column, string text)
		{
			if(!IsAttachedToListBox) throw new InvalidOperationException();
			return ListBox.StartTextEditor(this, column, ListBox.Font, text);
		}

		/// <summary>Called when <see cref="M:CheckedState"/> property value changes.</summary>
		protected virtual void OnCheckedStateChanged()
		{
			CheckedStateChanged.Raise(this);
			var lb = ListBox;
			if(lb != null)
			{
				lb.OnItemCheckedChanged(this);
			}
		}

		protected virtual void OnParentChanged()
		{
		}

		internal void SetChecked(CheckedState state)
		{
			if(_checkedState != state)
			{
				_checkedState = state;
				OnCheckedStateChanged();
			}
		}

		/// <summary>Invokes item activation events.</summary>
		public void Activate()
		{
			OnActivate();
			if(IsAttachedToListBox) ListBox.ActivateItem(this);
		}

		/// <summary>Invokes <see cref="Activated"/> event.</summary>
		protected virtual void OnActivate()
		{
			Activated.Raise(this);
		}

		internal void SetSelected(bool selected)
		{
			_selected = selected;
		}

		private static void _expand_all(CustomListBoxItem item)
		{
			item.IsExpanded = true;
			if(item.Items.Count != 0)
			{
				foreach(var i in item.Items)
				{
					_expand_all(i);
				}
			}
		}

		private static void _collapse_all(CustomListBoxItem item)
		{
			item.IsExpanded = false;
			if(item.Items.Count != 0)
			{
				foreach(var i in item.Items)
				{
					_collapse_all(i);
				}
			}
		}

		/// <summary>Expands this item.</summary>
		public void Expand()
		{
			if(!_expanded)
			{
				_expanded = true;
				if(IsAttachedToListBox && IsPresented && _items.Count != 0)
				{
					ListBox.ExpandItem(this);
				}
			}
		}

		/// <summary>Expands whole tree of items starting at this item.</summary>
		public void ExpandAll()
		{
			if(_items.Count == 0) return;
			bool p = IsPresented;
			bool hasListBox = IsAttachedToListBox;
			if(hasListBox && p)
			{
				ListBox.BeginUpdate();
			}
			_expand_all(this);
			if(hasListBox && p)
			{
				ListBox.EndUpdate();
			}
		}

		/// <summary>Collapses this item.</summary>
		public void Collapse()
		{
			if(_expanded)
			{
				_expanded = false;
				if(IsAttachedToListBox && IsPresented && _items.Count != 0)
				{
					ListBox.CollapseItem(this);
				}
			}
		}

		/// <summary>Collapses whole tree of items starting at this item.</summary>
		public void CollapseAll()
		{
			if(_items.Count == 0) return;
			bool p = IsPresented;
			bool hasListBox = IsAttachedToListBox;
			if(hasListBox && p)
			{
				ListBox.BeginUpdate();
			}
			_collapse_all(this);
			if(hasListBox && p)
			{
				ListBox.EndUpdate();
			}
		}

		/// <summary>Sets focus and selects this item.</summary>
		public void FocusAndSelect()
		{
			if(!IsAttachedToListBox) throw new InvalidOperationException();
			ListBox.FocusAndSelectItem(this);
		}

		/// <summary>Sets focus to this item.</summary>
		public void Focus()
		{
			if(!IsAttachedToListBox) throw new InvalidOperationException();
			ListBox.FocusItem(this);
		}

		/// <summary>Makes sure that item can be found in listbox.</summary>
		public void Present()
		{
			if(_parent == null) return;
			_parent.IsExpanded = true;
			_parent.Present();
		}

		/// <summary>Removes item from listbox.</summary>
		public void Remove()
		{
			if(_parent == null)
			{
				var ctl = ListBox;
				if(ctl != null)
				{
					ctl.Items.Remove(this);
				}
			}
			else
			{
				_parent.Items.Remove(this);
			}
		}

		/// <summary>Removes item from listbox thread-safe.</summary>
		public void RemoveSafe()
		{
			if(_parent == null)
			{
				var ctl = ListBox;
				if(ctl != null)
				{
					ctl.Items.RemoveSafe(this);
				}
			}
			else
			{
				_parent.Items.RemoveSafe(this);
			}
		}

		/// <summary>Ensures that this item's position satisfies sort order.</summary>
		/// <returns>True if position was not changed.</returns>
		protected bool EnsureSortOrderSafe()
		{
			bool keepPosition = true;
			var items = ParentItems;
			if(items != null && items.Count != 1 && items.SortOrder != SortOrder.None)
			{
				var comparison = items.Comparison;
				if(comparison != null)
				{
					int order = items.SortOrder == SortOrder.Ascending ? 1 : -1;
					var index = items.IndexOf(this);
					if(index == 0)
					{
						var next = items[index + 1];
						keepPosition = order * comparison(this, next) <= 0;
					}
					else if(index == items.Count - 1)
					{
						var prev = items[index - 1];
						keepPosition = order * comparison(prev, this) <= 0;
					}
					else
					{
						var prev = items[index - 1];
						var next = items[index + 1];
						keepPosition = (order * comparison(prev, this) <= 0) &&
									   (order * comparison(this, next) <= 0);
					}
					if(!keepPosition)
					{
						var listBox = items.ListBox;
						if(listBox != null && listBox.InvokeRequired)
						{
							listBox.BeginInvoke(new MethodInvoker(Reinsert), null);
						}
						else
						{
							Reinsert();
						}
					}
				}
			}
			return keepPosition;
		}

		/// <summary>Reinserts this item.</summary>
		private void Reinsert()
		{
			var items = ParentItems;
			if(items != null)
			{
				items.Remove(this);
				items.Add(this);
			}
		}

		/// <summary>Repaints item.</summary>
		public void Invalidate()
		{
			if(IsAttachedToListBox && IsPresented)
			{
				ListBox.InvalidateItem(this);
			}
		}

		/// <summary>Repaints item in a thread-safe way.</summary>
		public void InvalidateSafe()
		{
			var listBox = ListBox;
			if(listBox != null)
			{
				if(listBox.InvokeRequired)
				{
					Action<CustomListBoxItem> action = listBox.InvalidateItem;
					listBox.BeginInvoke(action, new object[] { this });
				}
				else
				{
					listBox.InvalidateItem(this);
				}
			}
		}

		/// <summary>Repaints subitem.</summary>
		public void InvalidateSubItem(int id)
		{
			if(IsAttachedToListBox && IsPresented)
			{
				ListBox.InvalidateSubItem(this, id);
			}
		}

		/// <summary>Repaints subitem in a thread-safe way.</summary>
		public void InvalidateSubItemSafe(int id)
		{
			var listBox = ListBox;
			if(listBox != null && IsPresented)
			{
				if(listBox.InvokeRequired)
				{
					Action<CustomListBoxItem, int> action = listBox.InvalidateSubItem;
					listBox.BeginInvoke(action, this, id);
				}
				else
				{
					listBox.InvalidateSubItem(this, id);
				}
			}
		}

		#region Overrides

		/// <summary>Called when item is attached to listbox.</summary>
		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			_items.ListBox = ListBox;
		}

		/// <summary>Called when item is detached from listbox.</summary>
		protected override void OnListBoxDetached()
		{
			_items.ListBox = null;
			base.OnListBoxDetached();
		}

		/// <summary>Paints item background.</summary>
		/// <param name="paintEventArgs">Painting options.</param>
		protected override void OnPaintBackground(ItemPaintEventArgs paintEventArgs)
		{
			var state = paintEventArgs.State;

			if(state == ItemState.None) return;

			bool hovered	= (state & ItemState.Hovered)  == ItemState.Hovered;
			bool selected	= (state & ItemState.Selected) == ItemState.Selected;
			bool focused	= (state & ItemState.Focused)  == ItemState.Focused;
			IBackgroundStyle background = null;
			if(selected)
			{
				if(paintEventArgs.HostControlFocused)
				{
					if(hovered)
					{
						background = BackgroundStyle.SelectedFocused;
					}
					else if(focused)
					{
						background = BackgroundStyle.SelectedFocused;
					}
					else
					{
						background = BackgroundStyle.Selected;
					}
				}
				else
				{
					if(hovered)
					{
						background = BackgroundStyle.SelectedFocused;
					}
					else
					{
						background = BackgroundStyle.SelectedNoFocus;
					}
				}
			}
			else
			{
				if(hovered)
				{
					if(focused && paintEventArgs.HostControlFocused)
					{
						background = BackgroundStyle.HoveredFocused;
					}
					else
					{
						background = BackgroundStyle.Hovered;
					}
				}
				else if(focused)
				{
					if(paintEventArgs.HostControlFocused)
					{
						background = BackgroundStyle.Focused;
					}
				}
			}
			if(background != null)
			{
				background.Draw(paintEventArgs.Graphics, paintEventArgs.Bounds);
			}
		}

		/// <summary>Paint item content.</summary>
		/// <param name="paintEventArgs">Painting options.</param>
		protected override void OnPaintContent(ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;

			#region clip invisible subitems

			var clip = paintEventArgs.ClipRectangle;
			var clipX1 = clip.X;
			var clipX2 = clip.Right;
			var cols = ListBox.Columns;
			int colCount = cols.Count;
			int x = rect.X;

			int firstColId;
			int startColId;
			int endColId;
			int startX;

			if(clipX1 <= rect.X && clipX2 >= rect.Right)
			{
				// all subitems should be painted
				startColId = 0;
				firstColId = 0;
				endColId = colCount - 1;
				startX = x;
			}
			else
			{
				firstColId = -1;
				startColId = -1;
				endColId = -1;
				startX = -1;
				// skip clipped subitems
				int prev = -1;
				for(int i = 0; i < colCount; ++i)
				{
					var col = cols[i];
					if(col.IsVisible)
					{
						if(firstColId == -1)
						{
							firstColId = i;
						}

						int x2 = x + col.Width;

						if(startColId == -1 && x2 > clipX1)
						{
							if(prev != -1 && cols[prev].ExtendsToRight)
							{
								startColId = prev;
								startX = x - cols[prev].Width;
							}
							else
							{
								startColId = i;
								startX = x;
							}
						}

						if(startColId != -1 && endColId == -1 && x2 >= clipX2)
						{
							endColId = i++;
							for(; i < colCount; ++i)
							{
								if(cols[i].IsVisible)
								{
									if(cols[i].ExtendsToLeft)
									{
										endColId = i;
									}
									break;
								}
							}
							break;
						}

						x = x2;
						prev = i;
					}
				}
				// no visible columns found
				if(startColId == -1) return;
				if(endColId == -1) endColId = prev;
			}

			#endregion

			x = startX;
			bool first = startColId == firstColId;
			var subrect = new Rectangle(0, rect.Y, 0, rect.Height);

			int hoveredPart = paintEventArgs.HoveredPart;

			for(int i = startColId; i <= endColId; ++i)
			{
				var col = cols[i];
				if(col.IsVisible)
				{
					int w = col.Width;

					if(first)
					{
						first = false;
						var level = Level;
						var listBox = ListBox;
						int offset = level * ListBoxConstants.LevelMargin + ListBoxConstants.RootMargin;
						int w2 = w - offset;

						#region paint plus/minus

						if(listBox.ShowTreeLines)
						{
							if(!listBox.ShowRootTreeLines)
							{
								if(level != 0)
								{
									offset -= ListBoxConstants.LevelMargin;
									w2 += ListBoxConstants.LevelMargin;
								}
							}
							if(level != 0 || listBox.ShowRootTreeLines)
							{
								if(w2 > ListBoxConstants.SpaceBeforePlusMinus && _items.Count != 0)
								{
									Bitmap image;
									if(hoveredPart == ItemHitTestResults.PlusMinus)
									{
										image = (_expanded) ? (ImgCollapseHovered) : (ImgExpandHovered);
									}
									else
									{
										image = (_expanded) ? (ImgCollapse) : (ImgExpand);
									}
									Rectangle destRect, srcRect;
									if(w2 < ListBoxConstants.PlusMinusImageWidth + ListBoxConstants.SpaceBeforePlusMinus)
									{
										destRect = new Rectangle(
											x + offset,
											subrect.Y + (subrect.Height - ListBoxConstants.PlusMinusImageWidth) / 2,
											w2 - ListBoxConstants.SpaceBeforePlusMinus,
											ListBoxConstants.PlusMinusImageWidth);
										srcRect = new Rectangle(
											0,
											0,
											w2 - ListBoxConstants.SpaceBeforePlusMinus,
											ListBoxConstants.PlusMinusImageWidth);
									}
									else
									{
										destRect = new Rectangle(
											x + offset,
											subrect.Y + (subrect.Height - ListBoxConstants.PlusMinusImageWidth) / 2,
											ListBoxConstants.PlusMinusImageWidth,
											ListBoxConstants.PlusMinusImageWidth);
										srcRect = new Rectangle(
											0, 0,
											ListBoxConstants.PlusMinusImageWidth,
											ListBoxConstants.PlusMinusImageWidth);
									}
									graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
								}
								offset += ListBoxConstants.PlusMinusAreaWidth;
								w2 -= ListBoxConstants.PlusMinusAreaWidth;
							}
						}

						#endregion

						#region paint checkbox

						if(listBox.ShowCheckBoxes && _checkedState != CheckedState.Unavailable)
						{
							Bitmap checkedStateImage = null;
							if(hoveredPart == ItemHitTestResults.CheckBox)
							{
								ImgCheckedStateHovered.TryGetValue(_checkedState, out checkedStateImage);
							}
							else
							{
								ImgCheckedState.TryGetValue(_checkedState, out checkedStateImage);
							}
							if(checkedStateImage != null && w2 > ListBoxConstants.SpaceBeforeCheckbox)
							{
								Rectangle destRect, srcRect;
								if(w2 < ListBoxConstants.CheckboxImageWidth + ListBoxConstants.SpaceBeforeCheckbox)
								{
									destRect = new Rectangle(
										x + offset + ListBoxConstants.SpaceBeforeCheckbox,
										rect.Y + (rect.Height - ListBoxConstants.CheckboxImageWidth) / 2,
										w2 - ListBoxConstants.SpaceBeforeCheckbox,
										ListBoxConstants.CheckboxImageWidth);
									srcRect = new Rectangle(
										0, 0,
										w2 - ListBoxConstants.SpaceBeforeCheckbox,
										ListBoxConstants.CheckboxImageWidth);
								}
								else
								{
									destRect = new Rectangle(
										x + offset + ListBoxConstants.SpaceBeforeCheckbox,
										rect.Y + (rect.Height - ListBoxConstants.CheckboxImageWidth) / 2,
										ListBoxConstants.CheckboxImageWidth,
										ListBoxConstants.CheckboxImageWidth);
									srcRect = new Rectangle(
										0, 0,
										ListBoxConstants.CheckboxImageWidth,
										ListBoxConstants.CheckboxImageWidth);
								}
								graphics.DrawImage(checkedStateImage, destRect, srcRect, GraphicsUnit.Pixel);
							}
							offset += ListBoxConstants.CheckBoxAreaWidth;
							w2 -= ListBoxConstants.CheckBoxAreaWidth;
						}

						#endregion

						subrect.X = x + offset;
						subrect.Width = w2;
						x += w;
						if(w2 <= 0) continue;
					}
					else
					{
						subrect.X = x;
						subrect.Width = w;
						x += w;
					}
					
					OnPaintSubItem(new SubItemPaintEventArgs(paintEventArgs.Graphics, clip, subrect, paintEventArgs.Index,
						paintEventArgs.State, hoveredPart, paintEventArgs.HostControlFocused, i, col));
				}
			}
		}

		#endregion

		/// <summary>Override this to paint part of your item.</summary>
		/// <param name="paintEventArgs">Paint event args.</param>
		protected abstract void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs);

		/// <summary>Override this to provide subitem measurement.</summary>
		/// <param name="measureEventArgs">Measure event args.</param>
		protected abstract Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs);

		public Size MeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			return OnMeasureSubItem(measureEventArgs);
		}

		public virtual void OnMouseDown(MouseButtons button, int x, int y)
		{
		}

		public virtual void OnDoubleClick(int x, int y)
		{
			if(Items.Count != 0)
			{
				if(Level != 0 || ListBox.ShowRootTreeLines || !IsExpanded)
				{
					IsExpanded = !IsExpanded;
				}
			}
		}

		/// <summary>Perform item hit-testing.</summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <returns>Hit area id.</returns>
		protected override int OnHitTest(int x, int y)
		{
			var listBox = ListBox;
			var level = Level;
			int start = level * ListBoxConstants.LevelMargin + ListBoxConstants.RootMargin;
			int end;
			x += listBox.HScrollPos;
			if(listBox.ShowTreeLines && (level != 0 || listBox.ShowRootTreeLines))
			{
				if(level != 0 && !listBox.ShowRootTreeLines)
				{
					start -= ListBoxConstants.LevelMargin;
				}
				if(level != 0 || listBox.ShowRootTreeLines)
				{
					start += ListBoxConstants.SpaceBeforePlusMinus;
					end = start + ListBoxConstants.PlusMinusImageWidth;
					if(x >= start && x < end)
					{
						return Items.Count != 0 ? ItemHitTestResults.PlusMinus : ItemHitTestResults.Default;
					}
				}
				if(listBox.ShowCheckBoxes && _checkedState != CheckedState.Unavailable)
				{
					start += ListBoxConstants.PlusMinusImageWidth +
							 ListBoxConstants.SpaceAfterPlusMinus +
							 ListBoxConstants.SpaceBeforeCheckbox;
					end = start + ListBoxConstants.CheckboxImageWidth;
					if(x >= start && x < end) return ItemHitTestResults.CheckBox;
				}
			}
			else if(listBox.ShowCheckBoxes && _checkedState != CheckedState.Unavailable)
			{
				start += ListBoxConstants.SpaceBeforeCheckbox;
				end = start + ListBoxConstants.CheckboxImageWidth;
				if(x >= start && x < end) return ItemHitTestResults.CheckBox;
			}
			return ItemHitTestResults.Default;
		}

		/// <summary>Gets the context menu.</summary>
		///	<param name="requestEventArgs">Request parameters.</param>
		/// <returns>Context menu for specified location.</returns>
		public virtual ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(requestEventArgs == null) return null;
			ContextMenuRequested.Raise(this, requestEventArgs);
			return requestEventArgs.ContextMenu;
		}
	}

	/// <summary>Item with some attached data.</summary>
	/// <typeparam name="TData">Data type.</typeparam>
	public abstract class CustomListBoxItem<TData> : CustomListBoxItem
	{
		private TData _data;

		/// <summary>Create <see cref="CustomListBoxItem&lt;TData&gt;"/></summary>
		/// <param name="data">Associated data.</param>
		public CustomListBoxItem(TData data)
		{
			_data = data;
		}

		/// <summary>Item associated data.</summary>
		public TData DataContext
		{
			get { return _data; }
			set { _data = value; }
		}

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="CustomListBoxItem&lt;TData&gt;"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="CustomListBoxItem&lt;TData&gt;"/>.</returns>
		public override string ToString()
		{
			return "item: " + (_data == null?"(null)":_data.ToString());
		}
	}
}

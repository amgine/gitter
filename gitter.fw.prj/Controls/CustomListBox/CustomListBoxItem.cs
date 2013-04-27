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
			Verify.State.IsTrue(IsAttachedToListBox);

			return ListBox.StartTextEditor(this, column, font, text);
		}

		public CustomListBoxTextEditor StartTextEditor(CustomListBoxColumn column, string text)
		{
			Verify.State.IsTrue(IsAttachedToListBox);

			return ListBox.StartTextEditor(this, column, ListBox.Font, text);
		}

		/// <summary>Called when <see cref="M:CheckedState"/> property value changes.</summary>
		protected virtual void OnCheckedStateChanged()
		{
			CheckedStateChanged.Raise(this);
			var lb = ListBox;
			if(lb != null)
			{
				lb.NotifyItemCheckedChanged(this);
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
			if(IsAttachedToListBox) ListBox.NotifyItemActivated(this);
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
			Verify.State.IsTrue(IsAttachedToListBox);

			ListBox.FocusAndSelectItem(this);
		}

		/// <summary>Sets focus to this item.</summary>
		public void Focus()
		{
			Verify.State.IsTrue(IsAttachedToListBox);

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
			ListBox.Renderer.OnPaintItemBackground(this, paintEventArgs);
		}

		/// <summary>Paint item content.</summary>
		/// <param name="paintEventArgs">Painting options.</param>
		protected override void OnPaintContent(ItemPaintEventArgs paintEventArgs)
		{
			ListBox.Renderer.OnPaintItemContent(this, paintEventArgs);
		}

		#endregion

		internal void PaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			OnPaintSubItem(paintEventArgs);
		}

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
	/// <typeparam name="T">Data type.</typeparam>
	public abstract class CustomListBoxItem<T> : CustomListBoxItem
	{
		private T _dataContext;

		/// <summary>Create <see cref="CustomListBoxItem&lt;TData&gt;"/></summary>
		/// <param name="dataContext">Associated data.</param>
		public CustomListBoxItem(T dataContext)
		{
			_dataContext = dataContext;
		}

		/// <summary>Item associated data.</summary>
		public T DataContext
		{
			get { return _dataContext; }
			set { _dataContext = value; }
		}

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="CustomListBoxItem&lt;TData&gt;"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="CustomListBoxItem&lt;TData&gt;"/>.</returns>
		public override string ToString()
		{
			return "item: " + (_dataContext == null?"(null)":_dataContext.ToString());
		}
	}
}

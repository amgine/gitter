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

		private CustomListBoxItem _parent;
		private bool _isSelected;
		private bool _isExpanded;
		private CheckedState _checkedState = CheckedState.Unchecked;
		private bool _threeStateCheckboxMode;
		private int _cachedLevel = -1;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="CustomListBoxItem"/> class.</summary>
		public CustomListBoxItem()
		{
			Items = new CustomListBoxItemsCollection(null, this);
			Items.Changing += OnItemsChanging;
			Items.Changed  += OnItemsChanged;
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
			get => _parent;
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
			=> _parent == null
				? ListBox?.Items
				: _parent.Items;

		/// <summary>Child items.</summary>
		public CustomListBoxItemsCollection Items { get; }

		/// <summary>Tooltip text for an item.</summary>
		public string ToolTipText => GetToolTipText();

		/// <summary>Current <see cref="CheckedState"/> of this item.</summary>
		public CheckedState CheckedState
		{
			get => _checkedState;
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
			get => _checkedState == CheckedState.Checked;
			set => CheckedState = value ? CheckedState.Checked : CheckedState.Unchecked;
		}

		/// <summary>Item should operate as a three-state checkbox.</summary>
		public bool ThreeStateCheckboxMode
		{
			get => _threeStateCheckboxMode;
			set => _threeStateCheckboxMode = value;
		}

		/// <summary>Item level.</summary>
		public int Level
		{
			get
			{
				if(_cachedLevel == -1)
				{
					var level = 0;
					var p = _parent;
					while(p is not null)
					{
						++level;
						p = p.Parent;
					}
					_cachedLevel = level;
				}
				return _cachedLevel;
			}
		}

		/// <summary>Checks if item is selected.</summary>
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if(_isSelected != value)
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
						_isSelected = value;
					}
				}
			}
		}

		/// <summary>Returns true if this item is currently focused.</summary>
		public bool IsFocused => ListBox?.FocusedItem == this;

		/// <summary>Gets/sets item expanded state.</summary>
		public bool IsExpanded
		{
			get => _isExpanded;
			set
			{
				if(_isExpanded != value)
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
			get => !_isExpanded;
			set
			{
				if(_isExpanded == value)
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
				var p = _parent;
				while(p is not null)
				{
					if(!p.IsExpanded) return false;
					p = p.Parent;
				}
				return true;
			}
		}

		#endregion

		/// <summary>Returns tooltip text for an item.</summary>
		/// <returns>Tooltip text for an item.</returns>
		protected virtual string GetToolTipText() => null;

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
			CheckedStateChanged?.Invoke(this, EventArgs.Empty);
			ListBox?.NotifyItemCheckedChanged(this);
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
			Activated?.Invoke(this, EventArgs.Empty);
		}

		internal void SetSelected(bool selected)
		{
			_isSelected = selected;
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
			if(!_isExpanded)
			{
				_isExpanded = true;
				if(IsAttachedToListBox && IsPresented && Items.Count != 0)
				{
					ListBox.ExpandItem(this);
				}
			}
		}

		/// <summary>Expands whole tree of items starting at this item.</summary>
		public void ExpandAll()
		{
			if(Items.Count == 0) return;
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
			if(_isExpanded)
			{
				_isExpanded = false;
				if(IsAttachedToListBox && IsPresented && Items.Count != 0)
				{
					ListBox.CollapseItem(this);
				}
			}
		}

		/// <summary>Collapses whole tree of items starting at this item.</summary>
		public void CollapseAll()
		{
			if(Items.Count == 0) return;
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
			if(_parent is null)
			{
				var ctl = ListBox;
				if(ctl is not null)
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
			if(_parent is null)
			{
				var ctl = ListBox;
				if(ctl is not null)
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
			if(items is { Count: > 1, SortOrder: not SortOrder.None })
			{
				var comparison = items.Comparison;
				if(comparison is not null)
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
			if(items is not null)
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
			if(listBox is not null)
			{
				if(listBox.InvokeRequired)
				{
					Action<CustomListBoxItem> action = listBox.InvalidateItem;
					try
					{
						listBox.BeginInvoke(action, new object[] { this });
					}
					catch
					{
					}
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
			if(listBox is not null && IsPresented)
			{
				if(listBox.InvokeRequired)
				{
					Action<CustomListBoxItem, int> action = listBox.InvalidateSubItem;
					try
					{
						listBox.BeginInvoke(action, this, id);
					}
					catch
					{
					}
				}
				else
				{
					listBox.InvalidateSubItem(this, id);
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			Items.ListBox = ListBox;
		}

		/// <inheritdoc/>
		protected override void OnListBoxDetached()
		{
			Items.ListBox = null;
			base.OnListBoxDetached();
		}

		/// <inheritdoc/>
		protected override void OnPaintBackground(ItemPaintEventArgs paintEventArgs)
			=> ListBox.Renderer.OnPaintItemBackground(this, paintEventArgs);

		/// <inheritdoc/>
		protected override void OnPaintContent(ItemPaintEventArgs paintEventArgs)
			=> ListBox.Renderer.OnPaintItemContent(this, paintEventArgs);

		internal void PaintSubItem(SubItemPaintEventArgs paintEventArgs)
			=> OnPaintSubItem(paintEventArgs);

		/// <summary>Override this to paint part of your item.</summary>
		/// <param name="paintEventArgs">Paint event args.</param>
		protected virtual void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
			=> paintEventArgs.Column.PaintSubItem(paintEventArgs);

		/// <summary>Override this to provide subitem measurement.</summary>
		/// <param name="measureEventArgs">Measure event args.</param>
		protected virtual Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			=> measureEventArgs.Column.MeasureSubItem(measureEventArgs);

		public Size MeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			=> OnMeasureSubItem(measureEventArgs);

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

		/// <inheritdoc/>
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
			if(requestEventArgs is null) return null;
			ContextMenuRequested?.Invoke(this, requestEventArgs);
			return requestEventArgs.ContextMenu;
		}
	}

	/// <summary>Item with some attached data.</summary>
	/// <typeparam name="T">Data type.</typeparam>
	public abstract class CustomListBoxItem<T> : CustomListBoxItem
	{
		/// <summary>Create <see cref="CustomListBoxItem{TData}"/></summary>
		/// <param name="dataContext">Associated data.</param>
		public CustomListBoxItem(T dataContext)
		{
			DataContext = dataContext;
		}

		/// <summary>Item associated data.</summary>
		public T DataContext { get; set; }

		/// <inheritdoc/>
		public override string ToString()
			=> "item: " + DataContext is null ? "(null)" : DataContext.ToString();
	}
}

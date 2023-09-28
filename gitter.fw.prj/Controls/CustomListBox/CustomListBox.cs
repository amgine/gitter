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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Services;
using gitter.Framework.Configuration;

using Resources = gitter.Framework.Properties.Resources;

/// <summary>Highly customizable user-drawn list box with tree view capabilities.</summary>
[ToolboxBitmap(typeof(CustomListBox), "gitter.Framework.Resources.ui-list-box.png")]
[DefaultEvent(nameof(ItemActivated)), DefaultProperty(nameof(Items))]
[DesignerCategory("")]
public class CustomListBox : ScrollableControl
{
	#region Constants

	protected const int DEFAULT_ITEM_HEIGHT_BASE = 21;

	#endregion

	#region Events

	private static readonly object ItemActivatedEvent = new();
	/// <summary>Item is activated.</summary>
	public event EventHandler<ItemEventArgs> ItemActivated
	{
		add    => Events.AddHandler    (ItemActivatedEvent, value);
		remove => Events.RemoveHandler (ItemActivatedEvent, value);
	}

	protected virtual void OnItemActivated(CustomListBoxItem item)
		=> ((EventHandler<ItemEventArgs>)Events[ItemActivatedEvent])?.Invoke(this, new ItemEventArgs(item));

	private static readonly object ItemCheckedChangedEvent = new();
	/// <summary>Item checked status is changed.</summary>
	public event EventHandler<ItemEventArgs> ItemCheckedChanged
	{
		add    => Events.AddHandler    (ItemCheckedChangedEvent, value);
		remove => Events.RemoveHandler (ItemCheckedChangedEvent, value);
	}

	protected virtual void OnItemCheckedChanged(CustomListBoxItem item)
		=> ((EventHandler<ItemEventArgs>)Events[ItemCheckedChangedEvent])?.Invoke(this, new ItemEventArgs(item));

	private static readonly object SelectionChangedEvent = new();
	/// <summary>Selection is changed.</summary>
	public event EventHandler SelectionChanged
	{
		add    => Events.AddHandler    (SelectionChangedEvent, value);
		remove => Events.RemoveHandler (SelectionChangedEvent, value);
	}

	protected virtual void OnSelectionChanged()
		=> ((EventHandler)Events[SelectionChangedEvent])?.Invoke(this, EventArgs.Empty);

	private static readonly object ContextMenuRequestedEvent = new();
	/// <summary>Context menu for free space is requested.</summary>
	public event EventHandler<ContextMenuRequestEventArgs> ContextMenuRequested
	{
		add    => Events.AddHandler    (ContextMenuRequestedEvent, value);
		remove => Events.RemoveHandler (ContextMenuRequestedEvent, value);
	}

	private static readonly object ItemContextMenuRequestedEvent = new();
	/// <summary>Context menu for item is requested.</summary>
	public event EventHandler<ItemContextMenuRequestEventArgs> ItemContextMenuRequested
	{
		add    => Events.AddHandler    (ItemContextMenuRequestedEvent, value);
		remove => Events.RemoveHandler (ItemContextMenuRequestedEvent, value);
	}

	private static readonly object ItemsContextMenuRequestedEvent = new();
	/// <summary>Context menu for several items is requested.</summary>
	public event EventHandler<ItemsContextMenuRequestEventArgs> ItemsContextMenuRequested
	{
		add    => Events.AddHandler    (ItemsContextMenuRequestedEvent, value);
		remove => Events.RemoveHandler (ItemsContextMenuRequestedEvent, value);
	}

	#endregion

	#region Data

	private readonly ToolTip _tooltip;
	private TextBox _textEditor;
	private CustomListBoxRenderer _renderer;

	private int _baseColumnHeaderHeight;
	private int _baseItemHeight;
	private int _currentColumnHeaderHeight;
	private int _currentItemHeight;
	private int _itemWidth;

	private bool _disableContextMenus;
	private bool _integralScroll;
	private bool _showTreeLines;
	private bool _showRootTreeLines;
	private bool _showCheckBoxes;
	private bool _multiselect;
	private ItemActivation _itemActivation;

	private readonly CustomListBoxItemsCollection _items;
	private readonly CustomListBoxSelectedItemsCollection _selectedItems;
	private readonly CustomListBoxColumnsCollection _columns;
	private readonly List<CustomListBoxItem> _itemPlainList;

	private readonly ProcessOverlay _processOverlay;

	private int _mouseDownX;
	private int _mouseDownY;
	private HitTestResult _oldHitTestResult;

	private Rectangle _itemsArea;
	private Rectangle _headersArea;

	private HeaderStyle _headerStyle;
	private bool _allowColumnReorder;
	private CustomListBoxColumn _draggedHeader;
	private int _draggedHeaderIndex;
	private int _draggedHeaderPositionIndex;
	private int _draggedHeaderPosition;
	private DragHelper _headerDragHelper;
	private bool _haveAutoSizeColumns;

	private ColumnResizeProcess _columnResizeProcess;

	private readonly TrackingService<CustomListBoxItem> _itemHover;
	private readonly TrackingService<CustomListBoxItem> _itemFocus;
	private readonly TrackingService<CustomListBoxColumn> _headerHover;

	private int _lastClickedItemIndex;

	private bool _extenderVisible;

	#endregion

	#region Helper Structs

	/// <summary>Result of hit-testing.</summary>
	protected struct HitTestResult
	{
		public HitTestArea Area;
		public int ItemIndex;
		public int ItemPart;

		public HitTestResult(HitTestArea area, int itemIndex, int itemPart)
		{
			Area      = area;
			ItemIndex = itemIndex;
			ItemPart  = itemPart;
		}

		public bool Check(HitTestArea area, int itemIndex, int itemPart)
			=> ItemIndex == itemIndex && Area == area && ItemPart == itemPart;

		public bool Check(HitTestArea area, int itemPart)
			=> Area == area && ItemPart == itemPart;
	}

	protected enum HitTestArea
	{
		NonClient,
		FreeSpace,
		Item,
		Header,
	}

	#endregion

	/// <summary>Create <see cref="CustomListBox"/>.</summary>
	public CustomListBox()
	{
		_items = new CustomListBoxItemsCollection(this, null);
		_items.Changing += OnItemsChanging;
		_items.Changed += OnItemsChanged;

		_columns = new CustomListBoxColumnsCollection(this);
		_columns.Changed += OnColumnsChanged;

		_selectedItems = new CustomListBoxSelectedItemsCollection(this);
		_selectedItems.Changed += OnSelectedItemsChanged;

		_itemPlainList = new List<CustomListBoxItem>();

		_processOverlay = new ProcessOverlay(this);
		_processOverlay.Renderer = Style.OverlayRenderer;

		_itemHover = new TrackingService<CustomListBoxItem>(OnItemHoverChanged);
		_itemFocus = new TrackingService<CustomListBoxItem>(OnItemFocusChanged);
		_headerHover = new TrackingService<CustomListBoxColumn>(OnHeaderHoverChanged);
		_headerDragHelper = new DragHelper();

		_showRootTreeLines = true;
		_allowColumnReorder = true;
		_integralScroll = true;
		_lastClickedItemIndex = -1;
		_draggedHeaderIndex = -1;
		_baseColumnHeaderHeight = DEFAULT_ITEM_HEIGHT_BASE;
		_baseItemHeight = DEFAULT_ITEM_HEIGHT_BASE;

		UpdateSizes();

		_renderer = Style.ListBoxRenderer;

		BackColor = Renderer.BackColor;
		ForeColor = Renderer.ForeColor;

		_tooltip = new ToolTip()
		{
			InitialDelay = 350,
		};
	}

	/// <inheritdoc/>
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		UpdateSizes();
	}

	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		UpdateSizes();
		NotifyContentSizeChanged();
		Invalidate();
		base.OnDpiChangedAfterParent(e);
	}

	private void UpdateSizes()
	{
		var dpi = DeviceDpi;
		_currentItemHeight         = _baseItemHeight         * dpi / 96;
		_currentColumnHeaderHeight = _baseColumnHeaderHeight * dpi / 96;
	}

	private void OnItemHoverChanged(object sender, TrackingEventArgs<CustomListBoxItem> e)
	{
		InvalidateItem(e.Index);
	}

	private void OnItemFocusChanged(object sender, TrackingEventArgs<CustomListBoxItem> e)
	{
		InvalidateItem(e.Index);
	}

	private void OnHeaderHoverChanged(object sender, TrackingEventArgs<CustomListBoxColumn> e)
	{
		InvalidateColumn(e.Index);
	}

	private void OnSelectedItemsChanged(object sender, NotifyCollectionEventArgs e)
	{
		OnSelectionChanged();
	}

	private int _setItemPos = -1;
	private CustomListBoxItem _oldItem;

	internal void OnItemsChanging(object sender, NotifyCollectionEventArgs e)
	{
		bool plainListChanged = false;
		var items = (CustomListBoxItemsCollection)sender;
		var item = items.Parent;
		switch(e.Event)
		{
			case NotifyEvent.Clear:
				if(item is null)
				{
					_lastClickedItemIndex = -1;
					_itemFocus.Drop();
					_itemHover.Drop();
					if(_itemPlainList.Count != 0)
					{
						plainListChanged = true;
						_itemPlainList.Clear();
						_selectedItems.Clear();
						if(Created)
						{
							NotifyContentSizeChanged();
						}
					}
				}
				else
				{
					if(item.IsPresented)
					{
						if(items.Count != 0)
						{
							if(item.IsExpanded)
							{
								RemoveItems(items, 0, items.Count - 1);
								plainListChanged = true;
							}
							else
							{
								_oldItem = item;
							}
						}
					}
				}
				break;
			case NotifyEvent.Set:
				if((item is null) || (item.IsPresented && item.IsExpanded))
				{
					_oldItem = items[e.StartIndex];
					_setItemPos = _itemPlainList.IndexOf(_oldItem);
				}
				break;
			case NotifyEvent.Remove:
				if((item is null) || (item.IsPresented && item.IsExpanded))
				{
					RemoveItems(items, e.StartIndex, e.EndIndex);
					plainListChanged = true;
				}
				if(_itemPlainList.Count == 0)
				{
					Invalidate(_itemsArea);
				}
				break;
		}
		if(_haveAutoSizeColumns && plainListChanged)
		{
			InvalidateAutoSizeColumns();
		}
	}

	internal void OnItemsChanged(object sender, NotifyCollectionEventArgs e)
	{
		bool plainListChanged = false;
		var items = (CustomListBoxItemsCollection)sender;
		var item = items.Parent;
		switch(e.Event)
		{
			case NotifyEvent.Clear:
				if(_oldItem is not null)
				{
					InvalidateItem(_oldItem);
					_oldItem = null;
				}
				break;
			case NotifyEvent.Remove:
				break;
			case NotifyEvent.Set:
				if(_setItemPos != -1)
				{
					var newitem = items[e.StartIndex];
					_itemPlainList[_setItemPos] = newitem;
					var y1 = GetItemY1Offset(_setItemPos);
					int id = _setItemPos + 1;
					if(_oldItem.IsExpanded && _oldItem.Items.Count != 0)
					{
						int level = _oldItem.Level;
						int end = id + 1;
						while(end < _itemPlainList.Count && _itemPlainList[end].Level > level)
						{
							++end;
						}
						if(_itemFocus.IsTracked)
						{
							if(_itemFocus.Index >= id)
							{
								if(_itemFocus.Index >= end)
								{
									_itemFocus.ResetIndex(_itemFocus.Index - (end - id));
								}
								else
								{
									_itemFocus.Drop();
								}
							}
						}
						if(_itemHover.IsTracked)
						{
							if(_itemHover.Index >= id)
							{
								if(_itemHover.Index >= end)
								{
									_itemHover.ResetIndex(_itemHover.Index - (end - id));
								}
								else
								{
									_itemHover.Drop();
								}
							}
						}
						_itemPlainList.RemoveRange(id, end - id);
						plainListChanged = true;
					}
					if(newitem.IsExpanded && newitem.Items.Count != 0)
					{
						int end = id;
						foreach(var i in newitem.Items)
						{
							end = InsertItem(id, i); 
						}
						plainListChanged = true;
					}
					NotifyContentSizeChanged();
					_setItemPos = -1;
					_oldItem = null;
				}
				break;
			case NotifyEvent.Insert:
				{
					bool noitems = _itemPlainList.Count == 0;
					if(item is null)
					{
						int start = e.StartIndex;
						int end = e.EndIndex;
						int id = 0;
						if(start != 0)
						{
							if(end == items.Count - 1)
							{
								id = _itemPlainList.Count - (end - start);
							}
							else
							{
								var sitem = items[start - 1];
								while(sitem.Items.Count != 0 && sitem.IsExpanded)
								{
									sitem = sitem.Items[sitem.Items.Count - 1];
								}
								id = _itemPlainList.IndexOf(sitem) + 1;
							}
						}
						for(int i = start; i <= end; ++i)
						{
							id = InsertItem(id, items[i]);
						}
						plainListChanged = true;
						NotifyContentSizeChanged();
					}
					else
					{
						if(item.IsPresented)
						{
							if(item.IsExpanded)
							{
								int start = e.StartIndex;
								int end = e.EndIndex;
								int id = 0;
								if(start == 0)
								{
									id = _itemPlainList.IndexOf(item) + 1;
								}
								else
								{
									var sitem = items[start - 1];
									while(sitem.Items.Count != 0 && sitem.IsExpanded)
									{
										sitem = sitem.Items[sitem.Items.Count - 1];
									}
									id = _itemPlainList.IndexOf(sitem) + 1;
								}
								for(int i = start; i <= end; ++i)
								{
									id = InsertItem(id, items[i]);
								}
								NotifyContentSizeChanged();
								plainListChanged = true;
							}
							else
							{
								if(items.Count == e.ModifiedItems)
								{
									InvalidateItem(item);
								}
							}
						}
					}
					if(noitems) Invalidate(_itemsArea);
				}
				break;
		}
		if(_haveAutoSizeColumns && plainListChanged)
		{
			InvalidateAutoSizeColumns();
		}
	}

	private void InvalidateAutoSizeColumns()
	{
		foreach(var c in Columns)
		{
			if(c.SizeMode == ColumnSizeMode.Auto)
			{
				c.ContentWidth = -1;
			}
		}
		NotifyColumnLayoutChanged();
	}

	private static CustomListBoxItem FindLastVisibleItem(CustomListBoxItem item)
	{
		if(item.Items.Count == 0 || !item.IsExpanded) return item;
		return FindLastVisibleItem(item.Items[item.Items.Count - 1]);
	}

	private void OnColumnsChanged(object sender, NotifyCollectionEventArgs e)
	{
		UpdateAutoSizeColumnsContentWidth();
		NotifyContentSizeChanged();
	}

	internal void InvalidateItem(int index)
	{
		var rc = GetItemDisplayBounds(index);
		Invalidate(rc);
	}

	internal void InvalidateItem(CustomListBoxItem item)
	{
		if(IsDisposed) return;
		int index = _itemPlainList.IndexOf(item);
		var rc = GetItemDisplayBounds(index);
		Invalidate(rc);
	}

	public int GetInsertIndexFormPoint(int x, int y, bool insertBetween, out CustomListBoxItemsCollection collection)
	{
		if(x < ClientRectangle.X || x >= ClientRectangle.Right)
		{
			collection = null;
			return - 1;
		}
		y -= ClientRectangle.Y;
		if(HeaderStyle != HeaderStyle.Hidden)
		{
			y -= _headersArea.Height;
		}
		y += VScrollPos;
		if(insertBetween)
		{
			y += CurrentItemHeight / 2;
		}
		else
		{
			y += 0;
		}
		int index = y / CurrentItemHeight;
		if(index > _itemPlainList.Count)
		{
			index = _itemPlainList.Count;
		}
		if(index < 0)
		{
			collection = Items;
			return 0;
		}
		if(index > 0)
		{
			var item = _itemPlainList[index == _itemPlainList.Count ? index - 1: index];
			if(item.Parent != null)
			{
				collection = item.Parent.Items;
				index = collection.IndexOf(item);
				return index;
			}
			else
			{
				collection = Items;
				return index;
			}
		}
		else
		{
			collection = Items;
			return index;
		}
	}

	internal void InvalidateSubItem(CustomListBoxItem item, int columnId)
	{
		if(IsDisposed) return;
		int columnIndex = -1;
		for(int i = 0; i < _columns.Count; ++i)
		{
			var column = _columns[i];
			if(column.Id == columnId)
			{
				if(!column.IsVisible) return;
				columnIndex = i;
				break;
			}
		}
		if(columnIndex == -1) return;
		int itemIndex = _itemPlainList.IndexOf(item);
		if(itemIndex == -1) return;
		if(GetItemDisplayBounds(itemIndex) is { Width: > 0, Height: > 0 } itemDisplayRectangle)
		{
			var columnContentBounds = GetExtendedColumnContentRectangle(columnIndex);
			if(Rectangle.Intersect(itemDisplayRectangle, columnContentBounds) is { Width: > 0, Height: > 0 } clipRectangle)
			{
				Invalidate(clipRectangle);
			}
		}
	}

	internal void InvalidateColumn(int index)
	{
		if(index >= 0 && index < _columns.Count)
		{
			InvalidateColumn(_columns[index]);
		}
	}

	internal void InvalidateColumn(CustomListBoxColumn column)
	{
		if(!column.IsVisible) return;

		var x = column.Left - HScrollPos;
		var w = column.Width;
		var width = w.Value * DeviceDpi / w.Dpi.X;
		var rect  = new Rectangle(x + _headersArea.X, _headersArea.Y, width, _headersArea.Height);
		Invalidate(rect);
	}

	private Rectangle GetExtendedColumnContentRectangle(int index)
	{
		int startId = index;
		if(_columns[index].ExtendsToLeft)
		{
			// find any affected columns to the left
			for(int i = startId - 1; index >= 0; --i)
			{
				if(_columns[i].IsVisible)
				{
					startId = i;
					if(!_columns[i].ExtendsToLeft) break;
				}
			}
		}
		// find any affecting columns to the left
		for(int i = startId - 1; i >= 0; --i)
		{
			if(_columns[i].IsVisible)
			{
				if(_columns[i].ExtendsToRight)
					startId = i;
				else
					break;
			}
		}
		int endId = index;
		if(_columns[index].ExtendsToRight)
		{
			// find any affected columns to the right
			for(int i = endId + 1; index < _columns.Count; ++i)
			{
				if(_columns[i].IsVisible)
				{
					endId = i;
					if(!_columns[i].ExtendsToRight) break;
				}
			}
		}
		// find any affecting columns to the right
		for(int i = endId + 1; i < _columns.Count; ++i)
		{
			if(_columns[i].IsVisible)
			{
				if(_columns[i].ExtendsToLeft)
					endId = i;
				else
					break;
			}
		}
		// compute invalidated rectangle
		int x1 = _columns[startId].Left - HScrollPos;
		int x2 = _columns[endId  ].Left + _columns[endId].CurrentWidth - HScrollPos;
		int h = Math.Min(_itemPlainList.Count * CurrentItemHeight, _itemsArea.Height);
		return new Rectangle(x1 + _itemsArea.X, _itemsArea.Y, x2 - x1, h);
	}

	internal void InvalidateColumnContent(int index)
	{
		var rect = GetExtendedColumnContentRectangle(index);
		Invalidate(rect);
	}

	internal void InvalidateColumnContent(CustomListBoxColumn column)
	{
		int id = _columns.IndexOf(column);
		InvalidateColumnContent(id);
	}

	public int CurrentItemHeight => _currentItemHeight;

	public int BaseItemHeight
	{
		get => _baseItemHeight;
		set
		{
			Verify.Argument.IsPositive(value);

			if(_baseItemHeight != value)
			{
				_baseItemHeight = value;
				UpdateSizes();
				if(_items.Count != 0)
				{
					NotifyContentSizeChanged();
					Invalidate(_itemsArea);
				}
			}
		}
	}

	private int InsertItem(int index, CustomListBoxItem item)
	{
		int id = index;
		_itemPlainList.Insert(id++, item);
		if(item.IsSelected)
		{
			_selectedItems.Add(item);
		}
		if(_itemFocus.Index >= index)
		{
			_itemFocus.ResetIndex(_itemFocus.Index + 1);
		}
		if(_itemHover.Index >= index)
		{
			_itemHover.ResetIndex(_itemHover.Index + 1);
		}
		if(item.IsExpanded)
		{
			foreach(var i in item.Items)
			{
				id = InsertItem(id++, i);
			}
		}
		return id;
	}

	private void RemoveItems(CustomListBoxItemsCollection items, int startIndex, int endIndex)
	{
		var start = _itemPlainList.IndexOf(items[startIndex]);
		if(start == -1) return;
		var endItem = items[endIndex];
		var end = (startIndex == endIndex)?(start + 1):(_itemPlainList.IndexOf(endItem) + 1);
		if(endItem.IsExpanded && endItem.Items.Count != 0)
		{
			int level = endItem.Level;
			while(end < _itemPlainList.Count && _itemPlainList[end].Level > level)
			{
				++end;
			}
		}
		if(_itemFocus.IsTracked)
		{
			if(_itemFocus.Index >= startIndex)
			{
				if(_itemFocus.Index > endIndex)
				{
					_itemFocus.ResetIndex(_itemFocus.Index - (endIndex - startIndex + 1));
				}
				else
				{
					_itemFocus.Drop();
				}
			}
		}
		if(_itemHover.IsTracked)
		{
			if(_itemHover.Index >= startIndex)
			{
				if(_itemHover.Index > endIndex)
				{
					_itemHover.ResetIndex(_itemHover.Index - (endIndex - startIndex + 1));
				}
				else
				{
					_itemHover.Drop();
				}
			}
		}
		int selected = _selectedItems.Count;
		if(selected != 0)
		{
			for(int i = start; i < end; ++i)
			{
				if(_itemPlainList[i].IsSelected)
				{
					_selectedItems.Remove(_itemPlainList[i]);
					--selected;
					if(selected == 0) break;
				}
			}
		}
		_itemPlainList.RemoveRange(start, end - start);
		NotifyContentSizeChanged();
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public CustomListBoxRenderer Renderer
	{
		get => _renderer;
		set
		{
			Verify.Argument.IsNotNull(value);

			if(_renderer != value)
			{
				_renderer = value;
				BackColor = Renderer.BackColor;
				ForeColor = Renderer.ForeColor;
			}
		}
	}

	/// <summary>A method of activating an item.</summary>
	[DefaultValue(ItemActivation.DoubleClick)]
	[Description("A method of activating an item")]
	public ItemActivation ItemActivation
	{
		get => _itemActivation;
		set => _itemActivation = value;
	}

	/// <summary>Behave as a TreeView control.</summary>
	[DefaultValue(false)]
	[Description("Behave as a TreeView control")]
	public bool ShowTreeLines
	{
		get => _showTreeLines;
		set
		{
			if(_showTreeLines != value)
			{
				_showTreeLines = value;
				if(_itemPlainList.Count != 0)
				{
					Invalidate(_itemsArea);
				}
			}
		}
	}

	/// <summary>Show tree lines for zero-level items.</summary>
	[DefaultValue(true)]
	[Description("Show tree lines for zero-level items")]
	public bool ShowRootTreeLines
	{
		get => _showRootTreeLines;
		set
		{
			if(_showRootTreeLines != value)
			{
				_showRootTreeLines = value;
				if(_itemPlainList.Count != 0)
				{
					Invalidate(_itemsArea);
				}
			}
		}
	}

	/// <summary>Allow user to move column headers.</summary>
	[DefaultValue(true)]
	[Description("Allow user to move column headers")]
	public bool AllowColumnReorder
	{
		get => _allowColumnReorder;
		set => _allowColumnReorder = value;
	}

	/// <summary>Show checkboxes for items which support them.</summary>
	[DefaultValue(false)]
	[Description("Show checkboxes for items which support them")]
	public bool ShowCheckBoxes
	{
		get => _showCheckBoxes;
		set
		{
			if(_showCheckBoxes != value)
			{
				_showCheckBoxes = value;
				if(_itemPlainList.Count != 0)
				{
					Invalidate(_itemsArea);
				}
			}
		}
	}

	/// <summary>Allow user to select multiple items.</summary>
	[DefaultValue(false)]
	[Description("Allow user to select multiple items")]
	public bool Multiselect
	{
		get => _multiselect;
		set => _multiselect = value;
	}

	/// <summary>Disable all context menus.</summary>
	[DefaultValue(false)]
	[Description("Disable all context menus")]
	public bool DisableContextMenus
	{
		get => _disableContextMenus;
		set => _disableContextMenus = value;
	}

	/// <summary>Column headers style.</summary>
	[DefaultValue(HeaderStyle.Visible)]
	[Description("Column headers style")]
	public HeaderStyle HeaderStyle
	{
		get => _headerStyle;
		set
		{
			if(_headerStyle != value)
			{
				_headerStyle = value;
				Invalidate(ClientArea);
			}
		}
	}

	/// <summary>Column collection.</summary>
	[MergableProperty(false)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public CustomListBoxColumnsCollection Columns => _columns;

	/// <summary>Items collection.</summary>
	[MergableProperty(false)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public CustomListBoxItemsCollection Items => _items;

	/// <summary>Selected items collection.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public CustomListBoxSelectedItemsCollection SelectedItems => _selectedItems;

	/// <summary>Returns currently focused item.</summary>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public CustomListBoxItem FocusedItem => _itemFocus.Item;

	/// <summary>Progress monitor for actions which update list contents.</summary>
	public IProgress<OperationProgress> ProgressMonitor => _processOverlay;

	internal void NotifyItemActivated(CustomListBoxItem item)
	{
		OnItemActivated(item);
	}

	internal void NotifyItemCheckedChanged(CustomListBoxItem item)
	{
		OnItemCheckedChanged(item);
	}

	internal void NotifyColumnLayoutChanged()
	{
		UpdateAutoSizeColumnsContentWidth();
		RecomputeHeaderSizes();
		NotifyContentSizeChanged();
		Invalidate(ClientArea);
	}

	internal void ExpandItem(CustomListBoxItem item)
	{
		Assert.IsNotNull(item);

		if(item.Items.Count == 0 || !item.IsPresented) return;
		var itemId = _itemPlainList.IndexOf(item);
		if(itemId == -1) return;
		var id = itemId + 1;
		foreach(var i in item.Items)
		{
			id = InsertItem(id, i);
		}
		if(_lastClickedItemIndex > itemId)
		{
			_lastClickedItemIndex += id - itemId - 1;
		}
		NotifyContentSizeChanged();
		if(_haveAutoSizeColumns)
		{
			InvalidateAutoSizeColumns();
		}
	}

	internal void CollapseItem(CustomListBoxItem item)
	{
		Assert.IsNotNull(item);

		if(item.Items.Count == 0 || !item.IsPresented) return;
		var itemId = _itemPlainList.IndexOf(item);
		if(itemId == -1) return;

		var id = itemId + 1;
		int pos = id;
		var level = _itemPlainList[pos].Level;
		++pos;
		while(pos < _itemPlainList.Count && _itemPlainList[pos].Level >= level) ++pos;
		for(int i = id; i < pos && _selectedItems.Count != 0; ++i)
		{
			var s = _selectedItems.IndexOf(_itemPlainList[i]);
			if(s != -1) _selectedItems.RemoveAt(s);
		}
		_itemPlainList.RemoveRange(id, pos - id);
		if(_itemHover.Index > id)
		{
			if(_itemHover.Index < pos)
			{
				_itemHover.Drop();
			}
			else
			{
				_itemHover.ResetIndex(_itemHover.Index - (pos - id));
			}
		}
		if(_itemFocus.Index > id)
		{
			if(_itemFocus.Index < pos)
			{
				_itemFocus.Drop();
			}
			else
			{
				_itemFocus.ResetIndex(_itemFocus.Index - (pos - id));
			}
		}
		if(_lastClickedItemIndex >= id)
		{
			if(_lastClickedItemIndex < pos)
			{
				_lastClickedItemIndex = -1;
			}
			else
			{
				_lastClickedItemIndex -= pos - id;
			}
		}
		NotifyContentSizeChanged();
		if(_haveAutoSizeColumns)
		{
			InvalidateAutoSizeColumns();
		}
	}

	protected int GetItemY1Offset(int itemIndex)
		=> itemIndex * CurrentItemHeight - VScrollPos;

	protected int GetItemY2Offset(int itemIndex)
		=> GetItemY1Offset(itemIndex) + CurrentItemHeight - 1;

	protected Rectangle GetItemDisplayBounds(int itemIndex, bool clipToDisplay = true)
	{
		if(itemIndex < 0 || itemIndex >= _itemPlainList.Count) return Rectangle.Empty;
		var h  = CurrentItemHeight;
		int y  = itemIndex * h - VScrollPos;
		var rc = new Rectangle(_itemsArea.X, _itemsArea.Y + y, _itemsArea.Width, h);
		return clipToDisplay ? Rectangle.Intersect(rc, _itemsArea) : rc;
	}

	protected Rectangle GetColumnDisplayBounds(int columnIndex)
	{
		return GetColumnDisplayBoundst(_columns[columnIndex]);
	}

	protected Rectangle GetColumnDisplayBoundst(CustomListBoxColumn column)
	{
		if(!column.IsVisible) return Rectangle.Empty;
		return new Rectangle(column.Left + _headersArea.Left, _headersArea.Top, column.CurrentWidth, _headersArea.Height);
	}

	/// <summary>Ensures that item with id <paramref name="itemIndex"/> in plain item list is visible to user.</summary>
	/// <param name="itemIndex">Item index in plain list.</param>
	/// <returns>amount of scrolling occurred.</returns>
	protected int EnsureVisible(int itemIndex)
	{
		var itemHeight   = CurrentItemHeight;
		int maxScrollPos = itemIndex * itemHeight;
		int minScrollPos = maxScrollPos - _itemsArea.Height + itemHeight;
		if(minScrollPos < 0) minScrollPos = 0;
		if(_integralScroll)
		{
			var d = minScrollPos % itemHeight;
			if(d != 0)
			{
				minScrollPos += itemHeight - d;
			}
		}
		if(maxScrollPos > MaxVScrollPos) maxScrollPos = MaxVScrollPos;
		if(minScrollPos > MaxVScrollPos) minScrollPos = MaxVScrollPos;
		if(VScrollPos > maxScrollPos)
		{
			int d = maxScrollPos - VScrollPos;
			VScrollBar.Value = maxScrollPos;
			return d;
		}
		else if(VScrollPos < minScrollPos)
		{
			int d = minScrollPos - VScrollPos;
			VScrollBar.Value = minScrollPos;
			return d;
		}
		return 0;
	}

	/// <summary>Ensures that <paramref name="item"/> is visible to user.</summary>
	/// <param name="item">Item to display.</param>
	public void EnsureVisible(CustomListBoxItem item)
	{
		Verify.Argument.IsNotNull(item);
		Verify.Argument.IsTrue(item.ListBox == this, nameof(item), "This item is not owned by this list box.");

		var parent = item.Parent;
		while(parent != null)
		{
			parent.IsExpanded = true;
			parent = parent.Parent;
		}
		var id = _itemPlainList.IndexOf(item);
		EnsureVisible(id);
	}

	protected int GetColumnIndex(ref int x, int y)
	{
		int offset = 0;
		var dpi    = DeviceDpi;
		for(int i = 0; i < _columns.Count; ++i)
		{
			var column = _columns[i];
			if(column.IsVisible)
			{
				var w  = column.Width;
				var cw = w.Value * dpi / w.Dpi.X;
				if(x >= offset && x - offset < cw)
				{
					x -= offset;
					return i;
				}
				offset += cw;
			}
		}
		return -1;
	}

	protected int GetColumnX(int index)
	{
		int offset = 0;
		var dpi    = DeviceDpi;
		for(int i = 0; i < index; ++i)
		{
			var column = _columns[i];
			if(column.IsVisible)
			{
				var w  = column.Width;
				var cw = w.Value * dpi / w.Dpi.X;
				offset += cw;
			}
		}
		return offset;
	}

	public int GetOptimalColumnWidth(CustomListBoxColumn column)
	{
		Verify.Argument.IsNotNull(column);
		var columnIndex = _columns.IndexOf(column);
		Verify.Argument.IsTrue(columnIndex != -1, nameof(column), "Column is not present in this collection.");

		int maxw = column.MinWidth;
		foreach(var item in _itemPlainList)
		{
			var s = item.MeasureSubItem(new SubItemMeasureEventArgs(GraphicsUtility.MeasurementGraphics, item, columnIndex, column));
			if(s.Width > maxw) maxw = s.Width;
		}
		return maxw;
	}

	public CustomListBoxColumn GetPrevVisibleColumn(int index)
	{
		--index;
		while(index >= 0)
		{
			if(_columns[index].IsVisible) return _columns[index];
			--index;
		}
		return null;
	}

	public int GetNextVisibleColumnIndex(int index)
	{
		++index;
		while(index < _columns.Count)
		{
			if(_columns[index].IsVisible) return index;
			++index;
		}
		return -1;
	}

	public CustomListBoxColumn GetNextVisibleColumn(int index)
	{
		++index;
		while(index < _columns.Count)
		{
			if(_columns[index].IsVisible) return _columns[index];
			++index;
		}
		return null;
	}

	protected HitTestResult HitTest(int x, int y)
	{
		if(_headerStyle != HeaderStyle.Hidden && _headersArea.Contains(x, y))
		{
			y -= _headersArea.Y;
			x -= _headersArea.X - HScrollPos;
			if(x >= _itemWidth) return new HitTestResult(HitTestArea.Header, -1, -1);
			for(int i = 0; i < _columns.Count; ++i)
			{
				var c = _columns[i];
				if(c.IsVisible)
				{
					var cw = c.CurrentWidth;
					if(x < cw)
					{
						var columnHtr = c.HitTest(x, y);
						return new HitTestResult(HitTestArea.Header, i, columnHtr);
					}
					else
					{
						x -= cw;
					}
				}
			}
			return new HitTestResult(HitTestArea.Header, -1, -1);
		}
		if(_itemsArea.Contains(x, y))
		{
			x -= _itemsArea.X;
			if(x >= _itemWidth) return new HitTestResult(HitTestArea.FreeSpace, -1, -1);
			y -= _itemsArea.Y;
			int itemId = (VScrollPos + y) / CurrentItemHeight;
			if(itemId >= _itemPlainList.Count || itemId < 0)
			{
				return new HitTestResult(HitTestArea.FreeSpace, -1, -1);
			}
			else
			{
				var item = _itemPlainList[itemId];
				y -= GetItemY1Offset(itemId);
				var itemHtr = item.HitTest(x, y);
				return new HitTestResult(HitTestArea.Item, itemId, itemHtr);
			}
		}
		return new HitTestResult(HitTestArea.NonClient, -1, -1);
	}

	internal void FocusAndSelectItem(CustomListBoxItem item)
	{
		Verify.Argument.IsNotNull(item);
		Verify.Argument.IsTrue(item.ListBox == this, nameof(item), "Item is not owned by this ListBox");

		item.Present();
		FocusAndSelectItem(_itemPlainList.IndexOf(item));
	}

	private void FocusAndSelectItem(int itemIndex)
	{
		if(itemIndex < 0 || itemIndex >= _itemPlainList.Count)
		{
			_itemFocus.Drop();
		}
		else
		{
			_itemFocus.Track(itemIndex, _itemPlainList[itemIndex]);
		}
		_lastClickedItemIndex = itemIndex;
		var item = _itemPlainList[itemIndex];
		int c = item.IsSelected ? _selectedItems.Count : _selectedItems.Count + 1;
		var indices = System.Buffers.ArrayPool<int>.Shared.Rent(c);
		try
		{
			int selectedIntemsCount = 0;
			if(!item.IsSelected)
			{
				indices[selectedIntemsCount++] = itemIndex;
			}
			for(int i = 0; i < _selectedItems.Count; ++i)
			{
				int id = _itemPlainList.IndexOf(_selectedItems[i]);
				indices[selectedIntemsCount++] = id;
			}
			_selectedItems.Clear();
			_selectedItems.Add(item);
			var d = EnsureVisible(_itemFocus.Index);
			if(Math.Abs(d) <= _itemsArea.Height)
			{
				var rect = _itemsArea;
				if(d > 0)
				{
					rect.Height -= d;
				}
				else
				{
					rect.Y -= d;
					rect.Height += d;
				}
				Array.Sort(indices, 0, selectedIntemsCount);
				var rc = Rectangle.Empty;
				bool rc_active = false;
				var itemHeight = CurrentItemHeight;
				for(int i = 0; i < selectedIntemsCount; ++i)
				{
					int y1 = GetItemY1Offset(indices[i]);
					int y2 = y1 + itemHeight;
					if(y2 < 0) continue;
					if(y1 >= _itemsArea.Height) break;
					if(!rc_active)
					{
						rc = new Rectangle(_itemsArea.X, _itemsArea.Y + y1, _itemWidth, itemHeight);
						rc_active = true;
					}
					else
					{
						if(indices[i - 1] != indices[i] + 1)
						{
							rc = Rectangle.Intersect(rc, rect);
							Invalidate(rc);
							rc = new Rectangle(_itemsArea.X, _itemsArea.Y + y1, _itemWidth, itemHeight);
						}
						else
						{
							rc.Height += itemHeight;
						}
					}
				}
				if(rc_active)
				{
					rc = Rectangle.Intersect(rc, rect);
					Invalidate(rc);
				}
			}
		}
		finally
		{
			System.Buffers.ArrayPool<int>.Shared.Return(indices);
		}
	}

	internal void FocusItem(CustomListBoxItem item)
	{
		item.Present();
		int index = _itemPlainList.IndexOf(item);
		FocusItem(index);
	}

	private void FocusItem(int itemIndex)
	{
		_itemFocus.Track(itemIndex, _itemPlainList[itemIndex]);
		EnsureVisible(itemIndex);
	}

	/// <inheritdoc/>
	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
		e.IsInputKey = true;
		switch(e.KeyCode)
		{
			case Keys.Enter:
				if(_itemFocus.IsTracked)
				{
					var item = _itemFocus.Item;
					item.Activate();
				}
				break;
			case Keys.Escape:
				if(_columnResizeProcess != null)
				{
					_columnResizeProcess.Cancel();
					_columnResizeProcess = null;
				}
				break;
			case Keys.Right:
				if(_itemPlainList.Count == 0) return;
				if(_itemFocus.Index < 0 || _itemFocus.Index >= _itemPlainList.Count)
				{
					FocusAndSelectItem(0);
				}
				else
				{
					var item = _itemPlainList[_itemFocus.Index];
					if(item.Items.Count == 0) return;
					if(item.IsExpanded)
					{
						FocusAndSelectItem(_itemFocus.Index + 1);
					}
					else
					{
						item.IsExpanded = true;
					}
				}
				break;
			case Keys.Left:
				if(_itemPlainList.Count == 0) return;
				if(_itemFocus.Index < 0 || _itemFocus.Index >= _itemPlainList.Count)
				{
					FocusAndSelectItem(_itemPlainList.Count - 1);
				}
				else
				{
					var item = _itemPlainList[_itemFocus.Index];
					if(item.IsExpanded)
					{
						if(_showRootTreeLines || item.Level != 0)
						{
							item.IsExpanded = false;
						}
					}
					else
					{
						if(item.Parent is null) return;
						FocusAndSelectItem(_itemPlainList.IndexOf(item.Parent));
					}
				}
				break;
			case Keys.Home:
				if(_itemPlainList.Count == 0) return;
				if(_itemFocus.Index != 0)
				{
					FocusAndSelectItem(0);
				}
				else
				{
					EnsureVisible(_itemFocus.Index);
				}
				break;
			case Keys.End:
				if(_itemPlainList.Count == 0) return;
				if(_itemFocus.Index != _itemPlainList.Count - 1)
				{
					FocusAndSelectItem(_itemPlainList.Count - 1);
				}
				else
				{
					EnsureVisible(_itemFocus.Index);
				}
				break;
			case Keys.PageUp:
				if(_itemPlainList.Count == 0) return;
				{
					var index = _itemFocus.Index - GetItemsOnPage();
					if(index < 0) index = 0;
					if(_itemFocus.Index != index)
					{
						FocusAndSelectItem(index);
					}
					else
					{
						EnsureVisible(_itemFocus.Index);
					}
				}
				break;
			case Keys.PageDown:
				if(_itemPlainList.Count == 0) return;
				{
					var index = _itemFocus.Index + GetItemsOnPage();
					if(index >= _itemPlainList.Count) index = _itemPlainList.Count - 1;
					if(_itemFocus.Index != index)
					{
						FocusAndSelectItem(index);
					}
					else
					{
						EnsureVisible(_itemFocus.Index);
					}
				}
				break;
			case Keys.Up:
				if(_itemPlainList.Count == 0) return;
				if(_itemFocus.Index > 0)
		        {
					int index = _itemFocus.Index - 1;
					if(Control.ModifierKeys == Keys.Control)
					{
						FocusItem(index);
					}
					else
					{
						FocusAndSelectItem(index);
					}
				}
		        break;
		    case Keys.Down:
				if(_itemPlainList.Count == 0) return;
				if(_itemFocus.Index < _itemPlainList.Count - 1)
				{
					int index = _itemFocus.Index + 1;
					if(Control.ModifierKeys == Keys.Control)
					{
						FocusItem(index);
					}
					else
					{
						FocusAndSelectItem(index);
					}
				}
				break;
			case Keys.Space:
				switch(_selectedItems.Count)
				{
					case 0:
						if(_itemFocus.IsTracked)
						{
							FocusAndSelectItem(_itemFocus.Index);
						}
						break;
					default:
						if(Control.ModifierKeys == Keys.Control)
						{
							if(_itemFocus.IsTracked)
							{
								var item = _itemPlainList[_itemFocus.Index];
								if(item.IsSelected)
								{
									_selectedItems.Remove(item);
									InvalidateItem(_itemFocus.Index);
								}
								else if(_multiselect)
								{
									_selectedItems.Add(item);
									InvalidateItem(_itemFocus.Index);
								}
							}
						}
						else
						{
							if(_selectedItems.Count == 1)
							{
								if(_showCheckBoxes)
								{
									var item = _selectedItems[0];
									switch(item.CheckedState)
									{
										case CheckedState.Checked:
											item.CheckedState = CheckedState.Unchecked;
											break;
										case CheckedState.Unchecked:
											item.CheckedState = CheckedState.Checked;
											break;
									}
								}
								else
								{
									FocusAndSelectItem(_itemFocus.Index);
								}
							}
						}
						break;
				}
				break;
			case Keys.A:
				if(_multiselect && Control.ModifierKeys == Keys.Control)
				{
					if(_selectedItems.Count != _itemPlainList.Count)
					{
						_selectedItems.Clear();
						_selectedItems.AddRange(_itemPlainList);
						Invalidate(_itemsArea);
					}
				}
				break;
			case (Keys)0x5D:
				{
					ToolStripDropDown menu;
					var p = _itemsArea.Location;
					p.X += CurrentItemHeight / 2;
					int columnIndex = -1;
					CustomListBoxColumn column = null;
					for(int i = 0; i < _columns.Count; ++i)
					{
						if(_columns[i].IsVisible)
						{
							columnIndex = i;
							column = _columns[i];
							break;
						}
					}
					switch(_selectedItems.Count)
					{
						case 0:
							{
								var args = new ContextMenuRequestEventArgs(column, columnIndex, p.X, p.Y);
								Events.Raise(ContextMenuRequestedEvent, this, args);
								if(args.OverrideDefaultMenu)
								{
									menu = args.ContextMenu;
								}
								else
								{
									menu = GetFreeSpaceContextMenu(args);
								}
							}
							break;
						case 1:
							{
								var index = _itemPlainList.IndexOf(_selectedItems[0]);
								EnsureVisible(index);
								var y1 = GetItemY1Offset(index) + _itemsArea.Y;
								p.Y = y1 + CurrentItemHeight / 2;
								var args = new ItemContextMenuRequestEventArgs(_selectedItems[0], column,
									new Rectangle(_itemsArea.X, y1, _itemsArea.Width, CurrentItemHeight), columnIndex, p.X, p.Y);
								Events.Raise(ItemContextMenuRequestedEvent, this, args);
								if(args.OverrideDefaultMenu)
								{
									menu = args.ContextMenu;
								}
								else
								{
									menu = _selectedItems[0].GetContextMenu(args);
								}
							}
							break;
						default:
							{
								var args = new ItemsContextMenuRequestEventArgs(_selectedItems, column, columnIndex, p.X, p.Y);
								Events.Raise(ItemsContextMenuRequestedEvent, this, args);
								if(args.OverrideDefaultMenu)
								{
									menu = args.ContextMenu;
								}
								else
								{
									menu = GetMultiselectContextMenu(args);
								}
							}
							break;
					}
					if(menu is not null)
					{
						menu.Renderer = Style.ToolStripRenderer;
						menu.Show(this, p, ToolStripDropDownDirection.Default);
					}
				}
				break;
			default:
				e.IsInputKey = false;
				break;
		}
		base.OnPreviewKeyDown(e);
	}

	protected override void UpdateHover(int x, int y)
	{
		var htr = HitTest(x, y);

		bool wasOverResizeGrip =
			_oldHitTestResult.Check(HitTestArea.Header, ColumnHitTestResults.LeftResizer) ||
			_oldHitTestResult.Check(HitTestArea.Header, ColumnHitTestResults.RightResizer);

		bool isOverResizeGrip =
			htr.Check(HitTestArea.Header, ColumnHitTestResults.LeftResizer) ||
			htr.Check(HitTestArea.Header, ColumnHitTestResults.RightResizer);

		if(wasOverResizeGrip && !isOverResizeGrip)
		{
			Cursor = Cursors.Default;
		}
		else if(isOverResizeGrip && !wasOverResizeGrip)
		{
			Cursor = Cursors.VSplit;
		}

		switch(htr.Area)
		{
			case HitTestArea.Item:
				_headerHover.Drop();
				if(htr.ItemIndex != -1)
				{
					_itemHover.Track(htr.ItemIndex, _itemPlainList[htr.ItemIndex], htr.ItemPart);
				}
				else
				{
					_itemHover.Drop();
				}
				break;
			case HitTestArea.Header:
				if(htr.ItemIndex != -1)
				{
					_headerHover.Track(htr.ItemIndex, _columns[htr.ItemIndex], htr.ItemPart);
				}
				else
				{
					_headerHover.Drop();
				}
				_itemHover.Drop();
				break;
			default:
				_headerHover.Drop();
				_itemHover.Drop();
				break;
		}

		_oldHitTestResult = htr;
	}

	private int GetItemsOnPage()
	{
		int items = _itemsArea.Height / CurrentItemHeight;
		if(items <= 0) return 1;
		return items;
	}

	private void PerformHeaderDrag(MouseEventArgs e)
	{
		int dx = e.X - _mouseDownX;
		if(dx != 0)
		{
			_draggedHeaderPosition += dx;

			int bestdiff = int.MaxValue;
			int bestpos = -1;

			int offset = 0;
			int d = 0;
			int lastindex = -1;
			for(int i = 0; i < _columns.Count; ++i)
			{
				var c = _columns[i];
				if(c.IsVisible)
				{
					d = offset - _draggedHeaderPosition;
					if(d < 0) d = -d;
					if(d < bestdiff)
					{
						bestdiff = d;
						bestpos = i;
					}
					if(i != _draggedHeaderIndex)
					{
						offset += c.CurrentWidth;
					}
					if(i == _draggedHeaderPositionIndex)
					{
						offset += _draggedHeader.CurrentWidth;
					}
					d = offset - (_draggedHeaderPosition + _draggedHeader.CurrentWidth);
					if(d < 0) d = -d;
					if(d < bestdiff)
					{
						bestdiff = d;
						bestpos = i + 1;
					}
					lastindex = i;
				}
			}
			d = offset - _draggedHeaderPosition;
			if(d < 0) d = -d;
			if(d < bestdiff)
			{
				bestdiff = d;
				bestpos = lastindex + 1;
			}

			_draggedHeaderPositionIndex = bestpos;

			Invalidate(_headersArea);
			_mouseDownX = e.X;
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if(_draggedHeaderIndex != -1)
		{
			if(_headerDragHelper.IsTracking && _headerDragHelper.Update(e.X, e.Y))
			{
				PerformHeaderDrag(e);
			}
		}
		else if(_columnResizeProcess != null)
		{
			_columnResizeProcess.Update(e.Location);
		}
		else
		{
			UpdateHover(e.X, e.Y);
		}
		base.OnMouseMove(e);
	}

	private void UpdateColumnOffsets()
	{
		int offset = 0;
		for(int i = 0; i < _columns.Count; ++i)
		{
			var column = _columns[i];
			if(column.IsVisible)
			{
				column.Left = offset;
				offset += column.CurrentWidth;
			}
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if(_draggedHeaderIndex != -1)
		{
			if(_headerDragHelper.IsDragging)
			{
				if(_draggedHeaderPositionIndex != _draggedHeaderIndex)
				{
					_columns.Changed -= OnColumnsChanged;
					_columns.RemoveAt(_draggedHeaderIndex);
					if(_draggedHeaderPositionIndex > _draggedHeaderIndex)
					{
						--_draggedHeaderPositionIndex;
					}
					_columns.Insert(_draggedHeaderPositionIndex, _draggedHeader);
					_columns.Changed += OnColumnsChanged;
				}
				_draggedHeaderIndex = -1;
				UpdateColumnOffsets();
				Invalidate(ClientArea);
			}
			else
			{
				_columns[_draggedHeaderIndex].NotifyClick();
				_draggedHeaderIndex = -1;
				Invalidate(_headersArea);
			}
		}
		if(_headerDragHelper.IsTracking)
		{
			_headerDragHelper.Stop();
		}
		if(_columnResizeProcess != null)
		{
			_columnResizeProcess.Update(e.Location);
			_columnResizeProcess = null;
		}
		base.OnMouseUp(e);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);

		bool wasOverResizeGrip = 
			_oldHitTestResult.Check(HitTestArea.Header, ColumnHitTestResults.LeftResizer) ||
			_oldHitTestResult.Check(HitTestArea.Header, ColumnHitTestResults.RightResizer);

		if(wasOverResizeGrip)
		{
			_oldHitTestResult.Area = HitTestArea.NonClient;
			Cursor = Cursors.Default;
		}

		_itemHover.Drop();
		if(!_extenderVisible)
		{
			_headerHover.Drop();
		}

		_tooltip.Hide(this);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		_mouseDownX = e.X;
		_mouseDownY = e.Y;

		var htr = HitTest(e.X, e.Y);
		switch(htr.Area)
		{
			case HitTestArea.NonClient:
				break;
			case HitTestArea.Header:
				HandleHeaderMouseDown(htr.ItemIndex, htr.ItemPart, e);
				break;
			case HitTestArea.Item:
				HandleItemMouseDown(htr.ItemIndex, htr.ItemPart, e);
				break;
			case HitTestArea.FreeSpace:
				HandleFreeSpaceMouseDown(e);
				break;
		}
	}

	private void HandleHeaderExtenderMouseDown(int itemIndex, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left)
		{
			var c = _columns[itemIndex];
			var extender = c.Extender;
			if(extender != null)
			{
				_headerHover.Track(itemIndex, _columns[itemIndex]);
				_extenderVisible = true;
				extender.Closed += OnExtenderClosed;
				extender.Show(this, new Point(GetColumnX(itemIndex) + c.CurrentWidth - HScrollPos + 1, _headersArea.Bottom), ToolStripDropDownDirection.Left);
			}
		}
	}

	private void HandleHeaderResizerMouseDown(int itemIndex, ColumnResizeSide resizeSide, MouseEventArgs e)
	{
		_columnResizeProcess = ColumnResizeProcess.FromActiveColumn(Columns, itemIndex, resizeSide, e.Location);
	}

	private void HandleHeaderDefaultMouseDown(int itemIndex, MouseEventArgs e)
	{
		switch(e.Button)
		{
			case MouseButtons.Left:
				if(_allowColumnReorder)
				{
					if(_columns.Count > 1)
					{
						_draggedHeaderPositionIndex = _draggedHeaderIndex = itemIndex;
					}
					if(_draggedHeaderIndex != -1)
					{
						_draggedHeader = _columns[itemIndex];
						_draggedHeaderPosition = GetColumnX(itemIndex) - HScrollPos;
						_headerDragHelper.Start(e.X, e.Y);
						InvalidateColumn(_draggedHeaderIndex);
					}
					else
					{
						_draggedHeader = null;
					}
				}
				else
				{
					_draggedHeader = null;
				}
				break;
			case MouseButtons.Right:
				{
					var menu = new ContextMenuStrip();
					foreach(var c in Columns)
					{
						var item = new ToolStripMenuItem(c.Name, null)
						{
							Tag = c,
							Checked = c.IsVisible,
						};
						item.Click += (s, args) =>
						{
							var h = (CustomListBoxColumn)((ToolStripMenuItem)s).Tag;
							h.IsVisible = !h.IsVisible;
						};
						menu.Items.Add(item);
					}
					menu.Items.Add(new ToolStripSeparator());
					menu.Items.Add(new ToolStripMenuItem(
						Resources.StrColumns.AddEllipsis(), null, (s, args) => StartColumnsDialog()));
					Utility.MarkDropDownForAutoDispose(menu);
					menu.Renderer = Style.ToolStripRenderer;
					menu.Show(this, e.Location, ToolStripDropDownDirection.Default);
				}
				break;
		}
	}

	private void OnExtenderClosed(object sender, EventArgs e)
	{
		var extender = (ToolStripDropDown)sender;
		_extenderVisible = false;
		extender.Closed -= OnExtenderClosed;
		_headerHover.Drop();
	}

	public void StartColumnsDialog()
	{
		using(var dialog = new ColumnsDialog(this))
		{
			dialog.Run(this);
		}
	}

	private void HandleItemPlusMinusMouseDown(int itemIndex, MouseEventArgs e)
	{
		var item = _itemPlainList[itemIndex];
		if(item.Items.Count != 0)
		{
			item.IsExpanded = !item.IsExpanded;
		}
	}

	private void HandleItemCheckboxMouseDown(int itemIndex, MouseEventArgs e)
	{
		var item = _itemPlainList[itemIndex];
		switch(item.CheckedState)
		{
			case CheckedState.Checked:
				if(item.ThreeStateCheckboxMode)
				{
					item.CheckedState = CheckedState.Indeterminate;
				}
				else
				{
					item.CheckedState = CheckedState.Unchecked;
				}
				break;
			case CheckedState.Indeterminate:
				item.CheckedState = CheckedState.Unchecked;
				break;
			case CheckedState.Unchecked:
				item.CheckedState = CheckedState.Checked;
				break;
		}
	}

	private void HandleItemDefaultMouseDown(int itemIndex, MouseEventArgs e)
	{
		EnsureVisible(itemIndex);
		switch(Control.ModifierKeys)
		{
			case Keys.Control:
				if(_multiselect)
				{
					var id = _selectedItems.IndexOf(_itemPlainList[itemIndex]);
					if(id != -1)
					{
						_selectedItems.RemoveAt(id);
					}
					else
					{
						_itemFocus.Track(itemIndex, _itemPlainList[itemIndex]);
						_selectedItems.Add(_itemPlainList[itemIndex]);
					}
					InvalidateItem(itemIndex);
					_lastClickedItemIndex = itemIndex;
				}
				break;
			case Keys.Shift:
				if(_multiselect)
				{
					int startId = _lastClickedItemIndex;
					if(startId == -1) startId = _itemFocus.Index;
					if(startId == -1) startId = 0;
					int endId = itemIndex;
					if(endId < startId)
					{
						var temp = endId;
						endId = startId;
						startId = temp;
					}
					int minY = GetItemY1Offset(startId);
					int maxY = GetItemY2Offset(endId);
					var itemHeight = CurrentItemHeight;
					for(int i = 0; i < _selectedItems.Count; ++i)
					{
						int id = _itemPlainList.IndexOf(_selectedItems[i]);
						var y1 = GetItemY1Offset(id);
						if(y1 >= _itemsArea.Height || y1 <= -itemHeight)
						{
							continue;
						}
						var y2 = y1 + itemHeight - 1;
						if(y1 < minY) minY = y1;
						if(y2 > maxY) maxY = y2;
					}
					_selectedItems.Clear();
					for(int i = startId; i <= endId; ++i)
					{
						_selectedItems.Add(_itemPlainList[i]);
					}
					Invalidate(Rectangle.Intersect(_itemsArea, new Rectangle(_itemsArea.X, _itemsArea.Y + minY, _itemsArea.Width, maxY - minY + 1)));
				}
				break;
			default:
				var item = _itemPlainList[itemIndex];
				if(e.Button != MouseButtons.Right || !_multiselect || !item.IsSelected)
				{
					_itemFocus.Track(itemIndex, item);
					_lastClickedItemIndex = itemIndex;
					var rc = GetItemDisplayBounds(itemIndex);
					if(_selectedItems.Count == 0)
					{
						_selectedItems.Add(item);
						Invalidate(GetItemDisplayBounds(itemIndex));
					}
					else
					{
						if(_selectedItems.Count == 1)
						{
							if(_selectedItems[0] != _itemPlainList[itemIndex])
							{
								var index = _itemPlainList.IndexOf(_selectedItems[0]);
								_selectedItems[0] = item;
								Invalidate(GetItemDisplayBounds(index));
								Invalidate(rc);
							}
						}
						else
						{
							if(_selectedItems.Contains(item))
							{
								var indices = new int[_selectedItems.Count - 1];
								int j = 0;
								for(int i = 0; i < _selectedItems.Count; ++i)
								{
									if(_selectedItems[i] != item)
									{
										indices[j++] = _itemPlainList.IndexOf(_selectedItems[i]);
									}
								}
								_selectedItems.Clear();
								_selectedItems.Add(item);
								for(int i = 0; i < indices.Length; ++i)
								{
									Invalidate(GetItemDisplayBounds(indices[i]));
								}
							}
							else
							{
								var indices = new int[_selectedItems.Count + 1];
								indices[0] = itemIndex;
								for(int i = 0; i < _selectedItems.Count; ++i)
								{
									indices[i + 1] = _itemPlainList.IndexOf(_selectedItems[i]);
								}
								_selectedItems.Clear();
								_selectedItems.Add(item);
								for(int i = 0; i < indices.Length; ++i)
								{
									Invalidate(GetItemDisplayBounds(indices[i]));
								}
							}
						}
					}
					item.OnMouseDown(e.Button, e.X - rc.X, e.Y - rc.Y);
				}
				switch(e.Button)
				{
					case MouseButtons.Left:
						if(_itemActivation == ItemActivation.SingleClick)
						{
							item.Activate();
						}
						break;
					case MouseButtons.Right:
						if(!_disableContextMenus)
						{
							var x = e.X;
							var y = e.Y;
							var cid = GetColumnIndex(ref x, y);
							var col = (cid != -1) ? (_columns[cid]) : null;
							switch(_selectedItems.Count)
							{
								case 1:
									{
										int cmnuX = e.X - _itemsArea.X;
										int cmnuY = e.Y - _itemsArea.Y;
										var args = new ItemContextMenuRequestEventArgs(item, col,
											new Rectangle(0, itemIndex * CurrentItemHeight - VScrollPos, _itemsArea.Width, CurrentItemHeight),
											cid, cmnuX, cmnuY);
										Events.Raise(ItemContextMenuRequestedEvent, this, args);
										var menu = args.OverrideDefaultMenu ?
											args.ContextMenu :
											item.GetContextMenu(args);
										if(menu is not null)
										{
											menu.Renderer = Style.ToolStripRenderer;
											menu.Show(this, e.Location);
										}
									}
									break;
								default:
									{
										int cmnuX = e.X - _itemsArea.X;
										int cmnuY = e.Y - _itemsArea.Y;
										var args = new ItemsContextMenuRequestEventArgs(_selectedItems, col, cid, cmnuX, cmnuY);
										Events.Raise(ItemsContextMenuRequestedEvent, this, args);
										var menu = args.OverrideDefaultMenu ?
											args.ContextMenu :
											GetMultiselectContextMenu(args);
										if(menu != null)
										{
											menu.Renderer = Style.ToolStripRenderer;
											menu.Show(this, e.Location);
										}
									}
									break;
							}
						}
						break;
				}
				break;
		}
	}

	private void HandleHeaderMouseDown(int itemIndex, int itemPart, MouseEventArgs e)
	{
		if(itemIndex < 0 || itemIndex >= _columns.Count) return;
		switch(e.Button)
		{
			case MouseButtons.Left:
				switch(itemPart)
				{
					case ColumnHitTestResults.Extender:
						HandleHeaderExtenderMouseDown(itemIndex, e);
						break;
					case ColumnHitTestResults.LeftResizer:
						HandleHeaderResizerMouseDown(itemIndex, ColumnResizeSide.Left, e);
						break;
					case ColumnHitTestResults.RightResizer:
						HandleHeaderResizerMouseDown(itemIndex, ColumnResizeSide.Right, e);
						break;
					default:
						HandleHeaderDefaultMouseDown(itemIndex, e);
						break;
				}
				break;
			case MouseButtons.Right:
				HandleHeaderDefaultMouseDown(itemIndex, e);
				break;
		}
	}

	private void HandleItemMouseDown(int itemIndex, int itemPart, MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Right)
		{
			HandleItemDefaultMouseDown(itemIndex, e);
		}
		else
		{
			switch(itemPart)
			{
				case ItemHitTestResults.PlusMinus:
					HandleItemPlusMinusMouseDown(itemIndex, e);
					break;
				case ItemHitTestResults.CheckBox:
					HandleItemCheckboxMouseDown(itemIndex, e);
					break;
				default:
					HandleItemDefaultMouseDown(itemIndex, e);
					break;
			}
		}
	}

	private void HandleFreeSpaceMouseDown(MouseEventArgs e)
	{
		if(_selectedItems.Count > 0)
		{
			int[] indices = new int[_selectedItems.Count];
			for(int i = 0; i < indices.Length; ++i)
			{
				indices[i] = _itemPlainList.IndexOf(_selectedItems[i]);
			}
			_selectedItems.Clear();
			for(int i = 0; i < indices.Length; ++i)
			{
				Invalidate(GetItemDisplayBounds(indices[i]));
			}
		}
		if(e.Button == MouseButtons.Right)
		{
			if(!_disableContextMenus)
			{
				var x = e.X;
				var y = e.Y;
				var cid = GetColumnIndex(ref x, y);
				var col = (cid != -1) ? (_columns[cid]) : null;

				var args = new ContextMenuRequestEventArgs(col, cid, x, y);
				Events.Raise(ContextMenuRequestedEvent, this, args);
				ContextMenuStrip menu;
				if(args.OverrideDefaultMenu)
				{
					menu = args.ContextMenu;
				}
				else
				{
					menu = GetFreeSpaceContextMenu(args);
				}
				if(menu != null)
				{
					menu.Renderer = Style.ToolStripRenderer;
					menu.Show(this, e.Location);
				}
			}
		}
	}

	internal CustomListBoxTextEditor StartTextEditor(CustomListBoxItem item, CustomListBoxColumn column, Font font, string text)
	{
		StopTextEditor();
		EnsureVisible(item);
		int id = _itemPlainList.IndexOf(item);
		int y  = _itemsArea.Top + GetItemY1Offset(id);
		int x  = column.Left;
		int w  = column.CurrentWidth;
		int h  = CurrentItemHeight;
		var rc = new Rectangle(x, y, w, h);
		_textEditor = new TextBox()
		{
			Font             = font,
			BorderStyle      = BorderStyle.FixedSingle,
			Text             = text,
			BackColor        = Style.Colors.Window,
			ForeColor        = Style.Colors.WindowText,
			MinimumSize      = rc.Size,
			MaximumSize      = rc.Size,
			Bounds           = rc,
			Parent           = this,
			CausesValidation = true,
		};
		_textEditor.Focus();
		_textEditor.SelectAll();
		return new CustomListBoxTextEditor(this, _textEditor);
	}

	internal void StopTextEditor()
	{
		if(_textEditor is not null)
		{
			_textEditor.Dispose();
			_textEditor = null;
		}
	}

	protected virtual ContextMenuStrip GetMultiselectContextMenu(ItemsContextMenuRequestEventArgs requestEventArgs)
	{
		return null;
	}

	protected virtual ContextMenuStrip GetFreeSpaceContextMenu(ContextMenuRequestEventArgs requestEventArgs)
	{
		return null;
	}

	protected override void OnDoubleClick(EventArgs e)
	{
		_columnResizeProcess = null;
		switch(_oldHitTestResult.Area)
		{
			case HitTestArea.Item:
				if(_oldHitTestResult.ItemPart >= 0 && _itemFocus.IsTracked && _itemFocus.Index >= 0 && _itemFocus.Index < _itemPlainList.Count)
				{
					var item = _itemPlainList[_itemFocus.Index];
					item.OnDoubleClick(_mouseDownX, _mouseDownY);
					if(_itemActivation == ItemActivation.DoubleClick)
					{
						item.Activate();
					}
				}
				break;
			case HitTestArea.Header:
				bool isOverLeftResizeGrip  = _oldHitTestResult.ItemPart == ColumnHitTestResults.LeftResizer;
				bool isOverRightResizeGrip = _oldHitTestResult.ItemPart == ColumnHitTestResults.RightResizer;
				if(isOverLeftResizeGrip || isOverRightResizeGrip)
				{
					var index = _oldHitTestResult.ItemIndex;
					if(index != -1)
					{
						var autosizingColumn = _columns[index];
						if(isOverLeftResizeGrip)
						{
							var previousColumn = _columns.FindPrevious(index, static column => column.IsVisible);
							if(previousColumn is { SizeMode: ColumnSizeMode.Sizeable })
							{
								autosizingColumn = previousColumn;
							}
						}
						else
						{
							if(autosizingColumn.SizeMode != ColumnSizeMode.Sizeable)
							{
								var nextColumn = _columns.FindNext(index, static column => column.IsVisible);
								if(nextColumn is { SizeMode: ColumnSizeMode.Sizeable })
								{
									autosizingColumn = nextColumn;
								}
							}
						}
						if(autosizingColumn is { SizeMode: ColumnSizeMode.Sizeable })
						{
							autosizingColumn.AutoSize();
						}
					}
				}
				break;
		}
		base.OnDoubleClick(e);
	}

	public override void Refresh()
	{
		NotifyContentSizeChanged();
		base.Refresh();
	}

	public void ExpandAll()
	{
		foreach(var item in Items)
		{
			item.ExpandAll();
		}
	}

	public void CollapseAll()
	{
		foreach(var item in Items)
		{
			item.CollapseAll();
		}
	}

	private void UpdateAutoSizeColumnsContentWidth()
	{
		_haveAutoSizeColumns = false;
		int index = 0;
		foreach(var c in _columns)
		{
			if(!c.IsVisible) continue;
			if(c.SizeMode == ColumnSizeMode.Auto)
			{
				if(c.ContentWidth <= 0)
				{
					int max = 0;
					var dpi = Dpi.FromControl(this);
					var checkBoxWidth = ShowCheckBoxes
						? ListBoxConstants.SpaceBeforeCheckbox.GetValue(dpi) +
							ListBoxConstants.CheckboxImageWidth.GetValue(dpi) +
							ListBoxConstants.SpaceAfterCheckbox.GetValue(dpi)
						: 0;
					var treeOffset =
						ListBoxConstants.SpaceBeforePlusMinus.GetValue(dpi) +
						ListBoxConstants.PlusMinusImageWidth.GetValue(dpi) +
						ListBoxConstants.SpaceAfterPlusMinus.GetValue(dpi);
					var levelMargin = ListBoxConstants.LevelMargin.GetValue(dpi);

					foreach(var item in _itemPlainList)
					{
						var width = item.MeasureSubItem(new SubItemMeasureEventArgs(GraphicsUtility.MeasurementGraphics, item, c.Index, c)).Width;
						if(index == 0 && ShowTreeLines)
						{
							var level = item.Level;
							width += level * levelMargin + ListBoxConstants.RootMargin;
							if(!ShowRootTreeLines)
							{
								if(level != 0)
								{
									width -= levelMargin;
								}
							}
							if(level != 0 || ShowRootTreeLines)
							{
								width += treeOffset;
							}
						}
						if(ShowCheckBoxes && item.CheckedState != CheckedState.Unavailable)
						{
							width += checkBoxWidth;
						}
						if(width > max) max = width;
					}
					c.ContentWidth = max;
				}
				_haveAutoSizeColumns = true;
			}
			++index;
		}
	}

	protected void RecomputeHeaderSizes()
	{
		int free = _headersArea.Width;
		int nfill = 0;
		_itemWidth = 0;
		foreach(var c in _columns)
		{
			if(c.IsVisible)
			{
				switch(c.SizeMode)
				{
					case ColumnSizeMode.Fill:
						++nfill;
						break;
					case ColumnSizeMode.Auto:
						{
							if(c.ContentWidth >= 0)
							{
								c.SetWidth(new(c.ContentWidth, Dpi.FromControl(this)));
							}
							var cw = c.CurrentWidth;
							_itemWidth += cw;
							free       -= cw;
						}
						break;
					default:
						{
							var cw = c.CurrentWidth;
							_itemWidth += cw;
							free       -= cw;
						}
						break;
				}
			}
		}
		int offset = 0;
		int w = free/((nfill != 0)?(nfill):(1));
		if(w < 10) w = 10;
		foreach(var c in _columns)
		{
			if(c.IsVisible)
			{
				switch(c.SizeMode)
				{
					case ColumnSizeMode.Fill:
						--nfill;
						if(nfill == 0)
						{
							if(free < 10) free = 10;
							c.SetWidth(new(free, Dpi.FromControl(this)));
							_itemWidth += free;
						}
						else
						{
							c.SetWidth(new(w, Dpi.FromControl(this)));
							_itemWidth += w;
							free -= w;
						}
						break;
					case ColumnSizeMode.Auto:
						if(nfill == 0 && free > 0)
						{
							_itemWidth += free;
							c.SetWidth(new(c.CurrentWidth + free, Dpi.FromControl(this)));
							free = 0;
						}
						break;
				}
				c.Left = offset;
				offset += c.CurrentWidth;
			}
		}
	}

	/// <inheritdoc/>
	protected override void OnLostFocus(EventArgs e)
	{
		base.OnLostFocus(e);
		if(_selectedItems.Count != 0)
		{
			Invalidate(_itemsArea);
		}
		else
		{
			if(_itemFocus.IsTracked)
			{
				InvalidateItem(_itemFocus.Index);
			}
		}
	}

	/// <inheritdoc/>
	protected override void OnGotFocus(EventArgs e)
	{
		base.OnGotFocus(e);
		if(_selectedItems.Count != 0)
		{
			if(_selectedItems.Count == 1)
			{
				InvalidateItem(_selectedItems[0]);
				if(_itemFocus.IsTracked)
				{
					InvalidateItem(_itemFocus.Index);
				}
			}
			else
			{
				Invalidate(_itemsArea);
			}
		}
		else
		{
			if(_itemFocus.IsTracked)
			{
				InvalidateItem(_itemFocus.Index);
			}
		}
	}

	/// <inheritdoc/>
	protected override void OnStyleChanged(EventArgs e)
	{
		Renderer = Style.ListBoxRenderer;
		_processOverlay.Renderer = Style.OverlayRenderer;
		base.OnStyleChanged(e);
	}

	/// <inheritdoc/>
	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
		if(Items.Count == 0)
		{
			Invalidate();
		}
	}

	/// <inheritdoc/>
	protected override Size MeasureContent()
	{
		int w = 0;
		for(int i = 0; i < _columns.Count; ++i)
		{
			if(_columns[i].IsVisible)
			{
				if(_columns[i].SizeMode == ColumnSizeMode.Fill)
				{
					w += _columns[i].MinWidth;
				}
				else
				{
					w += _columns[i].CurrentWidth;
				}
			}
		}

		return new Size(w, CurrentItemHeight * _itemPlainList.Count);
	}

	/// <inheritdoc/>
	protected override int TransformVScrollPos(int position)
	{
		return _integralScroll ? (position - position % CurrentItemHeight) : (position);
	}

	/// <inheritdoc/>
	protected override int TransformMaxVScrollPos(int position)
	{
		if(_integralScroll)
		{
			var itemHeight = CurrentItemHeight;
			var d = position % itemHeight;
			return (d == 0) ? (position) : (position + itemHeight - d);
		}
		else
		{
			return position;
		}
	}

	/// <inheritdoc/>
	protected override void OnContentAreaChanged()
	{
		_itemsArea = ContentArea;
		_headersArea = ClientArea;
		if(HeaderStyle == HeaderStyle.Hidden)
		{
			_headersArea.Height = 0;
		}
		else
		{
			_headersArea.Height = _currentColumnHeaderHeight;
		}
		RecomputeHeaderSizes();
	}

	/// <inheritdoc/>
	protected override Rectangle GetContentArea(Rectangle clientArea)
	{
		if(HeaderStyle != HeaderStyle.Hidden)
		{
			clientArea.Y      += _currentColumnHeaderHeight;
			clientArea.Height -= _currentColumnHeaderHeight;
		}
		return clientArea;
	}

	/// <inheritdoc/>
	protected override int GetVScrollSmallChange() => CurrentItemHeight;

	/// <inheritdoc/>
	protected override void OnHScroll(int dx)
	{
		base.OnHScroll(dx);
		if(_textEditor is not null)
		{
			_textEditor.Left += dx;
		}
	}

	/// <inheritdoc/>
	protected override void OnVScroll(int dy)
	{
		base.OnVScroll(dy);
		if(_textEditor is not null)
		{
			_textEditor.Top += dy;
		}
	}

	private void PaintHeaders(PaintEventArgs e)
	{
		if(HeaderStyle == HeaderStyle.Hidden) return;

		var graphics = e.Graphics;
		var clipRect = e.ClipRectangle;
		var clip = Rectangle.Intersect(_headersArea, clipRect);

		if(clip.Width <= 0 || clip.Height <= 0) return;

		int x = _headersArea.X - HScrollPos;
		int y = _headersArea.Y;
		int clipX1 = clip.X;
		int clipX2 = clip.Right;

		graphics.SetClip(clip);
		var columnBounds = new Rectangle(x, y, 0, _headersArea.Height);

		bool isControlFocused = Focused;

		var dpi = Dpi.FromControl(this);
		if(_draggedHeaderIndex != -1)
		{
			int i = 0;
			CustomListBoxColumn column;
			while(i < _columns.Count)
			{
				if(i == _draggedHeaderPositionIndex)
				{
					x += _columns[_draggedHeaderIndex].CurrentWidth;
				}
				if(i != _draggedHeaderIndex)
				{
					column = _columns[i];
					if(column.IsVisible)
					{
						var w = column.CurrentWidth;
						if(x + w > clipX1)
						{
							columnBounds.X = x;
							columnBounds.Width = column.CurrentWidth;
							column.Paint(new ItemPaintEventArgs(
								graphics, dpi, clip, columnBounds, i, ItemState.None, ColumnHitTestResults.Default, isControlFocused));
						}
						x += column.CurrentWidth;
						if(x >= clipX2) break;
					}
				}
				++i;
			}
			if(_draggedHeaderPosition < clipX2)
			{
				column = _columns[_draggedHeaderIndex];
				var w = column.CurrentWidth;
				if(_draggedHeaderPosition + w > clipX1)
				{
					columnBounds.X = _draggedHeaderPosition;
					columnBounds.Width = w;
					column.Paint(new ItemPaintEventArgs(
						graphics, dpi, clip, columnBounds, i, ItemState.Pressed, ColumnHitTestResults.Default, isControlFocused));
				}
			}
		}
		else
		{
			for(int i = 0; i < _columns.Count; ++i)
			{
				var column = _columns[i];
				if(column.IsVisible)
				{
					var w = column.CurrentWidth;
					if(x + w > clipX1)
					{
						bool hovered = _headerHover.Index == i;
						var state = ItemState.None;
						int hoveredPart;
						if(hovered)
						{
							state |= ItemState.Hovered;
							hoveredPart = _extenderVisible ?
								ColumnHitTestResults.Extender : _headerHover.PartId;
						}
						else
						{
							hoveredPart = ColumnHitTestResults.Default;
						}
						columnBounds.X = x;
						columnBounds.Width = w;
						column.Paint(new ItemPaintEventArgs(
							graphics, dpi, clip, columnBounds, i, state, hoveredPart, isControlFocused));
					}
					x += column.CurrentWidth;
					if(x >= clipX2) break;
				}
			}
		}

		graphics.ResetClip();
	}

	private void PaintItems(PaintEventArgs e)
	{
		var graphics = e.Graphics;
		var clipRect = e.ClipRectangle;
		var clip = Rectangle.Intersect(_itemsArea, clipRect);

		if(clip.Width <= 0 || clip.Height <= 0) return;

		if(_itemPlainList.Count == 0)
		{
			if(_processOverlay.IsVisible)
			{
				graphics.SetClip(clip);
				var overlayBounds = GetOverlayBounds();
				_processOverlay.OnPaint(graphics, overlayBounds);
			}
			else
			{
				if(!string.IsNullOrEmpty(Text))
				{
					graphics.SetClip(clip);
					var overlayBounds = GetOverlayBounds();
					_processOverlay.DrawMessage(graphics, overlayBounds, Text);
				}
			}
		}
		else
		{
			var dpi = Dpi.FromControl(this);

			var itemHeight = CurrentItemHeight;
			int index = (clip.Y - _itemsArea.Y + VScrollPos) / itemHeight;
			int y = _itemsArea.Y - VScrollPos + index * itemHeight;
			int x = _itemsArea.X - HScrollPos;
			var itemBounds = new Rectangle(x, y, _itemWidth, itemHeight);
			int clippingEdge = clip.Bottom;
			bool isControlFocused = Focused;

			while(index < _itemPlainList.Count && itemBounds.Y < clippingEdge)
			{
				var item     = _itemPlainList[index];
				var state    = GetItemState(item, index, out var hoveredPart);
				var itemClip = Rectangle.Intersect(itemBounds, clip);
				graphics.SetClip(itemClip);
				item.Paint(new ItemPaintEventArgs(
					graphics, dpi, itemClip, itemBounds, index, state, hoveredPart, isControlFocused));
				itemBounds.Y += itemHeight;
				++index;
			}

			if(_processOverlay.IsVisible)
			{
				var overlayBounds = GetOverlayBounds();
				if(Rectangle.Intersect(clip, overlayBounds) is { Width: > 0, Height: > 0 } overlayClip)
				{
					graphics.SetClip(overlayClip);
					_processOverlay.OnPaint(graphics, overlayBounds);
				}
			}
		}

		graphics.ResetClip();
	}

	private ItemState GetItemState(CustomListBoxItem item, int index, out int hoveredPart)
	{
		var state = ItemState.None;
		if(item.IsSelected)
		{
			state |= ItemState.Selected;
		}
		if(_itemFocus.Index == index)
		{
			state |= ItemState.Focused;
		}
		if(_itemHover.Index == index)
		{
			state |= ItemState.Hovered;
			hoveredPart = _itemHover.PartId;
		}
		else
		{
			hoveredPart = ItemHitTestResults.Default;
		}
		return state;
	}

	/// <inheritdoc/>
	protected override void OnPaintClientArea(PaintEventArgs paintEventArgs)
	{
		var graphics = paintEventArgs.Graphics;
		graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
		graphics.TextContrast      = GraphicsUtility.TextContrast;

		PaintHeaders(paintEventArgs);
		PaintItems(paintEventArgs);
	}

	/// <inheritdoc/>
	protected override void OnDragOver(DragEventArgs drgevent)
	{
		var p = PointToClient(new Point(drgevent.X, drgevent.Y));
		if(Math.Abs(p.Y - _itemsArea.Y) < 5 && VScrollPos != 0)
		{
			StartScrollTimer(-1 * CurrentItemHeight);
		}
		else if(Math.Abs(p.Y - _itemsArea.Bottom) < 5 && VScrollPos != MaxVScrollPos)
		{
			StartScrollTimer( 1 * CurrentItemHeight);
		}
		else
		{
			StopScrollTimer();
		}
		UpdateHover(p.X, p.Y);
		base.OnDragOver(drgevent);
	}

	/// <inheritdoc/>
	protected override void OnDragLeave(EventArgs e)
	{
		_itemHover.Drop();
		StopScrollTimer();
		base.OnDragLeave(e);
	}

	/// <inheritdoc/>
	protected override void OnDragDrop(DragEventArgs drgevent)
	{
		StopScrollTimer();
		base.OnDragDrop(drgevent);
	}

	private Rectangle GetOverlayBounds()
	{
		var w = Math.Min((int)(_itemsArea.Width  * 0.8), 300 * DeviceDpi / 96);
		var h = Math.Min((int)(_itemsArea.Height * 0.5),  85 * DeviceDpi / 96);
		return new Rectangle(
			_itemsArea.Left + (_itemsArea.Width  - w) / 2,
			_itemsArea.Top  + (_itemsArea.Height - h) / 2,
			w, h);
	}

	protected virtual void SaveMoreViewTo(Section section)
	{
	}

	public void SaveViewTo(Section section)
	{
		Verify.Argument.IsNotNull(section);

		var columnsSection = section.GetCreateSection("Columns");
		columnsSection.Clear();
		foreach(var column in Columns)
		{
			var columnSection = columnsSection.CreateSection(column.IdentificationString);
			column.SaveTo(columnSection);
		}
		SaveMoreViewTo(section);
	}

	protected virtual void LoadMoreViewFrom(Section section)
	{
	}

	public void LoadViewFrom(Section section)
	{
		Verify.Argument.IsNotNull(section);

		BeginUpdate();
		var columnsSection = section.TryGetSection("Columns");
		if(columnsSection != null)
		{
			var dict = new Dictionary<string, CustomListBoxColumn>(_columns.Count);
			foreach(var c in _columns)
			{
				dict.Add(c.IdentificationString, c);
			}
			_columns.Clear();

			foreach(var child in columnsSection.Sections)
			{
				var name = child.Name;
				if(dict.TryGetValue(name, out var column))
				{
					column.LoadFrom(child);
					_columns.Add(column);
					dict.Remove(name);
				}
			}
			foreach(var column in dict.Values)
			{
				_columns.Add(column);
			}
		}
		LoadMoreViewFrom(section);
		EndUpdate();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_tooltip.Dispose();

			_items.Changing -= OnItemsChanging;
			_items.Changed -= OnItemsChanged;
			_itemPlainList.Clear();
			_items.Clear();

			_columns.Clear();

			if(_textEditor is not null)
			{
				_textEditor.Dispose();
				_textEditor = null;
			}
		}
		base.Dispose(disposing);
	}
}

#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

#nullable enable

namespace gitter.Framework.Controls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

/// <summary>Splits controls by movable splitters.</summary>
[System.ComponentModel.ToolboxItem(defaultType: false)]
[System.ComponentModel.DesignerCategory("")]
internal sealed class ViewSplit : ContainerControl
{
	private readonly ViewSplitSlots _slots;
	private Size _size;

	/// <summary>
	/// Replace <paramref name="origin"/> with a <see cref="ViewSplit"/>, containing
	/// <paramref name="origin"/> and <paramref name="sibling"/> controls.
	/// </summary>
	/// <param name="origin">Original control.</param>
	/// <param name="sibling">Inserted control.</param>
	/// <param name="side"><paramref name="sibling"/> position.</param>
	/// <returns>Created <see cref="ViewSplit"/>.</returns>
	public static ViewSplit? Replace(Control origin, Control sibling, AnchorStyles side)
	{
		Control item1;
		Control item2;
		Orientation orientation;
		var bounds = origin.Bounds;
		var parent = origin.Parent;
		if(parent is null) return null;
		switch(side)
		{
			case AnchorStyles.Left:
				item1 = sibling;
				item2 = origin;
				orientation = Orientation.Horizontal;
				break;
			case AnchorStyles.Top:
				item1 = sibling;
				item2 = origin;
				orientation = Orientation.Vertical;
				break;
			case AnchorStyles.Right:
				item1 = origin;
				item2 = sibling;
				orientation = Orientation.Horizontal;
				break;
			case AnchorStyles.Bottom:
				item1 = origin;
				item2 = sibling;
				orientation = Orientation.Vertical;
				break;
			default:
				throw new ArgumentException($"Invalid side: {side}.", nameof(side));
		}
		parent.DisableRedraw();
		parent.SuspendLayout();
		try
		{
			Wrap(origin, origin.Bounds, orientation, item1, item2, out var slot1, out var slot2);
			var split = new ViewSplit(orientation, bounds, slot1, slot2);
			split.SuspendLayout();
			try
			{
				split.Bounds = bounds;
				split.Parent = parent;

				switch(parent)
				{
					case ViewSplit psplit:
						var index = psplit.IndexOf(origin);
						psplit[index].Content = split;
						break;
					case DockPanel ppanel:
						ppanel.RootControl = split;
						break;
					case FloatingViewForm pfloat:
						pfloat.RootControl = split;
						if(origin is ViewHost vh)
						{
							pfloat.EnterMulticontrolMode();
							pfloat.UpdateLayout();
							vh.Status = ViewHostStatus.DockedOnFloat;
						}
						break;
					default:
						throw new ApplicationException($"Unsupported parent control type: {parent?.GetType().Name}.");
				}

				item1.Parent = split;
				item2.Parent = split;
			}
			finally
			{
				split.ResumeLayout(performLayout: false);
			}
			return split;
		}
		finally
		{
			parent.ResumeLayout(performLayout: true);
			parent.EnableRedraw();
			parent.RedrawWindow();
		}
	}

	/// <summary>Gets optimal sizes for splitter controls.</summary>
	/// <param name="context">Control of interest.</param>
	/// <param name="available">Available space.</param>
	/// <param name="minimum">Minimum control size.</param>
	/// <param name="s1">Size of the first control.</param>
	/// <param name="s2">Size of the second control.</param>
	public static void OptimalSizes(Control context, int available, IDpiBoundValue<int> minimum, ref int s1, ref int s2)
	{
		Assert.IsNotNull(context);

		var dpi     = Dpi.FromControl(context);
		var spacing = ViewConstants.Spacing.GetValue(dpi).Width;
		var min     = minimum.GetValue(dpi);
		if(s1 >= available * 0.9 && s2 >= available * 0.9)
		{
			available -= spacing;
			s1 = available / 2;
			s2 = available - s1;
		}
		else
		{
			if(s1 == available)
			{
				s1 -= s2 + spacing;
				if(s1 < min)
				{
					int d = min - s1;
					s1 = min;
					s2 -= d;
				}
			}
			else if(s2 == available)
			{
				s2 -= s1 + spacing;
				if(s2 < min)
				{
					int d = min - s2;
					s2 = min;
					s1 -= d;
				}
			}
			else
			{
				int freew = available - (s1 + spacing + s2);
				if(freew != 0)
				{
					int f1 = freew / 2;
					int f2 = freew - f1;
					s1 += f1;
					s2 += f2;
				}
			}
		}
	}

	/// <summary>Compares control's priority.</summary>
	/// <param name="c1">First control.</param>
	/// <param name="c2">Second control.</param>
	/// <returns>Result of comparing <paramref name="c1"/> and <paramref name="c2"/>.</returns>
	private static int ComparePriority(Control c1, Control c2)
	{
		var dw1 = (c1 is ViewHost  { IsDocumentWell:       true })
		       || (c1 is ViewSplit { ContainsDocumentWell: true });
		var dw2 = (c2 is ViewHost  { IsDocumentWell:       true })
		       || (c2 is ViewSplit { ContainsDocumentWell: true });
		if(dw1 == dw2) return 0;
		if(dw1) return -1;
		return 1;
	}

	private static int GetSize(Rectangle bounds, Orientation orientation)
		=> orientation switch
		{
			Orientation.Horizontal => bounds.Width,
			Orientation.Vertical   => bounds.Height,
			_ => throw new ArgumentException(),
		};

	private static int GetSize(Size size, Orientation orientation)
		=> orientation switch
		{
			Orientation.Horizontal => size.Width,
			Orientation.Vertical   => size.Height,
			_ => throw new ArgumentException(),
		};

	private static ViewSplitSlotSize AbsoluteSize(Control item, Orientation orientation)
		=> ViewSplitSlotSize.Absolute(Dpi.FromControl(item), GetSize(item.Size, orientation));

	private static int GetMaxSize(Size size, Orientation orientation, Dpi dpi)
	{
		var conv = DpiConverter.FromDefaultTo(dpi);
		var maxSize = GetSize(size, orientation);
		switch(orientation)
		{
			case Orientation.Horizontal:
				maxSize -= ViewConstants.MinimumHostWidth.GetValue(dpi)  + ViewConstants.Spacing.GetValue(dpi).Width;
				break;
			case Orientation.Vertical:
				maxSize -= ViewConstants.MinimumHostHeight.GetValue(dpi) + ViewConstants.Spacing.GetValue(dpi).Height;
				break;
		}
		return maxSize;
	}

	public static void Wrap(Control? context, Rectangle bounds, Orientation orientation, Control item1, Control item2, out ViewSplitSlot slot1, out ViewSplitSlot slot2)
	{
		Verify.Argument.IsNotNull(item1);
		Verify.Argument.IsNotNull(item2);

		var dpi = context is not null ? Dpi.FromControl(context) : Dpi.Default;

		var w1 = item1 is ViewHost { IsDocumentWell: true } or ViewSplit { ContainsDocumentWell: true };
		var w2 = item2 is ViewHost { IsDocumentWell: true } or ViewSplit { ContainsDocumentWell: true };

		ViewSplitSlotSize size1;
		ViewSplitSlotSize size2;
		if(w1 != w2)
		{
			var spacing = orientation switch
			{
				Orientation.Horizontal => ViewConstants.Spacing.GetValue(dpi).Width,
				Orientation.Vertical   => ViewConstants.Spacing.GetValue(dpi).Height,
				_ => throw new ArgumentException($"Unknown orientation: {orientation}", nameof(orientation)),
			};

			if(w1)
			{
				size1 = ViewSplitSlotSize.Leftover();
				size2 = AbsoluteSize(item2, orientation);
				var maxSize = GetMaxSize(bounds.Size, orientation, dpi);
				if(size2.Size * size2.Dpi.GetValue(orientation) / dpi.GetValue(orientation) > maxSize)
				{
					size2 = ViewSplitSlotSize.Absolute(dpi, (GetSize(bounds, orientation) - spacing) / 2);
				}
			}
			else
			{
				size1 = AbsoluteSize(item1, orientation);
				size2 = ViewSplitSlotSize.Leftover();
				var maxSize = GetMaxSize(bounds.Size, orientation, dpi);
				if(size1.Size * size1.Dpi.GetValue(orientation) / dpi.GetValue(orientation) > maxSize)
				{
					size1 = ViewSplitSlotSize.Absolute(dpi, (GetSize(bounds, orientation) - spacing) / 2);
				}
			}
		}
		else
		{
			size1 = ViewSplitSlotSize.Relative(0.5f);
			size2 = ViewSplitSlotSize.Leftover();
		}

		slot1 = new ViewSplitSlot(item1, size1);
		slot2 = new ViewSplitSlot(item2, size2);
	}

	/// <summary>Initializes a new instance of the <see cref="ViewSplit"/> class.</summary>
	/// <param name="orientation">Control flow orientation.</param>
	/// <param name="bounds">Bounds.</param>
	/// <param name="slot1">First slot.</param>
	/// <param name="slot2">Second slot.</param>
	public ViewSplit(Orientation orientation, Rectangle bounds, ViewSplitSlot slot1, ViewSplitSlot slot2)
	{
		Verify.Argument.IsNotNull(slot1);
		Verify.Argument.IsNotNull(slot2);

		AutoScaleMode = AutoScaleMode.None;

		SetStyle(ControlStyles.ContainerControl, true);
		SetStyle(ControlStyles.Selectable, false);

		BackColor = ViewManager.Renderer.BackgroundColor;

		Orientation = orientation;
		_slots = new(this);
		_slots.Add(slot1);
		_slots.Add(slot2);
		_size = bounds.Size;
		slot1.Content.Dock = DockStyle.None;
		slot2.Content.Dock = DockStyle.None;
		_slots.Apply();
	}

	/// <summary>Initializes a new instance of the <see cref="ViewSplit"/> class.</summary>
	/// <param name="orientation">Control flow orientation.</param>
	/// <param name="bounds">Bounds.</param>
	/// <param name="slots">Slots.</param>
	public ViewSplit(Orientation orientation, Rectangle bounds, IEnumerable<ViewSplitSlot> slots)
	{
		Verify.Argument.IsNotNull(slots);

		AutoScaleMode = AutoScaleMode.None;

		SetStyle(ControlStyles.ContainerControl, true);
		SetStyle(ControlStyles.Selectable, false);

		BackColor = ViewManager.Renderer.BackgroundColor;

		Orientation = orientation;
		_slots = new(this);
		foreach(var slot in slots)
		{
			slot.Content.Dock = DockStyle.None;
			_slots.Add(slot);
		}
		_size = bounds.Size;
		_slots.Apply();
	}

	/// <summary>Layout orientation.</summary>
	public Orientation Orientation { get; }

	/// <summary>Gets or sets <see cref="ViewSplitSlot"/> at the specified index.</summary>
	public ViewSplitSlot this[int index]
	{
		get => _slots[index];
		set => _slots[index] = value;
	}

	/// <summary>Gets the hosted control count.</summary>
	/// <value>Hosted control count.</value>
	public int Count => _slots.Count;

	/// <summary>Returns index of hosted <paramref name="control"/>.</summary>
	/// <param name="control">Control to look for.</param>
	/// <returns>Index of specified <paramref name="control"/> or -1, if it was not found.</returns>
	public int IndexOf(Control control)
	{
		for(int i = 0; i < _slots.Count; ++i)
		{
			if(_slots[i].Content == control)
			{
				return i;
			}
		}
		return -1;
	}

	public IReadOnlyList<ViewSplitSlot> Slots => _slots;

	/// <summary>
	/// Gets a value indicating whether this <see cref="ViewSplit"/> contains <see cref="ViewHost"/> with
	/// <see cref="M:ViewHost.IsDocumentWell"/> == true.
	/// </summary>
	public bool ContainsDocumentWell
	{
		get
		{
			foreach(var ctl in _slots)
			{
				switch(ctl.Content)
				{
					case ViewHost host when host.IsDocumentWell:
						return true;
					case ViewSplit split when split.ContainsDocumentWell:
						return true;
				}
			}
			return false;
		}
	}

	/// <summary>Removes the specified control.</summary>
	/// <param name="item">Control to remove.</param>
	public void Remove(Control item)
	{
		Verify.Argument.IsNotNull(item);

		var index = IndexOf(item);
		if(index >= 0)
		{
			RemoveAt(index);
		}
	}

	/// <summary>Removes slot at specified position.</summary>
	/// <param name="index">Index of the slot to remove.</param>
	/// <returns>Removed slot.</returns>
	public ViewSplitSlot RemoveAt(int index)
	{
		Verify.Argument.IsValidIndex(index, _slots.Count);

		if(_slots.Count == 2)
		{
			int singleIndex = index == 0 ? 1 : 0;
			var parent = Parent;
			SuspendLayout();
			var removedItem = _slots[index];
			var singleItem  = _slots[singleIndex];
			_slots.Clear();
			removedItem.Content.Parent = null;
			singleItem.Content.Bounds = Bounds;
			singleItem.Content.Anchor = Anchor;
			singleItem.Content.Parent = parent;
			Parent = null;
			switch(parent)
			{
				case ViewSplit viewSplit:
					// embed remaining item into this split's old position.
					{
						bool hasWells = false;
						foreach(var slot in viewSplit._slots)
						{
							if(slot.Content == this)
							{
								slot.Content = singleItem.Content;
							}
							else
							{
								if(slot.Content is ViewHost { IsDocumentWell: true } or ViewSplit { ContainsDocumentWell: true })
								{
									hasWells = true;
								}
							}
						}
						var isWell = singleItem.Content is ViewHost { IsDocumentWell: true } or ViewSplit { ContainsDocumentWell: true };
						if(hasWells == isWell)
						{
							var total = GetSize(viewSplit.Size, viewSplit.Orientation);
							total -= ViewConstants.Spacing.GetValue(Dpi.FromControl(viewSplit)).Width * (viewSplit._slots.Count - 1);
							for(int i = 0; i < viewSplit._slots.Count - 1; ++i)
							{
								viewSplit._slots[i].Size = ViewSplitSlotSize.Relative(GetSize(viewSplit._slots[i].Content.Size, viewSplit.Orientation) / (float)total);
							}
							viewSplit._slots[viewSplit._slots.Count - 1].Size = ViewSplitSlotSize.Leftover();
						}
						else
						{
							foreach(var slot in viewSplit._slots)
							{
								if(slot.Content is ViewHost { IsDocumentWell: true } or ViewSplit { ContainsDocumentWell: true })
								{
									slot.Size = ViewSplitSlotSize.Leftover();
								}
								else
								{
									slot.Size = ViewSplitSlotSize.Absolute(Dpi.FromControl(slot.Content), GetSize(slot.Content.Size, viewSplit.Orientation));
								}
							}
						}
					}
					break;
				case DockPanel dockPanel:
					dockPanel.RootControl = singleItem.Content;
					break;
				case FloatingViewForm floatingForm:
					floatingForm.RootControl = singleItem.Content;
					if(singleItem.Content is ViewHost th)
					{
						floatingForm.LeaveMulticontrolMode();
						th.Status = ViewHostStatus.Floating;
					}
					singleItem.Content.Dock = DockStyle.Fill;
					break;
				default:
					throw new ApplicationException($"Unexpected {nameof(ViewSplit)}.Parent: {parent}");
			}
			ResumeLayout(false);
			Dispose();
			return removedItem;
		}
		else
		{
			var removedItem = _slots[index];
			removedItem.Content.Parent = null;
			_slots.RemoveAt(index);
			_slots.Apply();
			return removedItem;
		}
	}

	/// <inheritdoc/>
	protected override void OnCreateControl()
	{
		_slots.Apply();
		base.OnCreateControl();
	}

#if NETCOREAPP || NET48_OR_GREATER
	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void OnDpiChangedAfterParent(EventArgs e)
	{
		_slots.Apply();
		base.OnDpiChangedAfterParent(e);
	}
#endif

	/// <inheritdoc/>
	protected override void OnMouseEnter(EventArgs e)
	{
		Cursor = Orientation switch
		{
			Orientation.Horizontal => Cursors.SizeWE,
			Orientation.Vertical   => Cursors.SizeNS,
			_ => throw new ApplicationException(),
		};
		base.OnMouseEnter(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseLeave(EventArgs e)
	{
		Cursor = Cursors.Default;
		base.OnMouseLeave(e);
	}

	/// <inheritdoc/>
	protected override void OnResize(EventArgs eventargs)
	{
		base.OnResize(eventargs);
		if(Size is not { Width: > 0, Height: > 0 } size) return;
		_slots.Apply();
	}

	/// <inheritdoc/>
	protected override void OnMouseDown(MouseEventArgs e)
	{
		if(e.Button == MouseButtons.Left)
		{
			_slots.StartMoving(e.Location);
		}
		base.OnMouseDown(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseMove(MouseEventArgs e)
	{
		if(_slots.IsMoving)
		{
			_slots.UpdateMoving(e.Location);
		}
		base.OnMouseMove(e);
	}

	/// <inheritdoc/>
	protected override void OnMouseUp(MouseEventArgs e)
	{
		if(_slots.IsMoving)
		{
			_slots.CommitMoving(e.Location);
		}
		base.OnMouseUp(e);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_slots.Dispose();
		}
		base.Dispose(disposing);
	}
}

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

namespace gitter.Framework.Controls;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

sealed class ViewSplitSlots : IReadOnlyList<ViewSplitSlot>, IDisposable
{
	private readonly List<ViewSplitSlot> _slots = new(capacity: 2);

	internal ViewSplitSlots(ViewSplit viewSplit)
	{
		ViewSplit = viewSplit;
	}

	private ViewSplit ViewSplit { get; }

	public ViewSplitSlot this[int index]
	{
		get => _slots[index];
		set => _slots[index] = value;
	}

	public int Count => _slots.Count;

	private static int GetSize(ViewSplitSlotSize size, Dpi dpi, int total)
		=> size.Type switch
		{
			ViewSplitSlotSizeType.Absolute => size.Size * dpi.X / size.Dpi.X,
			ViewSplitSlotSizeType.Relative => (int)(total * size.Fraction + 0.5f),
			_ => 0,
		};

	public void Add(ViewSplitSlot slot)
	{
		_slots.Add(slot);
	}

	public void RemoveAt(int index)
	{
		_slots.RemoveAt(index);
	}

	public void Clear()
	{
		_slots.Clear();
	}

	public void Apply()
	{
		var width   = ViewSplit.Width;
		var height  = ViewSplit.Height;
		var dpi     = Dpi.FromControl(ViewSplit);
		var spacing = ViewConstants.Spacing.GetValue(dpi);
		int leftover;
		switch(ViewSplit.Orientation)
		{
			case Orientation.Horizontal:
				{
					width -= (_slots.Count - 1) * spacing.Width;
					leftover = width;
					foreach(var slot in _slots)
					{
						if(slot.Size.Type != ViewSplitSlotSizeType.Leftover)
						{
							var size = GetSize(slot.Size, dpi, width);
							if(size < 0) size = 0;
							slot.CurrentSize = size;
							leftover -= size;
						}
					}
				}
				break;
			case Orientation.Vertical:
				{
					height -= (_slots.Count - 1) * spacing.Height;
					leftover = height;
					foreach(var slot in _slots)
					{
						if(slot.Size.Type != ViewSplitSlotSizeType.Leftover)
						{
							var size = GetSize(slot.Size, dpi, height);
							if(size < 0) size = 0;
							slot.CurrentSize = size;
							leftover -= size;
						}
					}
				}
				break;
			default: throw new ApplicationException();
		}
		var offset = 0;
		switch(ViewSplit.Orientation)
		{
			case Orientation.Horizontal:
				{
					foreach(var slot in _slots)
					{
						if(slot.Size.Type == ViewSplitSlotSizeType.Leftover)
						{
							slot.CurrentSize = leftover;
						}
						slot.Content.Bounds = new Rectangle(offset, 0, slot.CurrentSize, height);
						offset += slot.CurrentSize + spacing.Width;
					}
				}
				break;
			case Orientation.Vertical:
				{
					foreach(var slot in _slots)
					{
						if(slot.Size.Type == ViewSplitSlotSizeType.Leftover)
						{
							slot.CurrentSize = leftover;
						}
						slot.Content.Bounds = new Rectangle(0, offset, width, slot.CurrentSize);
						offset += slot.CurrentSize + spacing.Height;
					}
				}
				break;
			default: throw new ApplicationException();
		}
	}

	private SplitterMarker? _movingSplitMarker;
	private int _movingOffset;
	private int _movingMin;
	private int _movingMax;

	private int GetSpacing()
	{
		var dpi  = Dpi.FromControl(ViewSplit);
		return ViewSplit.Orientation switch
		{
			Orientation.Horizontal => ViewConstants.Spacing.GetValue(dpi).Width,
			Orientation.Vertical   => ViewConstants.Spacing.GetValue(dpi).Height,
			_ => throw new ApplicationException(
				$"Unexpected {nameof(ViewSplit)}.{nameof(ViewSplit.Orientation)} value: {ViewSplit.Orientation}"),
		};
	}

	/// <summary>Gets minimum and maximum splitter positions for specified splitter.</summary>
	/// <param name="index">The index of splitter to check bounds for.</param>
	/// <param name="min">Minimum splitter position.</param>
	/// <param name="max">Maximum splitter position.</param>
	private void GetResizeBounds(int index, out int min, out int max)
	{
		min = 0;
		max = 0;
		var spacing = GetSpacing();
		for(int i = 0; i < index; ++i)
		{
			var d = _slots[i].CurrentSize + spacing;
			min += d;
			max += d;
		}
		max += _slots[index].CurrentSize + spacing;
		if(index < _slots.Count - 1)
		{
			max += _slots[index + 1].CurrentSize + spacing;
		}
	}

	/// <summary>Gets a value indicating whether some splitter is moving.</summary>
	/// <value><c>true</c> if some splitter is moving; otherwise, <c>false</c>.</value>
	public bool IsMoving { get; private set; }

	/// <summary>Gets the index of the moving splitter.</summary>
	/// <value>Index of the moving splitter.</value>
	public int MovingSplitterIndex { get; private set; }

	/// <summary>Finds splitter index which contains specified <paramref name="position"/>.</summary>
	/// <param name="position">Position to check for.</param>
	/// <param name="offset">SPlitter offset.</param>
	/// <returns>Splitter index or -1 if no splitter was found at specified <paramref name="position"/>.</returns>
	private int FindSplitter(Point position, out int offset)
	{
		var pos = ViewSplit.Orientation switch
		{
			Orientation.Horizontal => position.X,
			Orientation.Vertical   => position.Y,
			_ => throw new ApplicationException($"Unexpected {nameof(ViewSplit)}.{nameof(ViewSplit.Orientation)} value: {ViewSplit.Orientation}"),
		};
		offset = 0;
		var spacing = GetSpacing();
		for(int i = 0; i < _slots.Count - 1; ++i)
		{
			offset += _slots[i].CurrentSize;
			if(pos >= offset && pos < offset + spacing)
			{
				return i;
			}
			offset += spacing;
		}
		return -1;
	}

	/// <summary>Spawns the split marker.</summary>
	/// <param name="bounds">Split marker bounds.</param>
	private void SpawnSplitMarker(Rectangle bounds)
	{
		Verify.State.IsTrue(_movingSplitMarker is null);

		_movingSplitMarker = new SplitterMarker(bounds, ViewSplit.Orientation);
		_movingSplitMarker.Show();
	}

	/// <summary>Hide and dispose split marker.</summary>
	private void DisposeSplitMarker()
	{
		if(_movingSplitMarker is not null)
		{
			_movingSplitMarker.Dispose();
			_movingSplitMarker = null;
		}
	}

	/// <summary>Starts moving splitter closest to specified <paramref name="position"/>.</summary>
	/// <param name="position">Mouse position.</param>
	/// <returns>True is moving process was initiated.</returns>
	public bool StartMoving(Point position)
	{
		Verify.State.IsFalse(IsMoving);

		MovingSplitterIndex = FindSplitter(position, out var offset);
		if(MovingSplitterIndex < 0)
		{
			return false;
		}
		var toolSplitPosition = ViewSplit.PointToScreen(Point.Empty);
		var dpi = Dpi.FromControl(ViewSplit);
		Rectangle splitterBounds;
		switch(ViewSplit.Orientation)
		{
			case Orientation.Horizontal:
				{
					splitterBounds = new Rectangle(
						toolSplitPosition.X + offset,
						toolSplitPosition.Y,
						ViewConstants.Spacing.GetValue(dpi).Width,
						ViewSplit.Height);
					_movingOffset = position.X - offset;
					GetResizeBounds(MovingSplitterIndex, out var min, out var max);
					_movingMin = min + ViewConstants.MinimumHostWidth.GetValue(dpi);
					_movingMax = max - ViewConstants.MinimumHostWidth.GetValue(dpi);
					if(_movingMin >= _movingMax)
					{
						MovingSplitterIndex = -1;
						return false;
					}
				}
				break;
			case Orientation.Vertical:
				{
					splitterBounds = new Rectangle(
						toolSplitPosition.X,
						toolSplitPosition.Y + offset,
						ViewSplit.Width,
						ViewConstants.Spacing.GetValue(dpi).Height);
					_movingOffset = position.Y - offset;
					GetResizeBounds(MovingSplitterIndex, out var min, out var max);
					_movingMin = min + ViewConstants.MinimumHostHeight.GetValue(dpi);
					_movingMax = max - ViewConstants.MinimumHostHeight.GetValue(dpi);
					if(_movingMin >= _movingMax)
					{
						MovingSplitterIndex = -1;
						return false;
					}
				}
				break;
			default:
				throw new ApplicationException(
					$"Unexpected {nameof(ViewSplit)}.{nameof(ViewSplit.Orientation)} value: {ViewSplit.Orientation}");
		}
		IsMoving = true;
		SpawnSplitMarker(splitterBounds);
		return true;
	}

	/// <summary>Updates moving splitter position.</summary>
	/// <param name="position">Mouse position.</param>
	public void UpdateMoving(Point position)
	{
		Verify.State.IsTrue(IsMoving);

		int splitterPosition;
		switch(ViewSplit.Orientation)
		{
			case Orientation.Horizontal:
				position.X -= _movingOffset;
				if(position.X < _movingMin)
				{
					position.X = _movingMin;
				}
				else if(position.X > _movingMax)
				{
					position.X = _movingMax;
				}
				position.Offset(ViewSplit.PointToScreen(Point.Empty));
				splitterPosition = position.X;
				if(_movingSplitMarker is not null)
				{
					_movingSplitMarker.Left = splitterPosition;
				}
				break;
			case Orientation.Vertical:
				position.Y -= _movingOffset;
				if(position.Y < _movingMin)
				{
					position.Y = _movingMin;
				}
				else if(position.Y > _movingMax)
				{
					position.Y = _movingMax;
				}
				position.Offset(ViewSplit.PointToScreen(Point.Empty));
				splitterPosition = position.Y;
				if(_movingSplitMarker != null)
				{
					_movingSplitMarker.Top = splitterPosition;
				}
				break;
			default:
				throw new ApplicationException(
					$"Unexpected {nameof(ViewSplit)}.{nameof(ViewSplit.Orientation)} value: {ViewSplit.Orientation}");
		}
	}

	private int ClampMovingPosition(int position)
	{
		if(position <= _movingMin) return _movingMin;
		if(position >= _movingMax) return _movingMax;
		return position;
	}

	/// <summary>Commits new splitter position.</summary>
	/// <param name="position">Mouse position.</param>
	public void CommitMoving(Point position)
	{
		Verify.State.IsTrue(IsMoving);

		IsMoving = false;
		DisposeSplitMarker();
		var pos = ViewSplit.Orientation switch
		{
			Orientation.Horizontal => ClampMovingPosition(position.X - _movingOffset),
			Orientation.Vertical   => ClampMovingPosition(position.Y - _movingOffset),
			_ => throw new ApplicationException($"Unexpected {nameof(ViewSplit)}.{nameof(ViewSplit.Orientation)} value: {ViewSplit.Orientation}"),
		};
		int size = ViewSplit.Orientation switch
		{
			Orientation.Horizontal => ViewSplit.Width,
			Orientation.Vertical   => ViewSplit.Height,
			_ => throw new ApplicationException(),
		};
		var spacing = GetSpacing();
		size -= spacing * (_slots.Count - 1);
		var leftover = size;
		if(_slots.Count > 2)
		{
			for(int i = 0; i < _slots.Count; i++)
			{
				if(i < MovingSplitterIndex || i > MovingSplitterIndex + 1)
				{
					pos      -= _slots[i].CurrentSize + spacing;
					leftover -= _slots[i].CurrentSize;
				}
			}
		}
		var dpi = Dpi.FromControl(ViewSplit);
		ResizeSlot(_slots[MovingSplitterIndex + 0], dpi, pos, size);
		ResizeSlot(_slots[MovingSplitterIndex + 1], dpi, leftover - pos, size);
		MovingSplitterIndex = -1;
		Apply();
	}

	private static void ResizeSlot(ViewSplitSlot slot, Dpi dpi, int absoluteSize, int totalSize)
	{
		Assert.IsNotNull(slot);

		switch(slot.Size.Type)
		{
			case ViewSplitSlotSizeType.Relative:
				slot.Size = ViewSplitSlotSize.Relative(absoluteSize / (float)totalSize);
				break;
			case ViewSplitSlotSizeType.Absolute:
				slot.Size = ViewSplitSlotSize.Absolute(dpi, absoluteSize);
				break;
		}
	}

	/// <summary>Cancels splitter moving.</summary>
	public void CancelMoving()
	{
		Verify.State.IsTrue(IsMoving);

		IsMoving = false;
		DisposeSplitMarker();
	}

	public List<ViewSplitSlot>.Enumerator GetEnumerator() => _slots.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<ViewSplitSlot> IEnumerable<ViewSplitSlot>.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	public void Dispose()
	{
		DisposeSplitMarker();
	}
}

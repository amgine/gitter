using System.Collections;
using System.Collections.Generic;

namespace gitter.Framework.Controls
{
	/// <summary> Information about current resizing. </summary>
	public class ColumnResizing
	{
		private readonly CustomListBoxColumn _resizingColumn;
		private readonly int _activeColumnIndex;
		private readonly MoveAction _influence;
		private readonly int _initialWidth;

		/// <summary> Create resize information from column list, active (hovered) column and side (left or right) of resize. </summary>
		public static ColumnResizing FromActiveColumn(IList<CustomListBoxColumn> columns, int activeIndex, ColumnResizeSide side)
		{
			var activeColumn = columns[activeIndex];

			int autoSizeId = int.MaxValue;
			for (int i = 0; i < columns.Count; ++i)
			{
				if (columns[i].IsVisible && columns[i].SizeMode == ColumnSizeMode.Fill)
				{
					autoSizeId = i;
					break;
				}
			}
			if (side == ColumnResizeSide.FromLeft)
			{
				var prevColumn = columns.FindPrevious(activeIndex, column => column.IsVisible);
				if (prevColumn != null && activeIndex <= autoSizeId)
				{
					if (prevColumn.SizeMode == ColumnSizeMode.Sizeable)
					{
						return new ColumnResizing(prevColumn, activeIndex, MoveAction.IncreaseWidth);
					}
				}
				if (activeColumn.SizeMode != ColumnSizeMode.Sizeable)
				{
					return null;
				}
				return new ColumnResizing(activeColumn, activeIndex, MoveAction.DecreaseWidth);
			}
			else
			{
				if (activeColumn.SizeMode != ColumnSizeMode.Sizeable || activeIndex > autoSizeId)
				{
					var nextColumn = columns.FindNext(activeIndex, column => column.IsVisible);
					if (nextColumn != null)
					{
						return new ColumnResizing(nextColumn, activeIndex, MoveAction.DecreaseWidth);
					}
					else
					{
						return null;
					}
				}
				else
				{
					return new ColumnResizing(activeColumn, activeIndex, MoveAction.IncreaseWidth);
				}
			}
		}

		private ColumnResizing(CustomListBoxColumn resizingColumn, int activeColumnIndex, MoveAction influence)
		{
			_resizingColumn = resizingColumn;
			_activeColumnIndex = activeColumnIndex;
			_initialWidth = ResizingColumn.Width;
			_influence = influence;
		}

		/// <summary> Column actually resizing. May differ from active column. </summary>
		public CustomListBoxColumn ResizingColumn
		{
			get { return _resizingColumn; }
		}

		/// <see cref="MoveAction"/>
		public MoveAction Influence
		{
			get { return _influence; }
		}

		/// <summary> Initial width of resizing column </summary>
		public int InitialWidth
		{
			get { return _initialWidth; }
		}

		/// <summary> Index of active column. Needed only for double click handles </summary>
		public int ActiveColumnIndex
		{
			get { return _activeColumnIndex; }
		}
	}
}
namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	/// <summary>Collection of splitter positions.</summary>
	sealed class ViewSplitPositions : IEnumerable<double>, IDisposable
	{
		#region Data

		private readonly ViewSplit _viewSplit;
		private readonly List<double> _positions;

		private SplitterMarker _movingSplitMarker;
		private double _movingPosition;
		private int _movingSplitterIndex;
		private bool _isMoving;
		private int _movingOffset;
		private int _movingMin;
		private int _movingMax;

		private int _size;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="ViewSplitPositions"/>.</summary>
		/// <param name="viewSplit">Host <see cref="ViewSplit"/>.</param>
		/// <param name="size">Host size.</param>
		/// <param name="position">First splitter position.</param>
		public ViewSplitPositions(ViewSplit viewSplit, int size, double position)
		{
			Verify.Argument.IsNotNull(viewSplit, "viewSplit");
			Verify.Argument.IsNotNegative(size, "size");
			Verify.Argument.IsInRange(0.0, position, 1.0, "position");

			_viewSplit = viewSplit;
			_size = size;
			_movingSplitterIndex = -1;
			_positions = new List<double>() { position };
		}

		#endregion

		/// <summary>Gets minimum and maximum splitter positions for specified splitter.</summary>
		/// <param name="index">The index of splitter to check bounds for.</param>
		/// <param name="min">Minimum splitter position.</param>
		/// <param name="max">Maximum splitter position.</param>
		private void GetResizeBounds(int index, out double min, out double max)
		{
			int first = index - 1;
			int last = index + 1;

			while(first > 0)
			{
				var item = _viewSplit[first];
				var viewHost = item as ViewHost;
				if(viewHost != null && !viewHost.IsDocumentWell)
				{
					break;
				}
				var viewSplit = item as ViewSplit;
				if(viewSplit != null && !viewSplit.ContainsDocumentWell)
				{
					break;
				}
				--first;
			}

			while(last <= _positions.Count)
			{
				var item = _viewSplit[last];
				var viewHost = item as ViewHost;
				if(viewHost != null && !viewHost.IsDocumentWell)
				{
					break;
				}
				var viewSplit = item as ViewSplit;
				if(viewSplit != null && !viewSplit.ContainsDocumentWell)
				{
					break;
				}
				++last;
			}

			min = first < 0 ? 0.0 : _positions[first];
			max = last >= _positions.Count ? 1.0 : _positions[last];
		}

		/// <summary>Gets a value indicating whether some splitter is moving.</summary>
		/// <value><c>true</c> if some splitter is moving; otherwise, <c>false</c>.</value>
		public bool IsMoving
		{
			get { return _isMoving; }
		}

		/// <summary>Gets the index of the moving splitter.</summary>
		/// <value>Index of the moving splitter.</value>
		public int MovingSplitterIndex
		{
			get { return _movingSplitterIndex; }
		}

		/// <summary>Finds splitter index which contains specified <paramref name="position"/>.</summary>
		/// <param name="position">Position to check for.</param>
		/// <returns>Splitter index or -1 if no splitter was found at specified <paramref name="position"/>.</returns>
		private int FindSplitter(Point position)
		{
			int size;
			int pos;
			switch(_viewSplit.Orientation)
			{
				case Orientation.Horizontal:
					size = _viewSplit.Width;
					pos = position.X;
					break;
				case Orientation.Vertical:
					size = _viewSplit.Height;
					pos = position.Y;
					break;
				default:
					throw new ApplicationException(
						"Unexpected ToolSplit.Orientation value: " + _viewSplit.Orientation);
			}
			for(int i = 0; i < _positions.Count; ++i)
			{
				var splitter = _positions[i] * size + 0.5f;
				var min = splitter - ViewConstants.Spacing / 2.0;
				var max = min + ViewConstants.Spacing;
				if(splitter >= min && splitter <= max)
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>Gets splitter absolute position.</summary>
		/// <param name="value">Splitter relative position.</param>
		/// <returns>Splitter absolute position.</returns>
		private int GetSplitterPosition(double value)
		{
			int size;
			switch(_viewSplit.Orientation)
			{
				case Orientation.Horizontal:
					size = _viewSplit.Width;
					break;
				case Orientation.Vertical:
					size = _viewSplit.Height;
					break;
				default:
					throw new ApplicationException(
						"Unexpected ToolSplit.Orientation value: " + _viewSplit.Orientation);
			}
			return (int)(size * value);
		}

		/// <summary>Spawns the split marker.</summary>
		/// <param name="bounds">Split marker bounds.</param>
		private void SpawnSplitMarker(Rectangle bounds)
		{
			Verify.State.IsTrue(_movingSplitMarker == null);

			_movingSplitMarker = new SplitterMarker(bounds, _viewSplit.Orientation);
			_movingSplitMarker.Show();
		}

		/// <summary>Hide and dispose split marker.</summary>
		private void KillSplitMarker()
		{
			if(_movingSplitMarker != null)
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

			_movingSplitterIndex = FindSplitter(position);
			if(_movingSplitterIndex != -1)
			{
				_movingPosition = _positions[_movingSplitterIndex];
				var toolSplitPosition = _viewSplit.PointToScreen(Point.Empty);
				Rectangle splitterBounds;
				switch(_viewSplit.Orientation)
				{
					case Orientation.Horizontal:
						{
							var width = _viewSplit.Width;
							var splitterPosition = (int)(_movingPosition * width) - ViewConstants.Spacing / 2;
							splitterBounds = new Rectangle(
								toolSplitPosition.X + splitterPosition,
								toolSplitPosition.Y,
								ViewConstants.Spacing,
								_viewSplit.Height);
							_movingOffset = position.X - splitterPosition;
							double min, max;
							GetResizeBounds(_movingSplitterIndex, out min, out max);
							_movingMin = (int)(min * width) + ViewConstants.MinimumHostWidth;
							_movingMax = (int)(max * width) - ViewConstants.Spacing - ViewConstants.MinimumHostWidth;
							if(_movingMin >= _movingMax)
							{
								return false;
							}
						}
						break;
					case Orientation.Vertical:
						{
							var height = _viewSplit.Height;
							var splitterPosition = (int)(_movingPosition * _viewSplit.Height) - ViewConstants.Spacing / 2;
							splitterBounds = new Rectangle(
								toolSplitPosition.X,
								toolSplitPosition.Y + splitterPosition,
								_viewSplit.Width,
								ViewConstants.Spacing);
							_movingOffset = position.Y - splitterPosition;
							double min, max;
							GetResizeBounds(_movingSplitterIndex, out min, out max);
							_movingMin = (int)(min * height) + ViewConstants.MinimumHostHeight;
							_movingMax = (int)(max * height) - ViewConstants.Spacing - ViewConstants.MinimumHostHeight;
							if(_movingMin >= _movingMax)
							{
								return false;
							}
						}
						break;
					default:
						throw new ApplicationException(
							"Unexpected ToolSplit.Orientation value: " + _viewSplit.Orientation);
				}
				_isMoving = true;
				SpawnSplitMarker(splitterBounds);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>Updates moving splitter position.</summary>
		/// <param name="position">Mouse position.</param>
		public void UpdateMoving(Point position)
		{
			Verify.State.IsTrue(IsMoving);

			int splitterPosition;
			switch(_viewSplit.Orientation)
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
					position.Offset(_viewSplit.PointToScreen(Point.Empty));
					splitterPosition = position.X;
					_movingSplitMarker.Left = splitterPosition;
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
					position.Offset(_viewSplit.PointToScreen(Point.Empty));
					splitterPosition = position.Y;
					_movingSplitMarker.Top = splitterPosition;
					break;
				default:
					throw new ApplicationException(
						"Unexpected ToolSplit.Orientation value: " + _viewSplit.Orientation);
			}
		}

		/// <summary>Commits new splitter position.</summary>
		/// <param name="position">Mouse position.</param>
		public void CommitMoving(Point position)
		{
			Verify.State.IsTrue(IsMoving);

			_isMoving = false;
			KillSplitMarker();
			double pos;
			switch(_viewSplit.Orientation)
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
					pos = (double)(position.X + ViewConstants.Spacing / 2) / (double)_viewSplit.Width;
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
					pos = (double)(position.Y + ViewConstants.Spacing / 2) / (double)_viewSplit.Height;
					break;
				default:
					throw new ApplicationException(
						"Unexpected ToolSplit.Orientation value: " + _viewSplit.Orientation);
			}
			if(_positions[_movingSplitterIndex] != pos)
			{
				_positions[_movingSplitterIndex] = pos;
				Apply(_movingSplitterIndex);
				_movingSplitterIndex = -1;
			}
		}

		/// <summary>Cancels splitter moving.</summary>
		public void CancelMoving()
		{
			Verify.State.IsTrue(IsMoving);

			_isMoving = false;
			KillSplitMarker();
		}

		/// <summary>Applies the specified splitter position.</summary>
		/// <param name="splitterIndex">Index of the splitter.</param>
		public void Apply(int splitterIndex)
		{
			Verify.Argument.IsValidIndex(splitterIndex, _positions.Count, "splitterIndex");

			int offset = 0;
			int width = _viewSplit.Width;
			int height = _viewSplit.Height;
			int lastItem = _positions.Count;
			switch(_viewSplit.Orientation)
			{
				case Orientation.Horizontal:
					for(int i = 0; i < _positions.Count; ++i)
					{
						var pos = (int)(_positions[i] * width - ViewConstants.Spacing / 2.0);
						if(i >= splitterIndex)
						{
							_viewSplit[i].Bounds = new Rectangle(offset, 0, pos - offset, height);
							if(i > splitterIndex) return;
						}
						offset = pos + ViewConstants.Spacing;
					}
					_viewSplit[lastItem].Bounds = new Rectangle(offset, 0, width - offset, height);
					break;
				case Orientation.Vertical:
					for(int i = 0; i < _positions.Count; ++i)
					{
						var pos = (int)(_positions[i] * height - ViewConstants.Spacing / 2.0);
						if(i >= splitterIndex)
						{
							_viewSplit[i].Bounds = new Rectangle(0, offset, width, pos - offset);
							if(i > splitterIndex) return;
						}
						offset = pos + ViewConstants.Spacing;
					}
					_viewSplit[lastItem].Bounds = new Rectangle(0, offset, width, height - offset);
					break;
				default:
					throw new ApplicationException("Unexpected ToolSplit.Orientation value: " + _viewSplit.Orientation);
			}
		}

		/// <summary>Enforce control bounds according to splitters realtive positions.</summary>
		/// <remarks>Use this to resize and shift child controls according to splitter positions after host control resize.</remarks>
		public void Apply()
		{
			int offset = 0;
			int width = _viewSplit.Width;
			int height = _viewSplit.Height;
			int lastItem = _positions.Count;
			switch(_viewSplit.Orientation)
			{
				case Orientation.Horizontal:
					for(int i = 0; i < _positions.Count; ++i)
					{
						var pos = (int)(_positions[i] * width - ViewConstants.Spacing / 2.0);
						_viewSplit[i].Bounds = new Rectangle(offset, 0, pos - offset, height);
						offset = pos + ViewConstants.Spacing;
					}
					_viewSplit[lastItem].Bounds = new Rectangle(offset, 0, width - offset, height);
					break;
				case Orientation.Vertical:
					for(int i = 0; i < _positions.Count; ++i)
					{
						var pos = (int)(_positions[i] * height - ViewConstants.Spacing / 2.0);
						_viewSplit[i].Bounds = new Rectangle(0, offset, width, pos - offset);
						offset = pos + ViewConstants.Spacing;
					}
					_viewSplit[lastItem].Bounds = new Rectangle(0, offset, width, height - offset);
					break;
				default:
					throw new ApplicationException("Unexpected ToolSplit.Orientation value: " + _viewSplit.Orientation);
			}
		}

		/// <summary>Actualizes splitter positions in this <see cref="ViewSplitPositions"/>.</summary>
		/// <remarks>Use this if resize was handled by winforms layout engine.</remarks>
		public void Actualize()
		{
			switch(_viewSplit.Orientation)
			{
				case Orientation.Horizontal:
					{
						var size = _viewSplit.Width;
						var offset = 0;
						for(int i = 0; i < _positions.Count; ++i)
						{
							offset += _viewSplit[i].Width + ViewConstants.Spacing / 2;
							_positions[i] = (double)offset / size;
							offset += ViewConstants.Spacing - ViewConstants.Spacing / 2;
						}
					}
					break;
				case Orientation.Vertical:
					{
						var size = _viewSplit.Height;
						var offset = 0;
						for(int i = 0; i < _positions.Count; ++i)
						{
							offset += _viewSplit[i].Height + ViewConstants.Spacing / 2;
							_positions[i] = (double)offset / size;
							offset += ViewConstants.Spacing - ViewConstants.Spacing / 2;
						}
					}
					break;
				default:
					throw new ApplicationException(
						"Unexpected ToolSplit.Orientation value: " + _viewSplit.Orientation);
			}
		}

		/// <summary>Gets splitter position at the specified index.</summary>
		public double this[int index]
		{
			get { return _positions[index]; }
		}

		/// <summary>Gets the splitter count.</summary>
		public int Count
		{
			get { return _positions.Count; }
		}

		/// <summary>Inserts new splitter at specified <paramref name="index"/>.</summary>
		/// <param name="index">Splitter index.</param>
		/// <param name="position">Splitter position.</param>
		public void Insert(int index, double position)
		{
			_positions.Insert(index, position);
		}

		/// <summary>Adds new splitter.</summary>
		/// <param name="position">Splitter position.</param>
		public void Add(double position)
		{
			_positions.Add(position);
		}

		/// <summary>Removes splitter at specified <paramref name="index"/>.</summary>
		/// <param name="index">Index of splitter to remove.</param>
		public void RemoveAt(int index)
		{
			RemoveAt(index, false);
		}

		public void RemoveAt(int index, bool recalc)
		{
			if(recalc)
			{
				double size = _positions[index];
				double free = size / _positions.Count;
				for(int i = 0; i < index; ++i)
				{
					_positions[i] += free;
				}
				for(int i = index + 1; i < _positions.Count; ++i)
				{
					_positions[i] += free;
				}
			}
			_positions.RemoveAt(index);
		}

		public IEnumerator<double> GetEnumerator()
		{
			return _positions.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _positions.GetEnumerator();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			KillSplitMarker();
		}
	}
}

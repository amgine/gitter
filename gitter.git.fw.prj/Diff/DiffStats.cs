namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>Diff stats (line counters).</summary>
	public sealed class DiffStats : ICloneable
	{
		#region Data

		private int _contextLinesCount;
		private int _headerLinesCount;
		private int _addedLinesCount;
		private int _removedLinesCount;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="DiffStats"/>.</summary>
		public DiffStats()
		{
		}

		/// <summary>Create <see cref="DiffStats"/>.</summary>
		/// <param name="addedLinesCount">Added lines count.</param>
		/// <param name="removedLinesCount">Removed lines count.</param>
		/// <param name="contextLinesCount">Context lines count.</param>
		/// <param name="headerLinesCount">Header lines count.</param>
		public DiffStats(int addedLinesCount, int removedLinesCount, int contextLinesCount, int headerLinesCount)
		{
			if(addedLinesCount < 0) throw new ArgumentOutOfRangeException("addedLinesCount");
			if(removedLinesCount < 0) throw new ArgumentOutOfRangeException("removedLinesCount");
			if(contextLinesCount < 0) throw new ArgumentOutOfRangeException("contextLinesCount");
			if(headerLinesCount < 0) throw new ArgumentOutOfRangeException("headerLinesCount");

			_addedLinesCount = addedLinesCount;
			_removedLinesCount = removedLinesCount;
			_contextLinesCount = contextLinesCount;
			_headerLinesCount = headerLinesCount;
		}

		#endregion

		#region Properties

		/// <summary>Number of added lines.</summary>
		public int AddedLinesCount
		{
			get { return _addedLinesCount; }
			set
			{
				if(value < 0) throw new ArgumentOutOfRangeException("value");
				_addedLinesCount = value;
			}
		}

		/// <summary>Number of removed lines.</summary>
		public int RemovedLinesCount
		{
			get { return _removedLinesCount; }
			set
			{
				if(value < 0) throw new ArgumentOutOfRangeException("value");
				_removedLinesCount = value;
			}
		}

		/// <summary>Number of changed (added/removed) lines.</summary>
		public int ChangedLinesCount
		{
			get { return _addedLinesCount + _removedLinesCount; }
		}

		/// <summary>Number of context lines.</summary>
		public int ContextLinesCount
		{
			get { return _contextLinesCount; }
			set
			{
				if(value < 0) throw new ArgumentOutOfRangeException("value");
				_contextLinesCount = value;
			}
		}

		/// <summary>Total line count.</summary>
		public int TotalLinesCount
		{
			get { return _addedLinesCount + _removedLinesCount + _contextLinesCount + _headerLinesCount; }
		}

		#endregion

		#region Methods

		public void Add(DiffStats stats)
		{
			if(stats == null) throw new ArgumentNullException("stats");
			_addedLinesCount += stats._addedLinesCount;
			_removedLinesCount += stats._removedLinesCount;
			_contextLinesCount += stats._contextLinesCount;
			_headerLinesCount += stats._headerLinesCount;
		}

		public void Subtract(DiffStats stats)
		{
			if(stats == null) throw new ArgumentNullException("stats");
			_addedLinesCount -= stats._addedLinesCount;
			_removedLinesCount -= stats._removedLinesCount;
			_contextLinesCount -= stats._contextLinesCount;
			_headerLinesCount -= stats._headerLinesCount;
		}

		public void Increment(DiffLineState state)
		{
			switch(state)
			{
				case DiffLineState.Added:
					++_addedLinesCount;
					break;
				case DiffLineState.Removed:
					++_removedLinesCount;
					break;
				case DiffLineState.Context:
					++_contextLinesCount;
					break;
				default:
					++_headerLinesCount;
					break;
			}
		}

		public void Decrement(DiffLineState state)
		{
			switch(state)
			{
				case DiffLineState.Added:
					--_addedLinesCount;
					break;
				case DiffLineState.Removed:
					--_removedLinesCount;
					break;
				case DiffLineState.Context:
					--_contextLinesCount;
					break;
				default:
					--_headerLinesCount;
					break;
			}
		}

		public void Reset()
		{
			_addedLinesCount = 0;
			_removedLinesCount = 0;
			_contextLinesCount = 0;
			_headerLinesCount = 0;
		}

		public void Reset(int addedLinesCount, int removedLinesCount, int contextLinesCount, int headerLinesCount)
		{
			if(addedLinesCount < 0) throw new ArgumentOutOfRangeException("addedLinesCount");
			if(removedLinesCount < 0) throw new ArgumentOutOfRangeException("removedLinesCount");
			if(contextLinesCount < 0) throw new ArgumentOutOfRangeException("contextLinesCount");
			if(headerLinesCount < 0) throw new ArgumentOutOfRangeException("headerLinesCount");

			_addedLinesCount = addedLinesCount;
			_removedLinesCount = removedLinesCount;
			_contextLinesCount = contextLinesCount;
			_headerLinesCount = headerLinesCount;
		}

		#endregion

		#region ICloneable

		public DiffStats Clone()
		{
			return new DiffStats(
				_addedLinesCount,
				_removedLinesCount,
				_contextLinesCount,
				_headerLinesCount);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
	}
}

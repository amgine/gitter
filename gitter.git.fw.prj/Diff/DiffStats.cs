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
			Verify.Argument.IsNotNegative(addedLinesCount, "addedLinesCount");
			Verify.Argument.IsNotNegative(removedLinesCount, "removedLinesCount");
			Verify.Argument.IsNotNegative(contextLinesCount, "contextLinesCount");
			Verify.Argument.IsNotNegative(headerLinesCount, "headerLinesCount");

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
				Verify.Argument.IsNotNegative(value, "value");

				_addedLinesCount = value;
			}
		}

		/// <summary>Number of removed lines.</summary>
		public int RemovedLinesCount
		{
			get { return _removedLinesCount; }
			set
			{
				Verify.Argument.IsNotNegative(value, "value");

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
				Verify.Argument.IsNotNegative(value, "value");

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
			Verify.Argument.IsNotNull(stats, "stats");

			_addedLinesCount += stats._addedLinesCount;
			_removedLinesCount += stats._removedLinesCount;
			_contextLinesCount += stats._contextLinesCount;
			_headerLinesCount += stats._headerLinesCount;
		}

		public void Subtract(DiffStats stats)
		{
			Verify.Argument.IsNotNull(stats, "stats");

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
			Verify.Argument.IsNotNegative(addedLinesCount, "addedLinesCount");
			Verify.Argument.IsNotNegative(removedLinesCount, "removedLinesCount");
			Verify.Argument.IsNotNegative(contextLinesCount, "contextLinesCount");
			Verify.Argument.IsNotNegative(headerLinesCount, "headerLinesCount");

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

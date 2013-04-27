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

	/// <summary>Arguments for collection notification events.</summary>
	public sealed class NotifyCollectionEventArgs : EventArgs
	{
		#region Data

		private readonly int _startIndex;
		private readonly int _endIndex;
		private readonly NotifyEvent _event;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="NotifyCollectionEventArgs"/>.</summary>
		/// <param name="index">Changed item index.</param>
		/// <param name="evt">Change type.</param>
		public NotifyCollectionEventArgs(int index, NotifyEvent evt)
		{
			_startIndex = index;
			_endIndex = index;
			_event = evt;
		}

		/// <summary>Create <see cref="NotifyCollectionEventArgs"/>.</summary>
		/// <param name="startIndex">Index of first item in modified range.</param>
		/// <param name="endIndex">Index of last item in modified range.</param>
		/// <param name="evt">Change type.</param>
		public NotifyCollectionEventArgs(int startIndex, int endIndex, NotifyEvent evt)
		{
			_startIndex = startIndex;
			_endIndex = endIndex;
			_event = evt;
		}

		#endregion

		#region Properties

		/// <summary>Number of modified items.</summary>
		public int ModifiedItems
		{
			get { return _endIndex - _startIndex + 1; }
		}

		/// <summary>Index of first item in modified range.</summary>
		public int StartIndex
		{
			get { return _startIndex; }
		}

		/// <summary>Index of last item in modified range.</summary>
		public int EndIndex
		{
			get { return _endIndex; }
		}

		/// <summary>Change type.</summary>
		public NotifyEvent Event
		{
			get { return _event; }
		}

		#endregion
	}
}

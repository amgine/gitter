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

namespace gitter.Framework.Services
{
	using System;

	public sealed class TrackingEventArgs : EventArgs
	{
		#region Data

		private readonly int _index;
		private readonly bool _tracked;

		#endregion

		#region .ctor

		public TrackingEventArgs(bool tracked, int index)
		{
			_tracked = tracked;
			_index = index;
		}

		#endregion

		#region Properties

		public bool IsTracked
		{
			get { return _tracked; }
		}

		public int Index
		{
			get { return _index; }
		}

		#endregion
	}

	public sealed class TrackingEventArgs<T> : EventArgs
	{
		#region Data

		private readonly int _index;
		private readonly T _element;
		private readonly bool _tracked;

		#endregion

		#region .ctor

		public TrackingEventArgs(bool tracked, int index, T element)
		{
			_tracked = tracked;
			_index = index;
			_element = element;
		}

		#endregion

		#region Properties

		public bool IsTracked
		{
			get { return _tracked; }
		}

		public int Index
		{
			get { return _index; }
		}

		public T Item
		{
			get { return _element; }
		}

		#endregion
	}
}

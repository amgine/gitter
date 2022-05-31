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

/// <summary>Arguments for collection notification events.</summary>
public sealed class NotifyCollectionEventArgs : EventArgs
{
	/// <summary>Create <see cref="NotifyCollectionEventArgs"/>.</summary>
	/// <param name="index">Changed item index.</param>
	/// <param name="evt">Change type.</param>
	public NotifyCollectionEventArgs(int index, NotifyEvent evt)
	{
		StartIndex = index;
		EndIndex   = index;
		Event      = evt;
	}

	/// <summary>Create <see cref="NotifyCollectionEventArgs"/>.</summary>
	/// <param name="startIndex">Index of first item in modified range.</param>
	/// <param name="endIndex">Index of last item in modified range.</param>
	/// <param name="evt">Change type.</param>
	public NotifyCollectionEventArgs(int startIndex, int endIndex, NotifyEvent evt)
	{
		StartIndex = startIndex;
		EndIndex   = endIndex;
		Event      = evt;
	}

	/// <summary>Number of modified items.</summary>
	public int ModifiedItems => EndIndex - StartIndex + 1;

	/// <summary>Index of first item in modified range.</summary>
	public int StartIndex { get; }

	/// <summary>Index of last item in modified range.</summary>
	public int EndIndex { get; }

	/// <summary>Change type.</summary>
	public NotifyEvent Event { get; }
}

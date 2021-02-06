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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;

	/// <summary>Extension methods for <see cref="LinkedList{T}"/>.</summary>
	public static class LinkedListExtensions
	{
		/// <summary>Represent <see cref="LinkedList{T}"/> as <see cref="IEnumerable{LinkedListNode{T}}"/>.</summary>
		/// <typeparam name="T">Type of list elements.</typeparam>
		/// <param name="list">Linked list.</param>
		/// <returns>LinkedListNode-enumerable representation of linked list.</returns>
		public static IEnumerable<LinkedListNode<T>> EnumerateNodes<T>(this LinkedList<T> list)
		{
			Verify.Argument.IsNotNull(list, nameof(list));

			var node = list.First;
			while(node != null)
			{
				yield return node;
				node = node.Next;
			}
		}
	}
}

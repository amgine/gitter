namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;

	/// <summary>Extension methods for <see cref="LinkedList&lt;T&gt;"/>.</summary>
	public static class LinkedListExtensions
	{
		/// <summary>Represent <see cref="LinkedList&lt;T&gt;"/> as <see cref="IEnumerable&lt;LinkedListNode&lt;T&gt;&gt;"/>.</summary>
		/// <typeparam name="T">Type of list elements.</typeparam>
		/// <param name="list">Linked list.</param>
		/// <returns>LinkedListNode-enumerable representation of linked list.</returns>
		public static IEnumerable<LinkedListNode<T>> EnumerateNodes<T>(this LinkedList<T> list)
		{
			var node = list.First;
			while(node != null)
			{
				yield return node;
				node = node.Next;
			}
		}
	}
}

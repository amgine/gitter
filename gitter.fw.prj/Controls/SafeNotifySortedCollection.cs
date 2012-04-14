namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	/// <summary>Collection with auto-sorting, change notification & thread-safe operations invoked on host control.</summary>
	/// <typeparam name="T">Item type.</typeparam>
	public abstract class SafeNotifySortedCollection<T> : NotifySortedCollection<T>
	{
		#region .ctor

		/// <summary>Create <see cref="SafeNotifySortedCollection&lt;T&gt;"/>.</summary>
		public SafeNotifySortedCollection()
		{
		}

		#endregion

		#region Properties

		/// <summary>Object which is used to synchronize all *Safe() calls.</summary>
		protected abstract ISynchronizeInvoke SynchronizationObject { get; }

		#endregion

		#region Methods

		public void AddSafe(T item)
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				Add(item);
			}
			else
			{
				control.BeginInvoke(new Action<T>(Add), new object[] { item });
			}
		}

		public void AddRangeSafe(IEnumerable<T> list)
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				AddRange(list);
			}
			else
			{
				control.BeginInvoke(new Action<IEnumerable<T>>(AddRange), new object[] { list });
			}
		}

		public void InsertSafe(int index, T item)
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				Insert(index, item);
			}
			else
			{
				control.BeginInvoke(new Action<int, T>(Insert), new object[] { index, item });
			}
		}

		public void RemoveSafe(T item)
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				Remove(item);
			}
			else
			{
				control.BeginInvoke(new Func<T, bool>(Remove), new object[] { item });
			}
		}

		public void RemoveAtSafe(int index)
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				RemoveAt(index);
			}
			else
			{
				control.BeginInvoke(new Action<int>(RemoveAt), new object[] { index });
			}
		}

		public void RemoveRangeSafe(int index, int count)
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				RemoveRange(index, count);
			}
			else
			{
				control.BeginInvoke(new Action<int, int>(RemoveRange), new object[] { index, count});
			}
		}

		public void ClearSafe()
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				Clear();
			}
			else
			{
				control.BeginInvoke(new Action(Clear), null);
			}
		}

		public void InsertSortedFromTopSafe(T item, Func<T, T, int> comparer)
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				InsertSortedFromTop(item, comparer);
			}
			else
			{
				control.BeginInvoke(
					new Func<T, Func<T, T, int>, int>(InsertSortedFromTop),
					new object[] { item, comparer });
			}
		}

		public void InsertSortedFromBottomSafe(T item, Func<T, T, int> comparer)
		{
			var control = SynchronizationObject;
			if(control == null || !control.InvokeRequired)
			{
				InsertSortedFromBottom(item, comparer);
			}
			else
			{
				control.BeginInvoke(
					new Func<T, Func<T, T, int>, int>(InsertSortedFromBottom),
					new object[] { item, comparer });
			}
		}

		#endregion
	}
}

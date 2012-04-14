namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public static class ArrayExtensions
	{
		/// <summary>Set all values in array to a specified <paramref name="value"/>.</summary>
		/// <typeparam name="T">Type of array elements.</typeparam>
		/// <param name="array">Array to initialize.</param>
		/// <param name="value">Value to set.</param>
		public static void Initialize<T>(this T[] array, T value)
		{
			if(array == null) throw new ArgumentNullException("array");
			for(int i = 0; i < array.Length; ++i)
				array[i] = value;
		}

		/// <summary>Set all values in array to a result returned by specified <paramref name="getValue"/>.</summary>
		/// <typeparam name="T">Type of array elements.</typeparam>
		/// <param name="array">Array to initialize.</param>
		/// <param name="getValue">Function which returns value for each array element.</param>
		public static void Initialize<T>(this T[] array, Func<int, T> getValue)
		{
			if(array == null) throw new ArgumentNullException("array");
			for(int i = 0; i < array.Length; ++i)
				array[i] = getValue(i);
		}

		/// <summary>Set all values in array to a result returned by specified <paramref name="getValue"/>.</summary>
		/// <typeparam name="T">Type of array elements.</typeparam>
		/// <param name="array">Array to initialize.</param>
		/// <param name="getValue">Function which returns value for each array element.</param>
		public static void Initialize<T>(this T[] array, Func<int, T, T> getValue)
		{
			if(array == null) throw new ArgumentNullException("array");
			for(int i = 0; i < array.Length; ++i)
				array[i] = getValue(i, array[i]);
		}

		public static bool TrueForAll<T>(this T[] array, Predicate<T> match)
		{
			return Array.TrueForAll<T>(array, match);
		}

		public static bool TrueForAll<T>(this T[] array, Func<int, T, bool> match)
		{
			if(array == null) throw new ArgumentNullException("array");
			if(match == null) throw new ArgumentNullException("match");
			for(int i = 0; i < array.Length; ++i)
				if(!match(i, array[i])) return false;
			return true;
		}

		public static ArraySegment<T> GetSegment<T>(this T[] array, int offset, int count)
		{
			return new ArraySegment<T>(array, offset, count);
		}

		public static T[] CloneSegment<T>(this T[] array, int offset, int count)
		{
			if(array == null) throw new ArgumentNullException("array");
			if(offset < 0 || offset >= array.Length)
				throw new ArgumentOutOfRangeException("offset");
			if(offset + count >= array.Length)
				throw new ArgumentOutOfRangeException("count");
			var res = new T[count];
			Array.Copy(array, offset, res, 0, count);
			return res;
		}

		public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
		{
			if(array == null) throw new ArgumentNullException("array");
			return Array.AsReadOnly<T>(array);
		}

		public static T Find<T>(this T[] array, Predicate<T> match)
		{
			return Array.Find<T>(array, match);
		}

		public static T[] FindAll<T>(this T[] array, Predicate<T> match)
		{
			return Array.FindAll<T>(array, match);
		}

		public static void ForEach<T>(this T[] array, Action<T> action)
		{
			Array.ForEach<T>(array, action);
		}

		public static void ForEach<T>(this T[] array, Action<int, T> action)
		{
			if(array == null) throw new ArgumentNullException("array");
			if(action == null) throw new ArgumentNullException("action");
			for(int i = 0; i < array.Length; ++i)
			{
				action(i, array[i]);
			}
		}

		public static bool Contains<T>(this T[] array, T value)
		{
			return Array.IndexOf<T>(array, value) != -1;
		}

		public static TOutput[] Convert<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
		{
			return Array.ConvertAll<TInput, TOutput>(array, converter);
		}

		public static void Sort<T>(this T[] array)
		{
			Array.Sort<T>(array);
		}

		public static void Sort<T>(this T[] array, int index, int length)
		{
			Array.Sort<T>(array, index, length);
		}

		public static void Sort<T>(this T[] array, Comparison<T> comparison)
		{
			Array.Sort<T>(array, comparison);
		}

		public static void Sort<T>(this T[] array, IComparer<T> comparer)
		{
			Array.Sort<T>(array, comparer);
		}

		public static void Sort<T>(this T[] array, int index, int length, IComparer<T> comparer)
		{
			Array.Sort<T>(array, index, length, comparer);
		}

		public static int FindIndex<T>(this T[] array, Predicate<T> match)
		{
			if(array == null) throw new ArgumentNullException("array");
			if(match == null) throw new ArgumentNullException("match");
			for(int i = 0; i < array.Length; ++i)
			{
				if(match(array[i]))
					return i;
			}
			return -1;
		}

		public static int FindIndex<T>(this T[] array, Predicate<T> match, int offset)
		{
			if(array == null) throw new ArgumentNullException("array");
			if(match == null) throw new ArgumentNullException("match");
			if(offset < 0 || offset >= array.Length)
				throw new ArgumentOutOfRangeException("offset");
			for(int i = offset; i < array.Length; ++i)
			{
				if(match(array[i]))
					return i;
			}
			return -1;
		}

		public static int FindIndex<T>(this T[] array, Predicate<T> match, int offset, int count)
		{
			if(array == null) throw new ArgumentNullException("array");
			if(match == null) throw new ArgumentNullException("match");
			if(offset < 0 || offset >= array.Length)
				throw new ArgumentOutOfRangeException("offset");
			if(offset + count >= array.Length)
				throw new ArgumentOutOfRangeException("count");
			int end = offset + count;
			for(int i = offset; i < end; ++i)
			{
				if(match(array[i]))
					return i;
			}
			return -1;
		}

		public static int IndexOf<T>(this T[] array, T value)
		{
			return Array.IndexOf<T>(array, value);
		}

		public static int IndexOf<T>(this T[] array, T value, int startIndex)
		{
			return Array.IndexOf<T>(array, value, startIndex);
		}

		public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
		{
			return Array.IndexOf<T>(array, value, startIndex, count);
		}

		public static int LastIndexOf<T>(this T[] array, T value)
		{
			return Array.LastIndexOf<T>(array, value);
		}

		public static int LastIndexOf<T>(this T[] array, T value, int startIndex)
		{
			return Array.LastIndexOf<T>(array, value, startIndex);
		}

		public static int LastIndexOf<T>(this T[] array, T value, int startIndex, int count)
		{
			return Array.LastIndexOf<T>(array, value, count);
		}
	}
}

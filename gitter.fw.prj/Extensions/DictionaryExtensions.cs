namespace gitter.Framework.Extensions
{
	using System;
	using System.Collections.Generic;

	/// <summary>Extension methods for <see cref="System.Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/>.</summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <typeparam name="TKey">Type of the key.</typeparam>
		/// <typeparam name="TValue">Type of the value.</typeparam>
		/// <param name="dictionary">Dictionary to get value from.</param>
		/// <param name="key">The key whose value to get.</param>
		/// <returns>
		/// Value associated with the specified key or default(<typeparamref name="TValue"/>)
		/// if specified key is not present in the dictionary.
		/// </returns>
		public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			TValue value;
			if(dictionary.TryGetValue(key, out value)) return value;
			return default(TValue);
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <typeparam name="TKey">Type of the key.</typeparam>
		/// <typeparam name="TValue">Type of the value.</typeparam>
		/// <param name="dictionary">Dictionary to get value from.</param>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="defaultVaue">Value to return if specified key is not found.</param>
		/// <returns>
		/// Value associated with the specified key or <paramref name="defaultVaue"/>
		/// if specified key is not present in the dictionary.
		/// </returns>
		public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultVaue)
		{
			TValue value;
			if(dictionary.TryGetValue(key, out value)) return value;
			return defaultVaue;
		}
	}
}

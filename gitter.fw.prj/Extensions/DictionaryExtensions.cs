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
			=> dictionary.TryGetValue(key, out var value)
				? value
				: default;

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
			=> dictionary.TryGetValue(key, out var value)
				? value
				: defaultVaue;
	}
}

#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework;

using System;
using System.Collections;
using System.Collections.Generic;
#if NETCOREAPP
using System.Collections.Immutable;
#endif
using System.Runtime.InteropServices;
using System.Text;

public static class Many
{
	#if NETCOREAPP
	public static Many<T> Create<T>(params ReadOnlySpan<T> values)
		=> values.Length switch
		{
			0 => Many<T>.None,
			1 => new(values[0]),
			_ => new(values.ToArray()),
		};
	#endif

	public static Many<T> Create<T>(T value)
		=> new(value);

	public static Many<T> OneOrNone<T>(T? value) where T : notnull
		=> value is not null ? Create(value) : Many<T>.None;

	public static Many<T> FromSequence<T>(IEnumerable<T>? sequence)
	{
		if(sequence is null) return Many<T>.None;

		if(sequence is T[]     array) return new(array);
		if(sequence is List<T> list)  return new(list);

		var builder = new Many<T>.Builder();
		foreach(var value in sequence)
		{
			builder.Add(value);
		}
		return builder;
	}
}

/// <summary>
/// Read-only collection optimized for cases when it usually contains 1 or 0 items to
/// avoid unnecessary array allocations.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
#if NET8_0_OR_GREATER
[System.Runtime.CompilerServices.CollectionBuilder(typeof(Many), nameof(Many.Create))]
#endif
[StructLayout(LayoutKind.Auto)]
public readonly struct Many<T> : IReadOnlyList<T>
{
	/// <summary>Empty collection.</summary>
	public static readonly Many<T> None = new(0, default, default);

	private readonly int _count;
	private readonly T   _value = default!;
	private readonly T[] _array = default!;

	/// <summary>Collection enumerator.</summary>
	/// <param name="values">Enumerated collection.</param>
	public struct Enumerator(Many<T> values) : IEnumerator<T>
	{
		private int _index = -1;

		public readonly T Current => values[_index];

		readonly object? IEnumerator.Current => Current;

		public bool MoveNext()
		{
			var next = _index + 1;
			if(next >= values.Count) return false;
			_index = next;
			return true;
		}

		public void Reset() => _index = -1;

		public readonly void Dispose() { }
	}

	public struct Builder
	{
		private T? _value;
		private List<T>? _values;

		public int Count { readonly get; private set; }

		public void Add(T value)
		{
			if(_values is not null)
			{
				_values.Add(value);
				Count = _values.Count;
				return;
			}
			switch(Count)
			{
				case 0:
					_value  = value;
					Count   = 1;
					break;
				case 1:
					_values = [_value!, value];
					_value  = default;
					Count   = 2;
					break;
				default: throw new ApplicationException();
			}
		}

		public void AddRange(T[]? range)
		{
			if(range is not { Length: not 0 }) return;

			if(_values is not null)
			{
#if NET9_0_OR_GREATER
				_values.AddRange(range.AsSpan());
#else
				_values.AddRange(range);
#endif
				Count = _values.Count;
				return;
			}
			switch(Count)
			{
				case 0:
					if(range.Length == 1)
					{
						_value  = range[0];
						Count   = 1;
					}
					else
					{
						_values = [.. range];
						Count   = _values.Count;
					}
					break;
				case 1:
					_values = [_value!, .. range];
					_value  = default;
					Count   = _values.Count;
					break;
				default: throw new ApplicationException();
			}
		}

		public static implicit operator Many<T>(Builder builder) => builder.ToMany();

		public readonly Many<T> ToMany()
			=> Count switch
			{
				0 => None,
				1 => new(Count, _value!, default),
				_ => new(Count, default, [.. _values!]),
			};
	}

	private Many(int count, T? value, T[]? array)
	{
		_count = count;
		_value = value!;
		_array = array!;
	}

	public Many(T value) : this(1, value, default)
	{
	}

	public Many(T[]? values)
	{
		if(values is null || values.Length == 0)
		{
			_count = 0;
		}
		else if(values.Length == 1)
		{
			_count = 1;
			_value = values[0];
		}
		else
		{
			_count = values.Length;
			_array = values;
		}
	}

	public Many(List<T>? values)
	{
		if(values is null || values.Count == 0)
		{
			_count = 0;
		}
		else if(values.Count == 1)
		{
			_count = 1;
			_value = values[0];
		}
		else
		{
			_count = values.Count;
			_array = [.. values];
		}
	}

	public Many(IReadOnlyList<T>? values)
	{
		if(values is null)
		{
			_count = 0;
			return;
		}
		_count = values.Count;
		if(_count == 1)
		{
			_value = values[0];
		}
		else if(Count > 1)
		{
			_array = [.. values];
		}
	}

	public static implicit operator Many<T>(T value) => new(value);

	public static implicit operator Many<T>(T[]? values) => new(values);

	public static implicit operator Many<T>(List<T>? values) => new(values);

	/// <inheritdoc/>
	public readonly T this[int index] => Count switch
	{
		<= 0                 => throw new ArgumentOutOfRangeException(nameof(index)),
		<= 1 when index == 0 => _value,
		 > 1                 => _array[index],
		   _                 => throw new ArgumentOutOfRangeException(nameof(index)),
	};

	public readonly T First() => Count switch
	{
		<= 0 => throw new InvalidOperationException("Sequence contains no elements."),
		<= 1 => _value,
		 > 1 => _array[0],
	};

	public readonly T? FirstOrDefault() => Count switch
	{
		<= 0 => default,
		<= 1 => _value,
		 > 1 => _array[0],
	};

	public readonly T Single() => Count switch
	{
		1 => _value,
		_ => throw new InvalidOperationException("Sequence does not contain 1 element."),
	};

	public readonly T? SingleOrDefault() => Count switch
	{
		1 => _value,
		_ => default,
	};

	public int IndexOf(T item)
		=> Count switch
		{
			<= 0 => -1,
			<= 1 => EqualityComparer<T>.Default.Equals(_value, item) ? 0 : -1,
			 > 1 => Array.IndexOf(_array, item),
		};

	public bool Contains(T item)
		=> Count switch
		{
			<= 0 => false,
			<= 1 => EqualityComparer<T>.Default.Equals(_value, item),
			 > 1 => Array.IndexOf(_array, item) >= 0,
		};

	/// <inheritdoc/>
	public readonly int Count => _count;

	public readonly bool IsEmpty => Count <= 0;

	public Many<K> ConvertAll<K>(Converter<T, K> convert)
		=> Count switch
		{
			<= 0 => Many<K>.None,
			<= 1 => new(    1, convert(_value), default),
			 > 1 => new(Count, default, Array.ConvertAll(_array, convert)),
		};

	public readonly Many<T> With(T value)
	{
		switch(Count)
		{
			case <= 0: return new(value);
			case <= 1: return new(new[] { _value, value });
			case  > 1:
				var array = new T[_array.Length + 1];
				_array.CopyTo(array, 0);
				array[array.Length - 1] = value;
				return new(array);
		}
	}

	public readonly bool Any(Predicate<T> predicate)
		=> Count switch
		{
			<= 0 => false,
			<= 1 => predicate(_value),
			 > 1 => Array.Exists(_array, predicate),
		};

	public readonly int CountOf(Predicate<T> predicate)
	{
		switch(Count)
		{
			case <= 0: return 0;
			case <= 1: return predicate(_value) ? 1 : 0;
			case  > 1:
				var count = 0;
				foreach(var item in _array)
				{
					if(predicate(item)) ++count;
				}
				return count;
		}
	}

	public readonly List<T> ToList() => Count switch
	{
		<= 0 => [],
		<= 1 => [_value],
		 > 1 => [.. _array],
	};

	public readonly T[] ToArray() => Count switch
	{
		<= 0 => Preallocated<T>.EmptyArray,
		<= 1 => [_value],
		 > 1 => (T[])_array.Clone(),
	};

#if NETCOREAPP

	public readonly ImmutableArray<T> ToImmutableArray() => Count switch
	{
		<= 0 => ImmutableArray<T>.Empty,
		<= 1 => ImmutableArray.Create(_value!),
		 > 1 => ImmutableArray.Create(_array!),
	};

#endif

	public Builder ToBuilder()
	{
		var builder = new Builder();
		if(Count == 1)
		{
			builder.Add(_value);
		}
		else if(Count > 1)
		{
			builder.AddRange(_array);
		}
		return builder;
	}

	private static string ToString(T[] array)
	{
		Assert.IsNotNull(array);

		var sb = new StringBuilder();
		sb.Append('[');
		for(int i = 0; i < array.Length; ++i)
		{
			if(sb.Length > 128)
			{
				sb.Append(", ...");
				break;
			}
			if(i > 0)
			{
				sb.Append(", ");
			}
			sb.Append(array[i]);
		}
		sb.Append(']');
		return sb.ToString();
	}

	/// <inheritdoc/>
	public readonly override string ToString() => Count switch
	{
		<= 0 => "[]",
		<= 1 => $"[{_value?.ToString()}]",
		 > 1 => ToString(_array),
	};

	public Enumerator GetEnumerator() => new(this);

	/// <inheritdoc/>
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

#define HAS_AGGRESSIVE_INLINING

#nullable enable

namespace gitter;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// A static class for retail validated assertions.
/// Instead of breaking into the debugger an exception is thrown.
/// </summary>
internal static partial class Verify
{
	/// <summary>Method argument verification methods.</summary>
	public static partial class Argument
	{
		/// <summary>Ensure that a string argument is neither null nor empty.</summary>
		/// <param name="value">The string to validate.</param>
		/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNeitherNullNorEmpty([NotNull] string? value,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

#if NET7_0_OR_GREATER
			ArgumentException.ThrowIfNullOrEmpty(value, parameterName);
#else
			const string errorMessage = "The parameter can not be either null or empty.";
			if(value is null)
			{
				throw new ArgumentNullException(parameterName, errorMessage);
			}
			if(value.Length == 0)
			{
				throw new ArgumentException(errorMessage, parameterName);
			}
#endif
		}

		/// <summary>Ensure that a string argument is neither null nor does it consist only of whitespace.</summary>
		/// <param name="value">The string to validate.</param>
		/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNeitherNullNorWhitespace([NotNull] string? value,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

#if NET7_0_OR_GREATER
			ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
#else
			const string errorMessage = "The parameter can not be either null or empty or consist only of white space characters.";
			if(value is null)
			{
				throw new ArgumentNullException(parameterName, errorMessage);
			}
			if(string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException(errorMessage, parameterName);
			}
#endif
		}

		/// <summary>Verifies that an argument is not default value.</summary>
		/// <typeparam name="T">Type of the object to validate. Must be a class.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNotDefault<T>(T? obj,
			[CallerArgumentExpression(nameof(obj))] string? parameterName = null)
			where T : struct
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(default(T).Equals(obj))
			{
				throw new ArgumentException(
					string.Format(
						CultureInfo.InvariantCulture,
						"The parameter must not be the default value ({0}).",
						default(T).ToString()),
					parameterName);
			}
		}

		/// <summary>Verifies that an argument is not <c>null</c>.</summary>
		/// <typeparam name="T">Type of the object to validate.  Must be a class.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNotNull<T>([NotNull] T? obj,
			[CallerArgumentExpression(nameof(obj))] string? parameterName = null)
			where T : class
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(obj is null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		/// <summary>Verifies that an argument is <c>null</c>.</summary>
		/// <typeparam name="T">Type of the object to validate. Must be a class.</typeparam>
		/// <param name="obj">The object to validate.</param>
		/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNull<T>(T? obj,
			[CallerArgumentExpression(nameof(obj))] string? parameterName = null)
			where T : class
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(obj is not null)
			{
				throw new ArgumentException("The parameter must be null.", parameterName);
			}
		}

		/// <summary>
		/// Verifies that sequence contains no <c>null</c> items.
		/// Throws an <see cref="ArgumentException"/> if contains.
		/// </summary>
		/// <param name="sequence">Sequence to validate.</param>
		/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void HasNoNullItems<T>(IEnumerable<T> sequence,
			[CallerArgumentExpression(nameof(sequence))] string? parameterName = null)
			where T : class
		{
			Assert.IsNotNull(sequence);
			Assert.IsNeitherNullNorWhitespace(parameterName);

			foreach(var item in sequence)
			{
				if(item is null)
				{
					throw new ArgumentException("Sequence contains null elements.", parameterName);
				}
			}
		}

		/// <summary>
		/// Verifies that argument is not a negative number. Throws an ArgumentOutOfRangeException if it's not.
		/// </summary>
		/// <param name="value">The statement to be verified as true.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNotNegative(int value,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(value < 0) throw new ArgumentOutOfRangeException(parameterName, "Must be non-negative.");
		}

		/// <summary>
		/// Verifies that argument is not a negative number. Throws an ArgumentOutOfRangeException if it's not.
		/// </summary>
		/// <param name="value">The statement to be verified as true.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNotNegative(float value,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(value < 0) throw new ArgumentOutOfRangeException(parameterName, "Must be non-negative.");
		}

		/// <summary>
		/// Verifies that argument is not a negative number. Throws an ArgumentOutOfRangeException if it's not.
		/// </summary>
		/// <param name="value">The statement to be verified as true.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNotNegative(double value,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(value < 0) throw new ArgumentOutOfRangeException(parameterName, "Must be non-negative.");
		}

		/// <summary>
		/// Verifies that argument is a positive number (&gt; 0). Throws an ArgumentOutOfRangeException if it's not.
		/// </summary>
		/// <param name="value">The statement to be verified as true.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsPositive(int value,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(value < 1) throw new ArgumentOutOfRangeException(parameterName, "Must be non-negative.");
		}

		/// <summary>
		/// Verifies that argument is a positive number (&gt; 0). Throws an ArgumentOutOfRangeException if it's not.
		/// </summary>
		/// <param name="value">The statement to be verified as true.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsPositive(float value,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(value < 1) throw new ArgumentOutOfRangeException(parameterName, "Must be non-negative.");
		}

		/// <summary>
		/// Verifies that argument is a positive number (&gt; 0). Throws an ArgumentOutOfRangeException if it's not.
		/// </summary>
		/// <param name="value">The statement to be verified as true.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsPositive(double value,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(value < 1)
			{
				throw new ArgumentOutOfRangeException(parameterName, "Must be non-negative.");
			}
		}

		/// <summary>
		/// Verifies that argument is greater than specified number.
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if it's not.
		/// </summary>
		/// <param name="value">Argument value.</param>
		/// <param name="minValue">Value to compare with.</param>
		/// <param name="parameterName">Name of the parameter to include in the <see cref="ArgumentException"/>.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsGreaterThan(double value, double minValue,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(value <= minValue)
			{
				throw new ArgumentOutOfRangeException(
					string.Format(
						CultureInfo.InvariantCulture,
						"Must be greater than {0}.", minValue),
					parameterName);
			}
		}

		/// <summary>
		/// Verifies that argument is lesser than the specified number.
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if it's not.
		/// </summary>
		/// <param name="value">Argument value.</param>
		/// <param name="minValue">Value to compare with.</param>
		/// <param name="parameterName">Name of the parameter to include in the <see cref="ArgumentException"/>.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsLesserThan(double value, double minValue,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(value >= minValue)
			{
				throw new ArgumentOutOfRangeException(
					string.Format(
						CultureInfo.InvariantCulture,
						"Must be lesser than {0}.", minValue),
					parameterName);
			}
		}

		/// <summary>
		/// Verifies the specified statement is true.  Throws an ArgumentException if it's not.
		/// </summary>
		/// <param name="statement">The statement to be verified as true.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsTrue(bool statement,
			[CallerArgumentExpression(nameof(statement))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(!statement) throw new ArgumentException("", parameterName);
		}

		/// <summary>
		/// Verifies the specified statement is true.  Throws an ArgumentException if it's not.
		/// </summary>
		/// <param name="statement">The statement to be verified as true.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		/// <param name="message">The message to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsTrue(bool statement, string parameterName, string message)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(!statement) throw new ArgumentException(message, parameterName);
		}

		/// <summary>
		/// Verifies the specified statement is false. Throws an ArgumentException if it's not.
		/// </summary>
		/// <param name="statement">The statement to be verified as false.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsFalse(bool statement, string parameterName)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(statement) throw new ArgumentException("", parameterName);
		}

		/// <summary>
		/// Verifies the specified statement is false.  Throws an ArgumentException if it's not.
		/// </summary>
		/// <param name="statement">The statement to be verified as false.</param>
		/// <param name="parameterName">Name of the parameter to include in the ArgumentException.</param>
		/// <param name="message">The message to include in the ArgumentException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsFalse(bool statement, string parameterName, string message)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(statement)
			{
				throw new ArgumentException(message, parameterName);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void UriIsAbsolute(Uri uri, [CallerArgumentExpression(nameof(uri))] string? parameterName = null)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(uri == null)
			{
				throw new ArgumentNullException(parameterName, "The URI cannot be null.");
			}
			if(!uri.IsAbsoluteUri)
			{
				throw new ArgumentException("The URI must be absolute.", parameterName);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void AreEqual<T>(T expected, T actual, string parameterName, string message)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(null == expected)
			{
				// Two nulls are considered equal, regardless of type semantics.
				if(null != actual && !actual.Equals(expected))
				{
					throw new ArgumentException(message, parameterName);
				}
			}
			else if(!expected.Equals(actual))
			{
				throw new ArgumentException(message, parameterName);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void AreNotEqual<T>(T notExpected, T actual, string parameterName, string message)
		{
			Assert.IsNeitherNullNorWhitespace(parameterName);

			if(null == notExpected)
			{
				// Two nulls are considered equal, regardless of type semantics.
				if(null == actual || actual.Equals(notExpected))
				{
					throw new ArgumentException(message, parameterName);
				}
			}
			else if(notExpected.Equals(actual))
			{
				throw new ArgumentException(message, parameterName);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void TypeSupportsInterface(Type type, Type interfaceType, string parameterName)
		{
			Assert.IsNeitherNullNorEmpty(parameterName);

			IsNotNull(type, "type");
			IsNotNull(interfaceType, "interfaceType");

			if(type.GetInterface(interfaceType.Name) == null)
			{
				throw new ArgumentException(
					"The type of this parameter does not support a required interface",
					parameterName);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void FileExists(string filePath, string parameterName)
		{
			Assert.IsNeitherNullNorEmpty(parameterName);

			IsNeitherNullNorWhitespace(filePath, parameterName);
			if(!File.Exists(filePath))
			{
				throw new ArgumentException(
					string.Format(
						CultureInfo.InvariantCulture,
						"No file exists at \"{0}\"",
						filePath),
					parameterName);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void DirectoryExists(string directoryPath, string parameterName)
		{
			Assert.IsNeitherNullNorEmpty(parameterName);

			IsNeitherNullNorWhitespace(directoryPath, parameterName);
			if(!Directory.Exists(directoryPath))
			{
				throw new ArgumentException(
					string.Format(
						CultureInfo.InvariantCulture,
						"No file exists at \"{0}\"",
						directoryPath),
					parameterName);
			}
		}

		/// <summary>
		/// Verifies that the specified value is within the expected range.  The assertion fails if it isn't.
		/// </summary>
		/// <param name="value">The value to verify.</param>
		/// <param name="upperBoundExclusive">The upper bound exclusive value.</param>
		/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsValidIndex(int value, int upperBoundExclusive,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsTrue(upperBoundExclusive >= 0);
			Assert.IsNeitherNullNorEmpty(parameterName);

			if(value < 0 || value >= upperBoundExclusive)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					string.Format(
						CultureInfo.InvariantCulture,
						"The System.Int32 value must be bounded with [{0}, {1}]",
						0,
						upperBoundExclusive - 1));
			}
		}

		/// <summary>
		/// Verifies that the specified value is within the expected range.  The assertion fails if it isn't.
		/// </summary>
		/// <param name="lowerBoundInclusive">The lower bound inclusive value.</param>
		/// <param name="value">The value to verify.</param>
		/// <param name="upperBoundExclusive">The upper bound exclusive value.</param>
		/// <param name="parameterName">The name of the parameter that will be presented if an exception is thrown.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsValidIndex(int lowerBoundInclusive, int value, int upperBoundExclusive,
			[CallerArgumentExpression(nameof(value))] string? parameterName = default)
		{
			Assert.IsTrue(upperBoundExclusive >= lowerBoundInclusive);
			Assert.IsNeitherNullNorEmpty(parameterName);

			if(value < lowerBoundInclusive || value >= upperBoundExclusive)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					string.Format(
						CultureInfo.InvariantCulture,
						"The System.Int32 value must be bounded with [{0}, {1})",
						lowerBoundInclusive,
						upperBoundExclusive));
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsInRange(int lowerBoundInclusive, int value, int upperBoundInclusive, string message,
			[CallerArgumentExpression(nameof(value))] string? parameterName = default)
		{
			Assert.IsTrue(upperBoundInclusive >= lowerBoundInclusive);
			Assert.IsNeitherNullNorEmpty(parameterName);

			if(value < lowerBoundInclusive || value > upperBoundInclusive)
			{
				throw new ArgumentOutOfRangeException(parameterName, message);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsInRange(int lowerBoundInclusive, int value, int upperBoundInclusive,
			[CallerArgumentExpression(nameof(value))] string? parameterName = default)
		{
			Assert.IsTrue(upperBoundInclusive >= lowerBoundInclusive);
			Assert.IsNeitherNullNorEmpty(parameterName);

			if(value < lowerBoundInclusive || value > upperBoundInclusive)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					string.Format(
						CultureInfo.InvariantCulture,
						"The System.Int32 value must be bounded with [{0}, {1}]",
						lowerBoundInclusive,
						upperBoundInclusive));
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsInRange(double lowerBoundInclusive, double value, double upperBoundInclusive, string message, string parameterName)
		{
			Assert.IsTrue(upperBoundInclusive >= lowerBoundInclusive);
			Assert.IsNeitherNullNorEmpty(parameterName);

			if(value < lowerBoundInclusive || value > upperBoundInclusive)
			{
				throw new ArgumentOutOfRangeException(parameterName, message);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsInRange(float lowerBoundInclusive, float value, float upperBoundInclusive,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsTrue(upperBoundInclusive >= lowerBoundInclusive);
			Assert.IsNeitherNullNorEmpty(parameterName);

			if(value < lowerBoundInclusive || value > upperBoundInclusive)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					string.Format(
						CultureInfo.InvariantCulture,
						"The System.Single value must be bounded with [{0}, {1}]",
						lowerBoundInclusive,
						upperBoundInclusive));
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsInRange(double lowerBoundInclusive, double value, double upperBoundInclusive,
			[CallerArgumentExpression(nameof(value))] string? parameterName = null)
		{
			Assert.IsTrue(upperBoundInclusive >= lowerBoundInclusive);
			Assert.IsNeitherNullNorEmpty(parameterName);

			if(value < lowerBoundInclusive || value > upperBoundInclusive)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					string.Format(
						CultureInfo.InvariantCulture,
						"The System.Double value must be bounded with [{0}, {1}]",
						lowerBoundInclusive,
						upperBoundInclusive));
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void ImplementsInterface(object parameter, Type interfaceType, string parameterName)
		{
			Assert.IsNotNull(parameter);
			Assert.IsNotNull(interfaceType);
			Assert.IsTrue(interfaceType.IsInterface);
			Assert.IsNeitherNullNorEmpty(parameterName);

			bool isImplemented = false;
			foreach(var ifaceType in parameter.GetType().GetInterfaces())
			{
				if(ifaceType == interfaceType)
				{
					isImplemented = true;
					break;
				}
			}

			if(!isImplemented)
			{
				throw new ArgumentException(
					string.Format(
						CultureInfo.InvariantCulture,
						"The parameter must implement {0} interface.",
						interfaceType.ToString()),
					parameterName);
			}
		}
	}

	/// <summary>Object state verification methods.</summary>
	public static partial class State
	{
		/// <summary>
		/// Ensure that the current thread's apartment state is what's expected.
		/// </summary>
		/// <param name="requiredState">
		/// The required apartment state for the current thread.
		/// </param>
		/// <param name="message">
		/// The message string for the exception to be thrown if the state is invalid.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the calling thread's apartment state is not the same as the requiredState.
		/// </exception>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsApartmentState(ApartmentState requiredState, string message)
		{
			if(Thread.CurrentThread.GetApartmentState() != requiredState)
			{
				throw new InvalidOperationException(message);
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void PropertyIsNotNull<T>(T obj, string name) where T : class
		{
			if(obj is null)
			{
				throw new InvalidOperationException(
					string.Format(
						CultureInfo.InvariantCulture,
						"The property {0} cannot be null at this time.",
						name));
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void PropertyIsNull<T>(T obj, string name) where T : class
		{
			if(obj is not null)
			{
				throw new InvalidOperationException(
					string.Format(
					CultureInfo.InvariantCulture,
					"The property {0} must be null at this time.",
					name));
			}
		}

		/// <summary>
		/// Verifies the specified statement is true. Throws an <see cref="InvalidOperationException"/> if it's not.
		/// </summary>
		/// <param name="statement">The statement to be verified as true.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsTrue([DoesNotReturnIf(false)] bool statement)
		{
			if(!statement) throw new InvalidOperationException();
		}

		/// <summary>
		/// Verifies the specified statement is true. Throws an <see cref="InvalidOperationException"/> if it's not.
		/// </summary>
		/// <param name="statement">The statement to be verified as true.</param>
		/// <param name="message">The message to include in the InvalidOperationException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsTrue([DoesNotReturnIf(false)] bool statement, string message)
		{
			if(!statement) throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Verifies the specified statement is false. Throws an <see cref="InvalidOperationException"/> if it's not.
		/// </summary>
		/// <param name="statement">The statement to be verified as false.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsFalse([DoesNotReturnIf(true)] bool statement)
		{
			if(statement) throw new InvalidOperationException();
		}

		/// <summary>
		/// Verifies the specified statement is false. Throws an <see cref="InvalidOperationException"/> if it's not.
		/// </summary>
		/// <param name="statement">The statement to be verified as false.</param>
		/// <param name="message">The message to include in the InvalidOperationException.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsFalse([DoesNotReturnIf(true)] bool statement, string message)
		{
			if(statement) throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Verifies the specified instance is not disposed. Throws an <see cref="ObjectDisposedException"/> if it is.
		/// </summary>
		/// <param name="isDisposed">Disposed flag value.</param>
		/// <param name="instance">Object instance.</param>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[DebuggerStepThrough]
#if HAS_AGGRESSIVE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static void IsNotDisposed([DoesNotReturnIf(true)] bool isDisposed, object instance)
		{
			Assert.IsNotNull(instance);

#if NET9_0_OR_GREATER
			ObjectDisposedException.ThrowIf(isDisposed, instance);
#else
			if(isDisposed) throw new ObjectDisposedException(instance.GetType().Name);
#endif
		}
	}
}

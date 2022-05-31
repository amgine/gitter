#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.CLI;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Buffers;

/// <summary>Reads text from stdio/stderr.</summary>
public class AsyncTextReader : AsyncOutputReceiverBase, IOutputReceiver
{
	private readonly StringBuilder _stringBuilder;
	private char[] _charBuffer;
	private Decoder _decoder;

	/// <summary>Initializes a new instance of the <see cref="AsyncTextReader"/> class.</summary>
	/// <param name="bufferSize">Size of the internal buffer.</param>
	public AsyncTextReader(int bufferSize = 0x400) : base(bufferSize)
	{
		Verify.Argument.IsPositive(bufferSize);

		_stringBuilder = new StringBuilder(capacity: bufferSize);
	}

	/// <inheritdoc/>
	protected override void InitializeCore(Process process, StreamReader reader, ArraySegment<byte> buffer)
	{
		Assert.IsNotNull(process);
		Assert.IsNotNull(reader);

		var encoding = reader.CurrentEncoding;
		_decoder     = encoding.GetDecoder();
		_charBuffer  = ArrayPool<char>.Shared.Rent(encoding.GetMaxCharCount(buffer.Count) + 1);

		_stringBuilder.Clear();
	}

	/// <inheritdoc/>
	protected override void DeinitializeCore()
	{
		_charBuffer = default;
		_decoder    = default;
	}

	/// <inheritdoc/>
	protected sealed override void Process(ArraySegment<byte> buffer)
	{
		int charsCount = _decoder.GetChars(buffer.Array, buffer.Offset, buffer.Count, _charBuffer, 0);
		OnStringDecoded(_charBuffer, 0, charsCount);
	}

	/// <summary>Returns the length of composed text.</summary>
	public int Length => _stringBuilder.Length;

	/// <summary>Returns character at the specified position.</summary>
	/// <param name="index">Character index.</param>
	/// <returns>Character at the specified position.</returns>
	public char this[int index] => _stringBuilder[index];

	/// <summary>Returns composed text.</summary>
	/// <returns>Composed text.</returns>
	public string GetText() => _stringBuilder.ToString();

	/// <summary>Returns composed text.</summary>
	/// <param name="startIndex">Index of the first character.</param>
	/// <param name="length">Length of the returned string.</param>
	/// <returns>Composed text.</returns>
	public string GetText(int startIndex, int length)
		=> _stringBuilder.ToString(startIndex, length);

	/// <summary>
	/// Copies the characters from a specified segment of this instance to a specified
	/// segment of a destination System.Char array.
	/// </summary>
	/// <param name="sourceIndex">
	/// The starting position in this instance where characters will be copied from.
	/// The index is zero-based.
	/// </param>
	/// <param name="destination">The array where characters will be copied.</param>
	/// <param name="destinationIndex">
	/// The starting position in destination where characters will be copied.
	/// The index is zero-based.
	/// </param>
	/// <param name="count">The number of characters to be copied.</param>
	public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
	{
		_stringBuilder.CopyTo(sourceIndex, destination, destinationIndex, count);
	}

	/// <summary>Returns array of collected characters.</summary>
	/// <returns>Array of collected characters.</returns>
	public char[] GetCharArray()
	{
		var array = new char[_stringBuilder.Length];
		_stringBuilder.CopyTo(0, array, 0, _stringBuilder.Length);
		return array;
	}

	/// <summary>Clears internal buffer.</summary>
	public void Clear()
	{
		Verify.State.IsFalse(IsInitialized);

		_stringBuilder.Clear();
	}

	protected virtual void OnStringDecoded(char[] buffer, int startIndex, int length)
	{
		_stringBuilder.Append(_charBuffer, startIndex, length);
	}
}

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
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Text;

/// <summary>Reads text from <c>stdio</c>/<c>stderr</c> and parses it with a <see cref="ITextParser"/>.</summary>
/// <param name="parser">Output parser.</param>
/// <param name="bufferSize">Size of the internal buffer.</param>
public sealed class AsyncTextParser(ITextParser parser, int bufferSize = 0x400)
	: AsyncOutputReceiverBase(bufferSize)
{
#if !NETCOREAPP
	private readonly CharArrayTextSegment _textSegment = new();
#endif
	private char[]? _charBuffer;
	private Decoder? _decoder;

	/// <inheritdoc/>
	protected override void InitializeCore(Process process, StreamReader reader, ArraySegment<byte> buffer)
	{
		Assert.IsNotNull(process);
		Assert.IsNotNull(reader);

		var encoding = reader.CurrentEncoding;
		_decoder     = encoding.GetDecoder();
		_charBuffer  = ArrayPool<char>.Shared.Rent(encoding.GetMaxCharCount(buffer.Count) + 1);
	}

	/// <inheritdoc/>
	protected override void DeinitializeCore()
	{
		if(_charBuffer is not null)
		{
			ArrayPool<char>.Shared.Return(_charBuffer);
			_charBuffer = null;
		}
		_decoder    = null;
	}

	/// <inheritdoc/>
	protected override void Process(ArraySegment<byte> buffer)
	{
		int charsCount = _decoder!.GetChars(buffer.Array!, buffer.Offset, buffer.Count, _charBuffer!, 0);
		if(charsCount != 0)
		{
#if NETCOREAPP
			parser.Parse(new ReadOnlySpan<char>(_charBuffer, 0, charsCount));
#else
			_textSegment.SetBuffer(_charBuffer!, 0, charsCount);
			parser.Parse(_textSegment);
#endif
		}
	}

	/// <inheritdoc/>
	protected override void Complete() => parser.Complete();
}

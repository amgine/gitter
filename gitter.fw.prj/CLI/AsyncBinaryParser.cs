#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2023  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

/// <summary>Reads text from <c>stdio</c>/<c>stderr</c> and parses it with a <see cref="IBinaryParser"/>.</summary>
public sealed class AsyncBinaryParser : AsyncOutputReceiverBase
{
	/// <summary>Initializes a new instance of the <see cref="AsyncTextParser"/> class.</summary>
	/// <param name="parser">Output parser.</param>
	/// <param name="bufferSize">Size of the internal buffer.</param>
	public AsyncBinaryParser(IBinaryParser parser, int bufferSize = 0x400)
		: base(bufferSize)
	{
		Verify.Argument.IsNotNull(parser);

		Parser = parser;
	}

	private IBinaryParser Parser { get; }

	/// <inheritdoc/>
	protected override void Process(ArraySegment<byte> buffer)
		=> Parser.Parse(buffer);

	/// <inheritdoc/>
	protected override void Complete()
		=> Parser.Complete();
}

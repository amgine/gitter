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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.Text;

public interface IHashValue<TSelf>
	: IEquatable <TSelf>
	, IComparable<TSelf>
	, IComparable
	, IFormattable
#if NET7_0_OR_GREATER
	, System.Numerics.IEqualityOperators   <TSelf, TSelf, bool>
	, System.Numerics.IComparisonOperators <TSelf, TSelf, bool>
#endif
#if NET7_0_OR_GREATER
	where TSelf : System.Numerics.IEqualityOperators   <TSelf, TSelf, bool>,
	              System.Numerics.IComparisonOperators <TSelf, TSelf, bool>
#endif
{
#if NET7_0_OR_GREATER
	static abstract IEqualityComparer<TSelf> EqualityComparer { get; }
	static abstract IComparer        <TSelf> Comparer         { get; }

	static abstract int Size { get; }
	static abstract int HexStringLength { get; }

	static abstract unsafe bool TryParse(byte* hashStringUtf8,  out TSelf hash);
	static abstract unsafe bool TryParse(char* hashStringUtf16, out TSelf hash);

	static abstract unsafe bool TryParse(ReadOnlySpan<byte> hashStringUtf8,  out TSelf hash);
	static abstract unsafe bool TryParse(ReadOnlySpan<char> hashStringUtf16, out TSelf hash);

	static abstract unsafe TSelf Parse(byte* hashStringUtf8);
	static abstract unsafe TSelf Parse(char* hashStringUtf16);

	static abstract unsafe TSelf Parse(string hashStringUtf16);
	static abstract unsafe TSelf Parse(string hashStringUtf16, int offset);

	static abstract unsafe TSelf Parse(ReadOnlySpan<byte> hashStringUtf8);
	static abstract unsafe TSelf Parse(ReadOnlySpan<char> hashStringUtf16);
#endif

	unsafe void ToString(char* hashStringUtf16);

	unsafe void ToString(char* hashStringUtf16, int length);

	void ToString(StringBuilder stringBuilder);

	void ToString(StringBuilder stringBuilder, int length);

	string ToString(int length);
}

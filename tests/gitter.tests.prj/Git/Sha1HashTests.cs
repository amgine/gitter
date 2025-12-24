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
using System.Text;

using NUnit.Framework;

[TestFixture]
class Sha1HashTests
{
	[Test(Description = "Ensures SHA1 size is 20 bytes")]
	public unsafe void SizeOfIs20()
	{
		Assert.That(sizeof(Sha1Hash), Is.EqualTo(Sha1Hash.Size).And.EqualTo(20));
	}

	[Test]
	[TestCase("a8f5c3b2ef1b3a131f89eef419b0425664c9ddd7", ExpectedResult = true)]
	[TestCase("a8f5c3b2ef1b3v131f89eef419b0425664c9ddd7", ExpectedResult = false)]
	[TestCase("a8f5c3b2ef1b3a131f89eef419b0425664c9ddd7d", ExpectedResult = false)]
	[TestCase("a", ExpectedResult = false)]
	[TestCase("", ExpectedResult = false)]
	[TestCase(default(string), ExpectedResult = false)]
	public bool IsValidString(string value)
		=> Sha1Hash.IsValidString(value);

	[Test]
	[TestCase("a8f5c3b2ef1b32131f89eef419b0425664c9ddd7")]
	[TestCase("a8f5c3b2ef1b32131f89eef419b0425664c9ddd7x")]
	public void ParseValidString(string value)
	{
		Assert.That(
			Sha1Hash.Parse(value).ToByteArray(),
			Is.EqualTo(Convert.FromHexString(value.Substring(0, Sha1Hash.HexStringLength))));
	}

	[Test]
	[TestCase("a")]
	[TestCase("a8f5c3b2ef1b3v131f89eef419b0425664c9ddd7")]
	public void ParseInvalidStringThrowsArgumentException(string value)
	{
		Assert.That(() => Sha1Hash.Parse(value), Throws.ArgumentException);
	}

	[Test]
	public void ParseNullStringThrowsArgumentNullException()
	{
		Assert.That(() => Sha1Hash.Parse(default(string)), Throws.ArgumentNullException);
	}

	[Test]
	[TestCase("a8f5c3b2ef1b32131f89eef419b0425664c9ddd7")]
	[TestCase("a8f5c3b2e31b32131f89eefd19b0425664c9dddbx")]
	public void ParseValidUtf8Array(string value)
	{
		var hash = Sha1Hash.Parse(Encoding.UTF8.GetBytes(value));
		Assert.That(hash.ToString(), Is.EqualTo(value.Substring(0, Sha1Hash.HexStringLength)));
	}

	[Test]
	[TestCase("a8f5c3b2ef1b32131f89eef419b0425664c9ddd7", 0)]
	[TestCase("xa8f5c3b2ef1b32131f89eef419b0425664c9ddd7", 1)]
	public void ParseValidUtf8ArrayOffset(string value, int offset)
	{
		var hash = Sha1Hash.Parse(Encoding.UTF8.GetBytes(value), offset);
		Assert.That(hash.ToString(), Is.EqualTo(value.Substring(offset, Sha1Hash.HexStringLength)));
	}

#if NETCOREAPP

	[Test]
	[TestCase("a8f5c3b2ef1b32131f89eef419b0425664c9ddd7")]
	public void ParseValidUtf8Span(string value)
	{
		var hash = Sha1Hash.Parse(Encoding.UTF8.GetBytes(value).AsSpan());
		Assert.That(hash.ToString(), Is.EqualTo(value));
	}

	[Test]
	[TestCase("a")]
	[TestCase("a8f5c3b2ef1b3v131f89eef419b0425664c9ddd7")]
	public void ParseInvalidUtf8SpanThrowsArgumentException(string value)
	{
		Assert.That(() => Sha1Hash.Parse(Encoding.UTF8.GetBytes(value).AsSpan()), Throws.ArgumentException);
	}

#endif

	[Test(Description = "Ensures that SHA1 value can be reinterpreted as a hash byte sequence")]
	public unsafe void MemoryLayout()
	{
		var value = "a8f5c3b2ef1b3a131f89eef419b0425664c9ddd7";
		var hash  = Sha1Hash.Parse(value);
		var mem1  = new ReadOnlySpan<byte>(&hash, Sha1Hash.Size);
		var mem2  = new ReadOnlySpan<byte>(Convert.FromHexString(value));
		Assert.That(mem1.SequenceEqual(mem2), Is.True);
	}

	[Test]
	public void Equality()
	{
		var value = "a8f5c3b2ef1b32131f89eef419b0425664c9ddd7";
		var hash1 = Sha1Hash.Parse(value);
		var hash2 = Sha1Hash.Parse(value);
		Assert.That(hash1, Is.EqualTo(hash2));
	}

	[Test]
	public void Inequality()
	{
		var hash1 = Sha1Hash.Parse("a8f5c3b2ef1b32131f89eef419b0425664c9ddd7");
		var hash2 = Sha1Hash.Parse("a8f5c3b2ef1b32131f89eef419b0425664c9ddd3");
		Assert.That(hash1, Is.Not.EqualTo(hash2));
	}
}

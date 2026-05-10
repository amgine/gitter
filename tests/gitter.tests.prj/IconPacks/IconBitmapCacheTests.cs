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

namespace gitter.IconPacks;

using System.Drawing;

using NUnit.Framework;

[TestFixture]
class IconBitmapCacheTests
{
	private static Bitmap MakeBitmap(int size) => new(size, size);

	[Test]
	public void GetOrAdd_FirstCall_InvokesFactory()
	{
		using var cache = new IconBitmapCache(maxBytes: 1024 * 1024);
		var calls = 0;
		var b = cache.GetOrAdd("a", 16, sz => { calls++; return MakeBitmap(sz); });
		Assert.That(calls,    Is.EqualTo(1));
		Assert.That(b.Width,  Is.EqualTo(16));
	}

	[Test]
	public void GetOrAdd_SecondCall_ReturnsCachedAndDoesNotInvoke()
	{
		using var cache = new IconBitmapCache(maxBytes: 1024 * 1024);
		var first = cache.GetOrAdd("a", 16, sz => MakeBitmap(sz));
		var calls = 0;
		var second = cache.GetOrAdd("a", 16, sz => { calls++; return MakeBitmap(sz); });
		Assert.That(calls,  Is.EqualTo(0));
		Assert.That(second, Is.SameAs(first));
	}

	[Test]
	public void GetOrAdd_DifferentSizes_AreDistinctEntries()
	{
		using var cache = new IconBitmapCache(maxBytes: 1024 * 1024);
		var a16 = cache.GetOrAdd("a", 16, sz => MakeBitmap(sz));
		var a32 = cache.GetOrAdd("a", 32, sz => MakeBitmap(sz));
		Assert.That(a16, Is.Not.SameAs(a32));
		Assert.That(cache.Count, Is.EqualTo(2));
	}

	[Test]
	public void Eviction_OverCap_DropsLeastRecentlyUsed()
	{
		var perBitmap = 4L * 32 * 32;
		using var cache = new IconBitmapCache(maxBytes: perBitmap * 2);
		cache.GetOrAdd("a", 32, sz => MakeBitmap(sz));
		cache.GetOrAdd("b", 32, sz => MakeBitmap(sz));
		cache.GetOrAdd("c", 32, sz => MakeBitmap(sz));

		Assert.That(cache.TryGet("a", 32), Is.Null,    "oldest entry should be evicted");
		Assert.That(cache.TryGet("b", 32), Is.Not.Null);
		Assert.That(cache.TryGet("c", 32), Is.Not.Null);
		Assert.That(cache.Count, Is.EqualTo(2));
	}

	[Test]
	public void Access_PromotesToFront()
	{
		var perBitmap = 4L * 32 * 32;
		using var cache = new IconBitmapCache(maxBytes: perBitmap * 2);
		cache.GetOrAdd("a", 32, sz => MakeBitmap(sz));
		cache.GetOrAdd("b", 32, sz => MakeBitmap(sz));
		cache.TryGet("a", 32);
		cache.GetOrAdd("c", 32, sz => MakeBitmap(sz));

		Assert.That(cache.TryGet("a", 32), Is.Not.Null, "a was just accessed, should not be evicted");
		Assert.That(cache.TryGet("b", 32), Is.Null,     "b is now LRU and should be evicted");
		Assert.That(cache.TryGet("c", 32), Is.Not.Null);
	}

	[Test]
	public void Clear_DropsAllEntries()
	{
		using var cache = new IconBitmapCache(maxBytes: 1024 * 1024);
		cache.GetOrAdd("a", 16, sz => MakeBitmap(sz));
		cache.GetOrAdd("b", 16, sz => MakeBitmap(sz));
		cache.Clear();
		Assert.That(cache.Count,        Is.EqualTo(0));
		Assert.That(cache.CurrentBytes, Is.EqualTo(0));
	}
}

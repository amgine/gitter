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

namespace gitter.IconPacks;

using System.IO;
using System.Text;

using NUnit.Framework;

[TestFixture]
class SvgRasterizerTests
{
	private const string SimpleSvg = """
		<?xml version="1.0" encoding="UTF-8"?>
		<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
		  <rect x="2" y="2" width="20" height="20" fill="#ff0000"/>
		</svg>
		""";

	[Test]
	public void Rasterize_FromStream_ProducesBitmapOfRequestedSize()
	{
		using var stream = new MemoryStream(Encoding.UTF8.GetBytes(SimpleSvg));
		using var bitmap = SvgRasterizer.Rasterize(stream, 32);

		Assert.That(bitmap,        Is.Not.Null);
		Assert.That(bitmap.Width,  Is.EqualTo(32));
		Assert.That(bitmap.Height, Is.EqualTo(32));
	}

	[Test]
	public void Rasterize_DifferentSizes_ProducesIndependentBitmaps()
	{
		using var s16 = new MemoryStream(Encoding.UTF8.GetBytes(SimpleSvg));
		using var s64 = new MemoryStream(Encoding.UTF8.GetBytes(SimpleSvg));

		using var b16 = SvgRasterizer.Rasterize(s16, 16);
		using var b64 = SvgRasterizer.Rasterize(s64, 64);

		Assert.That(b16.Width, Is.EqualTo(16));
		Assert.That(b64.Width, Is.EqualTo(64));
	}

	[Test]
	public void Rasterize_FromFile_ReadsFromDisk()
	{
		var tmp = Path.Combine(Path.GetTempPath(), $"gitter_rast_{System.Guid.NewGuid():N}.svg");
		File.WriteAllText(tmp, SimpleSvg);
		try
		{
			using var bitmap = SvgRasterizer.Rasterize(tmp, 24);
			Assert.That(bitmap.Width,  Is.EqualTo(24));
			Assert.That(bitmap.Height, Is.EqualTo(24));
		}
		finally
		{
			File.Delete(tmp);
		}
	}
}

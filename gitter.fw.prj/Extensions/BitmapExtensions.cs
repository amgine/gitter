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

namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	public static class BitmapExtensions
	{
		public static void SetAlpha(this Bitmap bmp, byte alpha)
		{
			Verify.Argument.IsNotNull(bmp, nameof(bmp));

			var data = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height),
				System.Drawing.Imaging.ImageLockMode.ReadWrite,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			var line = data.Scan0;
			var eof = line + data.Height * data.Stride;
			while(line != eof)
			{
				var pixelAlpha = line + 3;
				var eol = pixelAlpha + data.Width * 4;
				while(pixelAlpha != eol)
				{
					Marshal.WriteByte(pixelAlpha, alpha);
					pixelAlpha += 4;
				}
				line += data.Stride;
			}
			bmp.UnlockBits(data);
		}
	}
}

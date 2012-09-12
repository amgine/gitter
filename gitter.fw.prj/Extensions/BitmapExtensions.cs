namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.Runtime.InteropServices;

	public static class BitmapExtensions
	{
		public static void SetAlpha(this Bitmap bmp, byte alpha)
		{
			Verify.Argument.IsNotNull(bmp, "bmp");

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

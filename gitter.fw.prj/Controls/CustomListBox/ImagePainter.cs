#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System.Drawing;
using System.Drawing.Drawing2D;

public static class ImagePainter
{
	private sealed class DefaultImagePainter : IImagePainter
	{
		public void Paint(Graphics graphics, Image image, Rectangle bounds)
			=> graphics.DrawImage(image, bounds);
	}

	abstract class TexturedBrushImagePainter : IImagePainter
	{
		protected abstract void Paint(Graphics graphics, TextureBrush brush, Rectangle bounds);

		private static Matrix GetTransform(Image image, Rectangle bounds)
		{
			var m = new Matrix();
			m.Translate(bounds.X + .5f, bounds.Y + .5f);
			m.Scale(bounds.Width / (float)image.Width, bounds.Height / (float)image.Height);
			return m;
		}

		public void Paint(Graphics graphics, Image image, Rectangle bounds)
		{
			using var brush = new TextureBrush(image)
			{
				WrapMode  = WrapMode.Clamp,
				Transform = GetTransform(image, bounds),
			};
			Paint(graphics, brush, bounds);
		}
	}

	sealed class CircleImagePainter : TexturedBrushImagePainter
	{
		protected override void Paint(Graphics graphics, TextureBrush brush, Rectangle bounds)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(brush);

			using(graphics.SwitchSmoothingMode(SmoothingMode.HighQuality))
			{
				graphics.FillEllipse(brush, bounds);
			}
		}
	}

	public static IImagePainter Default { get; } = new DefaultImagePainter();

	public static IImagePainter Circle { get ; } = new CircleImagePainter();
}

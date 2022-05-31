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

namespace gitter.Framework;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class DpiBoundValue
{
	internal abstract class CachedScalable<T> : IDpiBoundValue<T>
		where T : notnull
	{
		private readonly T _original;
		private readonly Dictionary<Dpi, T> _cache = new();

		public CachedScalable(T original)
		{
			_original = original;
			_cache.Add(Dpi.Default, original);
		}

		public CachedScalable(T original, Dpi dpi)
		{
			_original = original;
			_cache.Add(dpi, original);
		}

		protected abstract T Scale(T original, Dpi dpi);

		public T GetValue(Dpi dpi)
		{
			if(!_cache.TryGetValue(dpi, out var bitmap))
			{
				bitmap = Scale(_original, dpi);
				_cache.Add(dpi, bitmap);
			}
			return bitmap;
		}
	}

#if NET6_0_OR_GREATER
	[global::System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
	internal sealed class FontValueImpl : CachedScalable<Font>
	{
		private readonly Dpi _dpi;

		public FontValueImpl(Font font, Dpi dpi) : base(font, dpi) => _dpi = dpi;

		protected override Font Scale(Font original, Dpi dpi)
		{
			var emSize = original.Size * dpi.Y / _dpi.Y;
			return new Font(original.FontFamily,
				emSize,
				original.Style,
				original.Unit,
				original.GdiCharSet,
				original.GdiVerticalFont);
		}
	}

	sealed class IconImpl : IDpiBoundValue<Bitmap>
	{
		public IconImpl(CachedScaledImageResources resources, string name, int size)
		{
			Resources = resources;
			Name      = name;
			Size      = size;
		}

		private CachedScaledImageResources Resources { get; }

		private string Name { get; }

		private int Size { get; }

		public Bitmap GetValue(Dpi dpi)
		{
			var size = (int)(Size * dpi.X / 96);
			return Resources[Name, size];
		}
	}

	sealed class IconImpl2 : IDpiBoundValue<Bitmap>
	{
		public IconImpl2(IImageProvider imageProvider, int size)
		{
			ImageProvider = imageProvider;
			Size          = size;
		}

		private IImageProvider ImageProvider { get; }

		private int Size { get; }

		public Bitmap GetValue(Dpi dpi)
		{
			var size = (int)(Size * dpi.X / 96);
			return ImageProvider.GetImage(size) as Bitmap;
		}
	}

	sealed class ConstantImpl<T> : IDpiBoundValue<T>
	{
		public ConstantImpl(T value) => Value = value;

		public T Value { get; }

		public T GetValue(Dpi dpi) => Value;
	}

	sealed record class XScaledInt32Impl(int Value) : IDpiBoundValue<int>
	{
		public int GetValue(Dpi dpi) => (Value * dpi.X + 95) / 96;
	}

	sealed record class YScaledInt32Impl(int Value) : IDpiBoundValue<int>
	{
		public int GetValue(Dpi dpi) => (Value * dpi.Y + 95) / 96;
	}

	sealed record class ScaledSizeImpl(Size Value) : IDpiBoundValue<Size>
	{
		public Size GetValue(Dpi dpi) => new(
			(Value.Width  * dpi.X + 95) / 96,
			(Value.Height * dpi.Y + 95) / 96);
	}

	sealed record class ScaledSizeFImpl(SizeF Value) : IDpiBoundValue<SizeF>
	{
		public SizeF GetValue(Dpi dpi) => new(
			(Value.Width  * dpi.X) / 96,
			(Value.Height * dpi.Y) / 96);
	}

	sealed record class ScaledRectangleImpl(Rectangle Value) : IDpiBoundValue<Rectangle>
	{
		public Rectangle GetValue(Dpi dpi) => new(
			(Value.X      * dpi.X + 95) / 96,
			(Value.Y      * dpi.Y + 95) / 96,
			(Value.Width  * dpi.X + 95) / 96,
			(Value.Height * dpi.Y + 95) / 96);
	}

	sealed record class ScaledPaddingImpl(Padding Value) : IDpiBoundValue<Padding>
	{
		public Padding GetValue(Dpi dpi) => new(
			(Value.Left   * dpi.X + 95) / 96,
			(Value.Top    * dpi.Y + 95) / 96,
			(Value.Right  * dpi.X + 95) / 96,
			(Value.Bottom * dpi.Y + 95) / 96);
	}

	#if NET6_0_OR_GREATER
	[global::System.Runtime.Versioning.SupportedOSPlatform("windows")]
	#endif
	public static IDpiBoundValue<Bitmap> Icon(CachedScaledImageResources resources, string name, int size = 16)
	{
		Verify.Argument.IsNotNull(resources);
		Verify.Argument.IsNeitherNullNorWhitespace(name);
		Verify.Argument.IsPositive(size);

		return new IconImpl(resources, name, size);
	}

	#if NET6_0_OR_GREATER
	[global::System.Runtime.Versioning.SupportedOSPlatform("windows")]
	#endif
	public static IDpiBoundValue<Bitmap> Icon(IImageProvider imageProvider, int size = 16)
	{
		Verify.Argument.IsNotNull(imageProvider);
		Verify.Argument.IsPositive(size);

		return new IconImpl2(imageProvider, size);
	}

	#if NET6_0_OR_GREATER
	[global::System.Runtime.Versioning.SupportedOSPlatform("windows")]
	#endif
	public static IDpiBoundValue<Font> Font(Font font)
		=> new FontValueImpl(font, Dpi.System);

	public static IDpiBoundValue<T> Constant<T>(T value)
		=> new ConstantImpl<T>(value);

	public static IDpiBoundValue<int> ScaleX(int value)
		=> new XScaledInt32Impl(value);

	public static IDpiBoundValue<int> ScaleY(int value)
		=> new YScaledInt32Impl(value);

	public static IDpiBoundValue<Padding> Padding(Padding padding)
		=> new ScaledPaddingImpl(padding);

	public static IDpiBoundValue<Size> Size(Size size)
		=> new ScaledSizeImpl(size);

	public static IDpiBoundValue<SizeF> Size(SizeF size)
		=> new ScaledSizeFImpl(size);

	public static IDpiBoundValue<Rectangle> Rectangle(Rectangle rectangle)
		=> new ScaledRectangleImpl(rectangle);
}

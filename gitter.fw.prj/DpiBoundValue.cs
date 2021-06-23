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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	public class DpiBoundValue
	{
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

		public static IDpiBoundValue<Bitmap> Icon(CachedScaledImageResources resources, string name, int size = 16)
		{
			Verify.Argument.IsNotNull(resources, nameof(resources));
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));
			Verify.Argument.IsPositive(size, nameof(size));

			return new IconImpl(resources, name, size);
		}

		public static IDpiBoundValue<Bitmap> Icon(IImageProvider imageProvider, int size = 16)
		{
			Verify.Argument.IsNotNull(imageProvider, nameof(imageProvider));
			Verify.Argument.IsPositive(size, nameof(size));

			return new IconImpl2(imageProvider, size);
		}

		public static IDpiBoundValue<T> Constant<T>(T value)
			=> new ConstantImpl<T>(value);
	}
}

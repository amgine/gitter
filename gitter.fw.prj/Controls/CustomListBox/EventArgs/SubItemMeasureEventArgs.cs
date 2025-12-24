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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;

public class SubItemMeasureEventArgs(Graphics graphics, CustomListBoxItem item, int columnIndex, CustomListBoxColumn column)
	: EventArgs, IDpiConverterProvider
{
	public Graphics Gaphics { get; } = graphics;

	public CustomListBoxItem Item { get; } = item;

	public int SubItemId => Column.Id;

	public CustomListBox ListBox => Column.ListBox!;

	public CustomListBoxColumn Column { get; } = column;

	public Font Font => Column.ContentFont;

	public int ColumnIndex { get; } = columnIndex;

	public Dpi Dpi => ListBox is not null ? new Dpi(ListBox.DeviceDpi) : Dpi.Default;

	public DpiConverter DpiConverter => DpiConverter.FromDefaultTo(Dpi);

	public Size MeasureImage(Image? image)
	{
		int w = 2 * ListBoxConstants.ContentSpacing + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
		int h = 0;
		if(image is not null)
		{
			w += image.Width;
			h += image.Height;
		}
		else
		{
			w += ListBoxConstants.DefaultImageWidth;
			h += ListBoxConstants.DefaultImageHeight;
		}
		return new Size(w, h);
	}

	public Size MeasureIcon(Icon? icon)
	{
		int w = 2 * ListBoxConstants.ContentSpacing + ListBoxConstants.SpaceBeforeImage + ListBoxConstants.SpaceAfterImage;
		int h = 0;
		if(icon != null)
		{
			w += icon.Width;
			h += icon.Height;
		}
		else
		{
			w += ListBoxConstants.DefaultImageWidth;
			h += ListBoxConstants.DefaultImageHeight;
		}
		return new Size(w, h);
	}

	private Size MeasureTextCore(string? text, Font font)
	{
		if(text is not { Length: not 0 }) return Size.Empty;

		var s = GitterApplication.TextRenderer.MeasureText(
			Gaphics, text, font, int.MaxValue);

		return new Size((int)(s.Width + 1 + 2 * DpiConverter.ConvertX(ListBoxConstants.ContentSpacing)), s.Height);
	}

#if NET5_0_OR_GREATER

	private Size MeasureTextCore(ReadOnlySpan<char> text, Font font)
	{
		var s = GitterApplication.TextRenderer.MeasureText(
			Gaphics, text, font, int.MaxValue);

		return new Size((int)(s.Width + 1 + 2 * DpiConverter.ConvertX(ListBoxConstants.ContentSpacing)), s.Height);
	}

#endif

	public Size MeasureText(string? text, Font font)
	{
		Verify.Argument.IsNotNull(font);

		return MeasureTextCore(text, font);
	}

	public Size MeasureText(string? text)
	{
		return MeasureTextCore(text, Column.ContentFont);
	}

#if NET5_0_OR_GREATER

	public Size MeasureText(ReadOnlySpan<char> text, Font font)
	{
		Verify.Argument.IsNotNull(font);

		return MeasureTextCore(text, font);
	}

	public Size MeasureText(ReadOnlySpan<char> text)
	{
		return MeasureTextCore(text, Column.ContentFont);
	}

#endif

	private Size MeasureImageAndTextCore(Image? image, string? text, Font font)
	{
		var conv = DpiConverter;
		var s = GitterApplication.TextRenderer.MeasureText(
			Gaphics, text, font, int.MaxValue);
		var iconW = conv.ConvertX(ListBoxConstants.DefaultImageWidth);
		return new Size(s.Width + 1 +
			2 * conv.ConvertX(ListBoxConstants.ContentSpacing)
			+ (iconW + conv.ConvertX(ListBoxConstants.SpaceBeforeImage) + conv.ConvertX(ListBoxConstants.SpaceAfterImage)), s.Height);
	}

	public Size MeasureImageAndText(Image? image, string text, Font font)
	{
		Verify.Argument.IsNotNull(font);

		return MeasureImageAndTextCore(image, text, font);
	}

	public Size MeasureImageAndText(IImageProvider? image, string? text, Font font)
	{
		Verify.Argument.IsNotNull(font);

		return MeasureImageAndTextCore(null, text, font);
	}

	public Size MeasureImageAndText(Image? image, string? text)
	{
		return MeasureImageAndTextCore(image, text, Column.ContentFont);
	}

	public Size MeasureImageAndText(IImageProvider? image, string? text)
	{
		return MeasureImageAndTextCore(null, text, Column.ContentFont);
	}

	private Size MeasureIconAndTextCore(Icon? icon, string? text, Font font)
	{
		var conv = DpiConverter;
		var s = GitterApplication.TextRenderer.MeasureText(
			Gaphics, text, font, int.MaxValue);
		var iconW = icon is not null ? icon.Width : conv.ConvertX(ListBoxConstants.DefaultImageWidth);
		return new Size(s.Width + 1 +
			2 * conv.ConvertX(ListBoxConstants.ContentSpacing)
			+ (iconW + conv.ConvertX(ListBoxConstants.SpaceBeforeImage) + conv.ConvertX(ListBoxConstants.SpaceAfterImage)), s.Height);
	}

	public Size MeasureIconAndText(Icon? icon, string? text, Font font)
	{
		Verify.Argument.IsNotNull(font);

		return MeasureIconAndTextCore(icon, text, font);
	}

	public Size MeasureIconAndText(Icon? icon, string? text)
	{
		return MeasureIconAndTextCore(icon, text, Column.ContentFont);
	}
}

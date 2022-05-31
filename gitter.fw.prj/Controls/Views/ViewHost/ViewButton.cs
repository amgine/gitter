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

public sealed class ViewButton
{
	private static Bitmap LoadBitmap(string name)
	{
		using var stream = typeof(ViewButton).Assembly.GetManifestResourceStream(@"gitter.Framework.Resources.images." + name + ".png");
		if(stream is null) return default;
		return new Bitmap(stream);
	}

	private static readonly Lazy<Bitmap> ImgMenu        = new(() => LoadBitmap(@"arrow-small"));
	private static readonly Lazy<Bitmap> ImgNormalize   = new(() => LoadBitmap(@"normalize"));
	private static readonly Lazy<Bitmap> ImgMaximize    = new(() => LoadBitmap(@"maximize"));
	private static readonly Lazy<Bitmap> ImgPin         = new(() => LoadBitmap(@"pin-small"));
	private static readonly Lazy<Bitmap> ImgClose       = new(() => LoadBitmap(@"cross-small"));
	private static readonly Lazy<Bitmap> ImgScrollLeft  = new(() => LoadBitmap(@"tab-scroll-left"));
	private static readonly Lazy<Bitmap> ImgScrollRight = new(() => LoadBitmap(@"tab-scroll-right"));
	private static readonly Lazy<Bitmap> ImgTabMenu     = new(() => LoadBitmap(@"tab-menu"));
	private static readonly Lazy<Bitmap> ImgTabMenuExt  = new(() => LoadBitmap(@"tab-menu-extends"));

	internal ViewButton(int offset, ViewButtonType type)
	{
		Offset = offset;
		Type = type;
		Image = Type switch
		{
			ViewButtonType.Menu            => ImgMenu.Value,
			ViewButtonType.Pin             => ImgPin.Value,
			ViewButtonType.Unpin           => ImgPin.Value,
			ViewButtonType.Normalize       => ImgNormalize.Value,
			ViewButtonType.Maximize        => ImgMaximize.Value,
			ViewButtonType.Close           => ImgClose.Value,
			ViewButtonType.ScrollTabsLeft  => ImgScrollLeft.Value,
			ViewButtonType.ScrollTabsRight => ImgScrollRight.Value,
			ViewButtonType.TabsMenu        => ImgTabMenu.Value,
			ViewButtonType.TabsScrollMenu  => ImgTabMenuExt.Value,
			_ => null,
		};
	}

	public int Offset { get; }

	public ViewButtonType Type { get; }

	public Image Image { get; }

	internal void OnPaint(Graphics graphics, Dpi dpi, Rectangle bounds, bool focus, bool hover, bool pressed)
	{
		ViewManager.Renderer.RenderViewButton(this, graphics, dpi, bounds, focus, hover, pressed);
	}

	/// <inheritdoc/>
	public override string ToString() => Type.ToString();
}

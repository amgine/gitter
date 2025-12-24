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

namespace gitter;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;

[DesignerCategory("")]
sealed class LogoControl : Control
{
	private static Bitmap? LoadImage(string name)
	{
		using var stream = typeof(StartPageView)
			.Assembly
			.GetManifestResourceStream("gitter.Resources.images." + name + ".png");
		if(stream is null) return default;
		return new Bitmap(stream);
	}

	private static readonly Lazy<Bitmap?> ImgStartPageLogoDark         = new(() => LoadImage(@"start-page-logo-dark"));
	private static readonly Lazy<Bitmap?> ImgStartPageLogo             = new(() => LoadImage(@"start-page-logo"));
	private static readonly Lazy<Bitmap?> ImgStartPageLogoGradientDark = new(() => LoadImage(@"start-page-logo-gradient-dark"));
	private static readonly Lazy<Bitmap?> ImgStartPageLogoGradient     = new(() => LoadImage(@"start-page-logo-gradient"));

	public LogoControl()
	{
		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.ContainerControl, false);
	}

	public IGitterStyle Style { get; set; } = GitterApplication.Style;

	/// <inheritdoc/>
	protected override void OnPaint(PaintEventArgs e)
	{
		e.Graphics.GdiFill(BackColor, e.ClipRectangle);

		var logo = Style.Type == GitterStyleType.DarkBackground
			? ImgStartPageLogoDark.Value
			: ImgStartPageLogo.Value;

		if(logo is null) return;

		var h = Height;
		var w = logo.Width * h / logo.Height;

		var g = new Rectangle(w - 1, 0, Width - w + 1, h);
		if(g.Width > 0 && g.IntersectsWith(e.ClipRectangle))
		{
			var grad = Style.Type == GitterStyleType.DarkBackground
				? ImgStartPageLogoGradientDark.Value
				: ImgStartPageLogoGradient.Value;

			if(grad is not null)
			{
				e.Graphics.DrawImage(grad, g);
			}
		}
		e.Graphics.DrawImage(logo, new Rectangle(0, 0, w, h));
	}

	/// <inheritdoc/>
	protected override void OnPaintBackground(PaintEventArgs pevent) { }
}

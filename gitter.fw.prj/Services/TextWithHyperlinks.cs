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

namespace gitter.Framework;

using System;
using System.Drawing;

using gitter.Framework.Services;

public sealed class TextWithHyperlinks
{
	public event EventHandler InvalidateRequired
	{
		add    => _text.InvalidateRequired += value;
		remove => _text.InvalidateRequired -= value;
	}

	private readonly FormattedWrappedText _text;

	public TextWithHyperlinks(string text, IHyperlinkExtractor? extractor = null)
	{
		Text = text;
		_text = new(text);
		extractor ??= new AbsoluteUrlHyperlinkExtractor();
		if(extractor.ExtractHyperlinks(text) is { Count: not 0 } links)
		{
			foreach(var hyperlink in links)
			{
				_text.AddHyperlink(hyperlink);
			}
		}
	}

	public Hyperlink? HoveredHyperlink => _text.HoveredHyperlink;

	public string Text { get; }

	public void Render(IGitterStyle style, Graphics graphics, Font font, Rectangle rect)
		=> _text.Render(style, graphics, font, rect, rect);

	public Size Measure(Graphics graphics, Font font, Rectangle rect)
		=> _text.Measure(graphics, font, rect);

	public Size Measure(Font font, Rectangle rect)
		=> _text.Measure(font, rect);

	public Hyperlink? HitTest(Rectangle rect, Point p)
	{
		p.X -= rect.X;
		p.Y -= rect.Y;
		return _text.HitTest(p);
	}

	public void OnMouseMove(Rectangle rect, Point p)
	{
		p.X -= rect.X;
		p.Y -= rect.Y;
		_text.OnMouseMove(p);
	}

	public void OnMouseDown(Rectangle rect, Point p)
		=> HitTest(rect, p)?.Navigate();

	public void OnMouseLeave() => _text.OnMouseLeave();

	public override string ToString() => Text;
}

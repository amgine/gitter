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
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

using gitter.Framework.Services;

public sealed class TextWithHyperlinks
{
	#region Data

	private readonly StringFormat _sf;
	private readonly HyperlinkGlyph[] _glyphs;
	private RectangleF _cachedRect;

	#endregion

	#region Events

	public event EventHandler InvalidateRequired;

	#endregion

	private sealed class HyperlinkGlyph
	{
		private Region _region;

		public HyperlinkGlyph(Hyperlink href)
		{
			Verify.Argument.IsNotNull(href);

			Hyperlink = href;
		}

		public Hyperlink Hyperlink { get; }

		public int Start => Hyperlink.Text.Start;

		public int End => Hyperlink.Text.End;

		public int Length => Hyperlink.Text.Length;

		public Region Region
		{
			get => _region;
			set
			{
				if(_region != value)
				{
					_region?.Dispose();
					_region = value;
				}
			}
		}

		public bool IsHovered { get; set; }
	}

	private readonly TrackingService<HyperlinkGlyph> _hoveredLink;
	private bool _failedToSegment;

	public TextWithHyperlinks(string text, IHyperlinkExtractor extractor = null)
	{
		Text = text;
		_sf = (StringFormat)(StringFormat.GenericTypographic.Clone());
		_sf.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces;
		extractor ??= new AbsoluteUrlHyperlinkExtractor();
		_glyphs = extractor.ExtractHyperlinks(text)
							.Select(static h => new HyperlinkGlyph(h))
							.ToArray();
		try
		{
			_sf.SetMeasurableCharacterRanges(
				Array.ConvertAll(_glyphs, static l => new CharacterRange(l.Start, l.Length)));
		}
		catch
		{
			_failedToSegment = true;
		}

		_hoveredLink = new TrackingService<HyperlinkGlyph>();
		_hoveredLink.Changed += OnHoveredLinkChanged;
	}

	public Hyperlink HoveredHyperlink
		=> _hoveredLink.IsTracked
			? _hoveredLink.Item.Hyperlink
			: null;

	private void OnHoveredLinkChanged(object sender, TrackingEventArgs<HyperlinkGlyph> e)
	{
		Assert.IsNotNull(e);

		e.Item.IsHovered = e.IsTracked;
		InvalidateRequired?.Invoke(this, EventArgs.Empty);
	}

	public string Text { get; }

	public void Render(IGitterStyle style, Graphics graphics, Font font, Rectangle rect)
	{
		Assert.IsNotNull(style);
		Assert.IsNotNull(graphics);
		Assert.IsNotNull(font);

		if(_failedToSegment)
		{
			GitterApplication.TextRenderer.DrawText(
				graphics, Text, font, style.Colors.WindowText, rect, _sf);
			return;
		}

		bool useCache = _cachedRect == rect;
		if(useCache)
		{
			for(int i = 0; i < _glyphs.Length; ++i)
			{
				graphics.ExcludeClip(_glyphs[i].Region);
			}
		}
		else
		{
			var cr = graphics.MeasureCharacterRanges(Text, font, rect, _sf);
			for(int i = 0; i < _glyphs.Length; ++i)
			{
				_glyphs[i].Region = cr[i];
				graphics.ExcludeClip(cr[i]);
			}
		}
		GitterApplication.TextRenderer.DrawText(
			graphics, Text, font, style.Colors.WindowText, rect, _sf);
		graphics.ResetClip();
		bool clipIsSet = false;
		foreach(var glyph in _glyphs)
		{
			if(glyph != _hoveredLink.Item)
			{
				if(clipIsSet)
				{
					graphics.SetClip(glyph.Region, CombineMode.Union);
				}
				else
				{
					graphics.Clip = glyph.Region;
					clipIsSet = true;
				}
			}
		}
		if(clipIsSet)
		{
			GitterApplication.TextRenderer.DrawText(
				graphics, Text, font, style.Colors.HyperlinkText, rect, _sf);
		}
		if(_hoveredLink.IsTracked)
		{
			graphics.Clip = _hoveredLink.Item.Region;
			using var f = new Font(font, FontStyle.Underline);
			GitterApplication.TextRenderer.DrawText(
				graphics, Text, f, style.Colors.HyperlinkTextHotTrack, rect, _sf);
		}
		graphics.ResetClip();
		_cachedRect = rect;
	}

	public Hyperlink HitTest(RectangleF rect, Point p)
	{
		var index = HitTestCore(rect, p);
		return index >= 0
			? _glyphs[index].Hyperlink
			: default;
	}

	private int HitTestCore(RectangleF rect, Point p)
	{
		p.X += (int)(_cachedRect.X - rect.X);
		p.Y += (int)(_cachedRect.Y - rect.Y);
		for(int i = 0; i < _glyphs.Length; ++i)
		{
			var glyph = _glyphs[i];
			if(glyph.Region is not null && glyph.Region.IsVisible(p))
			{
				return i;
			}
		}
		return -1;
	}

	public void OnMouseMove(RectangleF rect, Point p)
	{
		var index = HitTestCore(rect, p);
		if(index != -1)
		{
			_hoveredLink.Track(index, _glyphs[index]);
		}
		else
		{
			_hoveredLink.Drop();
		}
	}

	public void OnMouseDown(RectangleF rect, Point p)
	{
		var index = HitTestCore(rect, p);
		if(index != -1) _glyphs[index].Hyperlink.Navigate();
	}

	public void OnMouseLeave() => _hoveredLink.Drop();

	public override string ToString() => Text;
}

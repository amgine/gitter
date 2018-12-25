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
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Drawing;
	using System.Drawing.Drawing2D;

	using gitter.Framework.Services;

	public sealed class TextWithHyperlinks
	{
		#region Data

		private readonly string _text;
		private readonly StringFormat _sf;
		private readonly HyperlinkGlyph[] _glyphs;
		private RectangleF _cachedRect;

		#endregion

		#region Events

		public event EventHandler InvalidateRequired;

		#endregion

		private sealed class HyperlinkGlyph
		{
			private readonly Hyperlink _href;
			private Region _region;

			public HyperlinkGlyph(Hyperlink href)
			{
				Verify.Argument.IsNotNull(href, nameof(href));

				_href = href;
			}

			public Hyperlink Hyperlink
			{
				get { return _href; }
			}

			public int Start
			{
				get { return _href.Text.Start; }
			}

			public int End
			{
				get { return _href.Text.End; }
			}

			public int Length
			{
				get { return _href.Text.Length; }
			}

			public Region Region
			{
				get { return _region; }
				set
				{
					if(_region != null) _region.Dispose();
					_region = value;
				}
			}

			public bool IsHovered { get; set; }
		}

		private TrackingService<HyperlinkGlyph> _hoveredLink;

		public TextWithHyperlinks(string text, HyperlinkExtractor extractor = null)
		{
			_text = text;
			_sf = (StringFormat)(StringFormat.GenericTypographic.Clone());
			_sf.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces;
			if(extractor == null) extractor = new HyperlinkExtractor();
			_glyphs = extractor.ExtractHyperlinks(text)
							   .Select(h => new HyperlinkGlyph(h))
							   .ToArray();
			_sf.SetMeasurableCharacterRanges(
				_glyphs.Select(l => new CharacterRange(l.Start, l.Length)).ToArray());

			_hoveredLink = new TrackingService<HyperlinkGlyph>();
			_hoveredLink.Changed += OnHoveredLinkChanged;
		}

		public Hyperlink HoveredHyperlink
		{
			get
			{
				if(_hoveredLink.IsTracked)
				{
					return _hoveredLink.Item.Hyperlink;
				}
				else
				{
					return null;
				}
			}
		}

		private void OnHoveredLinkChanged(object sender, TrackingEventArgs<TextWithHyperlinks.HyperlinkGlyph> e)
		{
			e.Item.IsHovered = e.IsTracked;
			InvalidateRequired.Raise(this);
		}

		public string Text
		{
			get { return _text; }
		}

		public void Render(IGitterStyle style, Graphics graphics, Font font, Rectangle rect)
		{
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
				var cr = graphics.MeasureCharacterRanges(_text, font, rect, _sf);
				for(int i = 0; i < _glyphs.Length; ++i)
				{
					_glyphs[i].Region = cr[i];
					graphics.ExcludeClip(cr[i]);
				}
			}
			using(var brush = new SolidBrush(style.Colors.WindowText))
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, _text, font, brush, rect, _sf);
			}
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
				using(var linkTextBrush = new SolidBrush(style.Colors.HyperlinkText))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, _text, font, linkTextBrush, rect, _sf);
				}
			}
			if(_hoveredLink.IsTracked)
			{
				graphics.Clip = _hoveredLink.Item.Region;
				using(var f = new Font(font, FontStyle.Underline))
				using(var linkTextBrush = new SolidBrush(style.Colors.HyperlinkTextHotTrack))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, _text, f, linkTextBrush, rect, _sf);
				}
			}
			graphics.ResetClip();
			_cachedRect = rect;
		}

		private int HitTest(RectangleF rect, Point p)
		{
			p.X += (int)(_cachedRect.X - rect.X);
			p.Y += (int)(_cachedRect.Y - rect.Y);
			for(int i = 0; i < _glyphs.Length; ++i)
			{
				var glyph = _glyphs[i];
				if(glyph.Region != null && glyph.Region.IsVisible(p))
				{
					return i;
				}
			}
			return -1;
		}

		public void OnMouseMove(RectangleF rect, Point p)
		{
			var index = HitTest(rect, p);
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
			var index = HitTest(rect, p);
			if(index != -1) _glyphs[index].Hyperlink.Navigate();
		}

		public void OnMouseLeave()
		{
			_hoveredLink.Drop();
		}

		public override string ToString()
		{
			return _text;
		}
	}
}

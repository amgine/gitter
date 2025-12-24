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

namespace gitter.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using gitter.Framework.Services;
using gitter.Native;

public sealed class FormattedWrappedText(string text)
{
	public event EventHandler? InvalidateRequired;

	private void OnInvalidateRequired(EventArgs e)
		=> InvalidateRequired?.Invoke(this, e);

	[Flags]
	enum FormattedTextFlags
	{
		None = 0,
		Tokenized  = 1 << 0,
		Measured   = 1 << 1,
		LayoutDone = 1 << 2,
	}

	private readonly List<Hyperlink> _links = [];
	private readonly List<Token> _tokens = [];
	private readonly List<MeasuredToken> _measured = [];
	private readonly List<Run> _runs = [];
	private FormattedTextFlags _flags;
	private Hyperlink? _hovered;

	private Font? _font;
	private int _width;

	private Size _measuredSize;

	public Hyperlink? HoveredHyperlink => _hovered;

	public void AddHyperlink(Hyperlink link)
	{
		_links.Add(link);
		_flags = FormattedTextFlags.None;
	}

	public void ClearHyperlinks()
	{
		if(_links.Count == 0) return;
		_links.Clear();
		_hovered = default;
		_flags = FormattedTextFlags.None;
	}

	public Hyperlink? HitTest(Point p)
	{
		if((_flags & FormattedTextFlags.LayoutDone) != FormattedTextFlags.LayoutDone)
		{
			return default;
		}
		foreach(var run in _runs)
		{
			if(run.Bounds.Contains(p)) return run.Link;
		}
		return default;
	}

	public bool OnMouseMove(Point p)
	{
		var link = HitTest(p);
		if(_hovered != link)
		{
			_hovered = link;
			OnInvalidateRequired(EventArgs.Empty);
		}
		return link is not null;
	}

	public void OnMouseLeave()
	{
		if(_hovered is null) return;
		_hovered = default;
		OnInvalidateRequired(EventArgs.Empty);
	}

	private unsafe void UpdateLayout(IntPtr hdc, char* p, Font font, Rectangle bounds)
	{
		if(_font != font)
		{
			_flags &= ~(FormattedTextFlags.LayoutDone | FormattedTextFlags.Measured);
		}
		if(_width != bounds.Width)
		{
			_flags &= ~(FormattedTextFlags.LayoutDone);
		}

		if((_flags & FormattedTextFlags.Tokenized) != FormattedTextFlags.Tokenized)
		{
			Tokenize();
			_flags |= FormattedTextFlags.Tokenized;
		}
		if((_flags & FormattedTextFlags.Measured) != FormattedTextFlags.Measured)
		{
			Measure(hdc, p);
			_flags |= FormattedTextFlags.Measured;
		}
		if((_flags & FormattedTextFlags.LayoutDone) != FormattedTextFlags.LayoutDone)
		{
			Layout(bounds.Size);
			_flags |= FormattedTextFlags.LayoutDone;
		}

		_font  = font;
		_width = bounds.Width;
	}

	static uint ToNative(Color color)
	{
		var value = color.ToArgb() & 0x00ffffff;
		return (uint)((value & 0x00ff00) | (value >> 16) | ((value << 16) & 0xff0000));
	}

	public Size Measure(Graphics graphics, Font font, Rectangle bounds)
	{
		var hdc = graphics.GetHdc();
		try
		{
			return Measure(hdc, font, bounds);
		}
		finally
		{
			graphics.ReleaseHdc(hdc);
		}
	}

	public unsafe Size Measure(IntPtr hdc, Font font, Rectangle bounds)
	{
		var hFont = GdiFontCache.Shared.GetFont(font);
		var oldFont = Gdi32.SelectObject(hdc, hFont);
		try
		{
			fixed(char* p = text)
			{
				UpdateLayout(hdc, p, font, bounds);
			}
		}
		finally
		{
			if(oldFont != IntPtr.Zero)
			{
				_ = Gdi32.SelectObject(hdc, oldFont);
			}
		}
		return _measuredSize;
	}

	public unsafe Size Measure(Font font, Rectangle bounds)
	{
		var hdc = User32.GetDC(IntPtr.Zero);
		try
		{
			return Measure(hdc, font, bounds);
		}
		finally
		{
			_ = User32.ReleaseDC(IntPtr.Zero, hdc);
		}
	}

	public void Render(IGitterStyle style, Graphics graphics, Font font, Rectangle bounds, Rectangle clip)
	{
		var hdc = graphics.GetHdc();
		try
		{
			Render(style, hdc, font, bounds, clip);
		}
		finally
		{
			graphics.ReleaseHdc(hdc);
		}
	}

	public unsafe void Render(IGitterStyle style, IntPtr hdc, Font font, Rectangle bounds, Rectangle clip)
	{
		const int TRANSPARENT = 1;

		var hf = GdiFontCache.Shared.GetFont(font);
		var uf = default(IntPtr);

		clip.X -= bounds.X;
		clip.Y -= bounds.Y;

		var state = Gdi32.SaveDC(hdc);
		if(state == 0) throw new Win32Exception($"{nameof(Gdi32.SaveDC)} failed.");
		try
		{
			var hFont = hf;
			uint color = ToNative(style.Colors.WindowText);
			_ = Gdi32.SelectObject(hdc, hFont);
			_ = Gdi32.SetTextColor(hdc, color);
			_ = Gdi32.SetBkMode   (hdc, TRANSPARENT);
			_ = Gdi32.IntersectClipRect(hdc, bounds.X, bounds.Y, bounds.Right, bounds.Bottom);

			fixed(char* p = text)
			{
				UpdateLayout(hdc, p, font, bounds);
				foreach(var run in _runs)
				{
					if(!run.Bounds.IntersectsWith(clip)) continue;
					uint c;
					IntPtr f;
					if(run.Link is null)
					{
						f = hf;
						c = ToNative(style.Colors.WindowText);
					}
					else if(run.Link == _hovered)
					{
						if(uf == IntPtr.Zero)
						{
							uf = GdiFontCache.Shared.GetFont(font, FontStyle.Underline);
						}
						c = ToNative(style.Colors.HyperlinkTextHotTrack);
						f = uf;
					}
					else
					{
						f = hf;
						c = ToNative(style.Colors.HyperlinkText);
					}
					if(color != c)
					{
						_ = Gdi32.SetTextColor(hdc, c);
						color = c;
					}
					if(hFont != f)
					{
						_ = Gdi32.SelectObject(hdc, f);
						hFont = f;
					}
					run.Render(hdc, bounds.X, bounds.Y, p);
				}
			}
		}
		finally
		{
			_ = Gdi32.RestoreDC(hdc, state);
		}
	}

	enum TokenType
	{
		Invalid,
		Word,
		Tab,
		Whitespace,
		NewLine,
	}

	readonly struct Token(int start, int length, TokenType type, Hyperlink? link = default)
	{
		public readonly int        Start  = start;
		public readonly int        Length = length;
		public readonly TokenType  Type   = type;
		public readonly Hyperlink? Link = link;
	}

	readonly struct MeasuredToken(Token token, SIZE size)
	{
		public readonly Token Token = token;
		public readonly SIZE  Size  = size;
	}

	readonly struct Run(int start, int length, Rectangle bounds, Hyperlink? link)
	{
		public readonly int        Start  = start;
		public readonly int        Length = length;
		public readonly Rectangle  Bounds = bounds;
		public readonly Hyperlink? Link   = link;

		public unsafe void Render(IntPtr hdc, int x, int y, char* p)
			=> Gdi32.TextOut(hdc, x + Bounds.X, y + Bounds.Y, p + Start, Length);
	}

	private void Tokenize()
	{
		const char NBSP = '\u00A0';

		_tokens.Clear();
		var context = new TokenizerContext(_tokens, _links);
		for(int i = 0; i < text.Length; ++i)
		{
			var c = text[i];
			if(!char.IsWhiteSpace(c)) context.Next(TokenType.Word, i);
			else if(c == '\t')        context.Next(TokenType.Tab,  i);
			else if(c == '\r')        context.Cr(i);
			else if(c == '\n')        context.Lf(i);
			else if(c == NBSP)        context.Next(TokenType.Word, i);
			else                      context.Next(TokenType.Whitespace, i);
		}
		context.Flush();
	}

	unsafe void Measure(IntPtr hdc, char* text)
	{
		_measured.Clear();
		foreach(var token in _tokens)
		{
			_ = Gdi32.GetTextExtentPoint32(hdc, text + token.Start, token.Length, out var size);
			_measured.Add(new(token, size));
		}
	}

	private void Layout(Size targetSize)
	{
		_runs.Clear();
		var context = new LayoutContext(_runs, targetSize);
		for(int i = 0; i < _measured.Count; ++i)
		{
			var token = _measured[i];
			var width = token.Size.cx;
			if(token.Token.Type == TokenType.Word)
			{
				for(int j = i + 1; j < _measured.Count; ++j)
				{
					var t2 = _measured[j];
					if(t2.Token.Type != TokenType.Word) break;
					width += t2.Size.cx;
				}
			}
			context.AddToken(token, width);
		}
		context.Flush();
		_measuredSize = context.GetMeasuredSize();
	}

	ref struct TokenizerContext(List<Token> tokens, List<Hyperlink> links)
	{
		private Hyperlink? _link;
		private TokenType _type;
		private int _start;
		private int _length;
		private bool _cr;

		private static bool LinkContains(Hyperlink link, int index)
			=> link.Text.Start <= index && link.Text.End >= index;

		private readonly Hyperlink? FindLink(int index)
		{
			if(_link is not null && LinkContains(_link, index))
			{
				return _link;
			}
			foreach(var link in links)
			{
				if(LinkContains(link, index)) return link;
			}
			return default;
		}

		private void PushCurrentToken()
		{
			if(_type == TokenType.Invalid) return;

			tokens.Add(new(_start, _length, _type, _link));
			_type = TokenType.Invalid;
		}

		public void Next(TokenType type, int index)
		{
			var link = FindLink(index);
			if(_type != type || _link != link)
			{
				PushCurrentToken();
				_type   = type;
				_start  = index;
				_link   = link;
				_length = 1;
			}
			else
			{
				++_length;
			}
			_cr = false;
		}

		private void EmitNewLine(int index)
		{
			tokens.Add(new(index, 1, TokenType.NewLine));
			_link   = FindLink(index);
			_start  = index;
			_length = 0;
		}

		public void Cr(int index)
		{
			PushCurrentToken();
			EmitNewLine(index);
			_cr = true;
		}

		public void Lf(int index)
		{
			PushCurrentToken();
			if(_cr)
			{
				_cr = false;
				return; // CRLF
			}
			EmitNewLine(index);
		}

		public void Flush() => PushCurrentToken();
	}

	ref struct LayoutContext(List<Run> runs, Size targetSize)
	{
		public readonly List<Run> Runs = runs;
		public readonly Size TargetSize = targetSize;

		private int _x;
		private int _y;
		private int _w;
		private int _h;

		private int _start = -1;
		private int _length;
		private int _trailingWhitespace;
		private int _trailingWhitespaceWidth;
		private bool _lineHasContent;
		private bool _canLinebreak;
		private bool _isAtLineStart = true;
		private int _maxWidth;
		private int _totalH;
		private bool _softbreak;
		private Hyperlink? _link;

		public readonly Size GetMeasuredSize() => new(_maxWidth, _totalH);

		public void AddToken(MeasuredToken token, int width)
		{
			switch(token.Token.Type)
			{
				case TokenType.Word:
					{
						CheckLink(token.Token.Link);
						if(_x + _w + width > TargetSize.Width && _canLinebreak)
						{
							NewLine(token.Size.cy);
							_length = token.Token.Length;
							_w = token.Size.cx;
						}
						else
						{
							_length += token.Token.Length;
							_w += token.Size.cx;
						}
						_h = Math.Max(_h, token.Size.cy);
						if(!_lineHasContent)
						{
							_lineHasContent = true;
							if(_start < 0)
							{
								_start = token.Token.Start;
							}
						}
						_trailingWhitespace = 0;
						_trailingWhitespaceWidth = 0;
						_canLinebreak = false;
						_isAtLineStart = false;
						_softbreak = false;
					}
					break;
				case TokenType.Whitespace:
					{
						CheckLink(token.Token.Link);
						var next = _w + width;
						if(_x + next > TargetSize.Width)
						{
							NewLine(token.Size.cy);
							_w = 0;
							_canLinebreak = false;
							_softbreak = true;
						}
						else
						{
							if(_start < 0)
							{
								_start = token.Token.Start;
								_isAtLineStart = false;
							}
							_length += token.Token.Length;
							_w = next;
							_trailingWhitespace = token.Token.Length;
							_trailingWhitespaceWidth = token.Size.cx;
							_canLinebreak = true;
						}
					}
					break;
				case TokenType.Tab:
					{
						CheckLink(token.Token.Link);
						var ts = width / token.Token.Length;
						var next = ts * token.Token.Length + ts * _w / ts;
						if(_x + next > TargetSize.Width)
						{
							NewLine(token.Size.cy);
							_w = 0;
							_canLinebreak = false;
							_softbreak = true;
						}
						else
						{
							if(_isAtLineStart)
							{
								_isAtLineStart = false;
							}
							else
							{
								Flush();
							}
							_x = next;
							_w = 0;
							_lineHasContent = false;
							_canLinebreak = true;
						}
					}
					break;
				case TokenType.NewLine:
					if(_softbreak)
					{
						_softbreak = false;
					}
					else
					{
						NewLine(token.Size.cy);
					}
					_link = default;
					_canLinebreak = false;
					_w = 0;
					break;
			}
		}

		private void CheckLink(Hyperlink? link)
		{
			if(link == _link) return;
			if(_lineHasContent)
			{
				var next = _x + _w;
				Flush();
				_x = next;
			}
			_link = link;
		}

		private void NewLine(int offset)
		{
			Flush();
			_y += offset;
			_x = 0;
			_w = 0;
			_isAtLineStart = true;
		}

		public void Flush()
		{
			if(_lineHasContent && _length > 0)
			{
				Runs.Add(new(_start, _length - _trailingWhitespace, new(_x, _y, _w - _trailingWhitespaceWidth, _h), _link));
				_totalH = _y + _h;
				_lineHasContent = false;
				_maxWidth = Math.Max(_maxWidth, _x + _w - _trailingWhitespaceWidth);
			}
			_trailingWhitespace = 0;
			_trailingWhitespaceWidth = 0;
			_start  = -1;
			_length = 0;
			_w = 0;
			_h = 0;
		}
	}
}

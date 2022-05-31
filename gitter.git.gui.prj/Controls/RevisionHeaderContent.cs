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

namespace gitter.Git.Gui;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Services;

using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed partial class RevisionHeaderContent
{
	#region Constants

	private static readonly int DefaultElementHeight = 16;
	private static readonly int HeaderWidth          = 70;
	private static readonly int MinWidth             = HeaderWidth + 295;

	#endregion

	#region Static

	private static readonly StringFormat HeaderFormat = new(StringFormat.GenericDefault)
	{
		Alignment = StringAlignment.Far,
		FormatFlags =
			StringFormatFlags.LineLimit |
			StringFormatFlags.NoClip |
			StringFormatFlags.NoWrap,
		LineAlignment = StringAlignment.Near,
		Trimming = StringTrimming.None,
	};

	private static readonly StringFormat ContentFormat = new(StringFormat.GenericTypographic)
	{
		Alignment = StringAlignment.Near,
		FormatFlags =
			StringFormatFlags.FitBlackBox,
		LineAlignment = StringAlignment.Near,
		Trimming = StringTrimming.EllipsisCharacter,
	};

	#endregion

	#region Elements

	enum Element
	{
		Hash,
		TreeHash,
		CommitDate,
		AuthorDate,
		Author,
		Committer,
		Subject,
		Body,
		References,
		Parents,
	}

	sealed class CursorChangedEventArgs : EventArgs
	{
		public CursorChangedEventArgs(Cursor cursor) => Cursor = cursor;

		public Cursor Cursor { get; }
	}

	/// <summary>Interface for a single data field.</summary>
	interface IRevisionHeaderElement
	{
		event EventHandler InvalidateRequired;

		event EventHandler<CursorChangedEventArgs> CursorChangeRequired;

		/// <summary>Displayed data.</summary>
		Element Element { get; }

		bool IsAvailableFor(Revision revision);

		ContextMenuStrip CreateContextMenu(Revision revision, Rectangle rect, int x, int y);

		Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width);

		void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect);

		void MouseMove(Rectangle rect, Point point);

		void MouseLeave();

		void MouseDown(Rectangle rect, MouseButtons button, int x, int y);
	}

	abstract class BaseElement : IRevisionHeaderElement
	{
		public event EventHandler InvalidateRequired;

		public event EventHandler<CursorChangedEventArgs> CursorChangeRequired;

		protected void OnInvalidateRequired()
			=> InvalidateRequired?.Invoke(this, EventArgs.Empty);

		protected void ChangeCursor(Cursor cursor)
			=> CursorChangeRequired?.Invoke(this, new CursorChangedEventArgs(cursor));

		private Dpi _lastDpi = Dpi.Default;

		protected BaseElement(RevisionHeaderContent owner)
		{
			Owner = owner;
		}

		public RevisionHeaderContent Owner { get; }

		public abstract Element Element { get; }

		protected IHyperlinkExtractor GetHyperlinkExtractor(Revision revision)
			=> Owner.GetHyperlinkExtractor(revision);

		public virtual bool IsAvailableFor(Revision revision) => true;

		public virtual ContextMenuStrip CreateContextMenu(Revision revision)
			=> default;

		public virtual ContextMenuStrip CreateContextMenu(Revision revision, Rectangle rect, int x, int y)
			=> CreateContextMenu(revision);

		public virtual Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
		{
			var conv = DpiConverter.FromDefaultTo(dpi);
			return new Size(width, conv.ConvertY(DefaultElementHeight));
		}

		protected static Size Measure(Graphics graphics, Dpi dpi, Font font, string text, int width)
		{
			var conv = DpiConverter.FromDefaultTo(dpi);
			var w = conv.ConvertX(HeaderWidth) + GitterApplication.TextRenderer.MeasureText(graphics, text, font, width, ContentFormat).Width;
			return new Size(w, conv.ConvertY(DefaultElementHeight));
		}

		protected static Size MeasureMultilineContent(Graphics graphics, Dpi dpi, string content, int width)
		{
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			return MeasureMultilineContent(graphics, dpi, font, content, width);
		}

		protected static Size MeasureMultilineContent(Graphics graphics, Dpi dpi, Font font, string content, int width)
		{
			var conv = DpiConverter.FromDefaultTo(dpi);
			var s    = GitterApplication.TextRenderer.MeasureText(graphics, content, font, width - conv.ConvertX(HeaderWidth), ContentFormat);
			var min  = conv.ConvertY(DefaultElementHeight);
			if(s.Height < min) s.Height = min;
			return new Size(conv.ConvertX(HeaderWidth) + s.Width, s.Height);
		}

		protected static int GetYOffset(Dpi dpi, Font font)
		{
			var conv   = DpiConverter.FromDefaultTo(dpi);
			int offset = (int)(conv.ConvertY(DefaultElementHeight) - GitterApplication.TextRenderer.GetFontHeight(font));
			if(GitterApplication.TextRenderer == GitterApplication.GdiTextRenderer)
			{
				--offset;
			}
			else
			{
				if(font.Name == "Consolas" || font.SizeInPoints < 8.5f) ++offset;
			}
			return offset;
		}

		protected void PaintHeader(Graphics graphics, Dpi dpi, string header, Rectangle rect)
		{
			var conv = DpiConverter.FromDefaultTo(dpi);
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			var r1   = new Rectangle(rect.X, rect.Y, conv.ConvertX(HeaderWidth) - conv.ConvertX(4), conv.ConvertY(DefaultElementHeight));
			r1.Y += GetYOffset(conv.To, font);
			using(var brush = new SolidBrush(Owner.Style.Colors.GrayText))
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, header, font, brush, r1, HeaderFormat);
			}
		}

		protected void DefaultPaint(Graphics graphics, Dpi dpi, string header, string content, Rectangle rect)
		{
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			DefaultPaint(graphics, dpi, font, header, content, rect);
		}

		protected void DefaultPaint(Graphics graphics, Dpi dpi, string header, TextWithHyperlinks content, Rectangle rect)
		{
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			DefaultPaint(graphics, dpi, font, header, content, rect);
		}

		protected Rectangle GetContentRectangle(Rectangle rect)
		{
			var headerWidth = (int)(HeaderWidth * _lastDpi.X / 96 + 0.5f);
			var r2 = new Rectangle(rect.X + headerWidth, rect.Y, rect.Width - headerWidth, rect.Height);
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(_lastDpi);
			r2.Y += GetYOffset(_lastDpi, font);
			return r2;
		}

		protected void DefaultPaint(Graphics graphics, Dpi dpi, Font font, string header, string content, Rectangle rect)
		{
			var conv = DpiConverter.FromDefaultTo(dpi);
			var r1 = new Rectangle(rect.X, rect.Y, conv.ConvertX(HeaderWidth) - conv.ConvertX(4), conv.ConvertY(DefaultElementHeight));
			var r2 = new Rectangle(rect.X + conv.ConvertX(HeaderWidth), rect.Y, rect.Width - conv.ConvertX(HeaderWidth), rect.Height);
			var headerFont = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			r1.Y += GetYOffset(dpi, headerFont);
			r2.Y += GetYOffset(dpi, font);
			GitterApplication.TextRenderer.DrawText(
				graphics, header, headerFont, Owner.Style.Colors.GrayText, r1, HeaderFormat);
			GitterApplication.TextRenderer.DrawText(
				graphics, content, font, Owner.Style.Colors.WindowText, r2, ContentFormat);
			_lastDpi = conv.To;
		}

		protected void DefaultPaint(Graphics graphics, Dpi dpi, Font font, string header, TextWithHyperlinks content, Rectangle rect)
		{
			var conv = DpiConverter.FromDefaultTo(dpi);
			var r1 = new Rectangle(rect.X, rect.Y, conv.ConvertX(HeaderWidth) - conv.ConvertX(4), conv.ConvertY(DefaultElementHeight));
			var r2 = new Rectangle(rect.X + conv.ConvertX(HeaderWidth), rect.Y, rect.Width - conv.ConvertX(HeaderWidth), rect.Height);
			var headerFont = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			r1.Y += GetYOffset(dpi, headerFont);
			r2.Y += GetYOffset(dpi, font);
			GitterApplication.TextRenderer.DrawText(
				graphics, header, headerFont, Owner.Style.Colors.GrayText, r1, HeaderFormat);
			content.Render(Owner.Style, graphics, font, r2);
			_lastDpi = conv.To;
		}

		public abstract void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect);

		public virtual void MouseMove(Rectangle rect, Point point)
		{
		}

		public virtual void MouseLeave()
		{
		}

		public virtual void MouseDown(Rectangle rect, MouseButtons button, int x, int y)
		{
		}
	}

	#endregion

	#region Data

	private readonly IRevisionHeaderElement[] _elements;
	private readonly Dictionary<Element, Size> _sizes;
	private readonly TrackingService _hoverElement;
	private readonly IEnumerable<IHyperlinkExtractor> _additionalHyperlinkExtractors;
	private int _measuredWidth;
	private int _measuredHeight;
	private Cursor _cursor;
	private IGitterStyle _style;

	private Revision _revision;

	#endregion

	#region Events

	public event EventHandler<ContentInvalidatedEventArgs> Invalidated;

	public event EventHandler<ContentContextMenuEventArgs> ContextMenuRequested;

	public event EventHandler CursorChanged;

	public event EventHandler SizeChanged;

	private void OnInvalidated(Rectangle bounds)
		=> Invalidated?.Invoke(this, new ContentInvalidatedEventArgs(bounds));

	private void OnCursorChanged()
		=> CursorChanged?.Invoke(this, EventArgs.Empty);

	private void OnSizeChanged()
		=> SizeChanged?.Invoke(this, EventArgs.Empty);

	private void OnContextMenuRequested(ContextMenuStrip contextMenu, Point position)
	{
		var handler = ContextMenuRequested;
		contextMenu.Renderer = Style.ToolStripRenderer;
		handler?.Invoke(this, new ContentContextMenuEventArgs(contextMenu, position));
	}

	#endregion

	public RevisionHeaderContent(IEnumerable<IHyperlinkExtractor> additionalHyperlinkExtractors = null)
	{
		_additionalHyperlinkExtractors = additionalHyperlinkExtractors;
		_elements = new IRevisionHeaderElement[]
		{
			new HashElement(this),
			new ParentsElement(this),
			new AuthorElement(this),
			new CommitterElement(this),
			new CommitDateElement(this),
			new SubjectElement(this),
			new BodyElement(this),
			new ReferencesElement(this),
		};
		foreach(var e in _elements)
		{
			e.InvalidateRequired   += (_, eargs) => OnSizeChanged();
			e.CursorChangeRequired += (_, eargs) => Cursor = eargs.Cursor;
		}
		_cursor = Cursors.Default;
		_sizes = new Dictionary<Element, Size>(_elements.Length);
		_hoverElement = new TrackingService(OnHoverChanged);
	}

	public Revision Revision
	{
		get => _revision;
		set
		{
			if(_revision != value)
			{
				if(_revision is not null)
				{
					_revision.Author.Avatar.Updated -= OnAuthorAvatarUpdated;
					_revision.References.Changed -= OnReferenceListChanged;
				}
				_revision = value;
				_measuredWidth = 0;
				if(_revision is not null)
				{
					_revision.Author.Avatar.Updated += OnAuthorAvatarUpdated;
					_revision.References.Changed += OnReferenceListChanged;
				}
			}
		}
	}

	private IHyperlinkExtractor GetHyperlinkExtractor(Revision revision)
	{
		var bugtrackerUrl = revision.Repository.Configuration.TryGetParameterValue("gitter.bugtracker.url");
		var issueIdRegex  = revision.Repository.Configuration.TryGetParameterValue("gitter.bugtracker.issueid");
		var extractors    = new List<IHyperlinkExtractor>();
		extractors.Add(new AbsoluteUrlHyperlinkExtractor());
		if(bugtrackerUrl is not null && issueIdRegex is not null)
		{
			extractors.Add(new RegexHyperlinkExtractor(issueIdRegex, bugtrackerUrl));
		}
		if(_additionalHyperlinkExtractors is not null)
		{
			extractors.AddRange(_additionalHyperlinkExtractors);
		}
		extractors.Add(new HashHyperlinkExtractor());
		return extractors.Count == 1
			? extractors[0]
			: new HyperlinkExtractor(extractors);
	}

	public Cursor Cursor
	{
		get => _cursor;
		set
		{
			if(_cursor != value)
			{
				_cursor = value;
				OnCursorChanged();
			}
		}
	}

	public IGitterStyle Style
	{
		get => _style ?? GitterApplication.Style;
		set => _style = value;
	}

	private void OnAuthorAvatarUpdated(object sender, EventArgs e)
	{
		OnInvalidated(new Rectangle(0, 0, _measuredWidth, _measuredHeight));
	}

	private void OnReferenceListChanged(object sender, EventArgs e)
	{
		_sizes.TryGetValue(Element.References, out var size);
		bool norefs;
		lock(_revision.References.SyncRoot)
		{
			norefs = _revision.References.Count == 0;
		}
		if((size.IsEmpty && !norefs) ||
			(!size.IsEmpty && norefs))
		{
			_measuredWidth = -1;
			OnSizeChanged();
		}
		else
		{
			OnInvalidated(new Rectangle(0, 0, _measuredWidth, _measuredHeight));
		}
	}

	private void OnHoverChanged(TrackingEventArgs e)
	{
		var bounds = GetElementBounds(e.Index);
		if(!e.IsTracked)
		{
			_elements[e.Index].MouseLeave();
		}
		OnInvalidated(bounds);
	}

	private Rectangle GetElementBounds(int index)
	{
		int cy = 0;
		for(int i = 0; i < _elements.Length; ++i)
		{
			if(_elements[i].IsAvailableFor(_revision))
			{
				if(!_sizes.TryGetValue(_elements[i].Element, out var size)) break;
				if(i == index) return new Rectangle(0, cy, size.Width, size.Height);
				var nexty = cy + size.Height;
				cy = nexty;
			}
		}
		return Rectangle.Empty;
	}

	private int HitTest(int x, int y)
	{
		int cy = 0;
		for(int i = 0; i < _elements.Length; ++i)
		{
			if(_elements[i].IsAvailableFor(_revision))
			{
				if(!_sizes.TryGetValue(_elements[i].Element, out var size)) break;
				var nexty = cy + size.Height;
				if(y < nexty)
				{
					if(x >= size.Width) break;
					return i;
				}
				cy = nexty;
			}
		}
		return -1;
	}

	private void Measure(Graphics graphics, Dpi dpi, int width)
	{
		int h = 0;
		for(int i = 0; i < _elements.Length; ++i)
		{
			if(_elements[i].IsAvailableFor(_revision))
			{
				var s = _elements[i].Measure(graphics, dpi, _revision, width);
				h += s.Height;
				_sizes[_elements[i].Element] = s;
			}
		}
		_measuredWidth = width;
		_measuredHeight = h;
	}

	public void OnMouseMove(int x, int y)
	{
		int element = HitTest(x, y);
		_hoverElement.Track(element);
		if(element != -1)
		{
			var bounds = GetElementBounds(element);
			_elements[element].MouseMove(bounds, new Point(x, y));
		}
	}

	public void OnMouseDown(int x, int y, MouseButtons button)
	{
		var index = HitTest(x, y);
		if(index != -1)
		{
			var bounds = GetElementBounds(index);
			_elements[index].MouseDown(bounds, button, x, y);
			if(button == MouseButtons.Right)
			{
				var menu = _elements[index].CreateContextMenu(_revision, bounds, x, y);
				if(menu != null)
				{
					OnContextMenuRequested(menu, new Point(x, y));
				}
			}
		}
	}

	public void OnMouseLeave()
	{
		_hoverElement.Drop();
	}

	public Size OnMeasure(Graphics graphics, Dpi dpi, int width)
	{
		if(_revision is null) return Size.Empty;
		var conv = DpiConverter.FromDefaultTo(dpi);
		var min  = conv.ConvertX(MinWidth);
		if(width < min) width = min;
		if(_measuredWidth != width) Measure(graphics, dpi, width);
		return new Size(width, _measuredHeight);
	}

	private async void UpdateAvatar(IAvatar avatar, DpiConverter conv)
	{
		await avatar.UpdateAsync();
		var size = conv.Convert(new Size(60, 60));
		if(_measuredWidth >= conv.ConvertX(MinWidth) + size.Width + conv.ConvertX(10))
		{
			OnInvalidated(new Rectangle(_measuredWidth - size.Width - conv.ConvertX(4), conv.ConvertY(4), size.Height, size.Width));
		}
	}

	public void OnPaint(Graphics graphics, Dpi dpi, Rectangle bounds, Rectangle clipRectangle)
	{
		if(_revision is null) return;
		var conv  = DpiConverter.FromDefaultTo(dpi);
		var width = bounds.Width;
		if(_measuredWidth != width) Measure(graphics, dpi, width);
		if(GitterApplication.IntegrationFeatures.Gravatar.IsEnabled)
		{
			var avatar = _revision.Author.Avatar;
			var image = avatar.Image;
			if(image is null)
			{
				UpdateAvatar(avatar, conv);
			}
			else
			{
				var size = conv.Convert(new Size(60, 60));
				if(bounds.Width >= conv.ConvertX(MinWidth) + size.Width + conv.ConvertX(10))
				{
					graphics.DrawImage(image, new Rectangle(
						bounds.Right - size.Width - conv.ConvertX(4),
						bounds.Y + conv.ConvertY(4),
						size.Width, size.Height));
				}
			}
		}
		var elementBounds = bounds;
		var headerWidth   = conv.ConvertX(HeaderWidth);
		for(int i = 0; i < _elements.Length; ++i)
		{
			var element = _elements[i];
			if(!element.IsAvailableFor(Revision)) continue;

			var size = _sizes[element.Element];
			if(size.Height > 0)
			{
				elementBounds.Height = size.Height;
				if(i == _hoverElement.Index)
				{
					Color trackColor1, trackColor2;
					if(Style.Type == GitterStyleType.LightBackground)
					{
						trackColor1 = Color.WhiteSmoke;
						trackColor2 = Color.FromArgb(238, 238, 238);
					}
					else
					{
						trackColor1 = Color.FromArgb(18, 18, 18);
						trackColor2 = Color.FromArgb(18, 18, 18);
					}

					if(trackColor1 == trackColor2)
					{
						var rcBackground = Rectangle.Intersect(clipRectangle,
							new Rectangle(elementBounds.X, elementBounds.Y, size.Width, size.Height));
						if(rcBackground is { Width: > 0, Height: > 0 })
						{
							graphics.GdiFill(trackColor1, rcBackground);
						}
					}
					else
					{
						var rcBackground = Rectangle.Intersect(clipRectangle,
							new Rectangle(elementBounds.X, elementBounds.Y, headerWidth, size.Height));
						if(rcBackground is { Width: > 0, Height: > 0 })
						{
							graphics.GdiFill(trackColor1, rcBackground);
						}
						rcBackground = Rectangle.Intersect(clipRectangle,
							new Rectangle(elementBounds.X + headerWidth, elementBounds.Y, size.Width - headerWidth, size.Height));
						if(rcBackground is { Width: > 0, Height: > 0 })
						{
							graphics.GdiFill(trackColor2, rcBackground);
						}
					}
				}
				element.Paint(graphics, dpi, Revision, elementBounds);
				elementBounds.Y += size.Height;
			}
		}
	}
}

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

namespace gitter.Git.Gui
{
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

	sealed class RevisionHeaderContent
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
				var font = GitterApplication.FontManager.UIFont.Font;
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
				var font = GitterApplication.FontManager.UIFont.Font;
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
				var font = GitterApplication.FontManager.UIFont.Font;
				DefaultPaint(graphics, dpi, font, header, content, rect);
			}

			protected void DefaultPaint(Graphics graphics, Dpi dpi, string header, TextWithHyperlinks content, Rectangle rect)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				DefaultPaint(graphics, dpi, font, header, content, rect);
			}

			protected Rectangle GetContentRectangle(Rectangle rect)
			{
				var headerWidth = (int)(HeaderWidth * _lastDpi.X / 96 + 0.5f);
				var r2 = new Rectangle(rect.X + headerWidth, rect.Y, rect.Width - headerWidth, rect.Height);
				r2.Y += GetYOffset(_lastDpi, GitterApplication.FontManager.UIFont);
				return r2;
			}

			protected void DefaultPaint(Graphics graphics, Dpi dpi, Font font, string header, string content, Rectangle rect)
			{
				var conv = DpiConverter.FromDefaultTo(dpi);
				var r1 = new Rectangle(rect.X, rect.Y, conv.ConvertX(HeaderWidth) - conv.ConvertX(4), conv.ConvertY(DefaultElementHeight));
				var r2 = new Rectangle(rect.X + conv.ConvertX(HeaderWidth), rect.Y, rect.Width - conv.ConvertX(HeaderWidth), rect.Height);
				var headerFont = GitterApplication.FontManager.UIFont.Font;
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
				var headerFont = GitterApplication.FontManager.UIFont.Font;
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

		sealed class HashElement : BaseElement
		{
			public HashElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override Element Element => Element.Hash;

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();

				var dpiBindings = new DpiBindings(menu);
				var factory     = new GuiItemFactory(dpiBindings);

				menu.Items.Add(factory.GetViewTreeItem<ToolStripMenuItem>(revision));
				menu.Items.Add(GuiItemFactory.GetSavePatchItem<ToolStripMenuItem>(revision));
				menu.Items.Add(factory.GetArchiveItem<ToolStripMenuItem>(revision));
				menu.Items.Add(new ToolStripSeparator());
				menu.Items.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, revision.HashString));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
				=> Measure(graphics, dpi, TreeHashColumn.Font, revision.HashString, width);

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
				=> DefaultPaint(graphics, dpi, HashColumn.Font, Resources.StrHash.AddColon(), revision.HashString, rect);
		}

		sealed class TreeHashElement : BaseElement
		{
			public TreeHashElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override Element Element => Element.TreeHash;

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu        = new ContextMenuStrip();
				var dpiBindings = new DpiBindings(menu);
				var factory     = new GuiItemFactory(dpiBindings);
				menu.Items.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, revision.TreeHashString));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
				=> Measure(graphics, dpi, TreeHashColumn.Font, revision.TreeHashString, width);

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
				=> DefaultPaint(graphics, dpi, TreeHashColumn.Font, Resources.StrTreeHash.AddColon(), revision.TreeHashString, rect);
		}

		sealed class ParentsElement : BaseElement
		{
			public ParentsElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override Element Element => Element.Parents;

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu        = new ContextMenuStrip();
				var dpiBindings = new DpiBindings(menu);
				var factory     = new GuiItemFactory(dpiBindings);
				var sb = new StringBuilder(41 * revision.Parents.Count);
				foreach(var p in revision.Parents)
				{
					sb.Append(p.Hash);
					sb.Append('\n');
				}
				sb.Remove(sb.Length - 1, 1);
				menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, sb.ToString()));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
			{
				switch(revision.Parents.Count)
				{
					case 0:
						return Size.Empty;
					case 1:
						return Measure(graphics, dpi, HashColumn.Font, revision.Parents[0].HashString, width);
					default:
						var sb = new StringBuilder(41 * revision.Parents.Count);
						bool first = true;
						for(int i = 0; i < revision.Parents.Count; ++i)
						{
							var p = revision.Parents[i];
							if(!first) sb.Append('\n');
							else first = false;
							sb.Append(p.Hash);
						}
						return MeasureMultilineContent(graphics, dpi, HashColumn.Font, sb.ToString(), width);
				}
			}

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
			{
				if(revision.Parents.Count == 1)
				{
					DefaultPaint(graphics, dpi, HashColumn.Font, Resources.StrParent.AddColon(), revision.Parents[0].HashString, rect);
				}
				else
				{
					var sb = new StringBuilder(41 * revision.Parents.Count);
					foreach(var p in revision.Parents)
					{
						sb.Append(p.Hash);
						sb.Append('\n');
					}
					DefaultPaint(graphics, dpi, HashColumn.Font, Resources.StrParents.AddColon(), sb.ToString(), rect);
				}
			}
		}

		sealed class AuthorElement : BaseElement
		{
			public AuthorElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override Element Element => Element.Author;

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu        = new ContextMenuStrip();
				var dpiBindings = new DpiBindings(menu);
				var factory     = new GuiItemFactory(dpiBindings);
				menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					string.Format("{0} <{1}>", revision.Author.Name, revision.Author.Email)));
				menu.Items.Add(GuiItemFactory.GetSendEmailItem<ToolStripMenuItem>(revision.Author.Email));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
				=> Measure(graphics, dpi, GitterApplication.FontManager.UIFont, string.Format("{0} <{1}>", revision.Author.Name, revision.Author.Email), width);

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
				=> DefaultPaint(graphics, dpi, Resources.StrAuthor.AddColon(), string.Format("{0} <{1}>", revision.Author.Name, revision.Author.Email), rect);
		}

		sealed class CommitterElement : BaseElement
		{
			public CommitterElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override bool IsAvailableFor(Revision revision)
				=> revision.Author != revision.Committer;

			public override Element Element => Element.Committer;

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu        = new ContextMenuStrip();
				var dpiBindings = new DpiBindings(menu);
				var factory     = new GuiItemFactory(dpiBindings);
				menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					string.Format("{0} <{1}>", revision.Committer.Name, revision.Committer.Email)));
				menu.Items.Add(GuiItemFactory.GetSendEmailItem<ToolStripMenuItem>(revision.Committer.Email));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
				=> Measure(graphics, dpi, GitterApplication.FontManager.UIFont, string.Format("{0} <{1}>", revision.Committer.Name, revision.Committer.Email), width);

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
				=> DefaultPaint(graphics, dpi, Resources.StrCommitter.AddColon(), string.Format("{0} <{1}>", revision.Committer.Name, revision.Committer.Email), rect);
		}

		sealed class CommitDateElement : BaseElement
		{
			public CommitDateElement(RevisionHeaderContent owner)
				: base(owner)
			{
				DateFormat = DateFormat.ISO8601;
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu        = new ContextMenuStrip();
				var dpiBindings = new DpiBindings(menu);
				var factory     = new GuiItemFactory(dpiBindings);
				menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					Utility.FormatDate(revision.CommitDate, DateFormat)));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public DateFormat DateFormat { get; set; }

			public override Element Element => Element.CommitDate;

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
				=> Measure(graphics, dpi, GitterApplication.FontManager.UIFont, Utility.FormatDate(revision.CommitDate, DateFormat), width);

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
				=> DefaultPaint(graphics, dpi, GitterApplication.FontManager.UIFont, Resources.StrDate.AddColon(), Utility.FormatDate(revision.CommitDate, DateFormat), rect);
		}

		sealed class AuthorDateElement : BaseElement
		{
			public AuthorDateElement(RevisionHeaderContent owner)
				: base(owner)
			{
				DateFormat = DateFormat.ISO8601;
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu        = new ContextMenuStrip();
				var dpiBindings = new DpiBindings(menu);
				var factory     = new GuiItemFactory(dpiBindings);
				menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					Utility.FormatDate(revision.AuthorDate, DateFormat)));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public DateFormat DateFormat { get; set; }

			public override Element Element => Element.AuthorDate;

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
				=> Measure(graphics, dpi, GitterApplication.FontManager.UIFont, Utility.FormatDate(revision.AuthorDate, DateFormat), width);

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
				=> DefaultPaint(graphics, dpi, GitterApplication.FontManager.UIFont, Resources.StrDate.AddColon(), Utility.FormatDate(revision.AuthorDate, DateFormat), rect);
		}

		abstract class TextWithHyperlinksElementBase : BaseElement
		{
			private readonly string _headerText;
			private TextWithHyperlinks _text;

			protected TextWithHyperlinksElementBase(RevisionHeaderContent owner, string headerText)
				: base(owner)
			{
				_headerText = headerText;
			}

			protected abstract string GetText(Revision revision);

			public override ContextMenuStrip CreateContextMenu(Revision revision, Rectangle bounds, int x, int y)
			{
				if(_text is not null)
				{
					var link = _text.HitTest(GetContentRectangle(bounds), new Point(x, y));
					if(link is not null) return new HyperlinkContextMenu(link);
				}
				var menu        = new ContextMenuStrip();
				var dpiBindings = new DpiBindings(menu);
				var factory     = new GuiItemFactory(dpiBindings);
				menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					GetText(revision), false));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
				=> MeasureMultilineContent(graphics, dpi, GetText(revision), width);

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
			{
				var text = GetText(revision);
				if(_text is null || _text.Text != text)
				{
					_text = new TextWithHyperlinks(text, GetHyperlinkExtractor(revision));
					_text.InvalidateRequired += OnTextInvalidateRequired;
				}

				DefaultPaint(graphics, dpi, _headerText, _text, rect);
			}

			private void OnTextInvalidateRequired(object sender, EventArgs e)
			{
				OnInvalidateRequired();
				if(_text.HoveredHyperlink is null)
				{
					ChangeCursor(Cursors.Default);
				}
				else
				{
					ChangeCursor(Cursors.Hand);
				}
			}

			public override void MouseMove(Rectangle rect, Point point)
				=> _text?.OnMouseMove(GetContentRectangle(rect), point);

			public override void MouseLeave()
				=> _text?.OnMouseLeave();

			public override void MouseDown(Rectangle rect, MouseButtons button, int x, int y)
			{
				if(_text is not null)
				{
					switch(button)
					{
						case MouseButtons.Left:
							_text.OnMouseDown(GetContentRectangle(rect), new Point(x, y));
							break;
					}
				}
			}
		}

		sealed class SubjectElement : TextWithHyperlinksElementBase
		{
			public SubjectElement(RevisionHeaderContent owner)
				: base(owner, Resources.StrSubject.AddColon())
			{
			}

			public override Element Element => Element.Subject;

			protected override string GetText(Revision revision) => revision.Subject;
		}

		sealed class BodyElement : TextWithHyperlinksElementBase
		{
			public BodyElement(RevisionHeaderContent owner)
				: base(owner, Resources.StrBody.AddColon())
			{
			}

			public override bool IsAvailableFor(Revision revision)
			{
				return !string.IsNullOrEmpty(revision.Body);
			}

			public override Element Element => Element.Body;

			protected override string GetText(Revision revision) => revision.Body;
		}

		sealed class ReferencesElement : BaseElement
		{
			private struct ReferenceVisual
			{
				public ReferenceVisual(Reference reference, Rectangle rectangle)
				{
					Reference = reference;
					Rectangle = rectangle;
				}

				public Reference Reference { get; }

				public Rectangle Rectangle { get; }
			}

			private LinkedList<ReferenceVisual> _drawnReferences = new();

			public ReferencesElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override bool IsAvailableFor(Revision revision)
				=> revision.References.Count != 0;

			public override Element Element => Element.References;

			public override void MouseDown(Rectangle rect, MouseButtons button, int x, int y)
			{
				if(button == MouseButtons.Right)
				{
					foreach(var reference in _drawnReferences)
					{
						if(reference.Rectangle.X <= x && reference.Rectangle.Right > x)
						{
							ContextMenuStrip menu = reference.Reference switch
							{
								BranchBase branch => new BranchMenu(branch),
								Tag        tag    => new TagMenu(tag),
								Head       head   => new HeadMenu(head),
								_ => default,
							};
							if(menu is not null)
							{
								Utility.MarkDropDownForAutoDispose(menu);
								Owner.OnContextMenuRequested(menu, new Point(x + 1, y + 1));
							}
							return;
						}
					}
				}
			}

			public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
			{
				var conv = DpiConverter.FromDefaultTo(dpi);
				var font = GitterApplication.FontManager.UIFont.Font;
				lock(revision.References.SyncRoot)
				{
					if(revision.References.Count == 0) return Size.Empty;
					int offset = 0;
					foreach(var reference in revision.References)
					{
						var name = ((INamedObject)reference).Name;
						var size = GitterApplication.TextRenderer.MeasureText(graphics, name, font, int.MaxValue, ContentFormat);
						offset += size.Width + conv.ConvertX(3) + conv.ConvertX(6);
					}
					return new Size(HeaderWidth + offset - conv.ConvertX(3), conv.ConvertY(DefaultElementHeight));
				}
			}

			public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
			{
				const float Radius = 3;

				_drawnReferences.Clear();
				PaintHeader(graphics, dpi, Resources.StrRefs.AddColon(), rect);
				var font = GitterApplication.FontManager.UIFont.Font;
				int offset = 0;
				using var tagBrush    = new SolidBrush(ColorScheme.TagBackColor);
				using var localBrush  = new SolidBrush(ColorScheme.LocalBranchBackColor);
				using var remoteBrush = new SolidBrush(ColorScheme.RemoteBranchBackColor);
				lock(revision.References.SyncRoot)
				{
					var conv = DpiConverter.FromDefaultTo(dpi);
					var r2 = new Rectangle(rect.X + conv.ConvertX(HeaderWidth), rect.Y, rect.Width - conv.ConvertX(HeaderWidth), rect.Height);
					r2.Y += GetYOffset(conv.To, font);
					foreach(var reference in revision.References)
					{
						var name = ((INamedObject)reference).Name;
						var size = GitterApplication.TextRenderer.MeasureText(
							graphics, name, font, int.MaxValue, ContentFormat);
						var r = new Rectangle(r2.X + offset, r2.Y + 1, size.Width + conv.ConvertX(6), conv.ConvertY(DefaultElementHeight));
						var brush = reference.Type switch
						{
							ReferenceType.LocalBranch  => localBrush,
							ReferenceType.RemoteBranch => remoteBrush,
							ReferenceType.Tag          => tagBrush,
							_ => Brushes.WhiteSmoke,
						};
						graphics.FillRoundedRectangle(brush, Pens.Black, r, Radius);
						var textRect = new Rectangle(r2.X + offset + conv.ConvertX(3), r2.Y, size.Width + conv.ConvertX(6) - 1, size.Height);
						GitterApplication.TextRenderer.DrawText(
							graphics, name, font, SystemBrushes.WindowText, textRect, ContentFormat);
						_drawnReferences.AddLast(new ReferenceVisual(reference, r));
						offset += size.Width + conv.ConvertX(3) + conv.ConvertX(6);
					}
				}
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
					avatar.BeginUpdate();
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
							Rectangle rcBackground;
							rcBackground = Rectangle.Intersect(clipRectangle,
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
}

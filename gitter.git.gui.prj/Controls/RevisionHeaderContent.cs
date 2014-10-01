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
	using System.Drawing.Drawing2D;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class RevisionHeaderContent
	{
		#region Constants

		private static readonly int DefaultElementHeight = SystemInformation.SmallIconSize.Height;
		private static readonly int HeaderWidth          = (int)((SystemInformation.SmallIconSize.Width / 16.0) * 70);
		private static readonly int MinWidth             = HeaderWidth + (int)((SystemInformation.SmallIconSize.Width / 16.0) * 295);

		#endregion

		#region Static

		private static readonly StringFormat HeaderFormat = new StringFormat(StringFormat.GenericDefault)
		{
			Alignment = StringAlignment.Far,
			FormatFlags =
				StringFormatFlags.LineLimit |
				StringFormatFlags.NoClip |
				StringFormatFlags.NoWrap,
			LineAlignment = StringAlignment.Near,
			Trimming = StringTrimming.None,
		};

		private static readonly StringFormat ContentFormat = new StringFormat(StringFormat.GenericTypographic)
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
			private readonly Cursor _cursor;

			public CursorChangedEventArgs(Cursor cursor)
			{
				_cursor = cursor;
			}

			public Cursor Cursor
			{
				get { return _cursor; }
			}
		}

		/// <summary>Interface for a single data field.</summary>
		interface IRevisionHeaderElement
		{
			event EventHandler InvalidateRequired;

			event EventHandler<CursorChangedEventArgs> CursorChangeRequired;

			/// <summary>Displayed data.</summary>
			Element Element { get; }

			bool IsAvailableFor(Revision revision);

			ContextMenuStrip CreateContextMenu(Revision revision);

			Size Measure(Graphics graphics, Revision revision, int width);

			void Paint(Graphics graphics, Revision revision, Rectangle rect);

			void MouseMove(Rectangle rect, Point point);

			void MouseLeave();

			void MouseDown(Rectangle rect, MouseButtons button, int x, int y);
		}

		abstract class BaseElement : IRevisionHeaderElement
		{
			public event EventHandler InvalidateRequired;

			public event EventHandler<CursorChangedEventArgs> CursorChangeRequired;

			protected void OnInvalidateRequired()
			{
				var handler = InvalidateRequired;
				if(handler != null) handler(this, EventArgs.Empty);
			}

			protected void ChangeCursor(Cursor cursor)
			{
				var handler = CursorChangeRequired;
				if(handler != null) handler(this, new CursorChangedEventArgs(cursor));
			}

			private readonly RevisionHeaderContent _owner;

			protected BaseElement(RevisionHeaderContent owner)
			{
				_owner = owner;
			}

			public RevisionHeaderContent Owner
			{
				get { return _owner; }
			}

			public abstract Element Element { get; }

			protected HyperlinkExtractor GetHyperlinkExtractor(Revision revision)
			{
				var bugtrackerUrl = revision.Repository.Configuration.TryGetParameterValue("gitter.bugtracker.url");
				var issueIdRegex = revision.Repository.Configuration.TryGetParameterValue("gitter.bugtracker.issueid");
				if(bugtrackerUrl != null && issueIdRegex != null)
				{
					return new HyperlinkExtractor(issueIdRegex, bugtrackerUrl);
				}
				else
				{
					return new HyperlinkExtractor();
				}
			}

			public virtual bool IsAvailableFor(Revision revision)
			{
				return true;
			}

			public virtual ContextMenuStrip CreateContextMenu(Revision revision)
			{
				return null;
			}

			public virtual Size Measure(Graphics graphics, Revision revision, int width)
			{
				return new Size(width, DefaultElementHeight);
			}

			protected static Size Measure(Graphics graphics, Font font, string text, int width)
			{
				var w = HeaderWidth + GitterApplication.TextRenderer.MeasureText(graphics, text, font, width, ContentFormat).Width;
				return new Size(w, DefaultElementHeight);
			}

			protected static Size MeasureMultilineContent(Graphics graphics, string content, int width)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				return MeasureMultilineContent(graphics, font, content, width);
			}

			protected static Size MeasureMultilineContent(Graphics graphics, Font font, string content, int width)
			{
				var s = GitterApplication.TextRenderer.MeasureText(graphics, content, font, width - HeaderWidth, ContentFormat);
				if(s.Height < DefaultElementHeight) s.Height = DefaultElementHeight;
				return new Size(HeaderWidth + s.Width, s.Height);
			}

			protected static int GetYOffset(Font font)
			{
				int offset = (int)(DefaultElementHeight - GitterApplication.TextRenderer.GetFontHeight(font));
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

			protected void PaintHeader(Graphics graphics, string header, Rectangle rect)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				var r1 = new Rectangle(rect.X, rect.Y, HeaderWidth - 4, DefaultElementHeight);
				r1.Y += GetYOffset(font);
				using(var brush = new SolidBrush(Owner.Style.Colors.GrayText))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, header, font, brush, r1, HeaderFormat);
				}
			}

			protected void DefaultPaint(Graphics graphics, string header, string content, Rectangle rect)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				DefaultPaint(graphics, font, header, content, rect);
			}

			protected void DefaultPaint(Graphics graphics, string header, TextWithHyperlinks content, Rectangle rect)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				DefaultPaint(graphics, font, header, content, rect);
			}

			protected static Rectangle GetContentRectangle(Rectangle rect)
			{
				var r2 = new Rectangle(rect.X + HeaderWidth, rect.Y, rect.Width - HeaderWidth, rect.Height);
				r2.Y += GetYOffset(GitterApplication.FontManager.UIFont);
				return r2;
			}

			protected void DefaultPaint(Graphics graphics, Font font, string header, string content, Rectangle rect)
			{
				var r1 = new Rectangle(rect.X, rect.Y, HeaderWidth - 4, DefaultElementHeight);
				var r2 = new Rectangle(rect.X + HeaderWidth, rect.Y, rect.Width - HeaderWidth, rect.Height);
				var headerFont = GitterApplication.FontManager.UIFont.Font;
				r1.Y += GetYOffset(headerFont);
				r2.Y += GetYOffset(font);
				using(var brush = new SolidBrush(Owner.Style.Colors.GrayText))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, header, headerFont, brush, r1, HeaderFormat);
				}
				using(var brush = new SolidBrush(Owner.Style.Colors.WindowText))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, content, font, brush, r2, ContentFormat);
				}
			}

			protected void DefaultPaint(Graphics graphics, Font font, string header, TextWithHyperlinks content, Rectangle rect)
			{
				var r1 = new Rectangle(rect.X, rect.Y, HeaderWidth - 4, DefaultElementHeight);
				var r2 = new Rectangle(rect.X + HeaderWidth, rect.Y, rect.Width - HeaderWidth, rect.Height);
				var headerFont = GitterApplication.FontManager.UIFont.Font;
				r1.Y += GetYOffset(headerFont);
				r2.Y += GetYOffset(font);
				using(var brush = new SolidBrush(Owner.Style.Colors.GrayText))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, header, headerFont, brush, r1, HeaderFormat);
				}
				content.Render(Owner.Style, graphics, font, r2);
			}

			public abstract void Paint(Graphics graphics, Revision revision, Rectangle rect);

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

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.Hash; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(revision));
				menu.Items.Add(GuiItemFactory.GetSavePatchItem<ToolStripMenuItem>(revision));
				menu.Items.Add(GuiItemFactory.GetArchiveItem<ToolStripMenuItem>(revision));
				menu.Items.Add(new ToolStripSeparator());
				menu.Items.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, revision.HashString));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return Measure(graphics, TreeHashColumn.Font, revision.HashString, width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				DefaultPaint(graphics, HashColumn.Font, Resources.StrHash.AddColon(), revision.HashString, rect);
			}
		}

		sealed class TreeHashElement : BaseElement
		{
			public TreeHashElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.TreeHash; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, revision.TreeHashString));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return Measure(graphics, TreeHashColumn.Font, revision.TreeHashString, width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				DefaultPaint(graphics, TreeHashColumn.Font, Resources.StrTreeHash.AddColon(), revision.TreeHashString, rect);
			}
		}

		sealed class ParentsElement : BaseElement
		{
			public ParentsElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.Parents; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				var sb = new StringBuilder(41 * revision.Parents.Count);
				foreach(var p in revision.Parents)
				{
					sb.Append(p.Hash);
					sb.Append('\n');
				}
				sb.Remove(sb.Length - 1, 1);
				menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, sb.ToString()));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				switch(revision.Parents.Count)
				{
					case 0:
						return Size.Empty;
					case 1:
						return Measure(graphics, HashColumn.Font, revision.Parents[0].HashString, width);
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
						return MeasureMultilineContent(graphics, HashColumn.Font, sb.ToString(), width);
				}
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				if(revision.Parents.Count == 1)
				{
					DefaultPaint(graphics, HashColumn.Font, Resources.StrParent.AddColon(), revision.Parents[0].HashString, rect);
				}
				else
				{
					var sb = new StringBuilder(41 * revision.Parents.Count);
					foreach(var p in revision.Parents)
					{
						sb.Append(p.Hash);
						sb.Append('\n');
					}
					DefaultPaint(graphics, HashColumn.Font, Resources.StrParents.AddColon(), sb.ToString(), rect);
				}
			}
		}

		sealed class AuthorElement : BaseElement
		{
			public AuthorElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.Author; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					string.Format("{0} <{1}>", revision.Author.Name, revision.Author.Email)));
				menu.Items.Add(GuiItemFactory.GetSendEmailItem<ToolStripMenuItem>(revision.Author.Email));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return Measure(graphics, GitterApplication.FontManager.UIFont, string.Format("{0} <{1}>", revision.Author.Name, revision.Author.Email), width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				DefaultPaint(graphics, Resources.StrAuthor.AddColon(), string.Format("{0} <{1}>", revision.Author.Name, revision.Author.Email), rect);
			}
		}

		sealed class CommitterElement : BaseElement
		{
			public CommitterElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override bool IsAvailableFor(Revision revision)
			{
				return revision.Author != revision.Committer;
			}

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.Committer; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					string.Format("{0} <{1}>", revision.Committer.Name, revision.Committer.Email)));
				menu.Items.Add(GuiItemFactory.GetSendEmailItem<ToolStripMenuItem>(revision.Committer.Email));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return Measure(graphics, GitterApplication.FontManager.UIFont, string.Format("{0} <{1}>", revision.Committer.Name, revision.Committer.Email), width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				DefaultPaint(graphics, Resources.StrCommitter.AddColon(), string.Format("{0} <{1}>", revision.Committer.Name, revision.Committer.Email), rect);
			}
		}

		sealed class CommitDateElement : BaseElement
		{
			private DateFormat _dateFormat;

			public CommitDateElement(RevisionHeaderContent owner)
				: base(owner)
			{
				_dateFormat = DateFormat.ISO8601;
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					Utility.FormatDate(revision.CommitDate, _dateFormat)));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public DateFormat DateFormat
			{
				get { return _dateFormat; }
				set { _dateFormat = value; }
			}

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.CommitDate; }
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return Measure(graphics, GitterApplication.FontManager.UIFont, Utility.FormatDate(revision.CommitDate, _dateFormat), width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				DefaultPaint(
					graphics,
					GitterApplication.FontManager.UIFont,
					Resources.StrDate.AddColon(),
					Utility.FormatDate(revision.CommitDate, _dateFormat),
					rect);
			}
		}

		sealed class AuthorDateElement : BaseElement
		{
			private DateFormat _dateFormat;

			public AuthorDateElement(RevisionHeaderContent owner)
				: base(owner)
			{
				_dateFormat = DateFormat.ISO8601;
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					Utility.FormatDate(revision.AuthorDate, _dateFormat)));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public DateFormat DateFormat
			{
				get { return _dateFormat; }
				set { _dateFormat = value; }
			}

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.AuthorDate; }
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return Measure(graphics, GitterApplication.FontManager.UIFont, Utility.FormatDate(revision.AuthorDate, _dateFormat), width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				DefaultPaint(
					graphics,
					GitterApplication.FontManager.UIFont,
					Resources.StrDate.AddColon(),
					Utility.FormatDate(revision.AuthorDate, _dateFormat),
					rect);
			}
		}

		sealed class SubjectElement : BaseElement
		{
			public SubjectElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			private TextWithHyperlinks _text;

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.Subject; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					revision.Subject, false));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return MeasureMultilineContent(graphics, revision.Subject, width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				if(_text == null || _text.Text != revision.Subject)
				{
					_text = new TextWithHyperlinks(revision.Subject, GetHyperlinkExtractor(revision));
					_text.InvalidateRequired += OnTextInvalidateRequired;
				}

				DefaultPaint(graphics, Resources.StrSubject.AddColon(), _text, rect);
			}

			private void OnTextInvalidateRequired(object sender, EventArgs e)
			{
				OnInvalidateRequired();
				if(_text.HoveredHyperlink == null)
				{
					ChangeCursor(Cursors.Default);
				}
				else
				{
					ChangeCursor(Cursors.Hand);
				}
			}

			public override void MouseMove(Rectangle rect, Point point)
			{
				if(_text != null)
				{
					_text.OnMouseMove(GetContentRectangle(rect), point);
				}
			}

			public override void MouseLeave()
			{
				if(_text != null)
				{
					_text.OnMouseLeave();
				}
			}

			public override void MouseDown(Rectangle rect, MouseButtons button, int x, int y)
			{
				if(_text != null && button == MouseButtons.Left)
				{
					_text.OnMouseDown(GetContentRectangle(rect), new Point(x, y));
				}
			}
		}

		sealed class BodyElement : BaseElement
		{
			private TextWithHyperlinks _text;

			public BodyElement(RevisionHeaderContent owner)
				: base(owner)
			{
			}

			public override bool IsAvailableFor(Revision revision)
			{
				return !string.IsNullOrEmpty(revision.Body);
			}

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.Body; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
					revision.Body, false));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				if(string.IsNullOrEmpty(revision.Body)) return Size.Empty;
				return MeasureMultilineContent(graphics, revision.Body, width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				if(_text == null || _text.Text != revision.Body)
				{
					_text = new TextWithHyperlinks(revision.Body, GetHyperlinkExtractor(revision));
					_text.InvalidateRequired += OnTextInvalidateRequired;
				}

				DefaultPaint(graphics, Resources.StrBody.AddColon(), _text, rect);
			}

			private void OnTextInvalidateRequired(object sender, EventArgs e)
			{
				OnInvalidateRequired();
				if(_text.HoveredHyperlink == null)
				{
					ChangeCursor(Cursors.Default);
				}
				else
				{
					ChangeCursor(Cursors.Hand);
				}
			}

			public override void MouseMove(Rectangle rect, Point point)
			{
				if(_text != null)
				{
					_text.OnMouseMove(GetContentRectangle(rect), point);
				}
			}

			public override void MouseLeave()
			{
				if(_text != null)
				{
					_text.OnMouseLeave();
				}
			}

			public override void MouseDown(Rectangle rect, MouseButtons button, int x, int y)
			{
				if(_text != null && button == MouseButtons.Left)
				{
					_text.OnMouseDown(GetContentRectangle(rect), new Point(x, y));
				}
			}
		}

		sealed class ReferencesElement : BaseElement
		{
			private struct ReferenceVisual
			{
				private readonly Reference _reference;
				private readonly Rectangle _rectangle;

				public ReferenceVisual(Reference reference, Rectangle rectangle)
				{
					_reference = reference;
					_rectangle = rectangle;
				}

				public Reference Reference
				{
					get { return _reference; }
				}

				public Rectangle Rectangle
				{
					get { return _rectangle; }
				}
			}

			private LinkedList<ReferenceVisual> _drawnReferences;

			public ReferencesElement(RevisionHeaderContent owner)
				: base(owner)
			{
				_drawnReferences = new LinkedList<ReferenceVisual>();
			}

			public override bool IsAvailableFor(Revision revision)
			{
				return revision.References.Count != 0;
			}

			public override Element Element
			{
				get { return RevisionHeaderContent.Element.References; }
			}

			public override void MouseDown(Rectangle rect, MouseButtons button, int x, int y)
			{
				if(button == MouseButtons.Right)
				{
					foreach(var reference in _drawnReferences)
					{
						if(reference.Rectangle.X <= x && reference.Rectangle.Right > x)
						{
							var branch = reference.Reference as BranchBase;
							if(branch != null)
							{
								var menu = new BranchMenu(branch);
								Utility.MarkDropDownForAutoDispose(menu);
								Owner.OnContextMenuRequested(menu, new Point(x + 1, y + 1));
								return;
							}
							var tag = reference.Reference as Tag;
							if(tag != null)
							{
								var menu = new TagMenu(tag);
								Utility.MarkDropDownForAutoDispose(menu);
								Owner.OnContextMenuRequested(menu, new Point(x + 1, y + 1));
								return;
							}
							var head = reference.Reference as Head;
							if(head != null)
							{
								var menu = new HeadMenu(head);
								Utility.MarkDropDownForAutoDispose(menu);
								Owner.OnContextMenuRequested(menu, new Point(x + 1, y + 1));
								return;
							}
							return;
						}
					}
				}
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				lock(revision.References.SyncRoot)
				{
					if(revision.References.Count == 0) return Size.Empty;
					int offset = 0;
					foreach(var reference in revision.References)
					{
						var name = ((INamedObject)reference).Name;
						var size = GitterApplication.TextRenderer.MeasureText(graphics, name, font, int.MaxValue, ContentFormat);
						offset += size.Width + 3 + 6;
					}
					return new Size(HeaderWidth + offset - 3, DefaultElementHeight);
				}
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				const float Radius = 3;

				_drawnReferences.Clear();
				PaintHeader(graphics, Resources.StrRefs.AddColon(), rect);
				var font = GitterApplication.FontManager.UIFont.Font;
				var r2 = new Rectangle(rect.X + HeaderWidth, rect.Y, rect.Width - HeaderWidth, rect.Height);
				r2.Y += GetYOffset(font);
				int offset = 0;
				using(var tagBrush = new SolidBrush(ColorScheme.TagBackColor))
				using(var localBrush = new SolidBrush(ColorScheme.LocalBranchBackColor))
				using(var remoteBrush = new SolidBrush(ColorScheme.RemoteBranchBackColor))
				{
					lock(revision.References.SyncRoot)
					{
						foreach(var reference in revision.References)
						{
							var name = ((INamedObject)reference).Name;
							var size = GitterApplication.TextRenderer.MeasureText(
								graphics, name, font, int.MaxValue, ContentFormat);
							var r = new Rectangle(r2.X + offset, r2.Y, size.Width + 6, DefaultElementHeight - 1);
							Brush brush;
							switch(reference.Type)
							{
								case ReferenceType.LocalBranch:
									brush = localBrush;
									break;
								case ReferenceType.RemoteBranch:
									brush = remoteBrush;
									break;
								case ReferenceType.Tag:
									brush = tagBrush;
									break;
								default:
									brush = Brushes.WhiteSmoke;
									break;
							}
							graphics.FillRoundedRectangle(brush, Pens.Black, r, Radius);
							var textRect = new Rectangle(r2.X + offset + 3, r2.Y, size.Width + 5, size.Height);
							GitterApplication.TextRenderer.DrawText(
								graphics, name, font, SystemBrushes.WindowText, textRect, ContentFormat);
							_drawnReferences.AddLast(new ReferenceVisual(reference, r));
							offset += size.Width + 3 + 6;
						}
					}
				}
			}
		}

		#endregion

		#region Data

		private readonly IRevisionHeaderElement[] _elements;
		private readonly Dictionary<Element, Size> _sizes;
		private readonly TrackingService _hoverElement;
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
		{
			var handler = Invalidated;
			if(handler != null) handler(this, new ContentInvalidatedEventArgs(bounds));
		}

		private void OnCursorChanged()
		{
			var handler = CursorChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		private void OnSizeChanged()
		{
			var handler = SizeChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		private void OnContextMenuRequested(ContextMenuStrip contextMenu, Point position)
		{
			var handler = ContextMenuRequested;
			contextMenu.Renderer = Style.ToolStripRenderer;
			if(handler != null) handler(this, new ContentContextMenuEventArgs(contextMenu, position));
		}

		#endregion

		public RevisionHeaderContent()
		{
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
				e.InvalidateRequired +=
					(sender, eargs) => OnSizeChanged();
				e.CursorChangeRequired +=
					(sender, eargs) => Cursor = eargs.Cursor;
			}
			_cursor = Cursors.Default;
			_sizes = new Dictionary<Element, Size>(_elements.Length);
			_hoverElement = new TrackingService(OnHoverChanged);
		}

		public Revision Revision
		{
			get { return _revision; }
			set
			{
				if(_revision != value)
				{
					if(_revision != null)
					{
						_revision.Author.Avatar.Updated -= OnAuthorAvatarUpdated;
						_revision.References.Changed -= OnReferenceListChanged;
					}
					_revision = value;
					_measuredWidth = 0;
					if(_revision != null)
					{
						_revision.Author.Avatar.Updated += OnAuthorAvatarUpdated;
						_revision.References.Changed += OnReferenceListChanged;
					}
				}
			}
		}

		public Cursor Cursor
		{
			get { return _cursor; }
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
			get
			{
				if(_style != null)
				{
					return _style;
				}
				else
				{
					return GitterApplication.Style;
				}
			}
			set
			{
				_style = value;
			}
		}

		private void OnAuthorAvatarUpdated(object sender, EventArgs e)
		{
			OnInvalidated(new Rectangle(0, 0, _measuredWidth, _measuredHeight));
		}

		private void OnReferenceListChanged(object sender, EventArgs e)
		{
			Size size;
			_sizes.TryGetValue(Element.References, out size);
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
					Size size;
					if(!_sizes.TryGetValue(_elements[i].Element, out size)) break;
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
					Size size;
					if(!_sizes.TryGetValue(_elements[i].Element, out size)) break;
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

		private void Measure(Graphics graphics, int width)
		{
			int h = 0;
			for(int i = 0; i < _elements.Length; ++i)
			{
				if(_elements[i].IsAvailableFor(_revision))
				{
					var s = _elements[i].Measure(graphics, _revision, width);
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
					var menu = _elements[index].CreateContextMenu(_revision);
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

		public Size OnMeasure(Graphics graphics, int width)
		{
			if(_revision == null) return Size.Empty;
			if(width < MinWidth) width = MinWidth;
			if(_measuredWidth != width) Measure(graphics, width);
			return new Size(width, _measuredHeight);
		}

		public void OnPaint(Graphics graphics, Rectangle bounds)
		{
			if(_revision == null) return;
			var width = bounds.Width;
			if(_measuredWidth != width) Measure(graphics, width);
			if(GitterApplication.IntegrationFeatures.Gravatar.IsEnabled)
			{
				var avatar = _revision.Author.Avatar;
				var image = avatar.Image;
				if(image == null)
				{
					avatar.BeginUpdate();
				}
				else
				{
					if(bounds.Width >= MinWidth + 70)
					{
						graphics.DrawImage(image, new Rectangle(bounds.Right - 64, bounds.Y + 4, 60, 60));
					}
				}
			}
			var elementBounds = bounds;
			for(int i = 0; i < _elements.Length; ++i)
			{
				var element = _elements[i];
				if(element.IsAvailableFor(Revision))
				{
					var size = _sizes[element.Element];
					var elementHeight = size.Height;
					if(elementHeight != 0)
					{
						elementBounds.Height = elementHeight;
						if(i == _hoverElement.Index)
						{
							Color trackColor1;
							Color trackColor2;
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
							var oldMode = graphics.SmoothingMode;
							graphics.SmoothingMode = SmoothingMode.None;
							using(var brush = new SolidBrush(trackColor1))
							{
								graphics.FillRectangle(brush, new Rectangle(elementBounds.X, elementBounds.Y, HeaderWidth, size.Height));
							}
							using(var brush = new SolidBrush(trackColor2))
							{
								graphics.FillRectangle(brush, new Rectangle(elementBounds.X + HeaderWidth, elementBounds.Y, size.Width - HeaderWidth, size.Height));
							}
							graphics.SmoothingMode = oldMode;
						}
						element.Paint(graphics, Revision, elementBounds);
						elementBounds.Y += elementHeight;
					}
				}
			}
		}
	}
}

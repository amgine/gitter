namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary><see cref="FlowPanel"/> which displays basic commit information: author, hash, date, subject, etc.</summary>
	public class RevisionHeaderPanel : FlowPanel
	{
		#region Constants

		private const int DefaultElementHeight = 16;
		private const int HeaderWidth = 70;
		private const int MinWidth = HeaderWidth + 295;

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

		private static readonly Brush HeaderTextBrush = SystemBrushes.GrayText;
		private static readonly Brush ContentTextBrush = SystemBrushes.WindowText;

		#endregion

		#region Elements

		protected enum Element
		{
			Hash,
			TreeHash,
			Date,
			Author,
			Committer,
			Subject,
			Body,
			References,
			Parents,
		}

		protected sealed class CursorChangedEventArgs : EventArgs
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
		protected interface IRevisionHeaderElement
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

		protected abstract class BaseElement : IRevisionHeaderElement
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

			protected static void PaintHeader(Graphics graphics, string header, Rectangle rect)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				var r1 = new Rectangle(rect.X, rect.Y, HeaderWidth - 4, DefaultElementHeight);
				r1.Y += GetYOffset(font);
				GitterApplication.TextRenderer.DrawText(
					graphics, header, font, HeaderTextBrush, r1, HeaderFormat);
			}

			protected static void DefaultPaint(Graphics graphics, string header, string content, Rectangle rect)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				DefaultPaint(graphics, font, header, content, rect);
			}

			protected static void DefaultPaint(Graphics graphics, string header, TextWithHyperlinks content, Rectangle rect)
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

			protected static void DefaultPaint(Graphics graphics, Font font, string header, string content, Rectangle rect)
			{
				var r1 = new Rectangle(rect.X, rect.Y, HeaderWidth - 4, DefaultElementHeight);
				var r2 = new Rectangle(rect.X + HeaderWidth, rect.Y, rect.Width - HeaderWidth, rect.Height);
				var headerFont = GitterApplication.FontManager.UIFont.Font;
				r1.Y += GetYOffset(headerFont);
				r2.Y += GetYOffset(font);
				GitterApplication.TextRenderer.DrawText(
					graphics, header, headerFont, HeaderTextBrush, r1, HeaderFormat);
				GitterApplication.TextRenderer.DrawText(
					graphics, content, font, ContentTextBrush, r2, ContentFormat);
			}

			protected static void DefaultPaint(Graphics graphics, Font font, string header, TextWithHyperlinks content, Rectangle rect)
			{
				var r1 = new Rectangle(rect.X, rect.Y, HeaderWidth - 4, DefaultElementHeight);
				var r2 = new Rectangle(rect.X + HeaderWidth, rect.Y, rect.Width - HeaderWidth, rect.Height);
				var headerFont = GitterApplication.FontManager.UIFont.Font;
				r1.Y += GetYOffset(headerFont);
				r2.Y += GetYOffset(font);
				GitterApplication.TextRenderer.DrawText(
					graphics, header, font, HeaderTextBrush, r1, HeaderFormat);
				content.Render(graphics, font, r2);
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

		protected sealed class HashElement : BaseElement
		{
			public override Element Element
			{
				get { return RevisionHeaderPanel.Element.Hash; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, revision.Name));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return Measure(graphics, TreeHashColumn.Font, revision.Name, width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				DefaultPaint(graphics, HashColumn.Font, Resources.StrHash.AddColon(), revision.Name, rect);
			}
		}

		protected sealed class TreeHashElement : BaseElement
		{
			public override Element Element
			{
				get { return RevisionHeaderPanel.Element.TreeHash; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				menu.Items.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, revision.TreeHash));
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				return Measure(graphics, TreeHashColumn.Font, revision.TreeHash, width);
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				DefaultPaint(graphics, TreeHashColumn.Font, Resources.StrTreeHash.AddColon(), revision.TreeHash, rect);
			}
		}

		protected sealed class ParentsElement : BaseElement
		{
			public override Element Element
			{
				get { return RevisionHeaderPanel.Element.Parents; }
			}

			public override ContextMenuStrip CreateContextMenu(Revision revision)
			{
				var menu = new ContextMenuStrip();
				var sb = new StringBuilder(41 * revision.Parents.Count);
				foreach(var p in revision.Parents)
				{
					sb.Append(p.Name);
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
						return Measure(graphics, HashColumn.Font, revision.Parents[0].Name, width);
					default:
						var sb = new StringBuilder(41 * revision.Parents.Count);
						bool first = true;
						for(int i = 0; i < revision.Parents.Count; ++i)
						{
							var p = revision.Parents[i];
							if(!first) sb.Append('\n');
							else first = false;
							sb.Append(p.Name);
						}
						return MeasureMultilineContent(graphics, HashColumn.Font, sb.ToString(), width);
				}
			}

			public override void Paint(Graphics graphics, Revision revision, Rectangle rect)
			{
				if(revision.Parents.Count == 1)
				{
					DefaultPaint(graphics, HashColumn.Font, Resources.StrParent.AddColon(), revision.Parents[0].Name, rect);
				}
				else
				{
					var sb = new StringBuilder(41 * revision.Parents.Count);
					foreach(var p in revision.Parents)
					{
						sb.Append(p.Name);
						sb.Append('\n');
					}
					DefaultPaint(graphics, HashColumn.Font, Resources.StrParents.AddColon(), sb.ToString(), rect);
				}
			}
		}

		protected sealed class AuthorElement : BaseElement
		{
			public override Element Element
			{
				get { return RevisionHeaderPanel.Element.Author; }
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

		protected sealed class CommitterElement : BaseElement
		{
			public override bool IsAvailableFor(Revision revision)
			{
				return revision.Author != revision.Committer;
			}

			public override Element Element
			{
				get { return RevisionHeaderPanel.Element.Committer; }
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

		protected sealed class DateElement : BaseElement
		{
			private DateFormat _dateFormat;

			public DateElement()
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
				get { return RevisionHeaderPanel.Element.Date; }
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

		protected sealed class SubjectElement : BaseElement
		{
			private TextWithHyperlinks _text;

			public override Element Element
			{
				get { return RevisionHeaderPanel.Element.Subject; }
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

		protected sealed class BodyElement : BaseElement
		{
			private TextWithHyperlinks _text;

			public override bool IsAvailableFor(Revision revision)
			{
				return !string.IsNullOrEmpty(revision.Body);
			}

			public override Element Element
			{
				get { return RevisionHeaderPanel.Element.Body; }
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

		protected sealed class ReferencesElement : BaseElement
		{
			public override bool IsAvailableFor(Revision revision)
			{
				lock(revision.RefsSyncRoot)
				{
					return revision.Refs.Count != 0;
				}
			}

			public override Element Element
			{
				get { return RevisionHeaderPanel.Element.References; }
			}

			public override Size Measure(Graphics graphics, Revision revision, int width)
			{
				var font = GitterApplication.FontManager.UIFont.Font;
				lock(revision.RefsSyncRoot)
				{
					if(revision.Refs.Count == 0) return Size.Empty;
					int offset = 0;
					foreach(var reference in revision.Refs.Values)
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

				PaintHeader(graphics, Resources.StrRefs.AddColon(), rect);
				var font = GitterApplication.FontManager.UIFont.Font;
				var r2 = new Rectangle(rect.X + HeaderWidth, rect.Y, rect.Width - HeaderWidth, rect.Height);
				r2.Y += GetYOffset(font);
				int offset = 0;
				using(var tagBrush = new SolidBrush(ColorScheme.TagBackColor))
				using(var localBrush = new SolidBrush(ColorScheme.LocalBranchBackColor))
				using(var remoteBrush = new SolidBrush(ColorScheme.RemoteBranchBackColor))
				{
					lock(revision.RefsSyncRoot)
					{
						foreach(var reference in revision.Refs.Values)
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

		private Revision _revision;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="RevisionHeaderPanel"/>.</summary>
		public RevisionHeaderPanel()
		{
			_elements = new IRevisionHeaderElement[]
			{
				new HashElement(),
				new ParentsElement(),
				new AuthorElement(),
				new CommitterElement(),
				new DateElement(),
				new SubjectElement(),
				new BodyElement(),
				new ReferencesElement(),
			};
			foreach(var e in _elements)
			{
				e.InvalidateRequired +=
					(sender, eargs) => Invalidate();
				e.CursorChangeRequired +=
					(sender, eargs) => FlowControl.Cursor = eargs.Cursor;
			}
			_sizes = new Dictionary<Element, Size>(_elements.Length);
			_hoverElement = new TrackingService(OnHoverChanged);
		}

		#endregion

		private void OnHoverChanged(TrackingEventArgs e)
		{
			var bounds = GetElementBounds(e.Index);
			if(!e.IsTracked)
			{
				_elements[e.Index].MouseLeave();
			}
			Invalidate(bounds);
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

		protected override void OnMouseMove(int x, int y)
		{
			base.OnMouseMove(x, y);

			int element = HitTest(x, y);
			_hoverElement.Track(element);
			if(element != -1)
			{
				var bounds = GetElementBounds(element);
				_elements[element].MouseMove(bounds, new Point(x, y));
			}
		}

		protected override void OnMouseLeave()
		{
			base.OnMouseLeave();
			_hoverElement.Drop();
		}

		/// <summary>Displayed <see cref="T:gitter.Git.Revision"/>.</summary>
		public Revision Revision
		{
			get { return _revision; }
			set
			{
				_revision = value;
				_measuredWidth = 0;
			}
		}

		protected override void OnFlowControlAttached()
		{
			_revision.Author.Avatar.Updated += OnAuthorAvatarUpdated;
			_revision.ReferenceListChanged += OnRefListChanged;
			base.OnFlowControlAttached();
		}

		protected override void OnFlowControlDetached()
		{
			_revision.Author.Avatar.Updated -= OnAuthorAvatarUpdated;
			_revision.ReferenceListChanged -= OnRefListChanged;
			base.OnFlowControlDetached();
		}

		private void OnAuthorAvatarUpdated(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		private void OnRefListChanged(object sender, EventArgs e)
		{
			Size size;
			_sizes.TryGetValue(Element.References, out size);
			bool norefs;
			lock(_revision.RefsSyncRoot)
			{
				norefs = _revision.Refs.Count == 0;
			}
			if((size.IsEmpty && !norefs) ||
				(!size.IsEmpty && norefs))
			{
				_measuredWidth = -1;
				InvalidateSize();
			}
			else
			{
				InvalidateSafe();
			}
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

		protected override void OnMouseDown(int x, int y, MouseButtons button)
		{
			base.OnMouseDown(x, y, button);

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
						ShowContextMenu(menu, x, y);
					}
				}
			}
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			var width = measureEventArgs.Width;
			if(_revision == null) return Size.Empty;
			if(width < MinWidth) width = MinWidth;
			if(_measuredWidth != width) Measure(measureEventArgs.Graphics, width);
			return new Size(width, _measuredHeight);
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			if(_revision == null) return;
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var width = rect.Width;
			if(_measuredWidth != width) Measure(graphics, width);
			var r = rect;
			if(RepositoryProvider.Integration.Gravatar.Enabled)
			{
				var avatar = _revision.Author.Avatar;
				var image = avatar.Image;
				if(image == null)
				{
					avatar.BeginUpdate();
				}
				else
				{
					if(r.Width >= MinWidth + 70)
					{
						graphics.DrawImage(image, new Rectangle(r.Right - 65, r.Y + 5, 60, 60));
					}
				}
			}
			for(int i = 0; i < _elements.Length; ++i)
			{
				if(_elements[i].IsAvailableFor(_revision))
				{
					var size = _sizes[_elements[i].Element];
					var h = size.Height;
					if(h != 0)
					{
						r.Height = h;
						if(i == _hoverElement.Index)
						{
							var sm = graphics.SmoothingMode;
							graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
							graphics.FillRectangle(Brushes.WhiteSmoke, new Rectangle(r.X, r.Y, HeaderWidth, size.Height));
							using(var b = new SolidBrush(Color.FromArgb(238, 238, 238)))
							{
								graphics.FillRectangle(b, new Rectangle(r.X + HeaderWidth, r.Y, size.Width - HeaderWidth, size.Height));
							}
							graphics.SmoothingMode = sm;
						}
						_elements[i].Paint(graphics, _revision, r);
						r.Y += h;
					}
				}
			}
		}
	}
}

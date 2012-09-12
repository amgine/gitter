namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Options;
	using gitter.Framework.Configuration;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Column for <see cref="CustomListBox"/>.</summary>
	public class CustomListBoxColumn : CustomListBoxHostedItem
	{
		#region Data

		private readonly int _id;

		private string _name;
		private int _width;
		private int _contentWidth;
		private ColumnSizeMode _sizeMode;
		private bool _isVisible;
		private int _left;
		private ToolStripDropDown _extender;

		private Font _contentFont;
		private Brush _contentBrush;
		private StringAlignment _contentAlignment;
		
		private Font _headerFont;
		private Brush _headerBrush;
		private StringAlignment _headerAlignment;

		#endregion

		#region Events

		public event EventHandler SizeModeChanged;

		public event EventHandler WidthChanged;

		public event EventHandler ContentFontChanged;

		public event EventHandler HeaderFontChanged;

		#endregion

		#region Static Data

		private static readonly Bitmap ImgColumnExtender = Resources.ImgColumnExtender;

		private static readonly Brush ExtenderHoveredBrush =
			new SolidBrush(Color.FromArgb(210, 229, 253));
		
		private static readonly Brush ExtenderBorderBrush =
			new LinearGradientBrush(Point.Empty, new Point(0, 23), Color.FromArgb(223, 234, 247), Color.FromArgb(238, 242, 249));
		
		private static readonly Pen ExtenderBorderPenHovered =
			new Pen(Color.FromArgb(215, 227, 241));

		#endregion

		#region Constants

		const int ExtenderButtonWidth = 16;
		const int ResizerProximity = 3;

		#endregion

		#region .ctor

		public CustomListBoxColumn(int id, string name, bool visible)
		{
			_id = id;
			_name = name;
			_isVisible = visible;
			_sizeMode = ColumnSizeMode.Sizeable;
			_contentWidth = -1;
			_contentAlignment = StringAlignment.Near;
			_headerAlignment = StringAlignment.Near;
			_contentBrush = SystemBrushes.WindowText;
			_headerBrush = SystemBrushes.GrayText;
		}

		public CustomListBoxColumn(int id, string name)
			: this(id, name, true)
		{
		}

		#endregion

		#region Properties

		public int Id
		{
			get { return _id; }
		}

		public ToolStripDropDown Extender
		{
			get { return _extender; }
			set { _extender = value; }
		}

		public string Name
		{
			get { return _name; }
			set
			{
				if(_name != value)
				{
					_name = value;
					Invalidate();
				}
			}
		}

		public int Left
		{
			get { return _left; }
			internal set { _left = value; }
		}

		public Font ContentFont
		{
			get
			{
				if(_contentFont != null)
				{
					return _contentFont;
				}
				if(IsAttachedToListBox)
				{
					return ListBox.Font;
				}
				return GitterApplication.FontManager.UIFont;
			}
			set
			{
				if(_contentFont != value)
				{
					_contentFont = value;
					ContentFontChanged.Raise(this);
				}
			}
		}

		public Brush ContentBrush
		{
			get { return _contentBrush; }
			set
			{
				if(_contentBrush != value)
				{
					_contentBrush = value;
					InvalidateContent();
				}
			}
		}

		public StringAlignment ContentAlignment
		{
			get { return _contentAlignment; }
			set
			{
				if(_contentAlignment != value)
				{
					_contentAlignment = value;
					InvalidateContent();
				}
			}
		}

		public Font HeaderFont
		{
			get
			{
				if(_headerFont != null)
				{
					return _headerFont;
				}
				if(IsAttachedToListBox)
				{
					return ListBox.Font;
				}
				return GitterApplication.FontManager.UIFont;
			}
			set
			{
				if(_headerFont != value)
				{
					_headerFont = value;
					Invalidate();
					HeaderFontChanged.Raise(this);
				}
			}
		}

		public Brush HeaderBrush
		{
			get { return _headerBrush; }
			set
			{
				if(_headerBrush != value)
				{
					_headerBrush = value;
					Invalidate();
				}
			}
		}

		public StringAlignment HeaderAlignment
		{
			get { return _headerAlignment; }
			set
			{
				if(_headerAlignment != value)
				{
					_headerAlignment = value;
					Invalidate();
				}
			}
		}

		public int Index
		{
			get
			{
				if(!IsAttachedToListBox) return -1;
				return ListBox.Columns.IndexOf(this);
			}
		}

		public CustomListBoxColumn PreviousVisibleColumn
		{
			get
			{
				if(!IsAttachedToListBox) return null;
				var index = ListBox.Columns.IndexOf(this);
				return ListBox.GetPrevVisibleColumn(index);
			}
		}

		public int PreviousVisibleColumnIndex
		{
			get
			{
				if(!IsAttachedToListBox) return -1;
				var index = ListBox.Columns.IndexOf(this);
				return ListBox.GetPrevVisibleColumnIndex(index);
			}
		}

		public CustomListBoxColumn NextVisibleColumn
		{
			get
			{
				if(!IsAttachedToListBox) return null;
				var index = ListBox.Columns.IndexOf(this);
				return ListBox.GetNextVisibleColumn(index);
			}
		}

		public int NextVisibleColumnIndex
		{
			get
			{
				if(!IsAttachedToListBox) return -1;
				var index = ListBox.Columns.IndexOf(this);
				return ListBox.GetNextVisibleColumnIndex(index);
			}
		}

		/// <summary>This column affects content of the column to the left.</summary>
		public virtual bool ExtendsToLeft
		{
			get { return false; }
		}

		/// <summary>This column affects content of the column to the right.</summary>
		public virtual bool ExtendsToRight
		{
			get { return false; }
		}

		public virtual int MinWidth
		{
			get { return 10; }
		}

		public ColumnSizeMode SizeMode
		{
			get { return _sizeMode; }
			set
			{
				if(_sizeMode != value)
				{
					_sizeMode = value;
					if(IsAttachedToListBox)
					{
						ListBox.ColumnLayoutChanged();
					}
					SizeModeChanged.Raise(this);
				}
			}
		}

		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				if(_isVisible != value)
				{
					_isVisible = value;
					if(IsAttachedToListBox)
					{
						ListBox.ColumnLayoutChanged();
					}
				}
			}
		}

		public int Width
		{
			get { return _width; }
			set
			{
				Verify.State.IsTrue(SizeMode != ColumnSizeMode.Fill);

				_width = value;
				if(IsAttachedToListBox)
				{
					ListBox.ColumnLayoutChanged();
				}
				WidthChanged.Raise(this);
			}
		}

		internal int ContentWidth
		{
			get { return _contentWidth; }
			set { _contentWidth = value; }
		}

		#endregion

		protected virtual Comparison<CustomListBoxItem> SortComparison
		{
			get { return null; }
		}

		protected override void OnListBoxDetached()
		{
			_contentWidth = -1;
			base.OnListBoxDetached();
		}

		public virtual void AutoSize()
		{
			AutoSize(MinWidth);
		}

		public virtual void AutoSize(int minWidth)
		{
			if(!IsAttachedToListBox) return;
			Verify.State.IsTrue(SizeMode != ColumnSizeMode.Fill);

			if(ListBox.Items.Count == 0)
			{
				return;
			}
			var w = ListBox.GetOptimalColumnWidth(this);
			if(w < minWidth) w = minWidth;
			Width = w;
		}

		internal void SetWidth(int width)
		{
			_width = width;
		}

		public void Invalidate()
		{
			if(IsAttachedToListBox && _isVisible)
			{
				ListBox.InvalidateColumn(this);
			}
		}

		public void InvalidateContent()
		{
			if(IsAttachedToListBox && _isVisible)
			{
				ListBox.InvalidateColumnContent(this);
			}
		}

		private static void PaintNormalBackground(ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var c1 = Color.FromArgb(223, 234, 247);
			var c2 = Color.FromArgb(255, 255, 255);
			var rc = new Rectangle(rect.Right - 1, 0, 1, rect.Height);
			using(var brush = new LinearGradientBrush(
				rc, c1, c2, LinearGradientMode.Vertical))
			{
				graphics.FillRectangle(brush, rc);
			}
		}

		private void PaintPressedBackground(ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var c1 = Color.FromArgb(192, 203, 217);
			var c2 = Color.FromArgb(246, 247, 248);
			var c3 = Color.FromArgb(193, 204, 218);
			var c4 = Color.FromArgb(215, 222, 231);
			var c5 = Color.FromArgb(235, 238, 242);
			using(var p = new Pen(c1))
			{
				var rc = rect;
				rc.Y -= 1;
				rc.X += 1;
				rc.Width -= 2;
				graphics.DrawRectangle(p, rc);
			}
			using(var b = new SolidBrush(c2))
			{
				var rc = rect;
				rc.Y += 3;
				rc.X += 2;
				rc.Width -= 4;
				rc.Height -= 4;
				graphics.FillRectangle(b, rc);
			}
			using(var p = new Pen(c3))
			{
				var rc = rect;
				graphics.DrawLine(p, rc.X + 1, rc.Y + 0, rc.Right - 2, rc.Y + 0);
			}
			using(var p = new Pen(c4))
			{
				var rc = rect;
				graphics.DrawLine(p, rc.X + 1, rc.Y + 1, rc.Right - 2, rc.Y + 1);
			}
			using(var p = new Pen(c5))
			{
				var rc = rect;
				graphics.DrawLine(p, rc.X + 1, rc.Y + 2, rc.Right - 2, rc.Y + 2);
			}
			if(_extender != null)
			{
				if(rect.Width > ExtenderButtonWidth)
				{
					graphics.FillRectangle(ExtenderBorderBrush, rect.Right - ExtenderButtonWidth - 0.5f, rect.Y, 1, rect.Height - 1);
					graphics.DrawImage(ImgColumnExtender, rect.Right - ExtenderButtonWidth + 4, rect.Y + 9, 7, 4);
				}
			}
		}

		private void PaintHoverBackground(ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var c1 = Color.FromArgb(227, 232, 238);
			var c2 = Color.FromArgb(241, 245, 251);
			using(var p = new Pen(c1))
			{
				var rc = rect;
				rc.Y -= 1;
				rc.Width -= 1;
				graphics.DrawRectangle(p, rc);
			}
			using(var b = new SolidBrush(c2))
			{
				var rc = rect;
				rc.X += 2;
				rc.Y += 1;
				rc.Width -= 4;
				rc.Height -= 3;
				graphics.FillRectangle(b, rc);
			}
			if(_extender != null)
			{
				if(rect.Width > ExtenderButtonWidth)
				{
					if(paintEventArgs.HoveredPart == ColumnHitTestResults.Extender)
					{
						graphics.FillRectangle(ExtenderHoveredBrush, rect.Right - ExtenderButtonWidth + 1.5f, rect.Y + 1.5f, ExtenderButtonWidth - 4, rect.Height - 4);
						graphics.DrawRectangle(ExtenderBorderPenHovered, rect.Right - ExtenderButtonWidth, 0, ExtenderButtonWidth - 1, rect.Height - 1);
					}
					else
					{
						graphics.FillRectangle(ExtenderBorderBrush, rect.Right - ExtenderButtonWidth - 0.5f, rect.Y, 1, rect.Height);
					}
					graphics.DrawImage(ImgColumnExtender, rect.Right - ExtenderButtonWidth + 4, rect.Y + 9, 7, 4);
				}
			}
		}

		protected override void OnPaintBackground(ItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.State)
			{
				case ItemState.None:
					PaintNormalBackground(paintEventArgs);
					break;
				case ItemState.Pressed:
					PaintPressedBackground(paintEventArgs);
					break;
				default:
					PaintHoverBackground(paintEventArgs);
					break;
			}
		}

		protected override void OnPaintContent(ItemPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			var font = HeaderFont;

			ItemPaintEventArgs.PrepareContentRectangle(ref rect);
			paintEventArgs.PrepareTextRectangle(font, font, ref rect);
			if(_extender != null && ((paintEventArgs.State & (ItemState.Hovered | ItemState.Pressed)) != ItemState.None))
			{
				rect.Width -= ExtenderButtonWidth;
				if(rect.Width <= 0) return;
			}
			StringFormat format;
			switch(_headerAlignment)
			{
				case StringAlignment.Near:
					format = GitterApplication.TextRenderer.LeftAlign;
					break;
				case StringAlignment.Far:
					format = GitterApplication.TextRenderer.RightAlign;
					break;
				case StringAlignment.Center:
					format = GitterApplication.TextRenderer.CenterAlign;
					break;
				default:
					format = GitterApplication.TextRenderer.LeftAlign;
					break;
			}
			GitterApplication.TextRenderer
				.DrawText(graphics, _name, font, _headerBrush, rect, format);
		}

		protected override int OnHitTest(int x, int y)
		{
			if(x < ResizerProximity)
			{
				if(_sizeMode == ColumnSizeMode.Sizeable)
				{
					if(PreviousVisibleColumn != null)
					{
						return ColumnHitTestResults.LeftResizer;
					}
				}
				int id = -1;
				int pid = -1;
				for(int i = 0; i < ListBox.Columns.Count; ++i)
				{
					if(ListBox.Columns[i] == this)
					{
						id = i;
						break;
					}
					if(ListBox.Columns[i].IsVisible)
					{
						pid = i;
					}
				}
				if(pid != -1)
				{
					if(ListBox.Columns[pid].SizeMode == ColumnSizeMode.Sizeable)
					{
						return ColumnHitTestResults.LeftResizer;
					}
				}
			}
			else if(_width - x < ResizerProximity)
			{
				if(_sizeMode == ColumnSizeMode.Sizeable)
				{
					if(NextVisibleColumn == null)
					{
						if(!ListBox.Columns.HasFillModeVisibleColumn)
						{
							return ColumnHitTestResults.RightResizer;
						}
					}
					else
					{
						return ColumnHitTestResults.RightResizer;
					}
				}
				int id = -1;
				int pid = -1;
				for(int i = ListBox.Columns.Count - 1; i >= 0; --i)
				{
					if(ListBox.Columns[i] == this)
					{
						id = i;
						break;
					}
					if(ListBox.Columns[i].IsVisible)
					{
						pid = i;
					}
				}
				if(pid != -1)
				{
					if(ListBox.Columns[pid].SizeMode == ColumnSizeMode.Sizeable)
					{
						return ColumnHitTestResults.RightResizer;
					}
				}
			}
			if(_extender != null)
			{
				if(_width - x < ExtenderButtonWidth)
				{
					return ColumnHitTestResults.Extender;
				}
			}
			return ColumnHitTestResults.Default;
		}

		internal void NotifyClick()
		{
			OnClick();
		}

		protected virtual void OnClick()
		{
			var comparison = SortComparison;
			if(comparison == null) return;
			if(ListBox.Items.Comparison == comparison)
			{
				switch(ListBox.Items.SortOrder)
				{
					case SortOrder.Ascending:
						ListBox.Items.SortOrder = SortOrder.Descending;
						break;
					default:
						ListBox.Items.SortOrder = SortOrder.Ascending;
						break;
				}
			}
			else
			{
				ListBox.Items.SortOrder = SortOrder.None;
				ListBox.Items.Comparison = comparison;
				ListBox.Items.SortOrder = SortOrder.Ascending;
			}
		}

		public virtual string IdentificationString
		{
			get { return "Column" + _id.ToString(System.Globalization.CultureInfo.InvariantCulture); }
		}

		protected virtual void SaveMoreTo(Section section)
		{
		}

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			section.SetValue("Visible", _isVisible);
			if(_sizeMode == ColumnSizeMode.Sizeable)
			{
				section.SetValue("Width", _width);
			}
			SaveMoreTo(section);
		}

		protected virtual void LoadMoreFrom(Section section)
		{
		}

		public void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			IsVisible = section.GetValue("Visible", _isVisible);
			if(_sizeMode != ColumnSizeMode.Fill)
			{
				Width = section.GetValue("Width", _width);
			}
			LoadMoreFrom(section);
		}

		public override string ToString()
		{
			return IdentificationString;
		}
	}
}

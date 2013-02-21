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

		#region Constants

		public const int ExtenderButtonWidth = 16;
		const int ResizerProximity = 3;

		#endregion

		#region .ctor

		public CustomListBoxColumn(int id, string name, bool visible)
		{
			_id					= id;
			_name				= name;
			_isVisible			= visible;
			_sizeMode			= ColumnSizeMode.Sizeable;
			_contentWidth		= -1;
			_contentAlignment	= StringAlignment.Near;
			_headerAlignment	= StringAlignment.Near;
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
			get
			{
				if(_contentBrush != null)
				{
					return _contentBrush;
				}
				else
				{
					if(IsAttachedToListBox)
					{
						return ListBox.Renderer.ForegroundBrush;
					}
					else
					{
						return CustomListBoxManager.Renderer.ForegroundBrush;
					}
				}
			}
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
			get
			{
				if(_headerBrush != null)
				{
					return _headerBrush;
				}
				else
				{
					if(IsAttachedToListBox)
					{
						return ListBox.Renderer.ColumnHeaderForegroundBrush;
					}
					else
					{
						return CustomListBoxManager.Renderer.ColumnHeaderForegroundBrush;
					}
				}
			}
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

		protected override void OnPaintBackground(ItemPaintEventArgs paintEventArgs)
		{
			ListBox.Renderer.OnPaintColumnBackground(this, paintEventArgs);
		}

		protected override void OnPaintContent(ItemPaintEventArgs paintEventArgs)
		{
			ListBox.Renderer.OnPaintColumnContent(this, paintEventArgs);
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

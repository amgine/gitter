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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Configuration;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Column for <see cref="CustomListBox"/>.</summary>
	public class CustomListBoxColumn : CustomListBoxHostedItem
	{
		#region Data

		private string _name;
		private int _width;
		private ColumnSizeMode _sizeMode;
		private bool _isAvailable = true;
		private bool _isVisible;
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

		public event EventHandler StyleChanged;

		protected virtual void OnStyleChanged(EventArgs e)
			=> StyleChanged?.Invoke(this, e);

		#endregion

		#region Constants

		public const int ExtenderButtonWidth = 16;
		const int ResizerProximity = 3;

		#endregion

		#region .ctor

		public CustomListBoxColumn(int id, string name, bool visible)
		{
			Id                = id;
			_name             = name;
			_isVisible        = visible;
			_sizeMode         = ColumnSizeMode.Sizeable;
			ContentWidth      = -1;
			_contentAlignment = StringAlignment.Near;
			_headerAlignment  = StringAlignment.Near;
		}

		public CustomListBoxColumn(int id, string name)
			: this(id, name, true)
		{
		}

		#endregion

		#region Properties

		public int Id { get; }

		public IGitterStyle Style => ListBox?.Style ?? GitterApplication.Style;

		public ToolStripDropDown Extender { get; set; }

		public string Name
		{
			get => _name;
			set
			{
				if(_name != value)
				{
					_name = value;
					Invalidate();
				}
			}
		}

		public int Left { get; internal set; }

		public Font ContentFont
		{
			get
			{
				if(_contentFont != null) return _contentFont;
				if(IsAttachedToListBox)  return ListBox.Font;
				return GitterApplication.FontManager.UIFont;
			}
			set
			{
				if(_contentFont != value)
				{
					_contentFont = value;
					ContentFontChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public Brush ContentBrush
		{
			get
			{
				if(_contentBrush != null) return _contentBrush;
				if(IsAttachedToListBox)   return ListBox.Renderer.ForegroundBrush;
				return CustomListBoxManager.Renderer.ForegroundBrush;
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
			get => _contentAlignment;
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
				if(_headerFont is not null) return _headerFont;
				if(IsAttachedToListBox) return ListBox.Font;
				return GitterApplication.FontManager.UIFont;
			}
			set
			{
				if(_headerFont != value)
				{
					_headerFont = value;
					Invalidate();
					HeaderFontChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public Brush HeaderBrush
		{
			get
			{
				if(_headerBrush is not null) return _headerBrush;
				if(IsAttachedToListBox)  return ListBox.Renderer.ColumnHeaderForegroundBrush;
				return CustomListBoxManager.Renderer.ColumnHeaderForegroundBrush;
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
			get => _headerAlignment;
			set
			{
				if(_headerAlignment != value)
				{
					_headerAlignment = value;
					Invalidate();
				}
			}
		}

		public int Index => IsAttachedToListBox
			? ListBox.Columns.IndexOf(this)
			: -1;

		public CustomListBoxColumn PreviousVisibleColumn
		{
			get
			{
				if(!IsAttachedToListBox) return null;
				var index = ListBox.Columns.IndexOf(this);
				return ListBox.GetPrevVisibleColumn(index);
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

		/// <summary>This column affects content of the column to the left.</summary>
		public virtual bool ExtendsToLeft => false;

		/// <summary>This column affects content of the column to the right.</summary>
		public virtual bool ExtendsToRight => false;

		public virtual int MinWidth => 10;

		public ColumnSizeMode SizeMode
		{
			get => _sizeMode;
			set
			{
				if(_sizeMode != value)
				{
					_sizeMode = value;
					if(IsAttachedToListBox)
					{
						ListBox.NotifyColumnLayoutChanged();
					}
					SizeModeChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public bool IsAvailable
		{
			get => _isAvailable;
			set
			{
				if(_isAvailable != value)
				{
					_isAvailable = value;
					if(IsAttachedToListBox && IsVisible)
					{
						ListBox.NotifyColumnLayoutChanged();
					}
				}
			}
		}

		public bool IsVisible
		{
			get => _isVisible;
			set
			{
				if(_isVisible != value)
				{
					_isVisible = value;
					if(IsAttachedToListBox && IsAvailable)
					{
						ListBox.NotifyColumnLayoutChanged();
					}
				}
			}
		}

		public int Width
		{
			get => _width;
			set
			{
				Verify.State.IsTrue(SizeMode != ColumnSizeMode.Fill);

				_width = value;
				if(IsAttachedToListBox)
				{
					ListBox.NotifyColumnLayoutChanged();
				}
				WidthChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		internal int ContentWidth { get; set; }

		#endregion

		protected virtual Comparison<CustomListBoxItem> SortComparison => null;

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			ListBox.StyleChanged += OnListBoxStyleChanged;
			if(ListBox.Style != GitterApplication.Style)
			{
				OnStyleChanged(EventArgs.Empty);
			}
		}

		protected override void OnListBoxDetached()
		{
			ListBox.StyleChanged -= OnListBoxStyleChanged;
			ContentWidth = -1;
			base.OnListBoxDetached();
		}

		private void OnListBoxStyleChanged(object sender, EventArgs e)
			=> OnStyleChanged(EventArgs.Empty);

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

		protected virtual void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
		}

		protected virtual Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			return Size.Empty;
		}

		public void PaintSubItem(SubItemPaintEventArgs subItemPaintEventArgs)
		{
			OnPaintSubItem(subItemPaintEventArgs);
		}

		public Size MeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			return OnMeasureSubItem(measureEventArgs);
		}

		protected override int OnHitTest(int x, int y)
		{
			var proximity = ResizerProximity;
			if(ListBox is not null)
			{
				proximity = proximity * ListBox.DeviceDpi / 96;
			}
			if(x < proximity)
			{
				if(_sizeMode == ColumnSizeMode.Sizeable)
				{
					if(PreviousVisibleColumn is not null)
					{
						return ColumnHitTestResults.LeftResizer;
					}
				}
				int id = -1;
				int pid = -1;
				for(int i = 0; i < ListBox.Columns.Count; ++i)
				{
					var column = ListBox.Columns[i];
					if(column == this)
					{
						id = i;
						break;
					}
					if(column.IsVisible && column.IsAvailable)
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
			else if(_width - x < proximity)
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
					var column = ListBox.Columns[i];
					if(column == this)
					{
						id = i;
						break;
					}
					if(column.IsVisible && column.IsAvailable)
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
			if(Extender is not null)
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
			if(comparison is null) return;
			if(ListBox.Items.Comparison == comparison)
			{
				ListBox.Items.SortOrder = ListBox.Items.SortOrder switch
				{
					SortOrder.Ascending => SortOrder.Descending,
					_ => SortOrder.Ascending,
				};
			}
			else
			{
				ListBox.Items.SortOrder  = SortOrder.None;
				ListBox.Items.Comparison = comparison;
				ListBox.Items.SortOrder  = SortOrder.Ascending;
			}
		}

		public virtual string IdentificationString
			=> "Column" + Id.ToString(System.Globalization.CultureInfo.InvariantCulture);

		protected virtual void SaveMoreTo(Section section)
		{
		}

		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, nameof(section));

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
			Verify.Argument.IsNotNull(section, nameof(section));

			IsVisible = section.GetValue("Visible", _isVisible);
			if(_sizeMode != ColumnSizeMode.Fill)
			{
				Width = section.GetValue("Width", _width);
			}
			LoadMoreFrom(section);
		}

		public override string ToString() => IdentificationString;
	}
}

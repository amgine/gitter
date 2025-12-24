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

namespace gitter.Framework.Options;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Framework.Properties.Resources;

[ToolboxItem(false)]
internal partial class FontsPage : PropertyPage, IExecutableDialog
{
	sealed class FontFamiliesListBox : CustomListBox
	{
	}

	sealed class FontStylesListBox : CustomListBox
	{
	}

	sealed class FontFamilyListItem(string name) : CustomListBoxItem<string>(name)
	{
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(paintEventArgs.Column.Id == 0)
			{
				paintEventArgs.PaintText(DataContext);
			}
			else
			{
				base.OnPaintSubItem(paintEventArgs);
			}
		}
	}

	sealed class FontStyleListItem(FontStyle style) : CustomListBoxItem<FontStyle>(style)
	{
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(paintEventArgs.Column.Id == 0)
			{
				paintEventArgs.PaintText(DataContext.ToString());
			}
			else
			{
				base.OnPaintSubItem(paintEventArgs);
			}
		}
	}

	sealed class FontPicker : CustomObjectPicker<FontFamiliesListBox, FontFamilyListItem, string?>
	{
		public FontPicker()
		{
			DropDownControl.Columns.Clear();
			DropDownControl.Columns.Add(new CustomListBoxColumn(0, "Name") { SizeMode = ColumnSizeMode.Fill });
		}

		protected override string? GetValue(FontFamilyListItem item)
			=> item?.DataContext;

		public void LoadData(FontFamily[] families)
		{
			if(families is { Length: not 0 })
			{
				DropDownControl.BeginUpdate();
				foreach(var f in families)
				{
					DropDownControl.Items.Add(new FontFamilyListItem(f.Name));
				}
				DropDownControl.EndUpdate();
			}
			else
			{
				DropDownControl.Items.Clear();
			}
		}
	}

	sealed class FontStylePicker : CustomObjectPicker<FontStylesListBox, FontStyleListItem, FontStyle>
	{
		public FontStylePicker()
		{
			DropDownControl.Columns.Clear();
			DropDownControl.Columns.Add(new CustomListBoxColumn(0, "Name") { SizeMode = ColumnSizeMode.Fill });
		}

		protected override FontStyle GetValue(FontStyleListItem item)
			=> item?.DataContext ?? FontStyle.Regular;

		public void LoadData(List<FontStyle> styles)
		{
			if(styles is { Count: not 0 })
			{
				DropDownControl.BeginUpdate();
				foreach(var s in styles)
				{
					DropDownControl.Items.Add(new FontStyleListItem(s));
				}
				DropDownControl.EndUpdate();
			}
			else
			{
				DropDownControl.Items.Clear();
			}
		}
	}

	readonly struct DialogControls
	{
		public readonly LabelControl _lblFonts;
		public readonly FontsListBox _lstFonts;
		public readonly LabelControl _lblName;
		public readonly LabelControl _lblSize;
		public readonly LabelControl _lblStyle;
		public readonly Label _lblSample;
		public readonly FontPicker _cmbFonts;
		public readonly TextBoxDecoratorWithUpDown _numSize;
		public readonly FontStylePicker _cmbStyle;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_lblFonts = new();
			_lstFonts = new() { Style = style, ItemActivation = Framework.Controls.ItemActivation.SingleClick, AllowColumnReorder = false };
			_lblName = new();
			_lblSize = new();
			_lblStyle = new();
			_lblSample = new()
			{
				TextAlign   = ContentAlignment.MiddleCenter,
				BorderStyle = BorderStyle.FixedSingle,
			};
			_cmbFonts = new();
			_numSize = new(new());
			_cmbStyle = new();
		}

		public void Localize()
		{
			_lblFonts.Text  = Resources.StrFonts.AddColon();
			_lblName.Text   = Resources.StrName.AddColon();
			_lblSize.Text   = Resources.StrSize.AddColon();
			_lblStyle.Text  = Resources.StrStyle.AddColon();
			_lblSample.Text = Resources.StrSample;
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						/* 0 */ LayoutConstants.LabelRowHeight,
						/* 1 */ LayoutConstants.LabelRowSpacing,
						/* 2 */ SizeSpec.Everything(),
						/* 3 */ SizeSpec.Absolute(4),
						/* 4 */ LayoutConstants.TextInputRowHeight,
						/* 5 */ LayoutConstants.TextInputRowHeight,
						/* 6 */ SizeSpec.Absolute(4),
						/* 7 */ SizeSpec.Absolute(50),
					],
					content:
					[
						new GridContent(new ControlContent(_lblFonts, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_lstFonts, marginOverride: LayoutConstants.NoMargin), row: 2),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(80),
								SizeSpec.Everything(),
								SizeSpec.Absolute(8),
								SizeSpec.Absolute(60),
								SizeSpec.Absolute(80),
							],
							rows:
							[
								LayoutConstants.TextInputRowHeight,
								LayoutConstants.TextInputRowHeight,
							],
							content:
							[
								new GridContent(new ControlContent(_lblName,  marginOverride: LayoutConstants.TextBoxLabelMargin), row: 0),
								new GridContent(new ControlContent(_cmbFonts, marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
								new GridContent(new ControlContent(_lblSize,  marginOverride: LayoutConstants.TextBoxLabelMargin), row: 0, column: 3),
								new GridContent(new ControlContent(_numSize,  marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 4),
								new GridContent(new ControlContent(_lblStyle, marginOverride: LayoutConstants.TextBoxLabelMargin), row: 1),
								new GridContent(new ControlContent(_cmbStyle, marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),
							]), row: 4, rowSpan: 2),
						new GridContent(new ControlContent(_lblSample, marginOverride: LayoutConstants.NoMargin), row: 7),
					]),
			};

			var tabIndex = 0;
			_lblFonts.TabIndex  = tabIndex++;
			_lstFonts.TabIndex  = tabIndex++;
			_lblName.TabIndex   = tabIndex++;
			_cmbFonts.TabIndex  = tabIndex++;
			_lblStyle.TabIndex  = tabIndex++;
			_cmbStyle.TabIndex  = tabIndex++;
			_lblSize.TabIndex   = tabIndex++;
			_numSize.TabIndex   = tabIndex++;
			_lblSample.TabIndex = tabIndex++;

			_lblFonts.Parent  = parent;
			_lstFonts.Parent  = parent;
			_lblName.Parent   = parent;
			_cmbFonts.Parent  = parent;
			_lblStyle.Parent  = parent;
			_cmbStyle.Parent  = parent;
			_lblSize.Parent   = parent;
			_numSize.Parent   = parent;
			_lblSample.Parent = parent;
		}
	}

	public static readonly new Guid Guid = new("EF348DA8-DC3E-4E23-A1EF-A5F9E37DA1E2");
	private readonly DialogControls _controls;
	private FontFamily[]? _families;
	private bool _blockSampleUpdate;

	public FontsPage()
		: base(Guid)
	{
		Name = nameof(FontsPage);
		Text = Resources.StrFonts;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		EnableControls(false);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._lstFonts.SelectionChanged     += OnFontsSelectionChanged;
		_controls._cmbFonts.SelectedValueChanged += OnFontFamilySelected;
		_controls._cmbStyle.SelectedValueChanged += OnFontStyleChanged;
		_controls._numSize.ValueChanged          += OnFontSizeChanged;
	}

	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(448, 375));

	protected override bool ScaleChildren => false;

	private void OnFontsSelectionChanged(object? sender, EventArgs e)
	{
		var selItem = _controls._lstFonts.SelectedItems.FirstOrDefault() as FontListItem;
		if(selItem != null)
		{
			_blockSampleUpdate = true;
			var font = selItem.Font;
			try
			{
				_controls._cmbFonts.SelectedValue = font.FontFamily.Name;
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
			}
			try
			{
				_controls._numSize.Value = (int)Math.Round(font.Size);
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
			}
			try
			{
				_controls._cmbStyle.SelectedValue = font.Style;
			}
			catch(Exception exc) when(!exc.IsCritical)
			{
			}
			UpdateSample();
			EnableControls(true);
			_blockSampleUpdate = false;
		}
		else
		{
			EnableControls(false);
			_controls._lblSample.Text = string.Empty;
		}
	}

	private void EnableControls(bool enable)
	{
		_controls._lblName.Enabled   = enable;
		_controls._lblSize.Enabled   = enable;
		_controls._lblStyle.Enabled  = enable;
		_controls._cmbFonts.Enabled  = enable;
		_controls._numSize.Enabled   = enable;
		_controls._cmbStyle.Enabled  = enable;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		_families = FontFamily.Families;
		_controls._cmbFonts.LoadData(_families);
	}

	private FontFamily? FindFamily(string? name)
	{
		if(name is null || _families is null) return default;
		foreach(var f in _families)
		{
			if(f.Name == name) return f;
		}
		return default;
	}

	private static readonly FontStyle[] FontStyles =
		[
			FontStyle.Regular,
			FontStyle.Bold,
			FontStyle.Italic,
			FontStyle.Bold | FontStyle.Italic,
		];

	private static List<FontStyle> GetSupportedFontStyles(FontFamily fontFamily)
	{
		Verify.Argument.IsNotNull(fontFamily);

		var styles = new List<FontStyle>(FontStyles.Length);
		for(int i = 0; i < FontStyles.Length; ++i)
		{
			if(fontFamily.IsStyleAvailable(FontStyles[i]))
			{
				styles.Add(FontStyles[i]);
			}
		}
		return styles;
	}

	private void SetAvailableStyles()
	{
		var f = FindFamily(_controls._cmbFonts.SelectedValue);
		_controls._cmbStyle.DropDownItems.Clear();
		if(f is not null)
		{
			_controls._cmbStyle.LoadData(GetSupportedFontStyles(f));
		}
		if(_controls._cmbStyle.DropDownItems.Count != 0)
		{
			_controls._cmbStyle.SelectedValue = ((FontStyleListItem)_controls._cmbStyle.DropDownItems[0]).DataContext;
		}
	}

	private FontStyle GetStyle()
	{
		return _controls._cmbStyle.SelectedValue;
	}

	private Font? GetFont()
	{
		var f = FindFamily(_controls._cmbFonts.SelectedValue);
		if(f == null) return default;
		var size = (float)_controls._numSize.Value;
		var style = GetStyle();

		return new Font(f, size, style, GraphicsUnit.Point);
	}

	private void UpdateSample()
	{
		var f = GetFont();
		if(f != null)
		{
			_controls._lblSample.Text = Resources.StrSample;
			_controls._lblSample.Font = f;

			var selItem = _controls._lstFonts.SelectedItems.FirstOrDefault() as FontListItem;
			if(selItem != null)
			{
				selItem.Font = f;
			}
		}
		else
		{
			_controls._lblSample.Text = string.Empty;
		}
	}

	private void OnFontSizeChanged(object? sender, EventArgs e)
	{
		if(!_blockSampleUpdate)
		{
			UpdateSample();
		}
	}

	private void OnFontStyleChanged(object? sender, EventArgs e)
	{
		if(!_blockSampleUpdate)
		{
			UpdateSample();
		}
	}

	private void OnFontFamilySelected(object? sender, EventArgs e)
	{
		SetAvailableStyles();
		OnFontSizeChanged(sender, e);
	}

	private void _lstFonts_ItemActivated(object? sender, ItemEventArgs e)
	{
		_blockSampleUpdate = true;
		var sf = ((FontListItem)e.Item).DataContext;
		_controls._cmbFonts.SelectedValue = sf.Font.FontFamily.Name;
		_controls._numSize.Value = (int)Math.Round(sf.Font.Size);
		_controls._cmbStyle.SelectedValue = sf.Font.Style;
		UpdateSample();
		_blockSampleUpdate = false;
	}

	public bool Execute()
	{
		foreach(FontListItem item in _controls._lstFonts.Items)
		{
			item.DataContext.Font = item.Font;
		}
		return true;
	}
}

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

namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Framework.Properties.Resources;

	[ToolboxItem(false)]
	internal partial class FontsPage : PropertyPage, IExecutableDialog
	{
		public static readonly new Guid Guid = new Guid("EF348DA8-DC3E-4E23-A1EF-A5F9E37DA1E2");
		private FontFamily[] _families;
		private bool _blockSampleUpdate;

		public FontsPage()
			: base(Guid)
		{
			InitializeComponent();

			Text = Resources.StrFonts;

			_lstFonts.Style = GitterApplication.DefaultStyle;
			_lblFonts.Text = Resources.StrFonts.AddColon();
			_lblName.Text = Resources.StrName.AddColon();
			_lblSize.Text = Resources.StrSize.AddColon();
			_lblStyle.Text = Resources.StrStyle.AddColon();

			_lblSample.Text = Resources.StrSample;

			_lstFonts.SelectionChanged += OnFontsSelectionChanged;
		}

		private void OnFontsSelectionChanged(object sender, EventArgs e)
		{
			var selItem = _lstFonts.SelectedItems.FirstOrDefault() as FontListItem;
			if(selItem != null)
			{
				_blockSampleUpdate = true;
				var font = selItem.Font;
				try
				{
					_cmbFonts.SelectedItem = font.FontFamily.Name;
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
				try
				{
					_numSize.Value = (decimal)font.Size;
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
				try
				{
					_cmbStyle.SelectedItem = font.Style;
				}
				catch(Exception exc)
				{
					if(exc.IsCritical())
					{
						throw;
					}
				}
				UpdateSample();
				_pnlSelectedFont.Enabled = true;
				_blockSampleUpdate = false;
			}
			else
			{
				_pnlSelectedFont.Enabled = false;
				_lblSample.Text = string.Empty;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			_families = FontFamily.Families;
			if(_families != null && _families.Length != 0)
			{
				_cmbFonts.BeginUpdate();
				foreach(var f in _families)
				{
					_cmbFonts.Items.Add(f.Name);
				}
				_cmbFonts.EndUpdate();
			}
		}

		private FontFamily FindFamily(string name)
		{
			foreach(var f in _families)
			{
				if(f.Name == name) return f;
			}
			return null;
		}

		private static readonly FontStyle[] FontStyles = new[]
			{
				FontStyle.Regular,
				FontStyle.Bold,
				FontStyle.Italic,
				FontStyle.Bold | FontStyle.Italic,
			};

		private static List<FontStyle> GetSupportedFontStyles(FontFamily fontFamily)
		{
			Verify.Argument.IsNotNull(fontFamily, "fontFamily");

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
			var f = FindFamily(_cmbFonts.Text);
			_cmbStyle.BeginUpdate();
			_cmbStyle.Items.Clear();
			if(f != null)
			{
				foreach(var style in GetSupportedFontStyles(f))
				{
					_cmbStyle.Items.Add(style);
				}
				if(_cmbStyle.Items.Count != 0)
				{
					_cmbStyle.SelectedIndex = 0;
				}
			}
			_cmbStyle.EndUpdate();
		}

		private FontStyle GetStyle()
		{
			if((_cmbStyle.Items.Count == 0) ||
				(_cmbStyle.SelectedIndex < 0) ||
				(_cmbStyle.SelectedIndex >= _cmbStyle.Items.Count))
			{
				return FontStyle.Regular;
			}
			return (FontStyle)_cmbStyle.SelectedItem;
		}

		private Font GetFont()
		{
			var f = FindFamily(_cmbFonts.Text);
			if(f == null) return null;
			var size = (float)_numSize.Value;
			var style = GetStyle();

			return new Font(f, size, style, GraphicsUnit.Point);
		}

		private void UpdateSample()
		{
			var f = GetFont();
			if(f != null)
			{
				_lblSample.Text = Resources.StrSample;
				_lblSample.Font = f;

				var selItem = _lstFonts.SelectedItems.FirstOrDefault() as FontListItem;
				if(selItem != null)
				{
					selItem.Font = f;
				}
			}
			else
			{
				_lblSample.Text = string.Empty;
			}
		}

		private void OnFontSizeChanged(object sender, EventArgs e)
		{
			if(!_blockSampleUpdate)
			{
				UpdateSample();
			}
		}

		private void OnFontStyleChanged(object sender, EventArgs e)
		{
			if(!_blockSampleUpdate)
			{
				UpdateSample();
			}
		}

		private void OnFontFamilySelected(object sender, EventArgs e)
		{
			SetAvailableStyles();
			OnFontSizeChanged(sender, e);
		}

		private void _lstFonts_ItemActivated(object sender, ItemEventArgs e)
		{
			_blockSampleUpdate = true;
			var sf = ((FontListItem)e.Item).DataContext;
			_cmbFonts.SelectedItem = sf.Font.FontFamily.Name;
			_numSize.Value = (decimal)sf.Font.Size;
			_cmbStyle.SelectedItem = sf.Font.Style;
			UpdateSample();
			_blockSampleUpdate = false;
		}

		public bool Execute()
		{
			foreach(FontListItem item in _lstFonts.Items)
			{
				item.DataContext.Font = item.Font;
			}
			return true;
		}
	}
}

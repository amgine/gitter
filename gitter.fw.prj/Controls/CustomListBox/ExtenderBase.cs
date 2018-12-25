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
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Options;

	/// <summary>Base class or column header extenders.</summary>
	[ToolboxItem(false)]
	public partial class ExtenderBase : UserControl
	{
		private CustomListBoxColumn _column;

		/// <summary>Create <see cref="ExtenderBase"/>.</summary>
		public ExtenderBase()
		{
			AutoScaleDimensions	= new SizeF(96F, 96F);
			AutoScaleMode		= AutoScaleMode.Dpi;
			BorderStyle			= BorderStyle.FixedSingle;
			if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
			{
				Font		= SystemFonts.MessageBoxFont;
				BackColor	= SystemColors.Window;
			}
			else
			{
				Font		= GitterApplication.FontManager.UIFont;
				BackColor	= GitterApplication.Style.Colors.Window;
				ForeColor	= GitterApplication.Style.Colors.WindowText;
			}
		}

		protected ExtenderBase(CustomListBoxColumn column)
		{
			Verify.Argument.IsNotNull(column, nameof(column));

			_column = column;

			AutoScaleDimensions	= new SizeF(96F, 96F);
			AutoScaleMode		= AutoScaleMode.Dpi;
			BorderStyle			= BorderStyle.FixedSingle;
			if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
			{
				Font		= SystemFonts.MessageBoxFont;
				BackColor	= SystemColors.Window;
			}
			else
			{
				Font		= GitterApplication.FontManager.UIFont;
				BackColor	= _column.Style.Colors.Window;
				ForeColor	= _column.Style.Colors.WindowText;
			}

			_column.StyleChanged += OnColumnStyleChanged;
		}

		protected IGitterStyle Style
		{
			get
			{
				if(_column != null)
				{
					return _column.Style;
				}
				else
				{
					return GitterApplication.Style;
				}
			}
		}

		public CustomListBoxColumn Column
		{
			get { return _column; }
		}

		private void OnColumnStyleChanged(object sender, EventArgs e)
		{
			OnStyleChanged();
		}

		protected virtual void OnStyleChanged()
		{
			BackColor = Style.Colors.Window;
			ForeColor = Style.Colors.WindowText;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_column != null)
				{
					_column.StyleChanged -= OnColumnStyleChanged;
				}
			}
			base.Dispose(disposing);
		}
	}
}

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

	public abstract class CustomListBoxRenderer
	{
		public virtual Color BackColor => SystemColors.Window;

		public virtual Color ForeColor => SystemColors.WindowText;

		public virtual Color ColumnHeaderForeColor => SystemColors.GrayText;

		private Brush _foregroundBrush;
		public Brush ForegroundBrush
		{
			get
			{
				if(_foregroundBrush == null)
				{
					_foregroundBrush = new SolidBrush(ForeColor);
				}
				return _foregroundBrush;
			}
		}

		private Brush _columnHeaderForegroundBrush;
		public Brush ColumnHeaderForegroundBrush
		{
			get
			{
				if(_columnHeaderForegroundBrush == null)
				{
					_columnHeaderForegroundBrush = new SolidBrush(ColumnHeaderForeColor);
				}
				return _columnHeaderForegroundBrush;
			}
		}

		public abstract void OnPaintColumnBackground(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs);

		public abstract void OnPaintColumnContent(CustomListBoxColumn column, ItemPaintEventArgs paintEventArgs);

		public abstract void OnPaintItemBackground(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs);

		public abstract void OnPaintItemContent(CustomListBoxItem item, ItemPaintEventArgs paintEventArgs);
	}
}

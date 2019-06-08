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

	/// <summary>Atomic listbox element - single cell.</summary>
	public abstract class CustomListBoxSubItem
	{
		protected CustomListBoxSubItem(int id)
		{
			Id = id;
		}

		public int Id { get; }

		public CustomListBoxItem Item { get; internal set; }

		public void Invalidate() => Item?.InvalidateSubItem(Id);

		public void InvalidateSafe() => Item?.InvalidateSubItemSafe(Id);

		public void Paint(SubItemPaintEventArgs paintEventArgs)
		{
			OnPaint(paintEventArgs);
		}

		public Size Measure(SubItemMeasureEventArgs measureEventArgs)
		{
			return OnMeasure(measureEventArgs);
		}

		protected abstract void OnPaint(SubItemPaintEventArgs paintEventArgs);

		protected abstract Size OnMeasure(SubItemMeasureEventArgs measureEventArgs);
	}
}

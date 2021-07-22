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

	/// <summary>Content item which can be directly hosted by <see cref="CustomListBox"/>.</summary>
	/// <seealso cref="CustomListBoxColumn"/>
	/// <seealso cref="CustomListBoxItem"/>
	public abstract class CustomListBoxHostedItem
	{
		/// <summary>Listbox which is currently hosting this item.</summary>
		private CustomListBox _listBox;

		/// <summary>Create <see cref="CustomListBoxHostedItem"/>.</summary>
		internal CustomListBoxHostedItem()
		{
		}

		/// <summary>Returns if this item is attached to a listbox.</summary>
		public bool IsAttachedToListBox => _listBox != null;

		/// <summary>Returns listbox which is currently hosting this item.</summary>
		/// <value>Listbox which is currently hosting this item.</value>
		public CustomListBox ListBox
		{
			get => _listBox;
			internal set
			{
				if(_listBox != value)
				{
					if(_listBox is not null)
					{
						OnListBoxDetached();
					}
					_listBox = value;
					if(_listBox is not null)
					{
						OnListBoxAttached();
					}
				}
			}
		}

		/// <summary>Called when item is attached to listbox.</summary>
		protected virtual void OnListBoxAttached()
		{
		}

		/// <summary>Called when item is detached from listbox.</summary>
		protected virtual void OnListBoxDetached()
		{
		}

		/// <summary>Paints item background.</summary>
		/// <param name="paintEventArgs">Painting options.</param>
		protected abstract void OnPaintBackground(ItemPaintEventArgs paintEventArgs);

		/// <summary>Paints item content.</summary>
		/// <param name="paintEventArgs">Painting options.</param>
		protected abstract void OnPaintContent(ItemPaintEventArgs paintEventArgs);

		/// <summary>Perform item hit-testing.</summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <returns>Hit area id.</returns>
		protected abstract int OnHitTest(int x, int y);

		/// <summary>Draw item.</summary>
		/// <param name="paintEventArgs">Paint options.</param>
		public void Paint(ItemPaintEventArgs paintEventArgs)
		{
			OnPaintBackground(paintEventArgs);
			OnPaintContent(paintEventArgs);
		}

		/// <summary>Perform item hit-testing.</summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <returns>Hit area id.</returns>
		public int HitTest(int x, int y) => OnHitTest(x, y);

		/// <summary>Perform item hit-testing.</summary>
		/// <param name="location">Coordinates.</param>
		/// <returns>Hit area id.</returns>
		public int HitTest(Point location) => OnHitTest(location.X, location.Y);
	}
}

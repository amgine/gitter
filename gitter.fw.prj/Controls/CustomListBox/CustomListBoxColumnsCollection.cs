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

	/// <summary>Collection of <see cref="CustomListBoxColumn"/>, hosted by <see cref="CustomListBox"/> control.</summary>
	public sealed class CustomListBoxColumnsCollection : NotifyCollection<CustomListBoxColumn>
	{
		/// <summary>Create <see cref="CustomListBoxColumnsCollection"/>.</summary>
		/// <param name="listBox">Host <see cref="CustomListBox"/> control.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="listBox"/> == <c>null</c>.</exception>
		internal CustomListBoxColumnsCollection(CustomListBox listBox)
		{
			Verify.Argument.IsNotNull(listBox, nameof(listBox));

			ListBox = listBox;
		}

		internal bool HasFillModeVisibleColumn
		{
			get
			{
				for(int i = 0; i < Count; ++i)
				{
					if(Items[i].IsVisible && Items[i].SizeMode == ColumnSizeMode.Fill)
					{
						return true;
					}
				}
				return false;
			}
		}

		public CustomListBox ListBox { get; }

		protected override void FreeItem(CustomListBoxColumn item)
		{
			item.ListBox = null;
		}

		protected override void AcquireItem(CustomListBoxColumn item)
		{
			item.ListBox = ListBox;
		}

		protected override bool VerifyItem(CustomListBoxColumn item)
		{
			return item != null && item.ListBox == null;
		}

		public CustomListBoxColumn GetById(int columnId)
		{
			for(int i = 0; i < Items.Count; ++i)
			{
				if(Items[i].Id == columnId) return Items[i];
			}
			return null;
		}

		public void ShowAll()
		{
			foreach(var c in this)
			{
				c.IsVisible = true;
			}
		}

		public void ShowAll(Predicate<CustomListBoxColumn> predicate)
		{
			Verify.Argument.IsNotNull(predicate, nameof(predicate));

			foreach(var c in this)
			{
				c.IsVisible = predicate(c);
			}
		}

		public int GetColumnIndex(int columnId)
		{
			for(int i = 0; i < Items.Count; ++i)
			{
				if(Items[i].Id == columnId) return i;
			}
			return -1;
		}

		public bool ColumnVisible(int columnId)
		{
			var id = GetColumnIndex(columnId);
			if(id == -1) return false;
			return Items[id].IsVisible;
		}
	}
}

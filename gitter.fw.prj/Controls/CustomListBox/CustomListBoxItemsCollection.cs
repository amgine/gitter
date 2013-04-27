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

	/// <summary>Collection of <see cref="CustomListBoxItem"/>.</summary>
	public sealed class CustomListBoxItemsCollection : SafeNotifySortedCollection<CustomListBoxItem>
	{
		#region Data

		private readonly CustomListBoxItem _parent;
		private CustomListBox _listBox;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="CustomListBoxItemsCollection"/>.</summary>
		/// <param name="listBox">Host <see cref="CustomListBox"/>.</param>
		/// <param name="parent">Parent <see cref="CustomListBoxItem"/>.</param>
		internal CustomListBoxItemsCollection(CustomListBox listBox, CustomListBoxItem parent)
		{
			_listBox = listBox;
			_parent = parent;
		}

		#endregion

		#region Properties

		/// <summary><see cref="CustomListBox"/> which hosts this collection or <see cref="CustomListBoxItem"/> owning this collection.</summary>
		internal CustomListBox ListBox
		{
			get { return _listBox; }
			set
			{
				if(_listBox != value)
				{
					_listBox = value;
					if(Items.Count != 0)
					{
						foreach(var item in Items)
						{
							item.ListBox = value;
						}
					}
				}
			}
		}

		/// <summary><see cref="CustomListBoxItem"/> which owns this collection (null if collection is owned by <see cref="CustomListBox"/> itself).</summary>
		internal CustomListBoxItem Parent
		{
			get { return _parent; }
		}

		#endregion

		#region Overrides

		/// <summary>Gets the synchronization object.</summary>
		/// <value>The synchronization object.</value>
		protected override ISynchronizeInvoke SynchronizeInvoke
		{
			get { return _listBox; }
		}

		/// <summary>Frees the item.</summary>
		/// <param name="item">The item.</param>
		protected override void FreeItem(CustomListBoxItem item)
		{
			item.ListBox = null;
			if(item.Parent != null && item.Parent.ListBox == _listBox)
			{
				item.Parent = null;
			}
		}

		/// <summary>Acquires the item.</summary>
		/// <param name="item">The item.</param>
		protected override void AcquireItem(CustomListBoxItem item)
		{
			item.ListBox = _listBox;
			item.Parent = _parent;
		}

		/// <summary>Verifies the item.</summary>
		/// <param name="item">The item.</param>
		/// <returns>true if item can be added to this <see cref="CustomListBoxItemsCollection"/>.</returns>
		protected override bool VerifyItem(CustomListBoxItem item)
		{
			return item != null && item.ListBox == null;
		}

		#endregion
	}
}

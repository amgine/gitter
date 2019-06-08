#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2019  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Views
{
	using System;

	using gitter.Framework.Controls;

	abstract class ListBoxSearch<T> : ISearch<T>
		where T : SearchOptions
	{
		protected ListBoxSearch(CustomListBox listBox)
		{
			Verify.Argument.IsNotNull(listBox, nameof(listBox));

			ListBox = listBox;
		}

		protected CustomListBox ListBox { get; }

		protected abstract bool TestItem(CustomListBoxItem item, T search);

		private bool Search(int start, T search, int direction)
		{
			if(search.Text.Length == 0) return true;
			int count = ListBox.Items.Count;
			if(count == 0) return false;
			int end;
			if(direction == 1)
			{
				start = (start + 1) % count;
				end = start - 1;
				if(end < 0) end += count;
			}
			else
			{
				start = (start - 1);
				if(start < 0) start += count;
				end = (start + 1) % count;
			}
			while(start != end)
			{
				var item = ListBox.Items[start];
				if(TestItem(item, search))
				{
					item.FocusAndSelect();
					return true;
				}
				if(direction == 1)
				{
					start = (start + 1) % count;
				}
				else
				{
					--start;
					if(start < 0) start = count - 1;
				}
			}
			return false;
		}

		public bool First(T search)
		{
			Verify.Argument.IsNotNull(search, nameof(search));

			return Search(-1, search, 1);
		}

		public bool Current(T search)
		{
			Verify.Argument.IsNotNull(search, nameof(search));

			if(search.Text.Length == 0) return true;
			if(ListBox.SelectedItems.Count == 0)
			{
				return Search(-1, search, 1);
			}
			var start = ListBox.Items.IndexOf(ListBox.SelectedItems[0]);
			return Search(start - 1, search, 1);
		}

		public bool Next(T search)
		{
			Verify.Argument.IsNotNull(search, nameof(search));

			if(search.Text.Length == 0) return true;
			if(ListBox.SelectedItems.Count == 0)
			{
				return Search(-1, search, 1);
			}
			var start = ListBox.Items.IndexOf(ListBox.SelectedItems[0]);
			return Search(start, search, 1);
		}

		public bool Previous(T search)
		{
			Verify.Argument.IsNotNull(search, nameof(search));

			if(search.Text.Length == 0) return true;
			if(ListBox.SelectedItems.Count == 0) return Search(-1, search, 1);
			var start = ListBox.Items.IndexOf(ListBox.SelectedItems[0]);
			return Search(start, search, -1);
		}
	}
}

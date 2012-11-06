namespace gitter.Git.Gui.Views
{
	using System;

	using gitter.Framework.Controls;

	abstract class ListBoxSearch<T> : ISearch<T>
		where T : SearchOptions
	{
		private readonly CustomListBox _listBox;

		protected ListBoxSearch(CustomListBox listBox)
		{
			Verify.Argument.IsNotNull(listBox, "listBox");

			_listBox = listBox;
		}

		protected CustomListBox ListBox
		{
			get { return _listBox; }
		}

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
			Verify.Argument.IsNotNull(search, "search");

			return Search(-1, search, 1);
		}

		public bool Next(T search)
		{
			Verify.Argument.IsNotNull(search, "search");

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
			Verify.Argument.IsNotNull(search, "search");

			if(search.Text.Length == 0) return true;
			if(ListBox.SelectedItems.Count == 0) return Search(-1, search, 1);
			var start = ListBox.Items.IndexOf(ListBox.SelectedItems[0]);
			return Search(start, search, -1);
		}
	}
}

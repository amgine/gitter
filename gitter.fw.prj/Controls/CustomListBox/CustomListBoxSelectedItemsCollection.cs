namespace gitter.Framework.Controls
{
	using System;

	public sealed class CustomListBoxSelectedItemsCollection : NotifyCollection<CustomListBoxItem>
	{
		private readonly CustomListBox _listBox;

		internal CustomListBoxSelectedItemsCollection(CustomListBox listBox)
		{
			if(listBox == null) throw new ArgumentNullException("listBox");
			_listBox = listBox;
		}

		internal CustomListBox ListBox
		{
			get { return _listBox; }
		}

		protected override void FreeItem(CustomListBoxItem item)
		{
			if(item.ListBox == _listBox)
			{
				item.SetSelected(false);
			}
		}

		protected override void AcquireItem(CustomListBoxItem item)
		{
			item.SetSelected(true);
		}

		protected override bool VerifyItem(CustomListBoxItem item)
		{
			return item != null && item.ListBox == _listBox;
		}
	}
}

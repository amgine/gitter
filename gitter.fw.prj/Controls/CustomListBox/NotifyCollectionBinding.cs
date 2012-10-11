namespace gitter.Framework.Controls
{
	using System;

	public class NotifyCollectionBinding<T> : IDisposable
	{
		private readonly CustomListBoxItemsCollection _itemsCollection;
		private readonly NotifyCollection<T> _boundCollection;
		private readonly Converter<T, CustomListBoxItem> _itemConverter;

		public NotifyCollectionBinding(
			CustomListBoxItemsCollection itemsCollection,
			NotifyCollection<T> boundCollection,
			Converter<T, CustomListBoxItem> itemConverter)
		{
			Verify.Argument.IsNotNull(itemsCollection, "itemsCollection");
			Verify.Argument.IsNotNull(boundCollection, "boundCollection");
			Verify.Argument.IsNotNull(itemConverter, "itemConverter");

			_itemsCollection = itemsCollection;
			_boundCollection = boundCollection;
			_itemConverter = itemConverter;

			_itemsCollection.Clear();
			foreach(var item in _boundCollection)
			{
				_itemsCollection.Add(_itemConverter(item));
			}
			_boundCollection.Changed += OnChanged;
		}

		private void OnChanged(object sender, NotifyCollectionEventArgs e)
		{
			switch(e.Event)
			{
				case NotifyEvent.Insert:
					for(int i = e.StartIndex; i <= e.EndIndex; ++i)
					{
						var item = _itemConverter(_boundCollection[i]);
						_itemsCollection.InsertSafe(i, item);
					}
					break;
				case NotifyEvent.Remove:
					_itemsCollection.RemoveRangeSafe(e.StartIndex, e.ModifiedItems);
					break;
				case NotifyEvent.Set:
					for(int i = e.StartIndex; i <= e.EndIndex; ++i)
					{
						var item = _itemConverter(_boundCollection[i]);
						_itemsCollection[i] = item;
					}
					break;
				case NotifyEvent.Clear:
					_itemsCollection.ClearSafe();
					break;
			}
		}

		public void Dispose()
		{
			_boundCollection.Changed -= OnChanged;
			_itemsCollection.Clear();
		}
	}
}

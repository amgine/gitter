namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	public class BoundItemActivatedEventArgs<T> : EventArgs
	{
		private readonly CustomListBoxItem _item;
		private readonly T _object;

		public BoundItemActivatedEventArgs(CustomListBoxItem item, T obj)
		{
			_item = item;
			_object = obj;
		}

		public CustomListBoxItem Item
		{
			get { return _item; }
		}

		public T Object
		{
			get { return _object; }
		}
	}
}

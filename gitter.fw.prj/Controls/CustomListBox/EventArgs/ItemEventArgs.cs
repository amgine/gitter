namespace gitter.Framework.Controls
{
	using System;

	/// <summary>Event args, prividing <see cref="CustomListBoxItem"/>.</summary>
	public sealed class ItemEventArgs : EventArgs
	{
		private readonly CustomListBoxItem _item;

		/// <summary>Create <see cref="ItemEventArgs"/>.</summary>
		/// <param name="item">Related <see cref="CustomListBoxItem"/>.</param>
		public ItemEventArgs(CustomListBoxItem item)
		{
			_item = item;
		}

		/// <summary>Related <see cref="CustomListBoxItem"/>.</summary>
		public CustomListBoxItem Item
		{
			get { return _item; }
		}
	}
}

namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	/// <summary>Custom lst box composite item.</summary>
	/// <typeparam name="TData">The type of the data.</typeparam>
	public class CustomListBoxRow<TData> : CustomListBoxItem<TData>, IEnumerable<CustomListBoxSubItem>
	{
		#region Data

		private readonly Dictionary<int, CustomListBoxSubItem> _subItems;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="CustomListBoxRow&lt;TData&gt;"/> class.</summary>
		/// <param name="data">Data.</param>
		/// <param name="subItems">Sub items.</param>
		public CustomListBoxRow(TData data, IEnumerable<CustomListBoxSubItem> subItems)
			: base(data)
		{
			Verify.Argument.IsNotNull(subItems, "subItems");
			Verify.Argument.HasNoNullItems(subItems, "subItems");

			_subItems = new Dictionary<int, CustomListBoxSubItem>();
			foreach(var subItem in subItems)
			{
				_subItems.Add(subItem.Id, subItem);
				subItem.Item = this;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="CustomListBoxRow&lt;TData&gt;"/> class.</summary>
		/// <param name="data">Row data.</param>
		/// <param name="subItems">Sub items.</param>
		public CustomListBoxRow(TData data, params CustomListBoxSubItem[] subItems)
			: base(data)
		{
			Verify.Argument.IsNotNull(subItems, "subItems");
			Verify.Argument.HasNoNullItems(subItems, "subItems");

			_subItems = new Dictionary<int, CustomListBoxSubItem>(subItems.Length);
			for(int i = 0; i < subItems.Length; ++i)
			{
				var subItem = subItems[i];
				_subItems.Add(subItems[i].Id, subItem);
				subItem.Item = this;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="CustomListBoxRow&lt;TData&gt;"/> class.</summary>
		/// <param name="data">Row data.</param>
		public CustomListBoxRow(TData data)
			: base(data)
		{
			_subItems = new Dictionary<int, CustomListBoxSubItem>();
		}

		#endregion

		#region Properties

		/// <summary>Gets the <see cref="CustomListBoxSubItem"/> with the specified id.</summary>
		/// <param name="id">Subitem id to get.</param>
		public CustomListBoxSubItem this[int id]
		{
			get { return _subItems[id]; }
		}

		/// <summary>Collection of subitems.</summary>
		public IEnumerable<CustomListBoxSubItem> SubItems
		{
			get { return _subItems.Values; }
		}

		/// <summary>Gets the subitem count.</summary>
		/// <value>The sub item count.</value>
		public int SubItemCount
		{
			get { return _subItems.Count; }
		}

		#endregion

		#region Methods

		/// <summary>Adds the subitem.</summary>
		/// <param name="subItem"><see cref="CustomListBoxSubItem"/> to add.</param>
		public void AddSubItem(CustomListBoxSubItem subItem)
		{
			Verify.Argument.IsNotNull(subItem, "subItem");

			_subItems.Add(subItem.Id, subItem);
			subItem.Item = this;
		}

		/// <summary>Removes the subitem.</summary>
		/// <param name="subItem"><see cref="CustomListBoxSubItem"/> to remove.</param>
		public void RemoveSubItem(CustomListBoxSubItem subItem)
		{
			Verify.Argument.IsNotNull(subItem, "subItem");

			_subItems.Remove(subItem.Id);
			subItem.Item = null;
		}

		/// <summary>Removes the subitem.</summary>
		/// <param name="id">Removed subitem's id.</param>
		public void RemoveSubItem(int id)
		{
			CustomListBoxSubItem subItem;
			if(_subItems.TryGetValue(id, out subItem))
			{
				subItem.Item = null;
				_subItems.Remove(id);
			}
		}

		/// <summary>Paint subitem..</summary>
		/// <param name="paintEventArgs">Paint event args.</param>
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			CustomListBoxSubItem subItem;
			if(_subItems.TryGetValue(paintEventArgs.SubItemId, out subItem))
			{
				subItem.Paint(paintEventArgs);
			}
		}

		/// <summary>Measure subitem.</summary>
		/// <param name="measureEventArgs">Measure event args.</param>
		/// <returns>Size of the subitem.</returns>
		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			CustomListBoxSubItem subItem;
			if(_subItems.TryGetValue(measureEventArgs.SubItemId, out subItem))
			{
				return subItem.Measure(measureEventArgs);
			}
			else
			{
				return Size.Empty;
			}
		}

		#endregion

		#region IEnumerable<CustomListBoxSubItem> Members

		public IEnumerator<CustomListBoxSubItem> GetEnumerator()
		{
			return _subItems.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _subItems.Values.GetEnumerator();
		}

		#endregion
	}
}

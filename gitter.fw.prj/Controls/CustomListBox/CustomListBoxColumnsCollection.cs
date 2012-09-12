namespace gitter.Framework.Controls
{
	using System;

	/// <summary>Collection of <see cref="CustomListBoxColumn"/>, hosted by <see cref="CustomListBox"/> control.</summary>
	public sealed class CustomListBoxColumnsCollection : NotifyCollection<CustomListBoxColumn>
	{
		#region Data

		private readonly CustomListBox _listBox;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="CustomListBoxColumnsCollection"/>.</summary>
		/// <param name="listBox">Host <see cref="CustomListBox"/> control.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="listBox"/> == <c>null</c>.</exception>
		internal CustomListBoxColumnsCollection(CustomListBox listBox)
		{
			Verify.Argument.IsNotNull(listBox, "listBox");

			_listBox = listBox;
		}

		#endregion

		#region Properties

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

		#endregion

		#region Overrides

		protected override void FreeItem(CustomListBoxColumn item)
		{
			item.ListBox = null;
		}

		protected override void AcquireItem(CustomListBoxColumn item)
		{
			item.ListBox = _listBox;
		}

		protected override bool VerifyItem(CustomListBoxColumn item)
		{
			return item != null && item.ListBox == null;
		}

		#endregion

		#region Methods

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
			Verify.Argument.IsNotNull(predicate, "predicate");

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

		#endregion
	}
}

#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Mvc.WinForms
{
	using gitter.Framework.Controls;

	public static class PickerInputSource
	{
		public static PickerInputSource<TListBox, TItem, TValue> Create<TListBox, TItem, TValue>(CustomObjectPicker<TListBox, TItem, TValue> picker)
			where TListBox : CustomListBox, new()
			where TItem : CustomListBoxItem
		{
			return new PickerInputSource<TListBox, TItem, TValue>(picker);
		}
	}

	public class PickerInputSource<TListBox, TItem, TValue> : ControlInputSource<CustomObjectPicker<TListBox, TItem, TValue>, TValue>
		where TListBox : CustomListBox, new()
		where TItem : CustomListBoxItem
	{
		#region .ctor

		public PickerInputSource(CustomObjectPicker<TListBox, TItem, TValue> picker)
			: base(picker)
		{
		}

		#endregion

		#region Methods

		protected override TValue FetchValue()
		{
			return Control.SelectedValue;
		}

		protected override void SetValue(TValue value)
		{
			Control.SelectedValue = value;
		}

		protected override void SubscribeToValueChangeEvent()
		{
			Control.SelectedValueChanged += OnControlValueChanged;
		}

		protected override void UnsubscribeToValueChangeEvent()
		{
			Control.SelectedValueChanged -= OnControlValueChanged;
		}

		#endregion
	}
}

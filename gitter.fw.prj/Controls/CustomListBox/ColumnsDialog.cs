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
	using System.Drawing;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Dialog for editing columns of <see cref="CustomListBox"/>.</summary>
	[ToolboxItem(false)]
	internal partial class ColumnsDialog : DialogBase, IExecutableDialog
	{
		private sealed class ColumnItem : CustomListBoxItem<CustomListBoxColumn>
		{
			public ColumnItem(CustomListBoxColumn column)
				: base(column)
			{
				CheckedState = column.IsVisible?CheckedState.Checked:CheckedState.Unchecked;
			}

			protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			{
				switch(measureEventArgs.SubItemId)
				{
					case 0:
						return measureEventArgs.MeasureText(DataContext.Name);
					default:
						return Size.Empty;
				}
			}

			protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
			{
				switch(paintEventArgs.SubItemId)
				{
					case 0:
						paintEventArgs.PaintText(DataContext.Name);
						break;
				}
			}
		}

		/// <summary>Create <see cref="ColumnsDialog"/>.</summary>
		/// <param name="listBox">Related <see cref="CustomListBox"/>.</param>
		public ColumnsDialog(CustomListBox listBox)
		{
			Verify.Argument.IsNotNull(listBox, nameof(listBox));

			ListBox = listBox;

			InitializeComponent();

			Text = Resources.StrColumns;

			_lblVisibleColumns.Text = Resources.StrVisibleColumns.AddColon();
			_btnUp.Text = Resources.StrUp;
			_btnDown.Text = Resources.StrDown;
			_btnShow.Text = Resources.StrShow;
			_btnHide.Text = Resources.StrHide;
			
			_lstColumns.BeginUpdate();
			_lstColumns.Style = GitterApplication.DefaultStyle;
			_lstColumns.HeaderStyle = HeaderStyle.Hidden;
			_lstColumns.Columns.Add(new CustomListBoxColumn(0, string.Empty, true) { SizeMode = ColumnSizeMode.Fill });
			foreach(var c in ListBox.Columns)
			{
				_lstColumns.Items.Add(new ColumnItem(c));
			}
			_lstColumns.EndUpdate();
		}

		/// <summary>Affected <see cref="CustomListBox"/>.</summary>
		public CustomListBox ListBox { get; }

		private void _lstColumns_SelectionChanged(object sender, EventArgs e)
		{
			if(_lstColumns.SelectedItems.Count == 0)
			{
				_btnShow.Enabled = false;
				_btnHide.Enabled = false;
				_btnUp.Enabled = false;
				_btnDown.Enabled = false;
			}
			else
			{
				_btnShow.Enabled = true;
				_btnHide.Enabled = true;
				var item = _lstColumns.SelectedItems[0];
				var index = _lstColumns.Items.IndexOf(item);
				if(ListBox.AllowColumnReorder)
				{
					_btnUp.Enabled = index != 0;
					_btnDown.Enabled = index != _lstColumns.Items.Count - 1;
				}
			}
		}

		private void _btnShow_Click(object sender, EventArgs e)
		{
			foreach(var item in _lstColumns.SelectedItems)
			{
				item.CheckedState = CheckedState.Checked;
			}
		}

		private void _btnHide_Click(object sender, EventArgs e)
		{
			foreach(var item in _lstColumns.SelectedItems)
			{
				item.CheckedState = CheckedState.Unchecked;
			}
		}

		private void _btnUp_Click(object sender, EventArgs e)
		{
			if(_lstColumns.SelectedItems.Count != 1) return;
			var item = _lstColumns.SelectedItems[0];
			var index = _lstColumns.Items.IndexOf(item);
			if(index == 0) return;
			_lstColumns.Items.RemoveAt(index);
			_lstColumns.Items.Insert(index - 1, item);
			item.IsSelected = true;
		}

		private void _btnDown_Click(object sender, EventArgs e)
		{
			if(_lstColumns.SelectedItems.Count != 1) return;
			var item = _lstColumns.SelectedItems[0];
			var index = _lstColumns.Items.IndexOf(item);
			if(index == _lstColumns.Items.Count - 1) return;
			_lstColumns.Items.RemoveAt(index);
			_lstColumns.Items.Insert(index + 1, item);
			item.IsSelected = true;
		}

		public bool Execute()
		{
			ListBox.BeginUpdate();
			ListBox.Columns.Clear();
			foreach(var item in _lstColumns.Items)
			{
				var c = ((ColumnItem)item).DataContext;
				c.IsVisible = item.CheckedState == CheckedState.Checked;
				ListBox.Columns.Add(c);
			}
			ListBox.NotifyColumnLayoutChanged();
			ListBox.EndUpdate();
			return true;
		}
	}
}

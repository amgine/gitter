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
						return measureEventArgs.MeasureText(Data.Name);
					default:
						return Size.Empty;
				}
			}

			protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
			{
				switch(paintEventArgs.SubItemId)
				{
					case 0:
						paintEventArgs.PaintText(Data.Name);
						break;
				}
			}
		}

		private readonly CustomListBox _listBox;

		/// <summary>Create <see cref="ColumnsDialog"/>.</summary>
		/// <param name="listBox">Related <see cref="CustomListBox"/>.</param>
		public ColumnsDialog(CustomListBox listBox)
		{
			if(listBox == null) throw new ArgumentNullException("listBox");
			_listBox = listBox;

			InitializeComponent();

			if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				Font = SystemFonts.MessageBoxFont;
			else
				Font = GitterApplication.FontManager.UIFont;

			Text = Resources.StrColumns;

			_lblVisibleColumns.Text = Resources.StrVisibleColumns.AddColon();
			_btnUp.Text = Resources.StrUp;
			_btnDown.Text = Resources.StrDown;
			_btnShow.Text = Resources.StrShow;
			_btnHide.Text = Resources.StrHide;
			
			_lstColumns.BeginUpdate();
			_lstColumns.HeaderStyle = HeaderStyle.Hidden;
			_lstColumns.Columns.Add(new CustomListBoxColumn(0, string.Empty, true) { SizeMode = ColumnSizeMode.Fill });
			foreach(var c in _listBox.Columns)
			{
				_lstColumns.Items.Add(new ColumnItem(c));
			}
			_lstColumns.EndUpdate();
		}

		/// <summary>Affected <see cref="CustomListBox"/>.</summary>
		public CustomListBox ListBox
		{
			get { return _listBox; }
		}

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
				if(_listBox.AllowColumnReorder)
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

		#region IExecutableDialog Members

		public bool Execute()
		{
			_listBox.BeginUpdate();
			_listBox.Columns.Clear();
			foreach(var item in _lstColumns.Items)
			{
				var c = ((ColumnItem)item).Data;
				c.IsVisible = item.CheckedState == CheckedState.Checked;
				_listBox.Columns.Add(c);
			}
			_listBox.ColumnLayoutChanged();
			_listBox.EndUpdate();
			return true;
		}

		#endregion
	}
}

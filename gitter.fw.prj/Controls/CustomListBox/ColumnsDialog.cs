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

namespace gitter.Framework.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Layout;

using Resources = gitter.Framework.Properties.Resources;

/// <summary>Dialog for editing columns of <see cref="CustomListBox"/>.</summary>
[ToolboxItem(false)]
[DesignerCategory("")]
internal partial class ColumnsDialog : DialogBase, IExecutableDialog
{
	readonly struct DialogControls
	{
		public readonly CustomListBox _lstColumns;
		public readonly IButtonWidget _btnHide;
		public readonly IButtonWidget _btnUp;
		public readonly IButtonWidget _btnShow;
		public readonly IButtonWidget _btnDown;
		public readonly LabelControl _lblVisibleColumns;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			var bf = style.ButtonFactory;

			_lblVisibleColumns = new();
			_lstColumns = new()
			{
				HeaderStyle = HeaderStyle.Hidden,
				ShowCheckBoxes = true,
				Style = style,
			};
			_btnHide = bf.Create();
			_btnUp   = bf.Create();
			_btnShow = bf.Create();
			_btnDown = bf.Create();

			_btnHide.Enabled = false;
			_btnUp.Enabled   = false;
			_btnShow.Enabled = false;
			_btnDown.Enabled = false;

			_lstColumns.Columns.Add(new CustomListBoxColumn(0, string.Empty, true) { SizeMode = ColumnSizeMode.Fill });
		}

		public void Localize()
		{
			_lblVisibleColumns.Text = Resources.StrVisibleColumns.AddColon();
			_btnUp.Text = Resources.StrUp;
			_btnDown.Text = Resources.StrDown;
			_btnShow.Text = Resources.StrShow;
			_btnHide.Text = Resources.StrHide;
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Everything(),
						LayoutConstants.ColumnSpacing,
						SizeSpec.Absolute(75),
					],
					rows:
					[
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblVisibleColumns, marginOverride: LayoutConstants.NoMargin), row: 0, column: 0),
						new GridContent(new ControlContent(_lstColumns,        marginOverride: LayoutConstants.NoMargin), row: 2, column: 0),
						new GridContent(new Grid(
							rows:
							[
								LayoutConstants.ButtonRowHeight,
								LayoutConstants.RowSpacing,
								LayoutConstants.ButtonRowHeight,
								LayoutConstants.RowSpacing,
								LayoutConstants.ButtonRowHeight,
								LayoutConstants.RowSpacing,
								LayoutConstants.ButtonRowHeight,
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new WidgetContent(_btnUp,   marginOverride: LayoutConstants.NoMargin), row: 0),
								new GridContent(new WidgetContent(_btnDown, marginOverride: LayoutConstants.NoMargin), row: 2),
								new GridContent(new WidgetContent(_btnHide, marginOverride: LayoutConstants.NoMargin), row: 4),
								new GridContent(new WidgetContent(_btnShow, marginOverride: LayoutConstants.NoMargin), row: 6),
							]), row: 2, column: 2),
					]),
			};

			var tabIndex = 0;
			_lblVisibleColumns.TabIndex = tabIndex++;
			_lstColumns.TabIndex = tabIndex++;
			_btnUp.TabIndex = tabIndex++;
			_btnDown.TabIndex = tabIndex++;
			_btnHide.TabIndex = tabIndex++;
			_btnShow.TabIndex = tabIndex++;

			_lblVisibleColumns.Parent = parent;
			_lstColumns.Parent = parent;
			_btnUp.Parent = parent;
			_btnDown.Parent = parent;
			_btnHide.Parent = parent;
			_btnShow.Parent = parent;
		}
	}

	protected override bool ScaleChildren => false;

	public override IDpiBoundValue<Size> ScalableSize => DpiBoundValue.Size(new(300, 247));

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

	private readonly DialogControls _controls;

	/// <summary>Create <see cref="ColumnsDialog"/>.</summary>
	/// <param name="listBox">Related <see cref="CustomListBox"/>.</param>
	public ColumnsDialog(CustomListBox listBox)
	{
		Verify.Argument.IsNotNull(listBox);

		ListBox = listBox;

		Name = nameof(ColumnsDialog);
		Text = Resources.StrColumns;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(false);
		PerformLayout();

		_controls._lstColumns.BeginUpdate();
		foreach(var c in ListBox.Columns)
		{
			_controls._lstColumns.Items.Add(new ColumnItem(c));
		}
		_controls._lstColumns.EndUpdate();

		_controls._btnHide.Click += _btnHide_Click;
		_controls._btnUp.Click   += _btnUp_Click;
		_controls._btnShow.Click += _btnShow_Click;
		_controls._btnDown.Click += _btnDown_Click;

		_controls._lstColumns.SelectionChanged += _lstColumns_SelectionChanged;
	}

	/// <summary>Affected <see cref="CustomListBox"/>.</summary>
	public CustomListBox ListBox { get; }

	private void _lstColumns_SelectionChanged(object? sender, EventArgs e)
	{
		if(_controls._lstColumns.SelectedItems.Count == 0)
		{
			_controls._btnShow.Enabled = false;
			_controls._btnHide.Enabled = false;
			_controls._btnUp.Enabled   = false;
			_controls._btnDown.Enabled = false;
		}
		else
		{
			_controls._btnShow.Enabled = true;
			_controls._btnHide.Enabled = true;
			var item  = _controls._lstColumns.SelectedItems[0];
			var index = _controls._lstColumns.Items.IndexOf(item);
			if(ListBox.AllowColumnReorder)
			{
				_controls._btnUp.Enabled   = index != 0;
				_controls._btnDown.Enabled = index != _controls._lstColumns.Items.Count - 1;
			}
		}
	}

	private void _btnShow_Click(object? sender, EventArgs e)
	{
		foreach(var item in _controls._lstColumns.SelectedItems)
		{
			item.CheckedState = CheckedState.Checked;
		}
	}

	private void _btnHide_Click(object? sender, EventArgs e)
	{
		foreach(var item in _controls._lstColumns.SelectedItems)
		{
			item.CheckedState = CheckedState.Unchecked;
		}
	}

	private void _btnUp_Click(object? sender, EventArgs e)
	{
		if(_controls._lstColumns.SelectedItems.Count != 1) return;
		var item  = _controls._lstColumns.SelectedItems[0];
		var index = _controls._lstColumns.Items.IndexOf(item);
		if(index == 0) return;
		_controls._lstColumns.Items.RemoveAt(index);
		_controls._lstColumns.Items.Insert(index - 1, item);
		item.IsSelected = true;
	}

	private void _btnDown_Click(object? sender, EventArgs e)
	{
		if(_controls._lstColumns.SelectedItems.Count != 1) return;
		var item =  _controls._lstColumns.SelectedItems[0];
		var index = _controls._lstColumns.Items.IndexOf(item);
		if(index == _controls._lstColumns.Items.Count - 1) return;
		_controls._lstColumns.Items.RemoveAt(index);
		_controls._lstColumns.Items.Insert(index + 1, item);
		item.IsSelected = true;
	}

	public bool Execute()
	{
		ListBox.BeginUpdate();
		ListBox.Columns.Clear();
		foreach(var item in _controls._lstColumns.Items)
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

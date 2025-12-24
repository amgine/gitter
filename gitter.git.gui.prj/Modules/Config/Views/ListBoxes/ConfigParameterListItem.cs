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

namespace gitter.Git.Gui.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="ConfigParameter"/> object.</summary>
public class ConfigParameterListItem : CustomListBoxItem<ConfigParameter>
{
	#region Comparers

	public static int CompareByName(ConfigParameterListItem item1, ConfigParameterListItem item2)
	{
		var data1 = item1.DataContext.Name;
		var data2 = item2.DataContext.Name;
		return string.Compare(data1, data2);
	}

	public static int CompareByName(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return CompareByName((ConfigParameterListItem)item1, (ConfigParameterListItem)item2);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return 0;
		}
	}

	public static int CompareByValue(ConfigParameterListItem item1, ConfigParameterListItem item2)
	{
		var data1 = item1.DataContext.Value;
		var data2 = item2.DataContext.Value;
		return string.Compare(data1, data2);
	}

	public static int CompareByValue(CustomListBoxItem item1, CustomListBoxItem item2)
	{
		try
		{
			return CompareByValue((ConfigParameterListItem)item1, (ConfigParameterListItem)item2);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return 0;
		}
	}

	#endregion

	#region .ctor

	/// <summary>Create <see cref="ConfigParameterListItem"/>.</summary>
	/// <param name="parameter">Related <see cref="ConfigParameter"/>.</param>
	/// <exception cref="ArgumentNullException"><paramref name="parameter"/> == <c>null</c>.</exception>
	public ConfigParameterListItem(ConfigParameter parameter)
		: base(parameter)
	{
		Verify.Argument.IsNotNull(parameter);
	}

	#endregion

	#region Methods

	public void StartValueEditor()
	{
		var column = ListBox?.Columns.GetById((int)ColumnId.Value);
		if(column is null) return;
		var editor = StartTextEditor(column, DataContext.Value);
		editor.Validating += OnEditorValidating;
	}

	#endregion

	#region Event Handlers

	private void OnDeleted(object? sender, EventArgs e)
	{
		RemoveSafe();
	}

	private void OnValueChanged(object? sender, EventArgs e)
	{
		InvalidateSubItemSafe((int)ColumnId.Value);
	}

	private void OnEditorValidating(object? sender, CancelEventArgs e)
	{
		var editor = (CustomListBoxTextEditor)sender!;
		editor.Validating -= OnEditorValidating;
		DataContext.Value = editor.Text.Trim();
	}

	private static Image? GetIcon(Dpi dpi)
		=> Icons.Config.GetImage(DpiConverter.FromDefaultTo(dpi).ConvertX(16));

	#endregion

	#region Overrides

	/// <inheritdoc/>
	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);
		DataContext.Deleted += OnDeleted;
		DataContext.ValueChanged += OnValueChanged;
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		base.OnListBoxDetached(listBox);
		DataContext.Deleted -= OnDeleted;
		DataContext.ValueChanged -= OnValueChanged;
	}

	/// <inheritdoc/>
	public override void OnDoubleClick(int x, int y)
	{
		base.OnDoubleClick(x, y);
		if(ListBox is not null)
		{
			int cid = ListBox.Columns.GetColumnIndex((int)ColumnId.Value);
			var column = ListBox.Columns[cid];
			if(x > column.Left && x < column.Left + column.CurrentWidth)
			{
				StartValueEditor();
			}
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
				return measureEventArgs.MeasureImageAndText(GetIcon(measureEventArgs.Dpi), DataContext.Name);
			case ColumnId.Value:
				return ConfigParameterValueColumn.OnMeasureSubItem(measureEventArgs, DataContext.Value);
			default:
				return Size.Empty;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Name:
				paintEventArgs.PaintImageAndText(GetIcon(paintEventArgs.Dpi), DataContext.Name);
				break;
			case ColumnId.Value:
				ConfigParameterValueColumn.OnPaintSubItem(paintEventArgs, DataContext.Value);
				break;
		}
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		var menu = new ConfigParameterMenu(this);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}

	#endregion
}

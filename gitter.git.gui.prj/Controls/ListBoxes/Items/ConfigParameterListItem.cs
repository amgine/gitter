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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="ConfigParameter"/> object.</summary>
	public class ConfigParameterListItem : CustomListBoxItem<ConfigParameter>
	{
		#region Static

		private static readonly Bitmap ImgConfig = CachedResources.Bitmaps["ImgConfig"];

		#endregion

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
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
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
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
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
			Verify.Argument.IsNotNull(parameter, "parameter");
		}

		#endregion

		#region Methods

		public void StartValueEditor()
		{
			var editor = StartTextEditor(ListBox.Columns.GetById((int)ColumnId.Value), DataContext.Value);
			editor.Validating += OnEditorValidating;
		}

		#endregion

		#region Event Handlers

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		private void OnValueChanged(object sender, EventArgs e)
		{
			InvalidateSubItemSafe((int)ColumnId.Value);
		}

		private void OnEditorValidating(object sender, CancelEventArgs e)
		{
			var editor = (CustomListBoxTextEditor)sender;
			editor.Validating -= OnEditorValidating;
			DataContext.Value = editor.Text.Trim();
		}

		#endregion

		#region Overrides

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnDeleted;
			DataContext.ValueChanged += OnValueChanged;
		}

		protected override void OnListBoxDetached()
		{
			base.OnListBoxDetached();
			DataContext.Deleted -= OnDeleted;
			DataContext.ValueChanged -= OnValueChanged;
		}

		public override void OnDoubleClick(int x, int y)
		{
			base.OnDoubleClick(x, y);
			int cid = ListBox.Columns.GetColumnIndex((int)ColumnId.Value);
			var column = ListBox.Columns[cid];
			if(x > column.Left && x < column.Left + column.Width)
			{
				StartValueEditor();
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgConfig, DataContext.Name);
				case ColumnId.Value:
					return ConfigParameterValueColumn.OnMeasureSubItem(measureEventArgs, DataContext.Value);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgConfig, DataContext.Name);
					break;
				case ColumnId.Value:
					ConfigParameterValueColumn.OnPaintSubItem(paintEventArgs, DataContext.Value);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new ConfigParameterMenu(this);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		#endregion
	}
}

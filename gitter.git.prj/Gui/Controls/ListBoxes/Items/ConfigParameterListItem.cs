namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="ConfigParameter"/> object.</summary>
	public sealed class ConfigParameterListItem : CustomListBoxItem<ConfigParameter>
	{
		private static readonly Bitmap ImgConfig = CachedResources.Bitmaps["ImgConfig"];

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
			catch
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
			catch
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
			if(parameter == null) throw new ArgumentNullException("parameter");
		}

		#endregion

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

		private void OnDeleted(object sender, EventArgs e)
		{
			RemoveSafe();
		}

		private void OnValueChanged(object sender, EventArgs e)
		{
			InvalidateSafe();
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

		private void OnEditorValidating(object sender, CancelEventArgs e)
		{
			var editor = (CustomListBoxTextEditor)sender;
			editor.Validating -= OnEditorValidating;
			DataContext.Value = editor.Text.Trim();
		}

		public void StartValueEditor()
		{
			var editor = StartTextEditor(ListBox.Columns.GetById((int)ColumnId.Value), DataContext.Value);
			editor.Validating += OnEditorValidating;
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
	}
}

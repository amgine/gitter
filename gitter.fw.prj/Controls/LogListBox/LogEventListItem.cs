namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Services;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class LogEventListItem : CustomListBoxItem<LogEvent>
	{
		public LogEventListItem(LogEvent logEvent)
			: base(logEvent)
		{
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((LogListBoxColumnId)measureEventArgs.SubItemId)
			{
				case LogListBoxColumnId.Type:
					return new Size(16, 16);
				case LogListBoxColumnId.Timestamp:
					return measureEventArgs.MeasureText(DataContext.Timestamp.FormatISO8601());
				case LogListBoxColumnId.Source:
					return measureEventArgs.MeasureText(DataContext.Source);
				case LogListBoxColumnId.Message:
					return measureEventArgs.MeasureText(DataContext.Message);
				case LogListBoxColumnId.Exception:
					return new Size(16, 16);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((LogListBoxColumnId)paintEventArgs.SubItemId)
			{
				case LogListBoxColumnId.Type:
					paintEventArgs.PaintImage(DataContext.Type.Image);
					break;
				case LogListBoxColumnId.Timestamp:
					paintEventArgs.PaintText(DataContext.Timestamp.FormatISO8601());
					break;
				case LogListBoxColumnId.Source:
					paintEventArgs.PaintText(DataContext.Source);
					break;
				case LogListBoxColumnId.Message:
					paintEventArgs.PaintText(DataContext.Message);
					break;
				case LogListBoxColumnId.Exception:
					paintEventArgs.PaintImage(null);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new ContextMenuStrip();
			menu.Items.Add(new ToolStripMenuItem("Copy to Clipboard", null, (s, e) => Clipboard.SetText(DataContext.Message)));
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}

namespace gitter.Redmine.Gui
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	internal static class RedmineGuiUtility
	{
		private static void PaintGrayText(string text, SubItemPaintEventArgs paintEventArgs)
		{
			var style = paintEventArgs.ListBox.Style;
			if((paintEventArgs.State & ItemState.Selected) == ItemState.Selected && style.Type == GitterStyleType.DarkBackground)
			{
				paintEventArgs.PaintText(text, paintEventArgs.Brush);
			}
			else
			{
				using(var brush = new SolidBrush(style.Colors.GrayText))
				{
					paintEventArgs.PaintText(text, brush);
				}
			}
		}

		public static void PaintOptionalContent(NamedRedmineObject data, SubItemPaintEventArgs paintEventArgs)
		{
			if(data == null)
			{
				PaintGrayText(Resources.StrsUnassigned.SurroundWith('<', '>'), paintEventArgs);
			}
			else
			{
				paintEventArgs.PaintText(data.Name, paintEventArgs.Brush);
			}
		}

		public static void PaintOptionalContent(DateTime? date, SubItemPaintEventArgs paintEventArgs)
		{
			if(!date.HasValue)
			{
				PaintGrayText(Resources.StrsUnassigned.SurroundWith('<', '>'), paintEventArgs);
			}
			else
			{
				DateColumn.OnPaintSubItem(paintEventArgs, date.Value);
			}
		}

		public static void PaintOptionalContent(string data, SubItemPaintEventArgs paintEventArgs)
		{
			if(data == null)
			{
				PaintGrayText(Resources.StrsUnassigned.SurroundWith('<', '>'), paintEventArgs);
			}
			else
			{
				paintEventArgs.PaintText(data, paintEventArgs.Brush);
			}
		}
	}
}

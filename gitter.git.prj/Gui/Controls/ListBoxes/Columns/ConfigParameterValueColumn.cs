namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>"Value" column.</summary>
	public class ConfigParameterValueColumn : CustomListBoxColumn
	{
		public ConfigParameterValueColumn(bool visible)
			: base((int)ColumnId.Value, Resources.StrValue, visible)
		{
			Width = 250;
			SizeMode = ColumnSizeMode.Fill;
		}

		public ConfigParameterValueColumn()
			: this(true)
		{
		}

		public static Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs, string value)
		{
			return measureEventArgs.MeasureText(value);
		}

		public static void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs, string value)
		{
			paintEventArgs.PaintText(value);
		}

		public override string IdentificationString
		{
			get { return "Value"; }
		}
	}
}

namespace gitter.Framework.Options
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Framework.Properties.Resources;

	public sealed class OptionsListBox : CustomListBox
	{
		public OptionsListBox()
		{
			Columns.Add(new CustomListBoxColumn(0, Resources.StrName) { SizeMode = ColumnSizeMode.Fill });
			HeaderStyle = gitter.Framework.Controls.HeaderStyle.Hidden;
			ShowTreeLines = true;
			ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick;

			Items.AddRange(GlobalOptions.GetListBoxItems());
		}
	}
}

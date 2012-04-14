namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Framework.Properties.Resources;

	internal sealed class FontsListBox : CustomListBox
	{
		public FontsListBox()
		{
			Columns.Add(new CustomListBoxColumn(0, Resources.StrName, true) { SizeMode = ColumnSizeMode.Fill });
			Columns.Add(new CustomListBoxColumn(1, Resources.StrFont, true) { Width = 200 });

			if(GitterApplication.FontManager != null)
			{
				foreach(var f in GitterApplication.FontManager)
				{
					Items.Add(new FontListItem(f));
				}
			}

			ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick;
		}
	}
}

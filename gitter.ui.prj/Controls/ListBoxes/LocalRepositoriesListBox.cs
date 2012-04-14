namespace gitter
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	internal sealed class LocalRepositoriesListBox : CustomListBox
	{
		public LocalRepositoriesListBox()
		{
			Columns.Add(new CustomListBoxColumn(1, Resources.StrName, true)
			{
				SizeMode = ColumnSizeMode.Fill
			});

			HeaderStyle = HeaderStyle.Hidden;
			ItemHeight = 36;
			AllowDrop = true;
		}
	}
}

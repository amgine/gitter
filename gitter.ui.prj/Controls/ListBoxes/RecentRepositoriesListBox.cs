namespace gitter
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	internal sealed class RecentRepositoriesListBox : CustomListBox
	{
		public RecentRepositoriesListBox()
		{
			Columns.Add(new CustomListBoxColumn(0, Resources.StrName, true)
			{
				SizeMode = ColumnSizeMode.Fill
			});

			HeaderStyle = HeaderStyle.Hidden;
		}
	}
}

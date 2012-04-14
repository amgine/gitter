namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class DescriptionColumn : CustomListBoxColumn
	{
		public DescriptionColumn()
			: base((int)ColumnId.Description, Resources.StrDescription, true)
		{
			SizeMode = ColumnSizeMode.Fill;
		}
	}
}

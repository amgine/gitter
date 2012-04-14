namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class NameColumn : CustomListBoxColumn
	{
		public NameColumn()
			: base((int)ColumnId.Name, Resources.StrName, true)
		{
			Width = 150;
		}
	}
}

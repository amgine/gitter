namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class DateColumn : CustomListBoxColumn
	{
		public DateColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
			Width = 106;
		}
	}
}

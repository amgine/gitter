namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class ProjectIdentifierColumn : DateColumn
	{
		public ProjectIdentifierColumn()
			: base((int)ColumnId.Identifier, Resources.StrIdentifier, false)
		{
		}
	}
}

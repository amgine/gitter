namespace gitter.Redmine.Gui.ListBoxes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	public class VersionsListBox : CustomListBox
	{
		public VersionsListBox()
		{
			Columns.AddRange(
				new CustomListBoxColumn[]
				{
					new VersionIdColumn(),
					new VersionNameColumn(),
					new VersionStatusColumn(),
					new VersionDescriptionColumn(),
					new VersionCreatedOnColumn(),
					new VersionUpdatedOnColumn(),
					new VersionDueDateColumn(),
				});
		}
	}
}

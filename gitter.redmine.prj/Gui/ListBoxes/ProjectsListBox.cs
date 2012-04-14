namespace gitter.Redmine.Gui.ListBoxes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	public class ProjectsListBox : CustomListBox
	{
		public ProjectsListBox()
		{
			Columns.AddRange(new CustomListBoxColumn[]
				{
					new IssueIdColumn(),
					new NameColumn(),
					new ProjectIdentifierColumn(),
					new DescriptionColumn(),
					new IssueCreatedOnColumn(),
					new IssueUpdatedOnColumn(),
				});
		}
	}
}

namespace gitter.Redmine.Gui.ListBoxes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	public class IssuesListBox : CustomListBox
	{
		public IssuesListBox()
		{
			Columns.AddRange(
				new CustomListBoxColumn[]
				{
					new IssueIdColumn(),
					new IssueCreatedOnColumn(),
					new IssueUpdatedOnColumn(),
					new IssueTrackerColumn(),
					new IssueStatusColumn(),
					new IssuePriorityColumn(),
					new IssueSubjectColumn(),
					new IssueAuthorColumn(),
					new IssueAssignedToColumn(),
					new IssueCategoryColumn(),
					new IssueStartDateColumn(),
					new IssueDueDateColumn(),
					new IssueDoneRatioColumn(),
				});
		}
	}
}

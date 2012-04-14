namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueTrackerColumn : CustomListBoxColumn
	{
		public IssueTrackerColumn()
			: base((int)ColumnId.Tracker, Resources.StrTracker, true)
		{
			Width = 60;
		}

		public override string IdentificationString
		{
			get { return "Tracker"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByTracker; }
		}
	}
}

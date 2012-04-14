namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssuePriorityColumn : CustomListBoxColumn
	{
		public IssuePriorityColumn()
			: base((int)ColumnId.Priority, Resources.StrPriority, true)
		{
			Width = 80;
		}

		public override string IdentificationString
		{
			get { return "Priority"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByPriority; }
		}
	}
}

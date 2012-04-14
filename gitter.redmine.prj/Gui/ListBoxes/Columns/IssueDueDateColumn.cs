namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class IssueDueDateColumn : DateColumn
	{
		public IssueDueDateColumn()
			: base((int)ColumnId.DueDate, Resources.StrDueDate, false)
		{
			Width = 55;
		}

		public override string IdentificationString
		{
			get { return "DueDate"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByDueDate; }
		}
	}
}

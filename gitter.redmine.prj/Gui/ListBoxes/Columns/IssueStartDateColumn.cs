namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class IssueStartDateColumn : DateColumn
	{
		public IssueStartDateColumn()
			: base((int)ColumnId.StartDate, Resources.StrStartDate, false)
		{
			Width = 55;
		}

		public override string IdentificationString
		{
			get { return "StartDate"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByStartDate; }
		}
	}
}

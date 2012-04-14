namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueStatusColumn : CustomListBoxColumn
	{
		public IssueStatusColumn()
			: base((int)ColumnId.Status, Resources.StrStatus, true)
		{
			Width = 60;
		}

		public override string IdentificationString
		{
			get { return "Status"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByStatus; }
		}
	}
}

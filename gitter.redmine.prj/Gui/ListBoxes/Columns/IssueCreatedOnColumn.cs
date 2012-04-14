namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class IssueCreatedOnColumn : DateColumn
	{
		public IssueCreatedOnColumn()
			: base((int)ColumnId.CreatedOn, Resources.StrCreatedOn, false)
		{
		}

		public override string IdentificationString
		{
			get { return "CreatedOn"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByCreatedOn; }
		}
	}
}

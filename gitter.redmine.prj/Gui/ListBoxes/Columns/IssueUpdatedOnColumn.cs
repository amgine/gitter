namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class IssueUpdatedOnColumn : DateColumn
	{
		public IssueUpdatedOnColumn()
			: base((int)ColumnId.UpdatedOn, Resources.StrUpdatedOn, false)
		{
		}

		public override string IdentificationString
		{
			get { return "UpdatedOn"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByUpdatedOn; }
		}
	}
}

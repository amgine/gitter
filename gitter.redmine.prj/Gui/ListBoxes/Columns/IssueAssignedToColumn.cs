namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueAssignedToColumn : CustomListBoxColumn
	{
		public IssueAssignedToColumn()
			: base((int)ColumnId.AssignedTo, Resources.StrAssignedTo, true)
		{
			Width = 100;
		}

		public override string IdentificationString
		{
			get { return "AssignedTo"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByAssignedTo; }
		}
	}
}

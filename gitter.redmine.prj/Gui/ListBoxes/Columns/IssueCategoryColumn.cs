namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueCategoryColumn : CustomListBoxColumn
	{
		public IssueCategoryColumn()
			: base((int)ColumnId.Category, Resources.StrCategory, true)
		{
			Width = 60;
		}

		public override string IdentificationString
		{
			get { return "Category"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByCategory; }
		}
	}
}

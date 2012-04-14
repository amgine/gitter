namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueAuthorColumn : CustomListBoxColumn
	{
		public IssueAuthorColumn()
			: base((int)ColumnId.Author, Resources.StrAuthor, false)
		{
			Width = 100;
		}

		public override string IdentificationString
		{
			get { return "Author"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByAuthor; }
		}
	}
}

namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class NewsAuthorColumn : CustomListBoxColumn
	{
		public NewsAuthorColumn()
			: base((int)ColumnId.Author, Resources.StrAuthor, true)
		{
			Width = 100;
		}

		public override string IdentificationString
		{
			get { return "Author"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return NewsListItem.CompareByAuthor; }
		}
	}
}

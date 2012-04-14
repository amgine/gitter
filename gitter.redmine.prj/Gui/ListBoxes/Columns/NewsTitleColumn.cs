namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class NewsTitleColumn : CustomListBoxColumn
	{
		public NewsTitleColumn()
			: base((int)ColumnId.Title, Resources.StrTitle, true)
		{
			SizeMode = ColumnSizeMode.Fill;
		}

		public override string IdentificationString
		{
			get { return "Title"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return NewsListItem.CompareByTitle; }
		}
	}
}

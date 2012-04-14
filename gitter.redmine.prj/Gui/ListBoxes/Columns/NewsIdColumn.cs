namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class NewsIdColumn : CustomListBoxColumn
	{
		public NewsIdColumn()
			: base((int)ColumnId.Id, "#", true)
		{
			Width = 35;
		}

		public override string IdentificationString
		{
			get { return "Id"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return NewsListItem.CompareById; }
		}
	}
}

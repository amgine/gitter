namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueIdColumn : CustomListBoxColumn
	{
		public IssueIdColumn()
			: base((int)ColumnId.Id, "#", true)
		{
			Width = 55;
		}

		public override string IdentificationString
		{
			get { return "Id"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareById; }
		}
	}
}

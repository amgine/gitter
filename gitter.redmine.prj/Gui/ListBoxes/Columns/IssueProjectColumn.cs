namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueProjectColumn : CustomListBoxColumn
	{
		public IssueProjectColumn()
			: base((int)ColumnId.Project, Resources.StrProject, false)
		{
			Width = 80;
		}

		public override string IdentificationString
		{
			get { return "Project"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByProject; }
		}
	}
}

namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueDoneRatioColumn : CustomListBoxColumn
	{
		public IssueDoneRatioColumn()
			: base((int)ColumnId.DoneRatio, Resources.StrDoneRation, true)
		{
			Width = 60;
		}

		public override string IdentificationString
		{
			get { return "DoneRatio"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareByDoneRatio; }
		}
	}
}

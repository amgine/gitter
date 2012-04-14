namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	sealed class IssueSubjectColumn : CustomListBoxColumn
	{
		public IssueSubjectColumn()
			: base((int)ColumnId.Subject, Resources.StrSubject, true)
		{
			SizeMode = ColumnSizeMode.Fill;
		}

		public override string IdentificationString
		{
			get { return "Subject"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return IssueListItem.CompareBySubject; }
		}
	}
}

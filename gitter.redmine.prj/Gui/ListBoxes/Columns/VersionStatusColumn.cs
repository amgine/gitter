namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class VersionStatusColumn : CustomListBoxColumn
	{
		public VersionStatusColumn()
			: base((int)ColumnId.Status, Resources.StrStatus, true)
		{
			Width = 45;
		}

		public override string IdentificationString
		{
			get { return "Status"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return VersionListItem.CompareByStatus; }
		}
	}
}

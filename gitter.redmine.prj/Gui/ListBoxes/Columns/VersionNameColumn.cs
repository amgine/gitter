namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class VersionNameColumn : CustomListBoxColumn
	{
		public VersionNameColumn()
			: base((int)ColumnId.Name, Resources.StrName, true)
		{
			Width = 75;
		}

		public override string IdentificationString
		{
			get { return "Name"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return VersionListItem.CompareByName; }
		}
	}
}

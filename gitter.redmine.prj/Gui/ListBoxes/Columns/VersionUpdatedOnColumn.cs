namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public class VersionUpdatedOnColumn : DateColumn
	{
		public VersionUpdatedOnColumn()
			: base((int)ColumnId.UpdatedOn, Resources.StrUpdatedOn, false)
		{
		}

		public override string IdentificationString
		{
			get { return "UpdatedOn"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return VersionListItem.CompareByUpdatedOn; }
		}
	}
}

namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class VersionDescriptionColumn : CustomListBoxColumn
	{
		public VersionDescriptionColumn()
			: base((int)ColumnId.Description, Resources.StrDescription, true)
		{
			SizeMode = ColumnSizeMode.Fill;
		}

		public override string IdentificationString
		{
			get { return "Description"; }
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return VersionListItem.CompareByDescription; }
		}
	}
}

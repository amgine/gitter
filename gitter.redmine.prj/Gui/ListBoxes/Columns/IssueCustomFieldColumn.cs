namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class IssueCustomFieldColumn : CustomListBoxColumn
	{
		private readonly CustomField _field;

		public IssueCustomFieldColumn(CustomField field)
			: base((int)ColumnId.CustomFieldOffset + field.Id, field.Name, false)
		{
			_field = field;
			Width = 100;
		}

		public override string IdentificationString
		{
			get { return Name; }
		}

		private int Compare(IssueListItem item1, IssueListItem item2)
		{
			var data1 = item1.DataContext.CustomFields[_field];
			var data2 = item2.DataContext.CustomFields[_field];
			if(data1 == data2) return 0;
			if(data1 == null) return 1;
			else if(data2 == null) return -1;
			return string.Compare(data1, data2);
		}

		private int Compare(CustomListBoxItem item1, CustomListBoxItem item2)
		{
			var i1 = item1 as IssueListItem;
			if(i1 == null) return 0;
			var i2 = item2 as IssueListItem;
			if(i2 == null) return 0;
			return Compare(i1, i2);
		}

		protected override Comparison<CustomListBoxItem> SortComparison
		{
			get { return Compare; }
		}
	}
}

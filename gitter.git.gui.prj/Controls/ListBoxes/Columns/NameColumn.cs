namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Name" column.</summary>
	public class NameColumn : CustomListBoxColumn
	{
		public NameColumn()
			: base((int)ColumnId.Name, Resources.StrName, true)
		{
			SizeMode = ColumnSizeMode.Fill;
		}

		public NameColumn(string name)
			: base((int)ColumnId.Name, name, true)
		{
			SizeMode = ColumnSizeMode.Fill;
		}

		public override int MinWidth
		{
			get { return 22; }
		}

		public override string IdentificationString
		{
			get { return "Name"; }
		}
	}
}

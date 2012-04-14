namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>"Path" column.</summary>
	public class PathColumn : CustomListBoxColumn
	{
		public PathColumn()
			: base((int)ColumnId.Path, Resources.StrPath, true)
		{
		}

		public PathColumn(int columnId, string name, bool visible)
			: base(columnId, name, visible)
		{
		}

		public override string IdentificationString
		{
			get { return "Path"; }
		}
	}
}

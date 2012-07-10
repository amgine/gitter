namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Url" column.</summary>
	public class UrlColumn : CustomListBoxColumn
	{
		public UrlColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
		}

		public UrlColumn()
			: this((int)ColumnId.Url, Resources.StrUrl, true)
		{
		}

		public override string IdentificationString
		{
			get { return "Url"; }
		}
	}
}

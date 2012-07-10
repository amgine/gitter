namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Fetch Url" column.</summary>
	public class FetchUrlColumn : UrlColumn
	{
		public FetchUrlColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
		}

		public FetchUrlColumn()
			: this((int)ColumnId.FetchUrl, Resources.StrFetchUrl, true)
		{
		}

		public override string IdentificationString
		{
			get { return "FetchUrl"; }
		}
	}
}

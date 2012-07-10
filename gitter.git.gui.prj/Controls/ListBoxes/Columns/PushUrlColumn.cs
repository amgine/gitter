namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Push Url" column.</summary>
	public class PushUrlColumn : UrlColumn
	{
		public PushUrlColumn(int id, string name, bool visible)
			: base(id, name, visible)
		{
		}

		public PushUrlColumn()
			: this((int)ColumnId.PushUrl, Resources.StrPushUrl, false)
		{
		}

		public override string IdentificationString
		{
			get { return "PushUrl"; }
		}
	}
}

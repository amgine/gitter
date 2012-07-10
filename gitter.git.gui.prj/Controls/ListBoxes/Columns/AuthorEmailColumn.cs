namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Author Email" column.</summary>
	public sealed class AuthorEmailColumn : EmailColumn
	{
		public AuthorEmailColumn(bool visible)
			: base((int)ColumnId.AuthorEmail, Resources.StrAuthorEmail, visible)
		{
		}

		public AuthorEmailColumn()
			: this(false)
		{
		}

		public override string IdentificationString
		{
			get { return "AuthorEmail"; }
		}
	}
}

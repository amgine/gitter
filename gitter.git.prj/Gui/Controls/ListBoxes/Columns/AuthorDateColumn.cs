namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>"Author Date" column.</summary>
	public sealed class AuthorDateColumn : DateColumn
	{
		public AuthorDateColumn()
			: base((int)ColumnId.AuthorDate, Resources.StrAuthorDate, false)
		{
			Width = 106;
		}

		public override string IdentificationString
		{
			get { return "AuthorDate"; }
		}
	}
}

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>"Author" column.</summary>
	public sealed class AuthorColumn : UserColumn
	{
		public AuthorColumn(bool visible)
			: base((int)ColumnId.Author, Resources.StrAuthor, visible)
		{
		}

		public AuthorColumn()
			: this(true)
		{
		}

		public override string IdentificationString
		{
			get { return "Author"; }
		}
	}
}

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>"Committer Email" column.</summary>
	public sealed class CommitterEmailColumn : EmailColumn
	{
		public CommitterEmailColumn(bool visible)
			: base((int)ColumnId.CommitterEmail, Resources.StrCommitterEmail, visible)
		{
		}

		public CommitterEmailColumn()
			: this(false)
		{
		}

		public override string IdentificationString
		{
			get { return "CommitterEmail"; }
		}
	}
}

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Committer" column.</summary>
	public sealed class CommitterColumn : UserColumn
	{
		public CommitterColumn(bool visible)
			: base((int)ColumnId.Committer, Resources.StrCommitter, visible)
		{
		}

		public CommitterColumn()
			: this(false)
		{
		}

		public override string IdentificationString
		{
			get { return "Committer"; }
		}
	}
}

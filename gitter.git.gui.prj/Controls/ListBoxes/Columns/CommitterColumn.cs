namespace gitter.Git.Gui.Controls
{
	using System;

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

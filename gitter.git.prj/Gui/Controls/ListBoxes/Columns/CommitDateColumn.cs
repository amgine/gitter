namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>"Commit Date" column.</summary>
	public sealed class CommitDateColumn : DateColumn
	{
		public CommitDateColumn()
			: base((int)ColumnId.CommitDate, Resources.StrCommitDate, true)
		{
			Width = 106;
		}

		public override string IdentificationString
		{
			get { return "CommitDate"; }
		}
	}
}

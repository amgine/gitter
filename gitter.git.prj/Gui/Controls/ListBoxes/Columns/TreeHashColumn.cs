namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>"Tree Hash" column.</summary>
	public sealed class TreeHashColumn : HashColumn
	{
		/// <summary>Create <see cref="TreeHashColumn"/>.</summary>
		public TreeHashColumn()
			: base((int)ColumnId.TreeHash, Resources.StrTreeHash, false)
		{
		}

		public override string IdentificationString
		{
			get { return "TreeHash"; }
		}
	}
}

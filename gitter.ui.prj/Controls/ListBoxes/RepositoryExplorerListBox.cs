namespace gitter
{
	using System;

	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	public sealed class RepositoryExplorerListBox : CustomListBox
	{
		private readonly CustomListBoxColumn _nameColumn;

		/// <summary>Create <see cref="RepositoryExplorerListBox"/>.</summary>
		public RepositoryExplorerListBox()
		{
			_nameColumn = new CustomListBoxColumn(0, Resources.StrName)
			{
				SizeMode = ColumnSizeMode.Auto
			};
			Columns.Add(_nameColumn);

			ShowTreeLines = true;
			HeaderStyle = HeaderStyle.Hidden;
			ShowRootTreeLines = false;
		}
	}
}

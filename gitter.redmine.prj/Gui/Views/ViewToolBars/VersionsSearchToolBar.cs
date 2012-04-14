namespace gitter.Redmine.Gui
{
	using System;
	using System.ComponentModel;

	[ToolboxItem(false)]
	internal sealed class VersionsSearchToolBar : SearchToolBar<VersionsView, VersionsSearchOptions>
	{
		public VersionsSearchToolBar(VersionsView view)
			: base(view)
		{
		}

		protected override VersionsSearchOptions CreateSearchOptions()
		{
			return new VersionsSearchOptions()
			{
				Text = SearchText,
			};
		}
	}
}

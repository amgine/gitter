namespace gitter.Redmine.Gui
{
	using System;
	using System.ComponentModel;

	[ToolboxItem(false)]
	internal sealed class IssuesSearchToolBar : SearchToolBar<IssuesView, IssuesSearchOptions>
	{
		public IssuesSearchToolBar(IssuesView view)
			: base(view)
		{
		}

		protected override IssuesSearchOptions CreateSearchOptions()
		{
			return new IssuesSearchOptions()
			{
				Text = SearchText,
			};
		}
	}
}

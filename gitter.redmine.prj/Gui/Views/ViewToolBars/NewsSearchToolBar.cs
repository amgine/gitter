namespace gitter.Redmine.Gui
{
	using System;
	using System.ComponentModel;

	[ToolboxItem(false)]
	internal sealed class NewsSearchToolBar : SearchToolBar<NewsView, NewsSearchOptions>
	{
		public NewsSearchToolBar(NewsView view)
			: base(view)
		{
		}

		protected override NewsSearchOptions CreateSearchOptions()
		{
			return new NewsSearchOptions()
			{
				Text = SearchText,
			};
		}
	}
}

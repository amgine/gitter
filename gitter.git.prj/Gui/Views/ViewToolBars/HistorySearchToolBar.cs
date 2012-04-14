namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;

	[ToolboxItem(false)]
	internal sealed class HistorySearchToolBar : SearchToolBar<HistoryView, HistorySearchOptions>
	{
		public HistorySearchToolBar(HistoryView tool)
			: base(tool)
		{
		}

		protected override HistorySearchOptions CreateSearchOptions()
		{
			return new HistorySearchOptions()
			{
				Text = SearchText,
			};
		}
	}
}

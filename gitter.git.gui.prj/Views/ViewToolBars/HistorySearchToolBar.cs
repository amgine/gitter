namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;

	using gitter.Framework.Controls;

	[ToolboxItem(false)]
	internal sealed class HistorySearchToolBar<T> : SearchToolBar<T, HistorySearchOptions>
		where T : GitViewBase, ISearchableView<HistorySearchOptions>
	{
		public HistorySearchToolBar(T view)
			: base(view)
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

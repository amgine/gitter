namespace gitter.Git.Gui.Views
{
	using System;
	using System.ComponentModel;

	using gitter.Framework.Controls;

	[ToolboxItem(false)]
	internal sealed class ReflogSearchToolBar<T> : SearchToolBar<T, ReflogSearchOptions>
		where T : GitViewBase, ISearchableView<ReflogSearchOptions>
	{
		public ReflogSearchToolBar(T view)
			: base(view)
		{
		}

		protected override ReflogSearchOptions CreateSearchOptions()
		{
			return new ReflogSearchOptions()
			{
				Text		= SearchText,
				MatchCase	= MatchCase,
			};
		}
	}
}

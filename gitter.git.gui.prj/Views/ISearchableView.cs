namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;

	interface ISearchableView<T>
		where T : SearchOptions
	{
		ISearch<T> Search { get; }

		bool SearchToolBarVisible { get; set; }
	}
}

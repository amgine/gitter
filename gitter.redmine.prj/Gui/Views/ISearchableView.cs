namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;

	interface ISearchableView<T>
		where T : SearchOptions
	{
		bool SearchToolBarVisible { get; set; }

		bool SearchFirst(T options);

		bool SearchNext(T options);

		bool SearchPrevious(T options);
	}
}

namespace gitter.Git.Gui.Views
{
	interface ISearch<T>
		where T : SearchOptions
	{
		bool Current(T search);

		bool First(T options);

		bool Next(T options);

		bool Previous(T options);
	}
}

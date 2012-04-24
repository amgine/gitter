namespace gitter.Git.Gui
{

	using gitter.Framework;

	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Properties.Resources;

	sealed class RepositoryHistoryListItem : RepositoryExplorerItemBase
	{
		private readonly IWorkingEnvironment _environment;

		public RepositoryHistoryListItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgHistory"], Resources.StrHistory)
		{
			if(environment == null) throw new System.ArgumentNullException("environment");

			_environment = environment;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			_environment.ViewDockService.ShowView(Guids.HistoryViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}
	}
}

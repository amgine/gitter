namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class RepositoryContributorsListItem : RepositoryExplorerItemBase
	{
		private readonly IWorkingEnvironment _environment;

		public RepositoryContributorsListItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgUsers"], Resources.StrContributors)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_environment = environment;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			_environment.ViewDockService.ShowView(Guids.ContributorsViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}
	}
}

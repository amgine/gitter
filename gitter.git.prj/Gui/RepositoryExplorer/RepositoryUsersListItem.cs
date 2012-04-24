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

	using Resources = gitter.Git.Properties.Resources;

	sealed class RepositoryUsersListItem : RepositoryExplorerItemBase
	{
		private readonly IWorkingEnvironment _environment;

		public RepositoryUsersListItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgUsers"], Resources.StrUsers)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			_environment = environment;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			_environment.ViewDockService.ShowView(Guids.UsersViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}
	}
}

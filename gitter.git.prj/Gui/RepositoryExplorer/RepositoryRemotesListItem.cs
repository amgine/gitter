namespace gitter.Git.Gui
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Properties.Resources;

	sealed class RepositoryRemotesListItem : RepositoryExplorerItemBase
	{
		private readonly IWorkingEnvironment _environment;
		private RemoteListBinding _binding;

		public RepositoryRemotesListItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgRemotes"], Resources.StrRemotes)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			_environment = environment;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			_environment.ViewDockService.ShowView(Guids.RemotesViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}

		protected override void DetachFromRepository()
		{
			_binding.Dispose();
			_binding = null;
		}

		protected override void AttachToRepository()
		{
			_binding = new RemoteListBinding(Items, Repository);
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(Repository != null)
			{
				var menu = new RemotesMenu(Repository);
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}
			else
			{
				return null;
			}
		}
	}
}

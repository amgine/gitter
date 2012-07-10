namespace gitter.Git.Gui
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class RepositorySubmodulesListItem : RepositoryExplorerItemBase
	{
		private readonly IWorkingEnvironment _environment;
		private SubmoduleListBinding _binding;

		public RepositorySubmodulesListItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgSubmodules"], Resources.StrSubmodules)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			_environment = environment;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			_environment.ViewDockService.ShowView(Guids.SubmodulesViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}

		protected override void DetachFromRepository()
		{
			_binding.Dispose();
			_binding = null;
			Collapse();
		}

		protected override void AttachToRepository()
		{
			_binding = new SubmoduleListBinding(Items, Repository);
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(Repository != null)
			{
				var menu = new SubmodulesMenu(Repository);
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

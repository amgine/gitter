namespace gitter.Git.Gui
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class RepositoryReferencesListItem : RepositoryExplorerItemBase
	{
		private readonly IWorkingEnvironment _environment;
		private ReferenceTreeBinding _refsBinding;

		public RepositoryReferencesListItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgBranch"], Resources.StrReferences)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			_environment = environment;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			_environment.ViewDockService.ShowView(Guids.ReferencesViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}

		protected override void AttachToRepository()
		{
			_refsBinding = new ReferenceTreeBinding(Items, Repository, true, true, null,
				ReferenceType.LocalBranch | ReferenceType.RemoteBranch | ReferenceType.Tag);
			_refsBinding.ReferenceItemActivated += OnReferenceItemActivated;
		}

		protected override void DetachFromRepository()
		{
			_refsBinding.ReferenceItemActivated -= OnReferenceItemActivated;
			_refsBinding.Dispose();
			_refsBinding = null;
			Collapse();
		}

		private void OnReferenceItemActivated(object sender, RevisionPointerEventArgs e)
		{
			var rev = e.Object;
			var view = (HistoryView)_environment.ViewDockService.ShowView(Guids.HistoryViewGuid, false);
			view.SelectRevision(rev);
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(Repository != null && !Repository.IsEmpty)
			{
				var menu = new ReferencesMenu(Repository);
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

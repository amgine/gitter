namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	sealed class RepositoryExplorerViewFactory : ViewFactoryBase
	{
		private readonly RepositoryRootItem _rootItem;

		public RepositoryExplorerViewFactory(IWorkingEnvironment environment)
			: base(Guids.RepositoryExplorerView, Resources.StrRepositoryExplorer, CachedResources.Bitmaps["ImgRepositoryExplorer"], true)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			_rootItem = new RepositoryRootItem(environment, null);
			DefaultViewPosition = ViewPosition.Left;
		}

		public RepositoryRootItem RootItem
		{
			get { return _rootItem; }
		}

		public void AddItem(CustomListBoxItem item)
		{
			if(item == null) throw new ArgumentNullException("item");
			_rootItem.Items.Add(item);
		}

		public void RemoveItem(CustomListBoxItem item)
		{
			_rootItem.Items.Remove(item);
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			var view = new RepositoryExplorerView(environment, parameters);
			view.AddItem(_rootItem);
			return view;
		}
	}

	sealed class StartPageViewFactory : ViewFactoryBase
	{
		public StartPageViewFactory()
			: base(Guids.StartPageView, Resources.StrStartPage, CachedResources.Bitmaps["ImgStartPage"], true)
		{
			DefaultViewPosition = ViewPosition.RootDocumentHost;
			ShowOnStartup = true;
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new StartPageView(environment, parameters, this);
		}

		public bool CloseAfterRepositoryLoad
		{
			get;
			set;
		}

		public bool ShowOnStartup
		{
			get;
			set;
		}
	}
}

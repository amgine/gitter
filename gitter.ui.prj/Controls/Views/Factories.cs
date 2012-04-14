namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	sealed class RepositoryExplorerViewFactory : ViewFactory
	{
		private readonly IWorkingEnvironment _environment;
		private readonly RepositoryRootItem _rootItem;

		public RepositoryExplorerViewFactory(IWorkingEnvironment environment)
			: base(Guids.RepositoryExplorerView, Resources.StrRepositoryExplorer, CachedResources.Bitmaps["ImgRepositoryExplorer"], true)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			_environment = environment;
			_rootItem = new RepositoryRootItem(environment, null);
			DefaultViewPosition = ViewPosition.LeftTool;
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

		protected override ViewBase CreateViewCore(IDictionary<string, object> parameters)
		{
			var tool = new RepositoryExplorerView(parameters, _environment);
			tool.AddItem(_rootItem);
			return tool;
		}
	}

	sealed class StartPageViewFactory : ViewFactory
	{
		private readonly IWorkingEnvironment _environment;

		public StartPageViewFactory(IWorkingEnvironment environment)
			: base(Guids.StartPageView, Resources.StrStartPage, CachedResources.Bitmaps["ImgStartPage"], true)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			_environment = environment;
			DefaultViewPosition = ViewPosition.RootDocumentHost;
			ShowOnStartup = true;
		}

		protected override ViewBase CreateViewCore(IDictionary<string, object> parameters)
		{
			return new StartPageView(parameters, _environment, this);
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

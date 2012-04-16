namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	sealed class CommitViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public CommitViewFactory(GuiProvider guiProvider)
			: base(Guids.CommitViewGuid, Resources.StrCommit, CachedResources.Bitmaps["ImgCommit"])
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new CommitView(parameters, _guiProvider);
		}
	}

	sealed class ConfigViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ConfigViewFactory(GuiProvider guiProvider)
			: base(Guids.ConfigViewGuid, Resources.StrConfig, CachedResources.Bitmaps["ImgConfiguration"])
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new ConfigView(parameters, _guiProvider);
		}
	}

	sealed class GitViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public GitViewFactory(GuiProvider guiProvider)
			: base(Guids.GitViewGuid, Resources.StrGit, CachedResources.Bitmaps["ImgGit"])
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new GitView(parameters, _guiProvider);
		}
	}

	sealed class HistoryViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public HistoryViewFactory(GuiProvider guiProvider)
			: base(Guids.HistoryViewGuid, Resources.StrHistory, CachedResources.Bitmaps["ImgHistory"], true)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new HistoryView(parameters, _guiProvider);
		}
	}

	sealed class ReflogViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ReflogViewFactory(GuiProvider guiProvider)
			: base(Guids.ReflogViewGuid, Resources.StrReflog, CachedResources.Bitmaps["ImgViewReflog"], false)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new ReflogView(parameters, _guiProvider);
		}
	}

	sealed class MaintenanceToolFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public MaintenanceToolFactory(GuiProvider guiProvider)
			: base(Guids.MaintenanceViewGuid, Resources.StrMaintenance, CachedResources.Bitmaps["ImgMaintenance"])
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new MaintenanceView(parameters, _guiProvider);
		}
	}

	sealed class ReferencesViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ReferencesViewFactory(GuiProvider guiProvider)
			: base(Guids.ReferencesViewGuid, Resources.StrReferences, CachedResources.Bitmaps["ImgBranch"], true)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.RootDocumentHost;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new ReferencesView(parameters, _guiProvider);
		}
	}

	sealed class RemotesViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public RemotesViewFactory(GuiProvider guiProvider)
			: base(Guids.RemotesViewGuid, Resources.StrRemotes, CachedResources.Bitmaps["ImgRemote"], true)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.BottomAutoHideTool;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new RemotesView(parameters, _guiProvider);
		}
	}

	sealed class StashViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public StashViewFactory(GuiProvider guiProvider)
			: base(Guids.StashViewGuid, Resources.StrStash, CachedResources.Bitmaps["ImgStash"], true)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.BottomAutoHideTool;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new StashView(parameters, _guiProvider);
		}
	}

	sealed class SubmodulesViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public SubmodulesViewFactory(GuiProvider guiProvider)
			: base(Guids.SubmodulesViewGuid, Resources.StrSubmodules, CachedResources.Bitmaps["ImgSubmodule"], true)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.BottomAutoHideTool;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new SubmodulesView(parameters, _guiProvider);
		}
	}

	sealed class UsersViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public UsersViewFactory(GuiProvider guiProvider)
			: base(Guids.UsersViewGuid, Resources.StrUsers, CachedResources.Bitmaps["ImgUsers"])
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new UsersView(parameters, _guiProvider);
		}
	}

	sealed class TreeViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public TreeViewFactory(GuiProvider guiProvider)
			: base(Guids.TreeViewGuid, Resources.StrWorkingTree, CachedResources.Bitmaps["ImgFolder"], false)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new TreeView(parameters, _guiProvider);
		}
	}

	sealed class DiffViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public DiffViewFactory(GuiProvider guiProvider)
			: base(Guids.DiffViewGuid, Resources.StrDiff, CachedResources.Bitmaps["ImgDiff"], false)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new DiffView(Guids.DiffViewGuid, parameters, _guiProvider);
		}
	}

	sealed class BlameViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public BlameViewFactory(GuiProvider guiProvider)
			: base(Guids.BlameViewGuid, Resources.StrBlame, CachedResources.Bitmaps["ImgBlame"], false)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new BlameView(parameters, _guiProvider);
		}
	}

	sealed class ContextualDiffViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ContextualDiffViewFactory(GuiProvider guiProvider)
			: base(Guids.ContextualDiffViewGuid, Resources.StrContextualDiff, CachedResources.Bitmaps["ImgDiff"], true)
		{
			if(guiProvider == null) throw new ArgumentNullException("guiProvider");
			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.SecondaryDocumentHost;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new DiffView(Guids.ContextualDiffViewGuid, parameters, _guiProvider);
		}
	}
}

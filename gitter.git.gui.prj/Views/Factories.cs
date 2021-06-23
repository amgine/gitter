#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.Gui.Views
{
	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class CommitViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public CommitViewFactory(GuiProvider guiProvider)
			: base(Guids.CommitViewGuid, Resources.StrCommit, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"commit"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new CommitView(_guiProvider);
	}

	sealed class ConfigViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ConfigViewFactory(GuiProvider guiProvider)
			: base(Guids.ConfigViewGuid, Resources.StrConfig, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"configuration"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new ConfigView(_guiProvider);
	}

	sealed class GitViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public GitViewFactory(GuiProvider guiProvider)
			: base(Guids.GitViewGuid, Resources.StrGit, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"git"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new GitView(_guiProvider);
	}

	sealed class HistoryViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public HistoryViewFactory(GuiProvider guiProvider)
			: base(Guids.HistoryViewGuid, Resources.StrHistory, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"history"), singleton: true)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new HistoryView(_guiProvider);
	}

	sealed class PathHistoryViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public PathHistoryViewFactory(GuiProvider guiProvider)
			: base(Guids.PathHistoryViewGuid, Resources.StrHistory, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"file.history"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new PathHistoryView(_guiProvider);
	}

	sealed class ReflogViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ReflogViewFactory(GuiProvider guiProvider)
			: base(Guids.ReflogViewGuid, Resources.StrReflog, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"branch.reflog"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new ReflogView(_guiProvider);
	}

	sealed class ReferencesViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ReferencesViewFactory(GuiProvider guiProvider)
			: base(Guids.ReferencesViewGuid, Resources.StrReferences, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"branch"), singleton: true)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.RootDocumentHost;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new ReferencesView(_guiProvider);
	}

	sealed class RemotesViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public RemotesViewFactory(GuiProvider guiProvider)
			: base(Guids.RemotesViewGuid, Resources.StrRemotes, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"remotes"), singleton: true)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.BottomAutoHide;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new RemotesView(_guiProvider);
	}

	sealed class RemoteViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public RemoteViewFactory(GuiProvider guiProvider)
			: base(Guids.RemoteViewGuid, Resources.StrRemote, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"remote"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.RootDocumentHost;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new RemoteView(_guiProvider);
	}

	sealed class StashViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public StashViewFactory(GuiProvider guiProvider)
			: base(Guids.StashViewGuid, Resources.StrStash, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"stash"), singleton: true)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.BottomAutoHide;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new StashView(_guiProvider);
	}

	sealed class SubmodulesViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public SubmodulesViewFactory(GuiProvider guiProvider)
			: base(Guids.SubmodulesViewGuid, Resources.StrSubmodules, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"submodules"), singleton: true)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.BottomAutoHide;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new SubmodulesView(_guiProvider);
	}

	sealed class ContributorsViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ContributorsViewFactory(GuiProvider guiProvider)
			: base(Guids.ContributorsViewGuid, Resources.StrContributors, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"users"), singleton: true)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new ContributorsView(_guiProvider);
	}

	sealed class TreeViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public TreeViewFactory(GuiProvider guiProvider)
			: base(Guids.TreeViewGuid, Resources.StrWorkingTree, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"folder"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new TreeView(_guiProvider);
	}

	sealed class DiffViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public DiffViewFactory(GuiProvider guiProvider)
			: base(Guids.DiffViewGuid, Resources.StrDiff, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"diff"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new DiffView(Guids.DiffViewGuid, _guiProvider);
	}

	sealed class BlameViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public BlameViewFactory(GuiProvider guiProvider)
			: base(Guids.BlameViewGuid, Resources.StrBlame, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"blame"))
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new BlameView(_guiProvider);
	}

	sealed class ContextualDiffViewFactory : ViewFactoryBase
	{
		private readonly GuiProvider _guiProvider;

		public ContextualDiffViewFactory(GuiProvider guiProvider)
			: base(Guids.ContextualDiffViewGuid, Resources.StrContextualDiff, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"diff"), singleton: true)
		{
			Verify.Argument.IsNotNull(guiProvider, nameof(guiProvider));

			_guiProvider = guiProvider;
			DefaultViewPosition = ViewPosition.SecondaryDocumentHost;
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
			=> new DiffView(Guids.ContextualDiffViewGuid, _guiProvider);
	}
}

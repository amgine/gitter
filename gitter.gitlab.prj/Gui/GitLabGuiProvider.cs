#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui
{
	using System;

	using gitter.Framework;
	using gitter.Framework.Configuration;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using gitter.Git;
	using gitter.Git.Gui.Controls;

	sealed class GitLabGuiProvider : IGuiProvider
	{
		private RepositoryExplorer _repositoryExplorer;

		private RegexHyperlinkExtractor IssueUrlExtractor { get; }

		private RegexHyperlinkExtractor MergeRequestUrlExtractor { get; }

		public GitLabGuiProvider(Repository repository, GitLabServiceContext serviceContext)
		{
			Repository     = repository;
			ServiceContext = serviceContext;

			IssueUrlExtractor = new RegexHyperlinkExtractor(
				@"(?<PROJ_NAME>[\w\/]+)\#(?<ID>\d+)", ServiceContext.ServiceUri + "%PROJ_NAME%/-/issues/%ID%");
			MergeRequestUrlExtractor = new RegexHyperlinkExtractor(
				@"(?<PROJ_NAME>[\w\/]+)\!(?<ID>\d+)", ServiceContext.ServiceUri + "%PROJ_NAME%/-/merge_requests/%ID%");
		}

		public Repository Repository { get; }

		public GitLabServiceContext ServiceContext { get; }

		private void OnDiffHeaderPanelsProviderCreatingPanels(object sender, CreatingPanelsEventArgs e)
		{
			switch(e.DiffSource)
			{
				case IRevisionDiffSource revisionSource:
					{
						foreach(var p in e.Panels)
						{
							if(p is RevisionHeaderPanel rhp)
							{
								rhp.AdditionalHyperlinkExtractors.Add(IssueUrlExtractor);
								rhp.AdditionalHyperlinkExtractors.Add(MergeRequestUrlExtractor);
								break;
							}
						}

						e.Panels.Add(new GitLabRevisionPanel(ServiceContext, revisionSource.Revision.Dereference()) { });
						e.Panels.Add(new FlowPanelSeparator { SeparatorStyle = FlowPanelSeparatorStyle.Line });
					}
					break;
			}
		}

		public void AttachToEnvironment(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));

			_repositoryExplorer = new RepositoryExplorer(environment, this);
			environment.ProvideRepositoryExplorerItem(_repositoryExplorer.RootItem);
			DiffHeaderPanelsProvider.CreatingPanels += OnDiffHeaderPanelsProviderCreatingPanels;
		}

		public void DetachFromEnvironment(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));

			DiffHeaderPanelsProvider.CreatingPanels -= OnDiffHeaderPanelsProviderCreatingPanels;
			//var views1 = environment.ViewDockService.FindViews(Guids.BuildTypeBuildsViewGuid).ToList();
			//foreach(var view in views1) view.Close();
			environment.RemoveRepositoryExplorerItem(_repositoryExplorer.RootItem);
			_repositoryExplorer = null;
		}

		public void SaveTo(Section section)
		{
		}

		public void LoadFrom(Section section)
		{
		}
	}
}

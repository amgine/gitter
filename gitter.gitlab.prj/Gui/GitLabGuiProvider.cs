#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System;
using System.Linq;

using Autofac;

using gitter.Framework;
using gitter.Framework.Configuration;
using gitter.Framework.Controls;
using gitter.Framework.Services;

using gitter.Git;
using gitter.Git.Gui.Controls;

sealed class GitLabGuiProvider : IGuiProvider
{
	private RepositoryExplorer _repositoryExplorer;
	private readonly IViewFactory[] _viewFactories;

	private RegexHyperlinkExtractor ShortIssueUrlExtractor { get; }

	private RegexHyperlinkExtractor FullIssueUrlExtractor { get; }

	private RegexHyperlinkExtractor ShortMergeRequestUrlExtractor { get; }

	private RegexHyperlinkExtractor FullMergeRequestUrlExtractor { get; }

	public GitLabGuiProvider(IComponentContext componentContext, Repository repository, GitLabServiceContext serviceContext)
	{
		Verify.Argument.IsNotNull(componentContext);

		const string AllowedProjectNameChars = @"\w\/\._\-";
		const string Prefix = @"(?:^|\s|\()";
		const string Suffix = @"(?:\s|\,|\.|\)|$)";

		Repository     = repository;
		ServiceContext = serviceContext;

		FullIssueUrlExtractor = new RegexHyperlinkExtractor(
			regexp:     Prefix + @"(?<LINK>(?<PROJ_NAME>[" + AllowedProjectNameChars + @"]+)\#(?<ID>\d+))" + Suffix,
			urlPattern: ServiceContext.ServiceUri + "%PROJ_NAME%/-/issues/%ID%",
			linkGroupName: @"LINK");

		ShortIssueUrlExtractor = new RegexHyperlinkExtractor(
			regexp:     Prefix + @"(?<LINK>\#(?<ID>\d+))" + Suffix,
			urlPattern: ServiceContext.ServiceUri + ServiceContext.DefaultProjectId.Name + "/-/issues/%ID%",
			linkGroupName: @"LINK");

		FullMergeRequestUrlExtractor = new RegexHyperlinkExtractor(
			regexp:     Prefix + @"(?<LINK>(?<PROJ_NAME>[" + AllowedProjectNameChars + @"]+)\!(?<ID>\d+))" + Suffix,
			urlPattern: ServiceContext.ServiceUri + "%PROJ_NAME%/-/merge_requests/%ID%",
			linkGroupName: @"LINK");

		ShortMergeRequestUrlExtractor = new RegexHyperlinkExtractor(
			regexp:     Prefix + @"(?<LINK>\!(?<ID>\d+))" + Suffix,
			urlPattern: ServiceContext.ServiceUri + ServiceContext.DefaultProjectId.Name + "/-/merge_requests/%ID%",
			linkGroupName: @"LINK");

		_viewFactories = componentContext.ResolveNamed<IViewFactory[]>(@"gitlab", TypedParameter.From(this));
	}

	public Repository Repository { get; }

	public GitLabServiceContext ServiceContext { get; }

	private void OnDiffHeaderPanelsProviderCreatingPanels(object sender, CreatingPanelsEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.DiffSource)
		{
			case IRevisionDiffSource revisionSource:
				{
					foreach(var p in e.Panels)
					{
						if(p is RevisionHeaderPanel rhp)
						{
							rhp.AdditionalHyperlinkExtractors.Add(FullIssueUrlExtractor);
							rhp.AdditionalHyperlinkExtractors.Add(ShortIssueUrlExtractor);
							rhp.AdditionalHyperlinkExtractors.Add(FullMergeRequestUrlExtractor);
							rhp.AdditionalHyperlinkExtractors.Add(ShortMergeRequestUrlExtractor);
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
		Verify.Argument.IsNotNull(environment);

		foreach(var factory in _viewFactories)
		{
			environment.ViewDockService.RegisterFactory(factory);
		}

		_repositoryExplorer = new RepositoryExplorer(environment, this);
		environment.ProvideRepositoryExplorerItem(_repositoryExplorer.RootItem);
		DiffHeaderPanelsProvider.CreatingPanels += OnDiffHeaderPanelsProviderCreatingPanels;
	}

	public void DetachFromEnvironment(IWorkingEnvironment environment)
	{
		Verify.Argument.IsNotNull(environment);

		foreach(var factory in _viewFactories)
		{
			factory.CloseAllViews();
			environment.ViewDockService.UnregisterFactory(factory);
		}

		DiffHeaderPanelsProvider.CreatingPanels -= OnDiffHeaderPanelsProviderCreatingPanels;
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

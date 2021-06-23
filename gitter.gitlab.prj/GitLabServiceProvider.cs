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

namespace gitter.GitLab
{
	using System;
	using System.Text;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using gitter.GitLab.Gui;

	using Resources = gitter.GitLab.Properties.Resources;

	public sealed class GitLabServiceProvider : IRepositoryServiceProvider
	{
		public static IWorkingEnvironment Environment { get; private set; }

		public string Name => "gitlab";

		public string DisplayName => Resources.StrGitLab;

		public bool CanBeAddedManually => false;

		public NotifyCollection<ServerInfo> Servers { get; } = new();

		public GitLabServiceProvider()
		{
			//gitter.Git.Gui.Controls.RevisionListBox.OnCreated(c => c.Columns.Add(new Gui.RevisionPipelineColumn()));
		}

		public Image Icon => CachedResources.Bitmaps["ImgGitLab"];

		/// <summary>Prepare for working inside specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		public bool LoadFor(IWorkingEnvironment environment, Section section)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));

			GlobalOptions.RegisterPropertyPageFactory(new PropertyPageFactory(
				Options.ConfigurationPage.Guid,
				Resources.StrGitLab,
				null,
				IntegrationOptionsPage.Guid,
				env => new Options.ConfigurationPage(env, this)));

			var dock = environment.ViewDockService;
			dock.RegisterFactory(new IssuesViewFactory());
			dock.RegisterFactory(new PipelinesViewFactory());

			if(section.TryGetSection("Servers", out var servers))
			{
				foreach(var server in servers.Sections)
				{
					Servers.Add(ServerInfo.LoadFrom(server));
				}
			}

			Environment = environment;

			return true;
		}

		/// <summary>Save configuration to <paramref name="section"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		public void SaveTo(Section section)
		{
			var servers = section.GetCreateEmptySection("Servers");
			int index = 0;
			foreach(var server in Servers)
			{
				server.SaveTo(servers.GetCreateEmptySection($"Server{index++}"));
			}
		}

		//public bool IsValidFor(IRepository repository)
		//{
		//	if(repository is not Git.Repository) return false;
		//	var issueTrackers = repository.ConfigSection.TryGetSection("IssueTrackers");
		//	if(issueTrackers != null)
		//	{
		//		var section = issueTrackers.TryGetSection("GitLab");
		//		if(section != null)
		//		{
		//			if(!section.ContainsParameter("ServiceUri")) return false;
		//			if(!section.ContainsParameter("ApiKey")) return false;
		//			if(!section.ContainsParameter("ProjectId")) return false;
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		public Control CreateSetupDialog(IRepository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			return new ProviderSetupControl(repository);
		}

		private static string Unmask(string str)
		{
			if(str == string.Empty) return string.Empty;

			return Encoding.UTF8.GetString(Convert.FromBase64String(str));
		}

		private static bool Match(Uri serverUrl, string remoteFetchUrl, out string projectId)
		{
			if(string.IsNullOrWhiteSpace(remoteFetchUrl))
			{
				projectId = default;
				return false;
			}
			if(remoteFetchUrl.StartsWith("git@"))
			{
				int sep = remoteFetchUrl.IndexOf(':');
				if(sep > 0 & sep < remoteFetchUrl.Length - 1)
				{
					var host = remoteFetchUrl.Substring("git@".Length, sep - "git@".Length);
					if(serverUrl.Host == host)
					{
						projectId = remoteFetchUrl.EndsWith(".git")
							? remoteFetchUrl.Substring(sep + 1, remoteFetchUrl.Length - sep - ".git".Length - 1)
							: remoteFetchUrl.Substring(sep + 1);
						return true;
					}
				}
			}
			else if(Uri.TryCreate(remoteFetchUrl, UriKind.Absolute, out var remoteUri))
			{
				if(remoteUri.Scheme == "http" || remoteUri.Scheme == "https")
				{
					if(remoteUri.Host == serverUrl.Host)
					{
						projectId = remoteUri.PathAndQuery.Substring(1);
						return true;
					}
				}
			}
			projectId = default;
			return false;
		}

		public bool TryCreateGuiProvider(IRepository repository, out IGuiProvider guiProvider)
		{
			if(Servers.Count == 0 || repository is not Git.Repository git)
			{
				guiProvider = default;
				return false;
			}

			if(git.Remotes.Count == 0)
			{
				guiProvider = default;
				return false;
			}

			var server    = default(ServerInfo);
			var projectId = default(string);
			foreach(var remote in git.Remotes)
			{
				foreach(var s in Servers)
				{
					if(Match(s.ServiceUrl, remote.FetchUrl, out projectId))
					{
						server = s;
						break;
					}
				}
				if(server is not null) break;
			}

			if(server is null)
			{
				guiProvider = default;
				return false;
			}

			var svc = new GitLabServiceContext(server.ServiceUrl, server.ApiKey)
			{
				DefaultProjectId = projectId,
			};

			guiProvider = new GitLabGuiProvider(git, svc);
			return true;
		}
	}
}

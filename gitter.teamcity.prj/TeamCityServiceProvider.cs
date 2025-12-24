#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Configuration;
using gitter.Framework.Controls;

using gitter.TeamCity.Gui;
using gitter.TeamCity.Gui.Views;

using Resources = gitter.TeamCity.Properties.Resources;

public sealed class TeamCityServiceProvider(HttpMessageInvoker httpMessageInvoker) : IRepositoryServiceProvider
{
	public static IWorkingEnvironment? Environment { get; private set; }

	public string Name => "teamcity";

	public string DisplayName => Resources.StrTeamCity;

	public bool CanBeAddedManually => true;

	public IImageProvider Icon => Icons.TeamCity;

	public NotifyCollection<ServerInfo> Servers { get; } = [];

	public HttpMessageInvoker HttpMessageInvoker { get; } = httpMessageInvoker;

	/// <summary>Prepare for working inside specified <paramref name="environment"/>.</summary>
	/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
	/// <param name="section">Provider configuration section.</param>
	public bool LoadFor(IWorkingEnvironment environment, Section section)
	{
		Verify.Argument.IsNotNull(environment);

		environment.ViewDockService.RegisterFactory(new BuildTypeBuildsViewFactory());

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

	public static bool IsValidFor(IRepository repository)
	{
		if(repository is null) return false;
		var issueTrackers = repository.ConfigSection.TryGetSection("IssueTrackers");
		if(issueTrackers is not null)
		{
			var section = issueTrackers.TryGetSection("TeamCity");
			if(section is not null)
			{
				if(!section.ContainsParameter("ServerName")) return false;
				if(!section.ContainsParameter("ProjectId")) return false;
				return true;
			}
		}
		return false;
	}

	public Control CreateSetupDialog(IRepository repository)
	{
		Verify.Argument.IsNotNull(repository);

		return new ProviderSetupDialog(repository, Servers);
	}

	public bool TryCreateGuiProvider(IRepository repository,
		[MaybeNullWhen(returnValue: false)] out IGuiProvider guiProvider)
	{
		if(!IsValidFor(repository))
		{
			guiProvider = default;
			return false;
		}

		var section = repository.ConfigSection.GetSection("IssueTrackers").GetSection("TeamCity");

		var name   = section.GetValue<string>("ServerName") ?? "";
		var pid    = section.GetValue<string>("ProjectId");
		var server = Servers.FirstOrDefault(s => s.Name == name);
		if(server is null)
		{
			guiProvider = default;
			return false;
		}

		var svc = new TeamCityServiceContext(HttpMessageInvoker, server)
		{
			DefaultProjectId = pid,
		};

		guiProvider = new TeamCityGuiProvider(repository, Servers, svc);
		return true;
	}
}

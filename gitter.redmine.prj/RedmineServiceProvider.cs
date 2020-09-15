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

namespace gitter.Redmine
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.Redmine.Gui;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class RedmineServiceProvider : IRepositoryServiceProvider
	{
		public static IWorkingEnvironment Environment { get; private set; }

		public string Name => "redmine";

		public string DisplayName => Resources.StrRedmine;

		public Image Icon => CachedResources.Bitmaps["ImgRedmine"];

		/// <summary>Prepare for working inside specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		public bool LoadFor(IWorkingEnvironment environment, Section section)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));

			environment.ViewDockService.RegisterFactory(new NewsViewFactory());
			environment.ViewDockService.RegisterFactory(new IssuesViewFactory());
			environment.ViewDockService.RegisterFactory(new VersionsViewFactory());
			Environment = environment;
			return true;
		}

		/// <summary>Save configuration to <paramref name="node"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		public void SaveTo(Section section)
		{
		}

		public bool IsValidFor(IRepository repository)
		{
			if(repository == null) return false;
			var issueTrackers = repository.ConfigSection.TryGetSection("IssueTrackers");
			if(issueTrackers != null)
			{
				var section = issueTrackers.TryGetSection("Redmine");
				if(section != null)
				{
					if(!section.ContainsParameter("ServiceUri")) return false;
					if(!section.ContainsParameter("ApiKey")) return false;
					if(!section.ContainsParameter("ProjectId")) return false;
					return true;
				}
			}
			return false;
		}

		public Control CreateSetupDialog(IRepository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			return new ProviderSetupControl(repository);
		}

		public IGuiProvider CreateGuiProvider(IRepository repository)
		{
			var section = repository.ConfigSection.GetSection("IssueTrackers").GetSection("Redmine");

			var uri = section.GetValue<string>("ServiceUri");
			var key = section.GetValue<string>("ApiKey");
			var pid = section.GetValue<string>("ProjectId");
			var svc = new RedmineServiceContext(new Uri(uri), key);
			svc.DefaultProjectId = pid;

			return new RedmineGuiProvider(repository, svc);
		}
	}
}

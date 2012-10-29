namespace gitter.TeamCity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.TeamCity.Gui;
	using gitter.TeamCity.Gui.Views;

	using Resources = gitter.TeamCity.Properties.Resources;

	public sealed class TeamCityServiceProvider : IRepositoryServiceProvider
	{
		private static IWorkingEnvironment _environment;

		public static IWorkingEnvironment Environment
		{
			get { return _environment; }
		}

		public string Name
		{
			get { return "teamcity"; }
		}

		public string DisplayName
		{
			get { return Resources.StrTeamCity; }
		}

		public Image Icon
		{
			get { return CachedResources.Bitmaps["ImgTeamCity"]; }
		}

		/// <summary>Prepare for working inside specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		public bool LoadFor(IWorkingEnvironment environment, Section section)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			environment.ViewDockService.RegisterFactory(new BuildTypeBuildsViewFactory());

			_environment = environment;
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
				var section = issueTrackers.TryGetSection("TeamCity");
				if(section != null)
				{
					if(!section.ContainsParameter("ServiceUri")) return false;
					if(!section.ContainsParameter("Username")) return false;
					if(!section.ContainsParameter("Password")) return false;
					if(!section.ContainsParameter("ProjectId")) return false;
					return true;
				}
			}
			return false;
		}

		public Control CreateSetupControl(IRepository repository)
		{
			Verify.Argument.IsNotNull(repository, "environment");

			return new ProviderSetupControl(repository);
		}

		private static string Unmask(string str)
		{
			if(str == string.Empty) return string.Empty;

			return Encoding.UTF8.GetString(Convert.FromBase64String(str));
		}

		public IGuiProvider CreateGuiProvider(IRepository repository)
		{
			var section = repository.ConfigSection.GetSection("IssueTrackers").GetSection("TeamCity");

			var uri = section.GetValue<string>("ServiceUri");
			var username = Unmask(section.GetValue<string>("Username"));
			var password = Unmask(section.GetValue<string>("Password"));
			var pid = section.GetValue<string>("ProjectId");
			var svc = new TeamCityServiceContext(new Uri(uri), username, password)
			{
				DefaultProjectId = pid,
			};

			return new TeamCityGuiProvider(repository, svc);
		}
	}
}

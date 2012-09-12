namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.Redmine.Gui;

	using Resources = gitter.Redmine.Properties.Resources;

	public sealed class RedmineServiceProvider : IIssueTrackerProvider
	{
		private static IWorkingEnvironment _environment;

		public static IWorkingEnvironment Environment
		{
			get { return _environment; }
		}

		public string Name
		{
			get { return "redmine"; }
		}

		public string DisplayName
		{
			get { return Resources.StrRedmine; }
		}

		public Image Icon
		{
			get { return CachedResources.Bitmaps["ImgRedmine"]; }
		}

		/// <summary>Prepare for working inside specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		public bool LoadFor(IWorkingEnvironment environment, Section section)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			environment.ViewDockService.RegisterFactory(new NewsViewFactory());
			environment.ViewDockService.RegisterFactory(new IssuesViewFactory());
			environment.ViewDockService.RegisterFactory(new VersionsViewFactory());
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

		public Control CreateSetupControl(IRepository repository)
		{
			Verify.Argument.IsNotNull(repository, "environment");

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

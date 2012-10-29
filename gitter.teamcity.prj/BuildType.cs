namespace gitter.TeamCity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;

	public sealed class BuildType : NamedTeamCityObject
	{
		#region Static

		public static readonly TeamCityObjectProperty<Project> ProjectProperty =
			new TeamCityObjectProperty<Project>("projectId", "Project");

		#endregion

		#region Data

		private readonly BuildTypeBuildsCollection _builds;
		private Project _project;

		#endregion

		#region .ctor

		internal BuildType(TeamCityServiceContext context, string id, string name)
			: base(context, id, name)
		{
			_builds = new BuildTypeBuildsCollection(this, Context.Builds);
		}

		internal BuildType(TeamCityServiceContext context, string id)
			: base(context, id)
		{
			_builds = new BuildTypeBuildsCollection(this, Context.Builds);
		}

		internal BuildType(TeamCityServiceContext context, XmlNode node)
			: base(context, node)
		{
			_builds = new BuildTypeBuildsCollection(this, Context.Builds);
			_project = Context.Projects.Lookup(TeamCityUtility.LoadString(node.Attributes["projectId"]), TeamCityUtility.LoadString(node.Attributes["projectName"]));
		}

		#endregion

		#region Methods

		internal override void Update(XmlNode node)
		{
			base.Update(node);
			Project = Context.Projects.Lookup(TeamCityUtility.LoadString(node.Attributes["projectId"]), TeamCityUtility.LoadString(node.Attributes["projectName"]));
		}

		public BuildTypeLocator CreateLocator()
		{
			return new BuildTypeLocator() { Id = Id };
		}

		#endregion

		#region Properties

		public BuildTypeBuildsCollection Builds
		{
			get { return _builds; }
		}

		public Project Project
		{
			get { return _project; }
			private set { UpdatePropertyValue(ref _project, value, ProjectProperty); }
		}

		#endregion
	}
}

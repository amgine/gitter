namespace gitter.TeamCity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;

	public sealed class Project : NamedTeamCityObject
	{
		#region Static

		#endregion

		#region Data

		private readonly ProjectBuildTypesCollection _buildTypes;

		#endregion

		#region .ctor

		public Project(TeamCityServiceContext context, string id, string name)
			: base(context, id, name)
		{
			_buildTypes = new ProjectBuildTypesCollection(this, context.BuildTypes);
		}

		public Project(TeamCityServiceContext context, string id)
			: base(context, id)
		{
			_buildTypes = new ProjectBuildTypesCollection(this, context.BuildTypes);
		}

		public Project(TeamCityServiceContext context, XmlNode node)
			: base(context, node)
		{
			_buildTypes = new ProjectBuildTypesCollection(this, context.BuildTypes);
		}

		#endregion

		#region Methods

		public ProjectLocator CreateLocator()
		{
			return new ProjectLocator() { Id = Id };
		}

		#endregion

		#region Properties

		public ProjectBuildTypesCollection BuildTypes
		{
			get { return _buildTypes; }
		}

		#endregion
	}
}

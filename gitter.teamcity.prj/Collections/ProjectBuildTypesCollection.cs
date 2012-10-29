namespace gitter.TeamCity
{
	using System;

	public sealed class ProjectBuildTypesCollection : CacheSegment<BuildType>
	{
		private readonly Project _project;

		internal ProjectBuildTypesCollection(Project project, BuildTypesCollection buildTypes)
			: base(buildTypes)
		{
			Verify.Argument.IsNotNull(project, "project");

			_project = project;
		}

		protected override bool IsIncluded(BuildType item)
		{
			return item.Project == _project;
		}

		public override void Refresh()
		{
			Context.BuildTypes.UpdateCache(_project.CreateLocator());
		}
	}
}

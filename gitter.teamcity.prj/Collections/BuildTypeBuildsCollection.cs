namespace gitter.TeamCity
{
	using System;

	public sealed class BuildTypeBuildsCollection : CacheSegment<Build>
	{
		private readonly BuildType _buildType;

		internal BuildTypeBuildsCollection(BuildType buildType, BuildsCollection builds)
			: base(builds)
		{
			Verify.Argument.IsNotNull(buildType, "buildType");

			_buildType = buildType;
		}

		protected override bool IsIncluded(Build item)
		{
			return item.BuildType == _buildType;
		}

		public override void Refresh()
		{
			Context.Builds.UpdateCache(new BuildLocator() { BuildType = _buildType.CreateLocator() });
		}
	}
}

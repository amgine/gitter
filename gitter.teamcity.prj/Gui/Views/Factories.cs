namespace gitter.TeamCity.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.TeamCity.Properties.Resources;

	sealed class BuildTypeBuildsViewFactory : ViewFactoryBase
	{
		public BuildTypeBuildsViewFactory()
			: base(Guids.BuildTypeBuildsViewGuid, Resources.StrBuilds, CachedResources.Bitmaps["ImgBuildType"], false)
		{
		}

		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new BuildTypeBuildsView(environment, parameters);
		}
	}
}

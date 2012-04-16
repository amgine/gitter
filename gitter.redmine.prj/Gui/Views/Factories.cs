namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	sealed class IssuesViewFactory : ViewFactoryBase
	{
		public IssuesViewFactory()
			: base(Guids.IssuesViewGuid, Resources.StrIssues, CachedResources.Bitmaps["ImgBug"])
		{
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new IssuesView(environment, parameters);
		}
	}

	sealed class NewsViewFactory : ViewFactoryBase
	{
		public NewsViewFactory()
			: base(Guids.NewsViewGuid, Resources.StrNews, CachedResources.Bitmaps["ImgNews"])
		{
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new NewsView(environment, parameters);
		}
	}

	sealed class VersionsViewFactory : ViewFactoryBase
	{
		public VersionsViewFactory()
			: base(Guids.VersionsViewGuid, Resources.StrVersions, CachedResources.Bitmaps["ImgVersion"])
		{
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new VersionsView(environment, parameters);
		}
	}
}

namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	sealed class IssuesViewFactory : ViewFactory
	{
		public IssuesViewFactory()
			: base(Guids.IssuesViewGuid, Resources.StrIssues, CachedResources.Bitmaps["ImgBug"])
		{
		}

		protected override ViewBase CreateViewCore(IDictionary<string, object> parameters)
		{
			return new IssuesView(parameters);
		}
	}

	sealed class NewsViewFactory : ViewFactory
	{
		public NewsViewFactory()
			: base(Guids.NewsViewGuid, Resources.StrNews, CachedResources.Bitmaps["ImgNews"])
		{
		}

		protected override ViewBase CreateViewCore(IDictionary<string, object> parameters)
		{
			return new NewsView(parameters);
		}
	}

	sealed class VersionsViewFactory : ViewFactory
	{
		public VersionsViewFactory()
			: base(Guids.VersionsViewGuid, Resources.StrVersions, CachedResources.Bitmaps["ImgVersion"])
		{
		}

		protected override ViewBase CreateViewCore(IDictionary<string, object> parameters)
		{
			return new VersionsView(parameters);
		}
	}
}

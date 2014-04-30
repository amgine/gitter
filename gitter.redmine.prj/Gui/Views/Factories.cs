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
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
		{
			return new IssuesView(environment);
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
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
		{
			return new NewsView(environment);
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
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
		{
			return new VersionsView(environment);
		}
	}
}

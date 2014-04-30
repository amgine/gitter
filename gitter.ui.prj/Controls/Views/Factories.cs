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

namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	sealed class RepositoryExplorerViewFactory : ViewFactoryBase
	{
		private readonly RepositoryRootItem _rootItem;

		public RepositoryExplorerViewFactory(IWorkingEnvironment environment)
			: base(Guids.RepositoryExplorerView, Resources.StrRepositoryExplorer, CachedResources.Bitmaps["ImgRepositoryExplorer"], true)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_rootItem = new RepositoryRootItem(environment, null);
			DefaultViewPosition = ViewPosition.Left;
		}

		public RepositoryRootItem RootItem
		{
			get { return _rootItem; }
		}

		public void AddItem(CustomListBoxItem item)
		{
			Verify.Argument.IsNotNull(item, "item");

			_rootItem.Items.Add(item);
		}

		public void RemoveItem(CustomListBoxItem item)
		{
			_rootItem.Items.Remove(item);
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
		{
			var view = new RepositoryExplorerView(environment);
			view.AddItem(_rootItem);
			return view;
		}
	}

	sealed class StartPageViewFactory : ViewFactoryBase
	{
		public StartPageViewFactory()
			: base(Guids.StartPageView, Resources.StrStartPage, CachedResources.Bitmaps["ImgStartPage"], true)
		{
			DefaultViewPosition = ViewPosition.RootDocumentHost;
			ShowOnStartup = true;
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
		{
			return new StartPageView(environment, this);
		}

		public bool CloseAfterRepositoryLoad
		{
			get;
			set;
		}

		public bool ShowOnStartup
		{
			get;
			set;
		}
	}
}

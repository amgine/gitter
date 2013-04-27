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

namespace gitter.Git.Gui
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.Gui.Views;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class RepositorySubmodulesListItem : RepositoryExplorerItemBase
	{
		private readonly IWorkingEnvironment _environment;
		private SubmoduleListBinding _binding;

		public RepositorySubmodulesListItem(IWorkingEnvironment environment)
			: base(CachedResources.Bitmaps["ImgSubmodules"], Resources.StrSubmodules)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			_environment = environment;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			_environment.ViewDockService.ShowView(Guids.SubmodulesViewGuid);
		}

		public override void OnDoubleClick(int x, int y)
		{
		}

		protected override void DetachFromRepository()
		{
			_binding.Dispose();
			_binding = null;
			Collapse();
		}

		protected override void AttachToRepository()
		{
			_binding = new SubmoduleListBinding(Items, Repository);
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(Repository != null)
			{
				var menu = new SubmodulesMenu(Repository);
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}
			else
			{
				return null;
			}
		}
	}
}

#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Controllers;

using System;
using System.IO;
using System.Windows.Forms;

using gitter.Framework.Mvc;

using gitter.Git.Gui.Interfaces;

sealed class CloneController(IGitRepositoryProvider gitRepositoryProvider)
	: ViewControllerBase<ICloneView>, ICloneController
{
	public bool TryClone()
	{
		var view = RequireView();

		if(gitRepositoryProvider.GitAccessor is null) return false;

		var url = view.Url.Value;
		if(!GitControllerUtility.ValidateUrl(url, view.Url, view.ErrorNotifier))
		{
			return false;
		}
		var path = view.RepositoryPath.Value.Trim();
		if(!GitControllerUtility.ValidateAbsolutePath(path, view.RepositoryPath, view.ErrorNotifier))
		{
			return false;
		}
		var remoteName = view.RemoteName.Value;
		if(!GitControllerUtility.ValidateRemoteName(remoteName, view.RemoteName, view.ErrorNotifier))
		{
			return false;
		}
		url = url!.Trim();
		bool shallow = view.ShallowClone.Value;
		int depth    = shallow ? view.Depth.Value : -1;
		var template = view.UseTemplate.Value ? view.TemplatePath.Value?.Trim() : null;
		if(!string.IsNullOrWhiteSpace(template) && !GitControllerUtility.ValidateAbsolutePath(template, view.TemplatePath, view.ErrorNotifier))
		{
			return false;
		}

		bool bare       = view.Bare.Value;
		bool mirror     = bare && view.Mirror.Value;
		bool noCheckout = view.NoCheckout.Value;
		bool recursive  = view.Recursive.Value;

		var status = GuiCommands.Clone(view as IWin32Window,
			gitRepositoryProvider.GitAccessor,
			url, path, template, remoteName,
			shallow, depth, bare, mirror, recursive, noCheckout);

		return status == GuiCommandStatus.Completed;
	}
}

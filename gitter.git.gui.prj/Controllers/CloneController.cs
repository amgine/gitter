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

namespace gitter.Git.Gui.Controllers
{
	using System;
	using System.IO;
	using System.Windows.Forms;

	using gitter.Framework.Mvc;

	using gitter.Git.Gui.Interfaces;

	sealed class CloneController : ViewControllerBase<ICloneView>, ICloneController
	{
		#region Data

		private readonly IGitRepositoryProvider _gitRepositoryProvider;

		#endregion

		#region .ctor

		public CloneController(IGitRepositoryProvider gitRepositoryProvider)
		{
			Verify.Argument.IsNotNull(gitRepositoryProvider, "gitRepositoryProvider");

			_gitRepositoryProvider = gitRepositoryProvider;
		}

		#endregion

		#region Properties

		private IGitRepositoryProvider GitRepositoryProvider
		{
			get { return _gitRepositoryProvider; }
		}

		#endregion

		#region ICloneController Members

		public bool TryClone()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var url  = View.Url.Value;
			if(!GitControllerUtility.ValidateUrl(url, View.Url, View.ErrorNotifier))
			{
				return false;
			}
			var path = View.RepositoryPath.Value.Trim();
			if(!GitControllerUtility.ValidateAbsolutePath(path, View.RepositoryPath, View.ErrorNotifier))
			{
				return false;
			}
			var remoteName = View.RemoteName.Value;
			if(!GitControllerUtility.ValidateRemoteName(remoteName, View.RemoteName, View.ErrorNotifier))
			{
				return false;
			}
			url = url.Trim();
			bool shallow = View.ShallowClone.Value;
			int depth = shallow ? View.Depth.Value : -1;
			string template = View.UseTemplate.Value ? View.TemplatePath.Value.Trim() : null;
			if(!string.IsNullOrWhiteSpace(template) && !GitControllerUtility.ValidateAbsolutePath(template, View.TemplatePath, View.ErrorNotifier))
			{
				return false;
			}

			bool bare       = View.Bare.Value;
			bool mirror     = bare && View.Mirror.Value;
			bool noCheckout = View.NoCheckout.Value;
			bool recursive  = View.Recursive.Value;

			var status = GuiCommands.Clone(View as IWin32Window,
				GitRepositoryProvider.GitAccessor,
				url, path, template, remoteName,
				shallow, depth, bare, mirror, recursive, noCheckout);

			return status == GuiCommandStatus.Completed;
		}

		#endregion
	}
}

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
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Mvc;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class AddSubmoduleController : ViewControllerBase<IAddSubmoduleView>, IAddSubmoduleController
	{
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public AddSubmoduleController(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		#endregion

		#region properties

		private Repository Repository
		{
			get { return _repository; }
		}

		#endregion

		#region IAddSubmoduleController Members

		public bool TryAddSubmodule()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var path = View.Path.Value.Trim();
			if(!GitControllerUtility.ValidateRelativePath(path, View.Path, View.ErrorNotifier))
			{
				return false;
			}
			var url = View.Url.Value.Trim();
			if(!GitControllerUtility.ValidateUrl(url, View.Url, View.ErrorNotifier))
			{
				return false;
			}
			string branch = null;
			if(View.UseCustomBranch.Value)
			{
				branch = View.BranchName.Value.Trim();
				if(!GitControllerUtility.ValidateBranchName(branch, View.BranchName, View.ErrorNotifier))
				{
					return false;
				}
			}
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					_repository.Submodules.Create(path, url, branch);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					string.Format(Resources.ErrFailedToAddSubmodule, path),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}

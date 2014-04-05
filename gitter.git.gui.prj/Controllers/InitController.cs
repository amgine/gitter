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

	using gitter.Framework;
	using gitter.Framework.Mvc;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class InitController : ViewControllerBase<IInitView>, IInitController
	{
		#region Data

		private readonly IGitRepositoryProvider _gitRepositoryProvider;

		#endregion

		#region .ctor

		public InitController(IGitRepositoryProvider gitRepositoryProvider)
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

		#region IInitController Members

		public bool TryInit()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var repositoryPath = View.RepositoryPath.Value.Trim();
			if(!GitControllerUtility.ValidateAbsolutePath(repositoryPath, View.RepositoryPath, View.ErrorNotifier))
			{
				return false;
			}
			string template = null;
			if(View.UseCustomTemplate.Value)
			{
				template = View.Template.Value.Trim();
				if(!GitControllerUtility.ValidateAbsolutePath(template, View.Template, View.ErrorNotifier))
				{
					return false;
				}
			}
			bool bare = View.Bare.Value;
			try
			{
				if(!Directory.Exists(repositoryPath))
				{
					Directory.CreateDirectory(repositoryPath);
				}
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					Resources.ErrFailedToCreateDirectory,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					Repository.Init(GitRepositoryProvider.GitAccessor, repositoryPath, template, bare);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					Resources.ErrFailedToInit,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}

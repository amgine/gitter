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
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Mvc;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class InitController : ViewControllerBase<IInitView>, IInitController
	{
		public InitController(IGitRepositoryProvider gitRepositoryProvider)
		{
			Verify.Argument.IsNotNull(gitRepositoryProvider, nameof(gitRepositoryProvider));

			GitRepositoryProvider = gitRepositoryProvider;
		}

		private IGitRepositoryProvider GitRepositoryProvider { get; }

		private bool ValidateInput(out string path, out string template, out bool bare)
		{
			path     = View.RepositoryPath.Value.Trim();
			template = null;
			bare     = View.Bare.Value;
			if(!GitControllerUtility.ValidateAbsolutePath(path, View.RepositoryPath, View.ErrorNotifier))
			{
				return false;
			}
			if(View.UseCustomTemplate.Value)
			{
				template = View.Template.Value.Trim();
				if(!GitControllerUtility.ValidateAbsolutePath(template, View.Template, View.ErrorNotifier))
				{
					return false;
				}
			}
			return true;
		}

		private void OnCreateDirectoryFailed(Exception exc)
			=> GitterApplication.MessageBoxService.Show(
				View as IWin32Window,
				exc.Message,
				Resources.ErrFailedToCreateDirectory,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);

		private void OnInitFailed(Exception exc)
			=> GitterApplication.MessageBoxService.Show(
				View as IWin32Window,
				exc.Message,
				Resources.ErrFailedToInit,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);

		public bool TryInit()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			if(!ValidateInput(out var path, out var template, out var bare)) return false;

			try
			{
				if(!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				OnCreateDirectoryFailed(exc);
				return false;
			}
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					Repository.Init(GitRepositoryProvider.GitAccessor, path, template, bare);
				}
			}
			catch(GitException exc)
			{
				OnInitFailed(exc);
				return false;
			}
			return true;
		}

		public async Task<bool> TryInitAsync()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			if(!ValidateInput(out var path, out var template, out var bare)) return false;

			try
			{
				if(!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				OnCreateDirectoryFailed(exc);
				return false;
			}
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					await Repository
						.InitAsync(GitRepositoryProvider.GitAccessor, path, template, bare)
						.ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch(GitException exc)
			{
				OnInitFailed(exc);
				return false;
			}
			return true;
		}
	}
}

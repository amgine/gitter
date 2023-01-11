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
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Services;

using gitter.Git.Gui.Dialogs;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class CreateBranchController : ViewControllerBase<ICreateBranchView>, ICreateBranchController
{
	public CreateBranchController(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;
	}

	private Repository Repository { get; }

	protected override void OnViewAttached()
	{
		base.OnViewAttached();
		var logallrefupdates = Repository.Configuration.TryGetParameterValue(GitConstants.CoreLogAllRefUpdatesParameter);
		if(logallrefupdates is not null && logallrefupdates == "true")
		{
			View.CreateReflog.Value      = true;
			View.CreateReflog.IsReadOnly = true;
		}
	}

	private async Task<bool> TryResetExistingBranchAsync(string branchName, string refspec, bool checkout, Branch existing)
	{
		if(GitterApplication.MessageBoxService.Show(
			View as IWin32Window,
			Resources.StrAskBranchExists.UseAsFormat(branchName),
			Resources.StrBranch,
			MessageBoxButtons.YesNo,
			MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return false;
		}
		var ptr = Repository.GetRevisionPointer(refspec);
		try
		{
			if(existing.IsCurrent)
			{
				ResetMode mode;
				using(var dlg = new SelectResetModeDialog())
				{
					if(dlg.Run(View as IWin32Window) != DialogResult.OK)
					{
						return false;
					}
					mode = dlg.ResetMode;
				}
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					await Repository.Head.ResetAsync(ptr, mode);
				}
			}
			else
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					existing.Reset(ptr);
					if(checkout)
					{
						await existing.CheckoutAsync(true);
					}
				}
			}
		}
		catch(UnknownRevisionException)
		{
			View.ErrorNotifier.NotifyError(View.StartingRevision,
				new UserInputError(
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown));
			return false;
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				View as IWin32Window,
				exc.Message,
				Resources.ErrFailedToReset,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}

	private async Task<bool> TryCreateNewBranchAsync(string branchName, string refspec, bool checkout, bool orphan, bool reflog)
	{
		var trackingMode = View.TrackingMode.Value;
		try
		{
			using(View.ChangeCursor(MouseCursor.WaitCursor))
			{
				var ptr = Repository.GetRevisionPointer(refspec);
				if(orphan)
				{
					await Repository.Refs.Heads.CreateOrphanAsync(
						branchName,
						ptr,
						trackingMode,
						reflog);
				}
				else
				{
					await Repository.Refs.Heads.CreateAsync(
						branchName,
						ptr,
						trackingMode,
						checkout,
						reflog);
				}
			}
		}
		catch(UnknownRevisionException)
		{
			View.ErrorNotifier.NotifyError(View.StartingRevision,
				new UserInputError(
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown));
			return false;
		}
		catch(BranchAlreadyExistsException)
		{
			View.ErrorNotifier.NotifyError(View.BranchName,
				new UserInputError(
					Resources.ErrInvalidBranchName,
					Resources.ErrBranchAlreadyExists));
			return false;
		}
		catch(InvalidBranchNameException exc)
		{
			View.ErrorNotifier.NotifyError(View.BranchName,
				new UserInputError(
					Resources.ErrInvalidBranchName,
					exc.Message));
			return false;
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				View as IWin32Window,
				exc.Message,
				string.Format(Resources.ErrFailedToCreateBranch, branchName),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}

	public Task<bool> TryCreateBranchAsync()
	{
		Verify.State.IsTrue(View is not null, "Controller is not attached to a view.");

		var branchName = View.BranchName.Value.Trim();
		if(!GitControllerUtility.ValidateBranchName(branchName, View.BranchName, View.ErrorNotifier))
		{
			return Task.FromResult(false);
		}
		var refspec = View.StartingRevision.Value.Trim();
		if(!GitControllerUtility.ValidateRefspec(refspec, View.StartingRevision, View.ErrorNotifier))
		{
			return Task.FromResult(false);
		}

		var checkout = View.Checkout.Value;
		var existing = Repository.Refs.Heads.TryGetItem(branchName);

		if(existing is not null)
		{
			return TryResetExistingBranchAsync(branchName, refspec, checkout, existing);
		}
		else
		{
			var orphan = checkout && View.Orphan.Value && GitFeatures.CheckoutOrphan.IsAvailableFor(Repository);
			var reflog = View.CreateReflog.Value;

			return TryCreateNewBranchAsync(branchName, refspec, checkout, orphan, reflog);
		}
	}
}

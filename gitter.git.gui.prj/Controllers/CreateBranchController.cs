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
using gitter.Git.AccessLayer;
using gitter.Git.Gui.Dialogs;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class CreateBranchController(Repository repository)
	: ViewControllerBase<ICreateBranchView>, ICreateBranchController
{
	readonly record struct UserInput(
		string BranchName,
		string Refspec,
		bool   Checkout,
		bool   Orphan,
		bool   CreateReflog,
		BranchTrackingMode TrackingMode);

	private bool TryCollectUserInput(out UserInput input)
	{
		var view = RequireView();
		var branchName = view.BranchName.Value?.Trim();
		if(!GitControllerUtility.ValidateBranchName(branchName, view.BranchName, view.ErrorNotifier))
		{
			goto fail;
		}
		var refspec = view.StartingRevision.Value.Trim();
		if(!GitControllerUtility.ValidateRefspec(refspec, view.StartingRevision, view.ErrorNotifier))
		{
			goto fail;
		}
		var checkout = view.Checkout.Value;

		input = new(
			BranchName:   branchName!,
			Refspec:      refspec,
			Checkout:     checkout,
			Orphan:       checkout && view.Orphan.Value && GitFeatures.CheckoutOrphan.IsAvailableFor(repository),
			CreateReflog: view.CreateReflog.Value,
			TrackingMode: view.TrackingMode.Value);
		return true;

	fail:
		input = default;
		return false;
	}

	/// <inheritdoc/>
	protected override void OnViewAttached(ICreateBranchView view)
	{
		base.OnViewAttached(view);

		var logallrefupdates = repository.Configuration.TryGetParameterValue(GitConstants.CoreLogAllRefUpdatesParameter);
		if(logallrefupdates is not null && logallrefupdates == "true")
		{
			view.CreateReflog.Value      = true;
			view.CreateReflog.IsReadOnly = true;
		}
	}

	private async Task<bool> TryResetExistingBranchAsync(UserInput input, Branch existing)
	{
		var view = RequireView();
		if(GitterApplication.MessageBoxService.Show(
			view as IWin32Window,
			Resources.StrAskBranchExists.UseAsFormat(input.BranchName),
			Resources.StrBranch,
			MessageBoxButtons.YesNo,
			MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return false;
		}
		var ptr = repository.GetRevisionPointer(input.Refspec);
		try
		{
			if(existing.IsCurrent)
			{
				ResetMode mode;
				using(var dialog = new SelectResetModeDialog())
				{
					if(dialog.Run(view as IWin32Window) != DialogResult.OK)
					{
						return false;
					}
					mode = dialog.ResetMode;
				}
				using(view.ChangeCursor(MouseCursor.WaitCursor))
				{
					await repository.Head.ResetAsync(ptr, mode);
				}
			}
			else
			{
				using(view.ChangeCursor(MouseCursor.WaitCursor))
				{
					existing.Reset(ptr);
					if(input.Checkout)
					{
						await existing.CheckoutAsync(true);
					}
				}
			}
		}
		catch(UnknownRevisionException)
		{
			view.ErrorNotifier.NotifyError(view.StartingRevision,
				new UserInputError(
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown));
			return false;
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				Resources.ErrFailedToReset,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}

	private async Task<bool> TryCreateNewBranchAsync(UserInput input)
	{
		var view = RequireView();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				var ptr = repository.GetRevisionPointer(input.Refspec);
				if(input.Orphan)
				{
					await repository.Refs.Heads.CreateOrphanAsync(
						input.BranchName,
						ptr,
						input.TrackingMode,
						input.CreateReflog);
				}
				else
				{
					await repository.Refs.Heads.CreateAsync(
						input.BranchName,
						ptr,
						input.TrackingMode,
						input.Checkout,
						input.CreateReflog);
				}
			}
		}
		catch(UnknownRevisionException)
		{
			view.ErrorNotifier.NotifyError(view.StartingRevision,
				new UserInputError(
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown));
			return false;
		}
		catch(BranchAlreadyExistsException)
		{
			view.ErrorNotifier.NotifyError(view.BranchName,
				new UserInputError(
					Resources.ErrInvalidBranchName,
					Resources.ErrBranchAlreadyExists));
			return false;
		}
		catch(InvalidBranchNameException exc)
		{
			view.ErrorNotifier.NotifyError(view.BranchName,
				new UserInputError(
					Resources.ErrInvalidBranchName,
					exc.Message));
			return false;
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				string.Format(Resources.ErrFailedToCreateBranch, input.BranchName),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}

	public Task<bool> TryCreateBranchAsync()
	{
		if(!TryCollectUserInput(out var input)) return Task.FromResult(false);

		return repository.Refs.Heads.TryGetItem(input.BranchName, out var existing)
			? TryResetExistingBranchAsync(input, existing)
			: TryCreateNewBranchAsync(input);
	}
}

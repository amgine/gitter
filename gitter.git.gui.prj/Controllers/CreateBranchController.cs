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

	using gitter.Git.Gui.Dialogs;
	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	sealed class CreateBranchController : ViewControllerBase<ICreateBranchView>, ICreateBranchController
	{
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public CreateBranchController(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		#endregion

		#region Prtoperties

		private Repository Repository
		{
			get { return _repository; }
		}

		#endregion

		#region Methods

		private bool TryResetExistingBranch(string branchName, string refspec, bool checkout, Branch existent)
		{
			if(GitterApplication.MessageBoxService.Show(
				View as IWin32Window,
				Resources.StrAskBranchExists.UseAsFormat(branchName),
				Resources.StrBranch,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question) == DialogResult.Yes)
			{
				var ptr = Repository.GetRevisionPointer(refspec);
				try
				{
					if(existent.IsCurrent)
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
							Repository.Head.Reset(ptr, mode);
						}
					}
					else
					{
						using(View.ChangeCursor(MouseCursor.WaitCursor))
						{
							existent.Reset(ptr);
							if(checkout)
							{
								existent.Checkout(true);
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
			else
			{
				return false;
			}
		}

		private bool TryCreateNewBranch(string branchName, string refspec, bool checkout, bool orphan, bool reflog)
		{
			var trackingMode = View.TrackingMode.Value;
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					var ptr = Repository.GetRevisionPointer(refspec);
					if(orphan)
					{
						Repository.Refs.Heads.CreateOrphan(
							branchName,
							ptr,
							trackingMode,
							reflog);
					}
					else
					{
						Repository.Refs.Heads.Create(
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

		#endregion

		#region ICreateBranchController Members

		public bool TryCreateBranch()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var branchName = View.BranchName.Value.Trim();
			var refspec    = View.StartingRevision.Value.Trim();
			var checkout   = View.Checkout.Value;
			var orphan     = checkout && View.Orphan.Value && GitFeatures.CheckoutOrphan.IsAvailableFor(Repository);
			var reflog     = View.CreateReflog.Value;
			var existent   = Repository.Refs.Heads.TryGetItem(branchName);

			if(!GitControllerUtility.ValidateBranchName(branchName, View.BranchName, View.ErrorNotifier))
			{
				return false;
			}
			if(!GitControllerUtility.ValidateRefspec(refspec, View.StartingRevision, View.ErrorNotifier))
			{
				return false;
			}
			if(existent != null)
			{
				return TryResetExistingBranch(branchName, refspec, checkout, existent);
			}
			else
			{
				return TryCreateNewBranch(branchName, refspec, checkout, orphan, reflog);
			}
		}

		#endregion
	}
}

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
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Services;

using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

sealed class CheckoutController(Repository repository)
	: ViewControllerBase<ICheckoutView>, ICheckoutController
{
	private void ProceedCheckout(IRevisionPointer revision)
	{
		var view = RequireView();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				revision.Checkout(force: true);
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				string.Format(Resources.ErrFailedToCheckout, revision.Pointer),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
	}

	public bool TryCheckout()
	{
		var view = RequireView();
		var revision = view.Revision.Value;
		if(string.IsNullOrWhiteSpace(revision))
		{
			return true;
		}
		revision = revision!.Trim();

		var pointer = repository.GetRevisionPointer(revision);
		var force = Control.ModifierKeys == Keys.Shift;

		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				pointer.Checkout(force);
			}
		}
		catch(UnknownRevisionException)
		{
			view.ErrorNotifier.NotifyError(view.Revision,
				new UserInputError(
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown));
			return false;
		}
		catch(UntrackedFileWouldBeOverwrittenException)
		{
			if(GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				string.Format(Resources.AskOverwriteUntracked, revision),
				Resources.StrCheckout,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				ProceedCheckout(pointer);
			}
		}
		catch(HaveLocalChangesException)
		{
			if(GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				string.Format(Resources.AskThrowAwayLocalChanges, revision),
				Resources.StrCheckout,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				ProceedCheckout(pointer);
			}
		}
		catch(HaveConflictsException)
		{
			if(GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				string.Format(Resources.AskThrowAwayConflictedChanges, revision),
				Resources.StrCheckout,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				ProceedCheckout(pointer);
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				string.Format(Resources.ErrFailedToCheckout, revision),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}
}

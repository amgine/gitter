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

	sealed class CheckoutController : ViewControllerBase<ICheckoutView>, ICheckoutController
	{
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public CheckoutController(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		#endregion

		#region Properties

		private Repository Repository
		{
			get { return _repository; }
		}

		#endregion

		#region ICheckoutController Members

		private void ProceedCheckout(IRevisionPointer revision)
		{
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					revision.Checkout(true);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					string.Format(Resources.ErrFailedToCheckout, revision.Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		public bool TryCheckout()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var revision = View.Revision.Value;
			if(string.IsNullOrWhiteSpace(revision))
			{
				return true;
			}
			revision = revision.Trim();

			var pointer = Repository.GetRevisionPointer(revision);
			bool force = Control.ModifierKeys == Keys.Shift;

			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					pointer.Checkout(force);
				}
			}
			catch(UnknownRevisionException)
			{
				View.ErrorNotifier.NotifyError(View.Revision,
					new UserInputError(
						Resources.ErrInvalidRevisionExpression,
						Resources.ErrRevisionIsUnknown));
				return false;
			}
			catch(UntrackedFileWouldBeOverwrittenException)
			{
				if(GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
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
					View as IWin32Window,
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
					View as IWin32Window,
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
					View as IWin32Window,
					exc.Message,
					string.Format(Resources.ErrFailedToCheckout, revision),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}

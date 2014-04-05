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

	sealed class AddNoteController : ViewControllerBase<IAddNoteView>, IAddNoteController
	{
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public AddNoteController(Repository repository)
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

		#region IAddNoteController Members

		public bool TryAddNote()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var revision = View.Revision.Value;
			var message  = View.Message.Value;
			if(!GitControllerUtility.ValidateRefspec(revision, View.Revision, View.ErrorNotifier))
			{
				return false;
			}
			if(string.IsNullOrWhiteSpace(message))
			{
				View.ErrorNotifier.NotifyError(View.Message,
					new UserInputError(
						Resources.ErrInvalidMessage,
						Resources.ErrMessageCannotBeEmpty));
				return false;
			}
			revision = revision.Trim();
			message  = message.Trim();
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					var ptr = Repository.GetRevisionPointer(revision);
					ptr.AddNote(message);
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
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					Resources.ErrFailedToAddNote,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}

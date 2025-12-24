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

sealed class AddNoteController(Repository repository)
	: ViewControllerBase<IAddNoteView>, IAddNoteController
{
	public bool TryAddNote()
	{
		var view     = RequireView();
		var revision = view.Revision.Value;
		if(!GitControllerUtility.ValidateRefspec(revision, view.Revision, view.ErrorNotifier))
		{
			return false;
		}
		var message = view.Message.Value?.Trim();
		if(message is not { Length: not 0 } || string.IsNullOrWhiteSpace(message))
		{
			view.ErrorNotifier.NotifyError(view.Message,
				new UserInputError(
					Resources.ErrInvalidMessage,
					Resources.ErrMessageCannotBeEmpty));
			return false;
		}
		revision = revision.Trim();
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				var ptr = repository.GetRevisionPointer(revision);
				ptr.AppendNote(message);
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
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				Resources.ErrFailedToAddNote,
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}
}

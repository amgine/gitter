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

sealed class CreateTagController(Repository repository)
	: ViewControllerBase<ICreateTagView>, ICreateTagController
{
	readonly record struct UserInput(
		string  TagName,
		string  Refspec,
		bool    Annotated,
		bool    Signed,
		string? Message,
		string? KeyId);

	private bool TryCollectUserInput(out UserInput input)
	{
		var view    = RequireView();
		var tagName = view.TagName.Value?.Trim();
		var refspec = view.Revision.Value.Trim();

		if(!GitControllerUtility.ValidateNewTagName(tagName, repository, view.TagName, view.ErrorNotifier))
		{
			goto fail;
		}
		if(!GitControllerUtility.ValidateRefspec(refspec, view.Revision, view.ErrorNotifier))
		{
			goto fail;
		}

		var message   = default(string);
		var signed    = view.Signed.Value;
		var annotated = signed || view.Annotated.Value;
		if(annotated)
		{
			message = view.Message.Value;
			if(string.IsNullOrWhiteSpace(message))
			{
				view.ErrorNotifier.NotifyError(view.Message,
					new UserInputError(
						Resources.ErrNoMessageSpecified,
						Resources.ErrMessageCannotBeEmpty));
				goto fail;
			}
			message = message!.Trim();
		}
		var keyId = default(string);
		if(signed)
		{
			if(view.UseKeyId.Value)
			{
				keyId = view.KeyId.Value;
				if(string.IsNullOrWhiteSpace(keyId))
				{
					view.ErrorNotifier.NotifyError(view.KeyId,
						new UserInputError(
							Resources.ErrNoKeyIdSpecified,
							Resources.ErrKeyIdCannotBeEmpty));
					goto fail;
				}
				keyId = keyId!.Trim();
			}
		}
		input = new(tagName!, refspec, annotated, signed, message, keyId);
		return true;
	fail:
		input = default;
		return false;
	}

	private Tag CreateTag(in UserInput input)
	{
		var ptr = repository.GetRevisionPointer(input.Refspec);
		if(input.Annotated)
		{
			if(input.Signed)
			{
				return input.KeyId is null
					? repository.Refs.Tags.Create(input.TagName, ptr, input.Message!, sign: true)
					: repository.Refs.Tags.Create(input.TagName, ptr, input.Message!, input.KeyId);
			}
			else
			{
				return repository.Refs.Tags.Create(input.TagName, ptr, input.Message!, sign: false);
			}
		}
		else
		{
			return repository.Refs.Tags.Create(input.TagName, ptr);
		}
	}

	public bool TryCreateTag()
	{
		var view = RequireView();

		if(!TryCollectUserInput(out var input)) return false;

		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				_ = CreateTag(in input);
			}
		}
		catch(TagAlreadyExistsException)
		{
			view.ErrorNotifier.NotifyError(view.TagName,
				new UserInputError(
					Resources.ErrInvalidTagName,
					Resources.ErrTagAlreadyExists));
			return false;
		}
		catch(UnknownRevisionException)
		{
			view.ErrorNotifier.NotifyError(view.Revision,
				new UserInputError(
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown));
			return false;
		}
		catch(InvalidTagNameException exc)
		{
			view.ErrorNotifier.NotifyError(view.TagName,
				new UserInputError(
					Resources.ErrInvalidTagName,
					exc.Message));
			return false;
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				view as IWin32Window,
				exc.Message,
				string.Format(Resources.ErrFailedToCreateTag, input.TagName),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
			return false;
		}
		return true;
	}
}

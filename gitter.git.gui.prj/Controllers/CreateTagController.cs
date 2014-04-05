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

	sealed class CreateTagController : ViewControllerBase<ICreateTagView>, ICreateTagController
	{
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public CreateTagController(Repository repository)
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

		#region ICreateTagController Members

		public bool TryCreateTag()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var tagName = View.TagName.Value.Trim();
			var refspec = View.Revision.Value.Trim();

			if(!GitControllerUtility.ValidateNewTagName(tagName, Repository, View.TagName, View.ErrorNotifier))
			{
				return false;
			}
			if(!GitControllerUtility.ValidateRefspec(refspec, View.Revision, View.ErrorNotifier))
			{
				return false;
			}

			string message = null;
			bool signed    = View.Signed.Value;
			bool annotated = signed || View.Annotated.Value;
			if(annotated)
			{
				message = View.Message.Value;
				if(string.IsNullOrWhiteSpace(message))
				{
					View.ErrorNotifier.NotifyError(View.Message,
						new UserInputError(
							Resources.ErrNoMessageSpecified,
							Resources.ErrMessageCannotBeEmpty));
					return false;
				}
				message = message.Trim();
			}
			string keyId = null;
			if(signed)
			{
				if(View.UseKeyId.Value)
				{
					keyId = View.KeyId.Value;
					if(string.IsNullOrWhiteSpace(keyId))
					{
						View.ErrorNotifier.NotifyError(View.KeyId,
							new UserInputError(
								Resources.ErrNoKeyIdSpecified,
								Resources.ErrKeyIdCannotBeEmpty));
						return false;
					}
					keyId = keyId.Trim();
				}
			}
			try
			{
				using(View.ChangeCursor(MouseCursor.WaitCursor))
				{
					var ptr = Repository.GetRevisionPointer(refspec);
					if(annotated)
					{
						if(signed)
						{
							if(keyId == null)
							{
								Repository.Refs.Tags.Create(tagName, ptr, message, true);
							}
							else
							{
								Repository.Refs.Tags.Create(tagName, ptr, message, keyId);
							}
						}
						else
						{
							Repository.Refs.Tags.Create(tagName, ptr, message, false);
						}
					}
					else
					{
						Repository.Refs.Tags.Create(tagName, ptr);
					}
				}
			}
			catch(TagAlreadyExistsException)
			{
				View.ErrorNotifier.NotifyError(View.TagName,
					new UserInputError(
						Resources.ErrInvalidTagName,
						Resources.ErrTagAlreadyExists));
				return false;
			}
			catch(UnknownRevisionException)
			{
				View.ErrorNotifier.NotifyError(View.Revision,
					new UserInputError(
						Resources.ErrInvalidRevisionExpression,
						Resources.ErrRevisionIsUnknown));
				return false;
			}
			catch(InvalidTagNameException exc)
			{
				View.ErrorNotifier.NotifyError(View.TagName,
					new UserInputError(
						Resources.ErrInvalidTagName,
						exc.Message));
				return false;
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					View as IWin32Window,
					exc.Message,
					string.Format(Resources.ErrFailedToCreateTag, tagName),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}

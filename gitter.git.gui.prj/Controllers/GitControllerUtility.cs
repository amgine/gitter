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

	using gitter.Framework.Mvc;

	using Resources = gitter.Git.Gui.Properties.Resources;

	static class GitControllerUtility
	{
		private static bool ValidatePartialPath(string path, int start, int end, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Assert.IsNotNull(path);
			Assert.IsNotNull(userInputSource);
			Assert.IsNotNull(inputErrorNotifier);

			var invalidPathChars = Path.GetInvalidFileNameChars();
			bool endsWithWhitespace = false;
			bool isPartStart = true;
			for(int i = start; i <= end; ++i)
			{
				char c = path[i];
				if(c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
				{
					if(i != start)
					{
						if(isPartStart)
						{
							inputErrorNotifier.NotifyError(userInputSource,
								new UserInputError(
									Resources.ErrInvalidPath,
									Resources.ErrPathCannotContainEmptyDirectoryName));
							return false;
						}
						if(endsWithWhitespace)
						{
							inputErrorNotifier.NotifyError(userInputSource,
								new UserInputError(
									Resources.ErrInvalidPath,
									Resources.ErrDirectoryNameCannotEndWithWhitespace));
							return false;
						}
						isPartStart = true;
					}
					continue;
				}
				if(Array.IndexOf(invalidPathChars, c) != -1)
				{
					inputErrorNotifier.NotifyError(userInputSource,
						new UserInputError(
							Resources.ErrInvalidPath,
							Resources.ErrPathCannotContainCharacter.UseAsFormat(c)));
					return false;
				}
				endsWithWhitespace = char.IsWhiteSpace(c);
				if(isPartStart)
				{
					if(endsWithWhitespace)
					{
						inputErrorNotifier.NotifyError(userInputSource,
							new UserInputError(
								Resources.ErrInvalidPath,
								Resources.ErrDirectoryNameCannotStartWithWhitespace));
						return false;
					}
					isPartStart = false;
				}
			}
			return true;
		}

		public static bool ValidateAbsolutePath(string path, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			int start = -1;
			int end = -1;
			for(int i = 0; i < path.Length; ++i)
			{
				if(!char.IsWhiteSpace(path[i]))
				{
					start = i;
					break;
				}
			}
			if(start == -1)
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrNoPathSpecified,
						Resources.ErrPathCannotBeEmpty));
				return false;
			}
			for(int i = path.Length - 1; i >= 0; --i)
			{
				if(!char.IsWhiteSpace(path[i]))
				{
					end = i;
					break;
				}
			}
			int length = end - start + 1;

			if(length >= 3)
			{
				var c0 = path[start + 0];
				var c1 = path[start + 1];
				var c2 = path[start + 2];
				if(c1 == Path.VolumeSeparatorChar)
				{
					if(!((c0 >= 'a' && c0 <= 'z') || (c0 >= 'A' && c0 <= 'Z')))
					{
						inputErrorNotifier.NotifyError(userInputSource,
							new UserInputError(
								Resources.ErrInvalidPath,
								Resources.ErrPathUnknownSchema));
						return false;
					}
					if(c2 != Path.DirectorySeparatorChar && c2 != Path.AltDirectorySeparatorChar)
					{
						inputErrorNotifier.NotifyError(userInputSource,
							new UserInputError(
								Resources.ErrInvalidPath,
								Resources.ErrPathUnknownSchema));
						return false;
					}
					start += 3;
					length -= 3;
				}
				else
				{
					if(c0 != Path.DirectorySeparatorChar && c0 != Path.AltDirectorySeparatorChar)
					{
						inputErrorNotifier.NotifyError(userInputSource,
							new UserInputError(
								Resources.ErrInvalidPath,
								Resources.ErrPathUnknownSchema));
						return false;
					}
					if(c1 != Path.DirectorySeparatorChar && c1 != Path.AltDirectorySeparatorChar)
					{
						inputErrorNotifier.NotifyError(userInputSource,
							new UserInputError(
								Resources.ErrInvalidPath,
								Resources.ErrPathUnknownSchema));
						return false;
					}
					start += 2;
					length += 2;
				}
			}
			else
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidPath,
						Resources.ErrPathIsTooShort));
				return false;
			}
			if(!ValidatePartialPath(path, start, end, userInputSource, inputErrorNotifier))
			{
				return false;
			}
			return true;
		}

		public static bool ValidateRelativePath(string path, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			int start = -1;
			int end = -1;
			for(int i = 0; i < path.Length; ++i)
			{
				if(!char.IsWhiteSpace(path[i]))
				{
					start = i;
					break;
				}
			}
			if(start == -1)
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrNoPathSpecified,
						Resources.ErrPathCannotBeEmpty));
				return false;
			}
			for(int i = path.Length - 1; i >= 0; --i)
			{
				if(!char.IsWhiteSpace(path[i]))
				{
					end = i;
					break;
				}
			}
			if(!ValidatePartialPath(path, start, end, userInputSource, inputErrorNotifier))
			{
				return false;
			}
			return true;
		}

		public static bool ValidateBranchName(string branchName, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			if(string.IsNullOrWhiteSpace(branchName))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrNoBranchNameSpecified,
						Resources.ErrBranchNameCannotBeEmpty));
				return false;
			}
			string errmsg;
			if(!Branch.ValidateName(branchName, out errmsg))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidBranchName,
						errmsg));
				return false;
			}
			return true;
		}

		public static bool ValidateNewBranchName(string branchName, Repository repository, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			if(!ValidateBranchName(branchName, userInputSource, inputErrorNotifier))
			{
				return false;
			}
			if(repository.Refs.Heads.Contains(branchName) ||
				repository.Refs.Remotes.Contains(branchName))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidBranchName,
						Resources.ErrBranchAlreadyExists));
				return false;
			}
			return true;
		}

		public static bool ValidateTagName(string tagName, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			if(string.IsNullOrWhiteSpace(tagName))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrNoTagNameSpecified,
						Resources.ErrTagNameCannotBeEmpty));
				return false;
			}
			string errmsg;
			if(!Tag.ValidateName(tagName, out errmsg))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidTagName,
						errmsg));
				return false;
			}
			return true;
		}

		public static bool ValidateNewTagName(string tagName, Repository repository, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			if(!ValidateTagName(tagName, userInputSource, inputErrorNotifier))
			{
				return false;
			}
			if(repository.Refs.Tags.Contains(tagName))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidTagName,
						Resources.ErrTagAlreadyExists));
				return false;
			}
			return true;
		}

		public static bool ValidateRefspec(string refspec, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			if(string.IsNullOrEmpty(refspec))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidRevisionExpression,
						Resources.ErrStartingRevisionCannotBeEmpty));
				return false;
			}
			bool encounteredNonWhitespace = false;
			bool encounteredTrailingWhitespace = false;
			for(int i = 0; i < refspec.Length; ++i)
			{
				if(char.IsWhiteSpace(refspec[i]))
				{
					if(encounteredNonWhitespace)
					{
						encounteredTrailingWhitespace = true;
					}
				}
				else
				{
					if(encounteredTrailingWhitespace)
					{
						inputErrorNotifier.NotifyError(userInputSource,
							new UserInputError(
								Resources.ErrInvalidRevisionExpression,
								Resources.ErrRefspecCannotContainSpaces));
					}
					encounteredNonWhitespace = true;
				}
			}
			if(!encounteredNonWhitespace)
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidRevisionExpression,
						Resources.ErrStartingRevisionCannotBeEmpty));
				return false;
			}
			return true;
		}

		public static bool ValidateUrl(string url, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			if(string.IsNullOrWhiteSpace(url))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidUrl,
						Resources.ErrUrlCannotBeEmpty));
				return false;
			}
			return true;
		}

		public static bool ValidateRemoteName(string remoteName, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			if(string.IsNullOrWhiteSpace(remoteName))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrNoRemoteNameSpecified,
						Resources.ErrRemoteNameCannotBeEmpty));
				return false;
			}
			string errorMessage;
			if(!Reference.ValidateName(remoteName, ReferenceType.Remote, out errorMessage))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidRemoteName,
						errorMessage));
				return false;
			}
			return true;
		}

		public static bool ValidateNewRemoteName(string remoteName, Repository repository, IUserInputSource userInputSource, IUserInputErrorNotifier inputErrorNotifier)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNotNull(userInputSource, "userInputSource");
			Verify.Argument.IsNotNull(inputErrorNotifier, "inputErrorNotifier");

			if(!ValidateRemoteName(remoteName, userInputSource, inputErrorNotifier))
			{
				return false;
			}
			if(repository.Remotes.Contains(remoteName))
			{
				inputErrorNotifier.NotifyError(userInputSource,
					new UserInputError(
						Resources.ErrInvalidRemoteName,
						Resources.ErrRemoteAlreadyExists));
				return false;
			}
			return true;
		}
	}
}

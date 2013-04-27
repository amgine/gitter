#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.AccessLayer
{
	internal sealed partial class RepositoryCLI
	{
		private static bool CheckValue(string data, int pos, string value)
		{
			if(pos + value.Length > data.Length) return false;
			return data.IndexOf(value, pos, value.Length) == pos;
		}

		private static bool CheckValues(string data, int pos, params string[] values)
		{
			for(int i = 0; i < values.Length; ++i)
			{
				if(!CheckValue(data, pos, values[i])) return false;
				pos += values[i].Length;
			}
			return true;
		}

		private static bool CheckValues(string data, params string[] values)
		{
			int pos = 0;
			for(int i = 0; i < values.Length; ++i)
			{
				if(!CheckValue(data, pos, values[i])) return false;
				pos += values[i].Length;
			}
			return pos == data.Length;
		}

		private static bool IsUnknownRevisionError(string err, string revision)
		{
			const string errPrefix =
				"fatal: ambiguous argument '";
			const string errPostfix =
				"': unknown revision or path not in the working tree.\n" +
				"Use '--' to separate paths from revisions\n";

			return (err.Length == errPrefix.Length + errPostfix.Length + revision.Length) &&
				CheckValues(err, errPrefix, revision, errPostfix);
		}

		private static bool IsRefrerenceIsNotATreeError(string error, string reference)
		{
			const string errPrefix = "fatal: reference is not a tree: ";
			const string errPostfix = "\n";

			return CheckValues(error, errPrefix, reference, errPostfix);
		}

		private static bool IsUnknownPathspecError(string error, string pathspec)
		{
			const string errPrefix = "error: pathspec '";
			const string errPostfix = "' did not match any file(s) known to git.\n";

			return CheckValues(error, errPrefix, pathspec, errPostfix);
		}

		private static bool IsBadObjectError(string error, string obj)
		{
			const string errPrefix = "fatal: bad object ";
			const string errPostfix = "\n";

			return CheckValues(error, errPrefix, obj, errPostfix);
		}

		private static bool IsBranchNotFullyMergedError(string error, string branchName)
		{
			const string errNotMerged0 =
				"warning: not deleting branch '";
			const string errNotMerged1 =
				"' that is not yet merged to\n         '";
			const string errNotMerged2 =
				"', even though it is merged to HEAD.\n";
			const string errNotMerged3 =
				"error: The branch '";
			const string errNotMerged4 =
				"' is not fully merged.\n" +
				"If you are sure you want to delete it, run 'git branch -D ";
			const string errNotMerged5 =
				"'.\n";

			if(error.Length == errNotMerged3.Length + errNotMerged4.Length + errNotMerged5.Length + 2 * branchName.Length)
			{
				return CheckValues(error, errNotMerged3, branchName, errNotMerged4, branchName, errNotMerged5);
			}
			else
			{
				if(CheckValues(error, 0, errNotMerged0, branchName, errNotMerged1))
				{
					int pos = error.IndexOf(errNotMerged2, errNotMerged0.Length + errNotMerged1.Length + branchName.Length);
					if(pos != -1)
					{
						pos += errNotMerged2.Length;
						return CheckValues(error, pos, errNotMerged3, branchName, errNotMerged4, branchName, errNotMerged5);
					}
				}
				return false;
			}
		}

		private static bool IsBranchAlreadyExistsError(string error, string branchName)
		{
			const string errPrefix = "fatal: A branch named '";
			const string errPostfix = "' already exists.\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + branchName.Length) &&
				CheckValues(error, errPrefix, branchName, errPostfix);
		}

		private static bool IsBranchNotFoundError(string error, bool remote, string branchName)
		{
			if(remote)
			{
				const string errPrefix = "error: remote branch '";
				const string errPostfix = "' not found.\n";

				return (error.Length == errPrefix.Length + errPostfix.Length + branchName.Length) &&
					CheckValues(error, errPrefix, branchName, errPostfix);
			}
			else
			{
				const string errPrefix = "error: branch '";
				const string errPostfix = "' not found.\n";

				return (error.Length == errPrefix.Length + errPostfix.Length + branchName.Length) &&
					CheckValues(error, errPrefix, branchName, errPostfix);
			}
		}

		private static bool IsInvalidBranchNameError(string error, string branchName)
		{
			const string errPrefix = "fatal: '";
			const string errPostfix = "' is not a valid branch name.\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + branchName.Length) &&
				CheckValues(error, errPrefix, branchName, errPostfix);
		}

		private static bool IsUntrackedFileWouldBeOverwrittenError(string error, out string fileName)
		{
			const string errPrefix = "error: Untracked working tree file '";
			const string errPostfix = "' would be overwritten by merge.\n";

			if((error.Length > errPrefix.Length + errPostfix.Length) &&
				error.StartsWith(errPrefix) && errPrefix.EndsWith(errPrefix))
			{
				fileName = error.Substring(errPrefix.Length, error.Length - errPrefix.Length - errPostfix.Length);
				return true;
			}
			else
			{
				fileName = null;
				return false;
			}
		}

		private static bool IsHaveLocalChangesError(string error, out string fileName)
		{
			const string errPrefix = "error: You have local changes to '";
			const string errPostfix = "'; cannot switch branches.\n";

			if((error.Length > errPrefix.Length + errPostfix.Length) &&
				error.StartsWith(errPrefix) && errPrefix.EndsWith(errPrefix))
			{
				fileName = error.Substring(errPrefix.Length, error.Length - errPrefix.Length - errPostfix.Length);
				return true;
			}
			else
			{
				fileName = null;
				return false;
			}
		}

		private static bool IsHaveLocalChangesMergeError(string error, out string fileName)
		{
			const string errPrefix =
				"error: Your local changes to '";

			const string errPostfix = "' would be overwritten by merge.  Aborting.\n" +
				"Please, commit your changes or stash them before you can merge.\n";

			if(error.Length > errPrefix.Length + errPostfix.Length)
			{
				if(error.StartsWith(errPrefix) && error.EndsWith(errPostfix))
				{
					fileName = error.Substring(errPrefix.Length, error.Length - errPrefix.Length - errPostfix.Length);
					return true;
				}
			}
			fileName = null;
			return false;
		}

		private static bool IsHaveConflictsError(string error)
		{
			const string err = "error: you need to resolve your current index first\n";

			return error == err;
		}

		private static bool IsCherryPickNotPossibleBecauseOfConflictsError(string error)
		{
			const string err =
				"fatal: 'cherry-pick' is not possible because you have unmerged files.\n" +
				"Please, fix them up in the work tree, and then use 'git add/rm <file>' as\n" +
				"appropriate to mark resolution and make a commit, or use 'git commit -a'.\n";

			return error == err;
		}

		private static bool IsCherryPickNotPossibleBecauseOfMergeCommit(string err, string revision)
		{
			const string errPrefix = "fatal: Commit ";
			const string errPostfix = " is a merge but no -m option was given.\n";

			return (err.Length == errPrefix.Length + errPostfix.Length + revision.Length) &&
				CheckValues(err, errPrefix, revision, errPostfix);
		}

		private static bool IsCherryPickNotPossibleBecauseOfMergeCommit(string err)
		{
			const string errPrefix = "fatal: Commit ";
			const string errPostfix = " is a merge but no -m option was given.\n";

			return (err.Length > errPrefix.Length + errPostfix.Length) &&
				err.StartsWith(errPrefix) && err.EndsWith(errPostfix);
		}

		private static bool IsTagAlreadyExistsError(string error, string tagName)
		{
			const string errPrefix = "fatal: tag '";
			const string errPostfix = "' already exists\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + tagName.Length) &&
				CheckValues(error, errPrefix, tagName, errPostfix);
		}

		private static bool IsTagNotFoundError(string error, string tagName)
		{
			const string errPrefix = "error: tag '";
			const string errPostfix = "' not found.\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + tagName.Length) &&
				CheckValues(error, errPrefix, tagName, errPostfix);
		}

		private static bool IsInvalidTagNameError(string error, string tagName)
		{
			const string errPrefix = "fatal: '";
			const string errPostfix = "' is not a valid tag name.\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + tagName.Length) &&
				CheckValues(error, errPrefix, tagName, errPostfix);
		}

		private static bool IsAutomaticMergeFailedError(string output)
		{
			const string error = "Automatic merge failed; fix conflicts and then commit the result.\n";

			return output.EndsWith(error);
		}

		private static bool IsAutomaticCherryPickFailedError(string error, string revision)
		{
			const string errPrefix =
				"Automatic cherry-pick failed.  After resolving the conflicts,\n" +
				"mark the corrected paths with 'git add <paths>' or 'git rm <paths>'\n" +
				"and commit the result with: \n\n" +
				"        git commit -c ";
			const string errPostfix = "\n\n";

			return (error.Length == errPrefix.Length + errPostfix.Length + revision.Length) &&
				CheckValues(error, errPrefix, revision, errPostfix);
		}

		private static bool IsCherryPickIsEmptyError(string error)
		{
			const string err =
				"The previous cherry-pick is now empty, possibly due to conflict resolution.\n" +
				"If you wish to commit it anyway, use:\n" +
				"\n" +
				"    git commit --allow-empty\n" +
				"\n" +
				"Otherwise, please use 'git reset'\n";

			return error == err;
		}

		private static bool IsAutomaticCherryPickFailedError(string error)
		{
			const string errPrefix =
				"Automatic cherry-pick failed.  After resolving the conflicts,\n" +
				"mark the corrected paths with 'git add <paths>' or 'git rm <paths>'\n" +
				"and commit the result with: \n\n" +
				"        git commit -c ";
			const string errPostfix = "\n\n";

			return error.Length > errPrefix.Length + errPostfix.Length &&
				error.StartsWith(errPrefix) && error.EndsWith(errPostfix);
		}

		private static bool IsNothingToApplyError(string error)
		{
			const string err = "Nothing to apply\n";

			return error == err;
		}

		private static bool IsCannotApplyToDirtyWorkingTreeError(string error)
		{
			const string err = "Cannot apply to a dirty working tree, please stage your changes\n";

			return error == err;
		}

		private static bool IsCantStashToEmptyRepositoryError(string error)
		{
			const string err =
				"fatal: bad revision 'HEAD'\n" +
				"fatal: bad revision 'HEAD'\n" +
				"fatal: Needed a single revision\n" +
				"You do not have the initial commit yet\n";

			return error == err;
		}

		private static bool IsNoHEADCommitToCompareWithError(string error)
		{
			const string err = "fatal: No HEAD commit to compare with (yet)\n";

			return error == err;
		}

		private static bool IsFileDoesNotNeedMergingError(string error, string file)
		{
			const string err = ": file does not need merging\n";

			return error == file + err;
		}
	}
}

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

namespace gitter.Git
{
	using System;

	public static class GitConstants
	{
		/// <summary>.git</summary>
		public const string GitDir = ".git";
		/// <summary>.lock</summary>
		public const string LockPostfix = ".lock";
		/// <summary>HEAD</summary>
		public const string HEAD = "HEAD";
		/// <summary>MERGE_HEAD</summary>
		public const string MERGE_HEAD = "MERGE_HEAD";
		/// <summary>CHERRY_PICK_HEAD</summary>
		public const string CHERRY_PICK_HEAD = "CHERRY_PICK_HEAD";
		/// <summary>REVERT_HEAD</summary>
		public const string REVERT_HEAD = "REVERT_HEAD";
		/// <summary>ORIG_HEAD</summary>
		public const string ORIG_HEAD = "ORIG_HEAD";
		/// <summary>FETCH_HEAD</summary>
		public const string FETCH_HEAD = "FETCH_HEAD";
		/// <summary>HEAD^</summary>
		public const string PrevHEAD = "HEAD^";
		/// <summary>(no branch)</summary>
		public const string NoBranch = "(no branch)";

		/// <summary>.gitignore</summary>
		public const string IgnoreFile = ".gitignore";
		/// <summary>.mailmap</summary>
		public const string MailMapFile = ".mailmap";
		/// <summary>COMMIT_MSG</summary>
		public const string CommitMessageFileName = "COMMIT_MSG";
		/// <summary>TAG_MSG</summary>
		public const string TagMessageFileName = "TAG_MSG";

		/// <summary>refs/</summary>
		public const string RefsPrefix = "refs/";
		/// <summary>refs/heads/</summary>
		public const string LocalBranchPrefix = "refs/heads/";
		/// <summary>heads/</summary>
		public const string LocalBranchShortPrefix = "heads/";
		/// <summary>refs/remotes/</summary>
		public const string RemoteBranchPrefix = "refs/remotes/";
		/// <summary>remotes/</summary>
		public const string RemoteBranchShortPrefix = "remotes/";
		/// <summary>refs/tags/</summary>
		public const string TagPrefix = "refs/tags/";
		/// <summary>tags/</summary>
		public const string TagShortPrefix = "tags/";
		/// <summary>refs/stash</summary>
		public const string StashFullName = "refs/stash";
		/// <summary>stash</summary>
		public const string StashName = "stash";

		/// <summary>origin</summary>
		public const string DefaultRemoteName = "origin";

		/// <summary>commit</summary>
		public const string CommitObjectType = "commit";
		/// <summary>blob</summary>
		public const string BlobObjectType = "blob";
		/// <summary>tag</summary>
		public const string TagObjectType = "tag";
		/// <summary>tree</summary>
		public const string TreeObjectType = "tree";

		/// <summary>^{}</summary>
		public const string DereferencedTagPostfix = "^{}";

		/// <summary>user.name</summary>
		public const string UserNameParameter = "user.name";
		/// <summary>user.email</summary>
		public const string UserEmailParameter = "user.email";
		/// <summary>merge.tool</summary>
		public const string MergeToolParameter = "merge.tool";
		/// <summary>core.logallrefupdates</summary>
		public const string CoreLogAllRefUpdatesParameter = "core.logallrefupdates";

		/// <summary>/dev/null</summary>
		public const string DevNull = "/dev/null";

		/// <summary>.gitmodules</summary>
		public const string SubmodulesConfigFile = ".gitmodules";

		/// <summary>shallow</summary>
		public const string ShallowFile = "shallow";

		/// <summary>1 Jan 1970</summary>
		public static readonly DateTime UnixEraStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>1 Jan 1970</summary>
		public static readonly DateTimeOffset UnixEraStartOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
	}
}

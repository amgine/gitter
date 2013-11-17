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
	/// <summary>Importaint environment variable names.</summary>
	public static class GitEnvironment
	{
		/// <summary>HOME</summary>
		public const string Home = "HOME";

		/// <summary>DISPLAY</summary>
		public const string Display = "DISPLAY";

		/// <summary>
		/// If this environment variable is set, then git commands which need to acquire passwords or
		/// passphrases (e.g. for HTTP or IMAP authentication) will call this program with a suitable
		/// prompt as command line argument and read the password from its STDOUT. See also the core.askpass
		/// option in git-config.
		/// </summary>
		/// <value>GIT_ASKPASS</value>
		public const string GitAskPass = "GIT_ASKPASS";

		/// <summary>SSH_ASKPASS</summary>
		public const string SshAskPass = "SSH_ASKPASS";

		/// <summary>PLINK_PROTOCOL</summary>
		public const string PlinkProtocol = "PLINK_PROTOCOL";

		/// <summary>GIT_NOTES_REF</summary>
		public const string GitNotesRef = "GIT_NOTES_REF";

		/// <summary>GIT_INDEX_FILE</summary>
		public const string GitIndexFile = "GIT_INDEX_FILE";

		/// <summary>GIT_OBJECT_DIRECTORY</summary>
		public const string GitObjectDircetory = "GIT_OBJECT_DIRECTORY";

		/// <summary>GIT_ALTERNATE_OBJECT_DIRECTORIES</summary>
		public const string GitAlternateObjectDirectories = "GIT_ALTERNATE_OBJECT_DIRECTORIES";

		/// <summary>GIT_DIR</summary>
		public const string GitDir = "GIT_DIR";

		/// <summary>GIT_WORK_TREE</summary>
		public const string GitWorkTree = "GIT_WORK_TREE";

		/// <summary>GIT_NAMESPACE</summary>
		public const string GitNamespace = "GIT_NAMESPACE";

		/// <summary>GIT_CEILING_DIRECTORIES</summary>
		public const string GitCeilingDirectories = "GIT_CEILING_DIRECTORIES";

		/// <summary>GIT_DISCOVERY_ACROSS_FILESYSTEM</summary>
		public const string GitDiscoveryAcrossFilesystem = "GIT_DISCOVERY_ACROSS_FILESYSTEM";

		/// <summary>GIT_AUTHOR_NAME</summary>
		public const string GitAuthorName = "GIT_AUTHOR_NAME";

		/// <summary>GIT_AUTHOR_EMAIL</summary>
		public const string GitAuthorEmail = "GIT_AUTHOR_EMAIL";

		/// <summary>GIT_AUTHOR_DATE</summary>
		public const string GitAuthorDate = "GIT_AUTHOR_DATE";

		/// <summary>GIT_COMMITTER_NAME</summary>
		public const string GitCommitterName = "GIT_COMMITTER_NAME";

		/// <summary>GIT_COMMITTER_EMAIL</summary>
		public const string GitCommitterEmail = "GIT_COMMITTER_EMAIL";

		/// <summary>GIT_COMMITTER_DATE</summary>
		public const string GitCommitterDate = "GIT_COMMITTER_DATE";

		/// <summary>EMAIL</summary>
		public const string Email = "EMAIL";

		/// <summary>
		/// Only valid setting is "--unified=??" or "-u??" to set the number of context lines shown when a unified diff
		/// is created. This takes precedence over any "-U" or "--unified" option value passed on the git diff command line.
		/// </summary>
		/// <value>GIT_DIFF_OPTS</value>
		public const string GitDiffOpts = "GIT_DIFF_OPTS";

		/// <summary>
		/// When the environment variable GIT_EXTERNAL_DIFF is set, the program named by it is called, instead of the diff
		/// invocation described above. For a path that is added, removed, or modified, GIT_EXTERNAL_DIFF is called
		/// with 7 parameters:
		/// path old-file old-hex old-mode new-file new-hex new-mode
		/// </summary>
		/// <value>GIT_EXTERNAL_DIFF</value>
		public const string GitExternalDiff = "GIT_EXTERNAL_DIFF";

		/// <summary>
		/// A number controlling the amount of output shown by the recursive merge strategy. Overrides merge.verbosity.
		/// See git-merge.
		/// </summary>
		/// <value>GIT_MERGE_VERBOSITY</value>
		public const string GitMergeVerbosity = "GIT_MERGE_VERBOSITY";

		/// <summary>
		/// This environment variable overrides $PAGER. If it is set to an empty string or to the value "cat", git
		/// will not launch a pager. See also the core.pager option in git-config.
		/// </summary>
		/// <value>GIT_PAGER</value>
		public const string GitPager = "GIT_PAGER";

		/// <summary>
		/// This environment variable overrides $EDITOR and $VISUAL. It is used by several git commands when, on
		/// interactive mode, an editor is to be launched. See also git-var and the core.editor option in git-config.
		/// </summary>
		/// <value>GIT_EDITOR</value>
		public const string GitEditor = "GIT_EDITOR";

		/// <summary>
		/// If this environment variable is set then git fetch and git push will use this command instead of ssh when they
		/// need to connect to a remote system. The $GIT_SSH command will be given exactly two arguments: the username@host
		/// (or just host) from the URL and the shell command to execute on that remote system.
		/// To pass options to the program that you want to list in GIT_SSH you will need to wrap the program and options
		/// into a shell script, then set GIT_SSH to refer to the shell script.
		/// Usually it is easier to configure any desired options through your personal .ssh/config file. Please consult
		/// your ssh documentation for further details.
		/// </summary>
		/// <value>GIT_SSH</value>
		public const string GitSsh = "GIT_SSH";

		/// <summary>
		/// Whether to skip reading settings from the system-wide $(prefix)/etc/gitconfig file. This environment variable
		/// can be used along with $HOME and $XDG_CONFIG_HOME to create a predictable environment for a picky script, or
		/// you can set it temporarily to avoid using a buggy /etc/gitconfig file while waiting for someone with sufficient
		/// permissions to fix it.
		/// </summary>
		/// <value>GIT_CONFIG_NOSYSTEM</value>
		public const string GitConfigNoSystem = "GIT_CONFIG_NOSYSTEM";

		/// <summary>
		/// If this environment variable is set to "1", then commands such as git blame (in incremental mode), git rev-list,
		/// git log, and git whatchanged will force a flush of the output stream after each commit-oriented record have been
		/// flushed. If this variable is set to "0", the output of these commands will be done using completely buffered I/O.
		/// If this environment variable is not set, git will choose buffered or record-oriented flushing based on whether
		/// stdout appears to be redirected to a file or not.
		/// </summary>
		/// <value>GIT_FLUSH</value>
		public const string GitFlush = "GIT_FLUSH";

		/// <summary>
		/// If this variable is set to "1", "2" or "true" (comparison is case insensitive), git will print trace: messages on
		/// stderr telling about alias expansion, built-in command execution and external command execution. If this variable
		/// is set to an integer value greater than 1 and lower than 10 (strictly) then git will interpret this value as an
		/// open file descriptor and will try to write the trace messages into this file descriptor. Alternatively, if this
		/// variable is set to an absolute path (starting with a / character), git will interpret this as a file path and will
		/// try to write the trace messages into it.
		/// </summary>
		/// <value>GIT_TRACE</value>
		public const string GitTrace = "GIT_TRACE";
	}
}

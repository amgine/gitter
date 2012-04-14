namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	public sealed class KnownConfigParameter
	{
		private readonly string _name;
		private readonly string _defaultValue;
		private readonly string[] _validValues;
		private readonly string _description;

		internal KnownConfigParameter(string name, string defaultValue, string[] validValues, string description)
		{
			_name = name;
			_defaultValue = defaultValue;
			_validValues = validValues;
			_description = description;
		}

		public string Name
		{
			get { return _name; }
		}

		public string DefaultValue
		{
			get { return _defaultValue; }
		}

		public string[] ValidValues
		{
			get { return _validValues; }
		}

		public string Description
		{
			get { return _description; }
		}
	}

	public static class KnownConfigParameters
	{
		private static readonly Dictionary<string, KnownConfigParameter> _knownParameters = new Dictionary<string, KnownConfigParameter>();
		private static readonly string[] _booleanValues = new string[] { "true", "false" };
		private static readonly string[] _compressionLevels = new string[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

		private static void AddParameter(KnownConfigParameter parameter)
		{
			_knownParameters.Add(parameter.Name, parameter);
		}

		static KnownConfigParameters()
		{
			#region advice
			AddParameter(new KnownConfigParameter("advice.pushNonFastForward", "true", _booleanValues, @"Advice shown when 'git push' refuses non-fast-forward refs."));
			AddParameter(new KnownConfigParameter("advice.statusHints", "true", _booleanValues, @"Directions on how to stage/unstage/add shown in the output of 'git status' and the template shown when writing commit messages."));
			AddParameter(new KnownConfigParameter("advice.commitBeforeMerge", "true", _booleanValues, @"Advice shown when 'git merge' refuses to merge to avoid overwritting local changes."));
			AddParameter(new KnownConfigParameter("advice.resolveConflict", "true", _booleanValues, @"Advices shown by various commands when conflicts prevent the operation from being performed."));
			AddParameter(new KnownConfigParameter("advice.implicitIdentity", "true", _booleanValues, @"Advice on how to set your identity configuration when your information is guessed from the system username and domain name."));
			AddParameter(new KnownConfigParameter("advice.detachedHead", "true", _booleanValues, @"Advice shown when you used 'git checkout' to move to the detach HEAD state, to instruct how to create a local branch after the fact."));
			#endregion
			#region core
			AddParameter(new KnownConfigParameter("core.filemode", "true", _booleanValues, @"If false, the executable bit differences between the index and the working copy are ignored; useful on broken filesystems like FAT."));
			AddParameter(new KnownConfigParameter("core.ignorecygwinfstricks", "true", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.ignorecase", "false", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.trustctime", "true", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.quotepath", "true", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.eol", "native", new[]{"lf", "crlf", "native"}, @""));
			AddParameter(new KnownConfigParameter("core.safecrlf", "true", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.autocrlf", "true", new[]{"true", "false", "input"}, @""));
			AddParameter(new KnownConfigParameter("core.symlinks", "true", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.gitProxy", "", null, @""));
			AddParameter(new KnownConfigParameter("core.ignoreStat", "false", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.preferSymlinkRefs", "false", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.bare", "false", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.worktree", "", null, @""));
			AddParameter(new KnownConfigParameter("core.logallrefupdates", "true", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.repositoryformatversion", "", null, @""));
			AddParameter(new KnownConfigParameter("core.sharedRepository", "false", new[] {"group", "true", "world", "everybody", "umask", "false"}, @""));
			AddParameter(new KnownConfigParameter("core.warnambiguousrefs", "true", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.compression", "-1", _compressionLevels, @""));
			AddParameter(new KnownConfigParameter("core.packedGitWindowSize", "32m", null, @""));
			AddParameter(new KnownConfigParameter("core.packedGitLimit", "256m", null, @""));
			AddParameter(new KnownConfigParameter("core.deltaBaseCacheLimit", "16m", null, @""));
			AddParameter(new KnownConfigParameter("core.bigFileThreshold", "512m", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.excludesfile", "", null, @""));
			AddParameter(new KnownConfigParameter("core.editor", "", null, @""));
			AddParameter(new KnownConfigParameter("core.pager", "", null, @""));
			AddParameter(new KnownConfigParameter("core.whitespace", "", null, @""));
			AddParameter(new KnownConfigParameter("core.fsyncobjectfiles", "false", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.preloadindex", "false", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("core.createObject", "link", new[]{"link", "rename"}, @""));
			AddParameter(new KnownConfigParameter("core.notesRef", "refs/notes/commits", null, @""));
			AddParameter(new KnownConfigParameter("core.sparseCheckout", "false", _booleanValues, @""));
			#endregion
			#region add
			AddParameter(new KnownConfigParameter("add.ignore-errors", "false", _booleanValues, @""));
			#endregion
			#region commit
			AddParameter(new KnownConfigParameter("commit.status", "true", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("commit.template", "", null, @""));
			#endregion
			#region i18n
			AddParameter(new KnownConfigParameter("i18n.commitEncoding", "utf-8", _booleanValues, @""));
			AddParameter(new KnownConfigParameter("i18n.logOutputEncoding", "utf-8", null, @""));
			#endregion
			#region user
			AddParameter(new KnownConfigParameter(GitConstants.UserEmailParameter, "", null, @"Your email address to be recorded in any newly created commits. Can be overridden by the GIT_AUTHOR_EMAIL, GIT_COMMITTER_EMAIL, and EMAIL environment variables."));
			AddParameter(new KnownConfigParameter(GitConstants.UserNameParameter, "", null, @"Your full name to be recorded in any newly created commits. Can be overridden by the GIT_AUTHOR_NAME and GIT_COMMITTER_NAME environment variables."));
			AddParameter(new KnownConfigParameter("user.signingkey", "", null, @"If 'git tag' is not selecting the key you want it to automatically when creating a signed tag, you can override the default selection with this variable. This option is passed unchanged to gpg's --local-user parameter, so you may specify a key using any method that gpg supports."));
			#endregion
		}
	}
}

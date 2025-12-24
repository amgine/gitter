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

namespace gitter.Git;

using System;

using Resources = gitter.Git.AccessLayer.Properties.Resources;

/// <summary>git command failed.</summary>
[Serializable]
public class GitException : Exception
{
	public GitException() { }

	public GitException(string message)
		: base(message) { }

	public GitException(string message, Exception inner)
		: base(message, inner) { }
}

/// <summary>Generic parser exception.</summary>
[Serializable]
public class GitParserException : GitException
{
	public GitParserException() { }

	public GitParserException(string message)
		: base(message) { }

	public GitParserException(string message, Exception inner)
		: base(message, inner) { }
}

/// <summary>Notifies that <see cref="M:BranchName"/> is not merged.</summary>
[Serializable]
public class BranchIsNotFullyMergedException : GitException
{
	public BranchIsNotFullyMergedException(string branchName)
		: base(string.Format(Resources.ExcBranchIsNotFullyMerged, branchName))
	{
		BranchName = branchName;
	}

	public BranchIsNotFullyMergedException(string branchName, Exception inner)
		: base(string.Format(Resources.ExcBranchIsNotFullyMerged, branchName), inner)
	{
		BranchName = branchName;
	}

	public string BranchName { get; }
}

/// <summary>Notifies that <see cref="M:BranchName"/> already exists.</summary>
[Serializable]
public class BranchAlreadyExistsException : GitException
{
	public BranchAlreadyExistsException(string branchName)
		: base(string.Format(Resources.ExcBranchAlreadyExists, branchName))
	{
		BranchName = branchName;
	}

	public BranchAlreadyExistsException(string branchName, Exception inner)
		: base(string.Format(Resources.ExcBranchAlreadyExists, branchName), inner)
	{
		BranchName = branchName;
	}

	public string BranchName { get; }
}

/// <summary>Notifies that <see cref="M:BranchName"/> not found.</summary>
[Serializable]
public class BranchNotFoundException : GitException
{
	public BranchNotFoundException(string branchName)
		: base(string.Format(Resources.ExcBranchNotFound, branchName))
	{
		BranchName = branchName;
	}

	public BranchNotFoundException(string branchName, Exception inner)
		: base(string.Format(Resources.ExcBranchNotFound, branchName), inner)
	{
		BranchName = branchName;
	}

	public string BranchName { get; }
}

/// <summary>Notifies that <see cref="M:BranchName"/> is invalid.</summary>
[Serializable]
public class InvalidBranchNameException : GitException
{
	public InvalidBranchNameException(string branchName)
		: base(string.Format(Resources.ExcInvalidBranchName, branchName))
	{
		BranchName = branchName;
	}

	public InvalidBranchNameException(string branchName, Exception inner)
		: base(string.Format(Resources.ExcInvalidBranchName, branchName), inner)
	{
		BranchName = branchName;
	}

	public string BranchName { get; }
}

/// <summary>Notifies that <see cref="M:TagName"/> already exists.</summary>
[Serializable]
public class TagAlreadyExistsException : GitException
{
	public TagAlreadyExistsException(string tagName)
		: base(string.Format(Resources.ExcTagAlreadyExists, tagName))
	{
		TagName = tagName;
	}

	public TagAlreadyExistsException(string tagName, Exception inner)
		: base(string.Format(Resources.ExcTagAlreadyExists, tagName), inner)
	{
		TagName = tagName;
	}

	public string TagName { get; }
}

/// <summary>Notifies that <see cref="M:TagName"/> not found.</summary>
[Serializable]
public class TagNotFoundException : GitException
{
	public TagNotFoundException(string tagName)
		: base(string.Format(Resources.ExcTagNotFound, tagName))
	{
		TagName = tagName;
	}

	public TagNotFoundException(string tagName, Exception inner)
		: base(string.Format(Resources.ExcTagNotFound, tagName), inner)
	{
		TagName = tagName;
	}

	public string TagName { get; }
}

/// <summary>Notifies that <see cref="M:TagName"/> is invalid.</summary>
[Serializable]
public class InvalidTagNameException : GitException
{
	public InvalidTagNameException(string tagName)
		: base(string.Format(Resources.ExcTagNotFound, tagName))
	{
		TagName = tagName;
	}

	public InvalidTagNameException(string tagName, Exception inner)
		: base(string.Format(Resources.ExcTagNotFound, tagName), inner)
	{
		TagName = tagName;
	}

	public string TagName { get; }
}

/// <summary>Notifies that <see cref="M:Revision"/> is unknown.</summary>
[Serializable]
public class UnknownRevisionException : GitException
{
	public UnknownRevisionException(string revision)
		: base(string.Format(Resources.ExcUnknownRevision, revision))
	{
		Revision = revision;
	}
		
	public UnknownRevisionException(string revision, Exception inner)
		: base(string.Format(Resources.ExcUnknownRevision, revision), inner)
	{
		Revision = revision;
	}

	public string Revision { get; }
}

/// <summary>Is thrown when operation can't execute due to presence of untracked file.</summary>
[Serializable]
public class UntrackedFileWouldBeOverwrittenException : DirtyWorkingDirectoryException
{
	public UntrackedFileWouldBeOverwrittenException(string fileName)
		: base(string.Format(Resources.ExcUntrackedFileWouldBeOverwrittenException, fileName))
	{
		FileName = fileName;
	}

	public UntrackedFileWouldBeOverwrittenException(string fileName, Exception inner)
		: base(string.Format(Resources.ExcUntrackedFileWouldBeOverwrittenException, fileName), inner)
	{
		FileName = fileName;
	}

	public string FileName { get; }
}

/// <summary>Is thrown when operation can't execute on dirty working tree.</summary>
[Serializable]
public class HaveLocalChangesException : DirtyWorkingDirectoryException
{
	public HaveLocalChangesException(string fileName)
		: base(string.Format(Resources.ExcYouHaveLocalChanges, fileName))
	{
		FileName = fileName;
	}

	public HaveLocalChangesException(string fileName, Exception inner)
		: base(string.Format(Resources.ExcYouHaveLocalChanges, fileName), inner)
	{
		FileName = fileName;
	}

	public string FileName { get; }
}

/// <summary>Is thrown when operation can't execute on working tree containing unresolved conflicts.</summary>
[Serializable]
public class HaveConflictsException : DirtyWorkingDirectoryException
{
	public HaveConflictsException()
		: base(Resources.ExcHaveConflicts)
	{
	}

	public HaveConflictsException(string message)
		: base(message)
	{
	}
		
	public HaveConflictsException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

/// <summary>Is thrown when operation can't execute because the commit is a merge.</summary>
[Serializable]
public class CommitIsMergeException : GitException
{
	public CommitIsMergeException()
		: base(Resources.ExcCommitIsMerge)
	{
	}

	public CommitIsMergeException(string message)
		: base(message)
	{
	}

	public CommitIsMergeException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

/// <summary>Is thrown when operation can't execute on dirty working directory.</summary>
[Serializable]
public class DirtyWorkingDirectoryException : GitException
{
	public DirtyWorkingDirectoryException()
		: base(Resources.ExcDirtyWorkingDirectory)
	{
	}

	public DirtyWorkingDirectoryException(string message)
		: base(message)
	{
	}

	public DirtyWorkingDirectoryException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

/// <summary>Is thrown when merge resulted in conflicts.</summary>
[Serializable]
public class AutomaticMergeFailedException : GitException
{
	public AutomaticMergeFailedException()
		: base(Resources.ExcAutomaticMergeFailed)
	{
	}

	public AutomaticMergeFailedException(string message)
		: base(message)
	{
	}

	public AutomaticMergeFailedException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

/// <summary>Is thrown when cherry-pick resulted in conflicts.</summary>
[Serializable]
public class AutomaticCherryPickFailedException : GitException
{
	public AutomaticCherryPickFailedException()
		: base(Resources.ExcAutomaticCherryPickFailed)
	{
	}

	public AutomaticCherryPickFailedException(string message)
		: base(message)
	{
	}

	public AutomaticCherryPickFailedException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

/// <summary>Is thrown when cherry-pick resulted in no changes.</summary>
[Serializable]
public class CherryPickIsEmptyException : GitException
{
	public CherryPickIsEmptyException()
		: base(Resources.ExcAutomaticCherryPickFailed)
	{
	}

	public CherryPickIsEmptyException(string message)
		: base(message)
	{
	}

	public CherryPickIsEmptyException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

/// <summary>Is thrown when attempted to operate on empty stash.</summary>
[Serializable]
public class StashIsEmptyException : GitException
{
	public StashIsEmptyException()
	{
	}

	public StashIsEmptyException(string message)
		: base(message)
	{
	}

	public StashIsEmptyException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

/// <summary>Is thrown when attempted to operate on empty repository.</summary>
[Serializable]
public class RepositoryIsEmptyException : GitException
{
	public RepositoryIsEmptyException()
		: base(Resources.ExcCantDoOnEmptyRepository)
	{
	}

	public RepositoryIsEmptyException(string message)
		: base(message)
	{
	}

	public RepositoryIsEmptyException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

/// <summary>Thrown when failed to communicate with remote.</summary>
[Serializable]
public class ConnectionException : GitException
{
	public ConnectionException()
		: base(Resources.ExcConnectionFailure)
	{
	}

	public ConnectionException(string message)
		: base(message)
	{
	}

	public ConnectionException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

[Serializable]
public class ConfigParameterDoesNotExistException : GitException
{
	public ConfigParameterDoesNotExistException()
		: base(Resources.ExcConfigParameterDoesNotExist)
	{
	}

	public ConfigParameterDoesNotExistException(string message)
		: base(message)
	{
	}

	public ConfigParameterDoesNotExistException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

[Serializable]
public class InvalidConfigFileException : GitException
{
	public InvalidConfigFileException()
		: base(Resources.ExcInvalidConfigFile)
	{
	}

	public InvalidConfigFileException(string message)
		: base(message)
	{
	}

	public InvalidConfigFileException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

[Serializable]
public class CannotWriteConfigFileException : GitException
{
	public CannotWriteConfigFileException()
		: base(Resources.ExcCannotWriteConfigFile)
	{
	}

	public CannotWriteConfigFileException(string message)
		: base(message)
	{
	}

	public CannotWriteConfigFileException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

[Serializable]
public class NoSectionProvidedException : GitException
{
	public NoSectionProvidedException()
		: base(Resources.ExcNoSectionProvided)
	{
	}

	public NoSectionProvidedException(string message)
		: base(message)
	{
	}

	public NoSectionProvidedException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

[Serializable]
public class InvalidSectionOrKeyException : GitException
{
	public InvalidSectionOrKeyException()
		: base(Resources.ExcInvalidSectionOrKey)
	{
	}

	public InvalidSectionOrKeyException(string message)
		: base(message)
	{
	}

	public InvalidSectionOrKeyException(string message, Exception inner)
		: base(message, inner)
	{
	}
}

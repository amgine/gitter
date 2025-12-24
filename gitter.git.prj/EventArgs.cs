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

using gitter.Framework;

/// <summary><see cref="EventArgs"/> identifying some object of type <typeparamref name="T"/>.</summary>
/// <typeparam name="T">Type of associated object.</typeparam>
/// <param name="object">Object, associated with event.</param>
public class ObjectEventArgs<T>(T @object) : EventArgs
{
	/// <summary>Object, associated with event.</summary>
	public T Object { get; } = @object;
}

/// <summary><see cref="EventArgs"/> specifying changed value of type <typeparamref name="T"/>.</summary>
/// <typeparam name="T">Type of associated object.</typeparam>
/// <param name="oldValue">Old value.</param>
/// <param name="newValue">New value.</param>
public class ObjectChangedEventArgs<T>(T? oldValue, T? newValue) : EventArgs
{
	/// <summary>Old value.</summary>
	public T? OldValue { get; } = oldValue;

	/// <summary>New value.</summary>
	public T? NewValue { get; } = newValue;
}

/// <summary><see cref="EventArgs"/> identifying <see cref="Branch"/>.</summary>
/// <param name="branch"><see cref="Branch"/> which is related to event.</param>
public class BranchEventArgs(Branch branch) : ObjectEventArgs<Branch>(branch);

/// <summary>Arguments for branch renamed event.</summary>
/// <param name="branch"><see cref="Branch"/> which is related to event.</param>
/// <param name="oldName">Old branch name.</param>
public class BranchRenamedEventArgs(Branch branch, string oldName) : BranchEventArgs(branch)
{
	/// <summary>Returns old branch name.</summary>
	/// <value>Old branch name.</value>
	public string OldName { get; } = oldName;
}

/// <summary><see cref="EventArgs"/> identifying <see cref="RemoteBranch"/>.</summary>
/// <param name="branch"><see cref="RemoteBranch"/> which is related to event.</param>
public class RemoteBranchEventArgs(RemoteBranch branch) : ObjectEventArgs<RemoteBranch>(branch);

/// <summary><see cref="EventArgs"/> identifying <see cref="Tag"/>.</summary>
/// <param name="tag"><see cref="Tag"/> which is related to event.</param>
public class TagEventArgs(Tag tag) : ObjectEventArgs<Tag>(tag);

/// <summary><see cref="EventArgs"/> identifying <see cref="ReflogRecord"/>.</summary>
/// <param name="reflogRecord"><see cref="ReflogRecord"/> which is related to event.</param>
public class ReflogRecordEventArgs(ReflogRecord reflogRecord) : ObjectEventArgs<ReflogRecord>(reflogRecord);

/// <summary><see cref="EventArgs"/> identifying <see cref="IRevisionPointer"/>.</summary>
/// <param name="revision"><see cref="IRevisionPointer"/> which is related to event.</param>
public class RevisionPointerEventArgs(IRevisionPointer revision) : ObjectEventArgs<IRevisionPointer>(revision);

/// <summary><see cref="EventArgs"/> identifying <see cref="Revision"/>.</summary>
/// <param name="revision"><see cref="Revision"/> which is related to event.</param>
public class RevisionEventArgs(Revision revision) : ObjectEventArgs<Revision>(revision);

/// <summary><see cref="EventArgs"/> identifying change of <see cref="Revision"/>.</summary>
/// <param name="oldRevision">Old revision.</param>
/// <param name="newRevision">New revision.</param>
public class RevisionChangedEventArgs(Revision? oldRevision, Revision? newRevision) : ObjectChangedEventArgs<Revision>(oldRevision, newRevision);

/// <summary><see cref="EventArgs"/> identifying change of <see cref="IRevisionPointer"/>.</summary>
/// <param name="oldRevision">Old revision.</param>
/// <param name="newRevision">New revision.</param>
public class RevisionPointerChangedEventArgs(IRevisionPointer oldRevision, IRevisionPointer newRevision)
	: ObjectChangedEventArgs<IRevisionPointer>(oldRevision, newRevision);

/// <summary><see cref="EventArgs"/> identifying <see cref="Remote"/>.</summary>
/// <param name="remote"><see cref="Remote"/> which is related to event.</param>
public class RemoteEventArgs(Remote remote) : ObjectEventArgs<Remote>(remote);

public abstract class RemoteOperationCompletedEventArgs(Remote? remote, Many<ReferenceChange> changes) : EventArgs
{
	public Remote? Remote { get; } = remote;

	public Many<ReferenceChange> Changes { get; } = changes;
}

public class FetchCompletedEventArgs(Remote? remote, Many<ReferenceChange> changes)
	: RemoteOperationCompletedEventArgs(remote, changes);

public class PullCompletedEventArgs(Remote? remote, Many<ReferenceChange> changes)
	: RemoteOperationCompletedEventArgs(remote, changes);

public class PruneCompletedEventArgs(Remote? remote, Many<ReferenceChange> changes)
	: RemoteOperationCompletedEventArgs(remote, changes);

/// <summary><see cref="EventArgs"/> identifying <see cref="StashedState"/>.</summary>
/// <param name="stashedState"><see cref="StashedState"/> which is related to event.</param>
public class StashedStateEventArgs(StashedState stashedState) : ObjectEventArgs<StashedState>(stashedState);

/// <summary><see cref="EventArgs"/> identifying <see cref="ConfigParameter"/>.</summary>
/// <param name="configParameter"><see cref="ConfigParameter"/> which is related to event.</param>
public class ConfigParameterEventArgs(ConfigParameter configParameter) : ObjectEventArgs<ConfigParameter>(configParameter);

/// <summary><see cref="EventArgs"/> identifying <see cref="Submodule"/>.</summary>
/// <param name="submodule"><see cref="Submodule"/> which is related to event.</param>
public class SubmoduleEventArgs(Submodule submodule) : ObjectEventArgs<Submodule>(submodule);

/// <summary><see cref="EventArgs"/> identifying <see cref="User"/>.</summary>
/// <param name="user"><see cref="User"/> which is related to event.</param>
public class UserEventArgs(User user) : ObjectEventArgs<User>(user);

/// <summary><see cref="EventArgs"/>for renaming events.</summary>
/// <param name="oldName">Old object's name.</param>
/// <param name="newName">New object's name</param>
public class NameChangeEventArgs(string oldName, string newName) : EventArgs
{
	/// <summary>New object's name.</summary>
	public string NewName { get; } = newName;

	/// <summary>Old object's name.</summary>
	public string OldName { get; } = oldName;
}

/// <summary><see cref="EventArgs"/> identifying <see cref="TreeFile"/>.</summary>
public class TreeFileEventArgs(TreeFile file) : EventArgs
{
	public TreeFile File { get; } = file;
}

/// <summary><see cref="EventArgs"/> identifying <see cref="TreeCommit"/>.</summary>
public class TreeCommitEventArgs(TreeCommit commit) : ObjectEventArgs<TreeCommit>(commit);

/// <summary><see cref="EventArgs"/> identifying <see cref="TreeDirectory"/>.</summary>
public class TreeDirectoryEventArgs(TreeDirectory folder) : EventArgs
{
	public TreeDirectory Folder { get; } = folder;
}

/// <summary><see cref="EventArgs"/> identifying <see cref="Note"/>.</summary>
/// <param name="note"><see cref="Note"/> which is related to event.</param>
public class NoteEventArgs(Note note) : EventArgs
{
	/// <summary><see cref="Note"/> which is related to event.</summary>
	public Note Note { get; } = note;
}

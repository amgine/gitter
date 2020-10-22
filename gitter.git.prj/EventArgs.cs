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

	/// <summary><see cref="EventArgs"/> identifying some object of type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">Type of associated object.</typeparam>
	public class ObjectEventArgs<T> : EventArgs
	{
		/// <summary>Create <see cref="ObjectEventArgs&lt;T&gt;"/>.</summary>
		/// <param name="object">Object, associated with event.</param>
		public ObjectEventArgs(T @object)
		{
			Object = @object;
		}

		/// <summary>Object, associated with event.</summary>
		public T Object { get; }
	}

	/// <summary><see cref="EventArgs"/> specifying changed value of type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">Type of associated object.</typeparam>
	public class ObjectChangedEventArgs<T> : EventArgs
	{
		/// <summary>Create <see cref="ObjectChangedEventArgs&lt;T&gt;"/>.</summary>
		/// <param name="oldValue">Old value.</param>
		/// <param name="newValue">New value.</param>
		public ObjectChangedEventArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		/// <summary>Old value.</summary>
		public T OldValue { get; }

		/// <summary>New value.</summary>
		public T NewValue { get; }
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="Branch"/>.</summary>
	public class BranchEventArgs : ObjectEventArgs<Branch>
	{
		/// <summary>Create <see cref="BranchEventArgs"/>.</summary>
		/// <param name="branch"><see cref="Branch"/> which is related to event.</param>
		public BranchEventArgs(Branch branch)
			: base(branch)
		{
		}
	}

	/// <summary>Arguments for branch renamed event.</summary>
	public class BranchRenamedEventArgs : BranchEventArgs
	{
		/// <summary>Create <see cref="BranchRenamedEventArgs"/>.</summary>
		/// <param name="branch"><see cref="Branch"/> which is related to event.</param>
		public BranchRenamedEventArgs(Branch branch, string oldName)
			: base(branch)
		{
			OldName = oldName;
		}

		/// <summary>Returns old branch name.</summary>
		/// <value>Old branch name.</value>
		public string OldName { get; }
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="RemoteBranch"/>.</summary>
	public class RemoteBranchEventArgs : ObjectEventArgs<RemoteBranch>
	{
		/// <summary>Create <see cref="RemoteBranchEventArgs"/>.</summary>
		/// <param name="branch"><see cref="RemoteBranch"/> which is related to event.</param>
		public RemoteBranchEventArgs(RemoteBranch branch)
			: base(branch)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="Tag"/>.</summary>
	public class TagEventArgs : ObjectEventArgs<Tag>
	{
		/// <summary>Create <see cref="TagEventArgs"/>.</summary>
		/// <param name="tag"><see cref="Tag"/> which is related to event.</param>
		public TagEventArgs(Tag tag)
			: base(tag)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="ReflogRecord"/>.</summary>
	public class ReflogRecordEventArgs : ObjectEventArgs<ReflogRecord>
	{
		/// <summary>Create <see cref="ReflogRecordEventArgs"/>.</summary>
		/// <param name="reflogRecord"><see cref="ReflogRecord"/> which is related to event.</param>
		public ReflogRecordEventArgs(ReflogRecord reflogRecord)
			: base(reflogRecord)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="IRevisionPointer"/>.</summary>
	public class RevisionPointerEventArgs : ObjectEventArgs<IRevisionPointer>
	{
		/// <summary>Create <see cref="RevisionPointerEventArgs"/>.</summary>
		/// <param name="revision"><see cref="IRevisionPointer"/> which is related to event.</param>
		public RevisionPointerEventArgs(IRevisionPointer revision)
			: base(revision)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="Revision"/>.</summary>
	public class RevisionEventArgs : ObjectEventArgs<Revision>
	{
		/// <summary>Create <see cref="RevisionEventArgs"/>.</summary>
		/// <param name="revision"><see cref="Revision"/> which is related to event.</param>
		public RevisionEventArgs(Revision revision)
			: base(revision)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying change of <see cref="Revision"/>.</summary>
	public class RevisionChangedEventArgs : ObjectChangedEventArgs<Revision>
	{
		/// <summary>Create <see cref="RevisionChangedEventArgs"/>.</summary>
		/// <param name="oldRevision">Old revision.</param>
		/// <param name="newRevision">New revision.</param>
		public RevisionChangedEventArgs(Revision oldRevision, Revision newRevision)
			: base(oldRevision, newRevision)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying change of <see cref="IRevisionPointer"/>.</summary>
	public class RevisionPointerChangedEventArgs : ObjectChangedEventArgs<IRevisionPointer>
	{
		/// <summary>Create <see cref="RevisionPointerChangedEventArgs"/>.</summary>
		/// <param name="oldRevision">Old revision.</param>
		/// <param name="newRevision">New revision.</param>
		public RevisionPointerChangedEventArgs(IRevisionPointer oldRevision, IRevisionPointer newRevision)
			: base(oldRevision, newRevision)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="Remote"/>.</summary>
	public class RemoteEventArgs : ObjectEventArgs<Remote>
	{
		/// <summary>Create <see cref="RemoteEventArgs"/>.</summary>
		/// <param name="remote"><see cref="Remote"/> which is related to event.</param>
		public RemoteEventArgs(Remote remote)
			: base(remote)
		{
		}
	}

	public abstract class RemoteOperationCompletedEventArgs : EventArgs
	{
		#region .ctor

		protected RemoteOperationCompletedEventArgs(Remote remote, ReferenceChange[] changes)
		{
			Remote  = remote;
			Changes = changes;
		}

		#endregion

		#region Properties

		public Remote Remote { get; }

		public ReferenceChange[] Changes { get; }

		#endregion
	}

	public class FetchCompletedEventArgs : RemoteOperationCompletedEventArgs
	{
		public FetchCompletedEventArgs(Remote remote, ReferenceChange[] changes)
			: base(remote, changes)
		{
		}
	}

	public class PullCompletedEventArgs : RemoteOperationCompletedEventArgs
	{
		public PullCompletedEventArgs(Remote remote, ReferenceChange[] changes)
			: base(remote, changes)
		{
		}
	}

	public class PruneCompletedEventArgs : RemoteOperationCompletedEventArgs
	{
		public PruneCompletedEventArgs(Remote remote, ReferenceChange[] changes)
			: base(remote, changes)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="StashedState"/>.</summary>
	public class StashedStateEventArgs : ObjectEventArgs<StashedState>
	{
		/// <summary>Create <see cref="StashedStateEventArgs"/>.</summary>
		/// <param name="stashedState"><see cref="StashedState"/> which is related to event.</param>
		public StashedStateEventArgs(StashedState stashedState)
			: base(stashedState)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="ConfigParameter"/>.</summary>
	public class ConfigParameterEventArgs : ObjectEventArgs<ConfigParameter>
	{
		/// <summary>Create <see cref="ConfigParameterEventArgs"/>.</summary>
		/// <param name="configParameter"><see cref="ConfigParameter"/> which is related to event.</param>
		public ConfigParameterEventArgs(ConfigParameter configParameter)
			: base(configParameter)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="Submodule"/>.</summary>
	public class SubmoduleEventArgs : ObjectEventArgs<Submodule>
	{
		/// <summary>Create <see cref="SubmoduleEventArgs"/>.</summary>
		/// <param name="submodule"><see cref="Submodule"/> which is related to event.</param>
		public SubmoduleEventArgs(Submodule submodule)
			: base(submodule)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="User"/>.</summary>
	public class UserEventArgs : ObjectEventArgs<User>
	{
		/// <summary>Create <see cref="UserEventArgs"/>.</summary>
		/// <param name="user"><see cref="User"/> which is related to event.</param>
		public UserEventArgs(User user)
			: base(user)
		{
		}
	}

	/// <summary><see cref="EventArgs"/>for renaming events.</summary>
	public class NameChangeEventArgs : EventArgs
	{
		/// <summary>Create <see cref="NameChangeEventArgs"/>.</summary>
		/// <param name="oldName">Old object's name.</param>
		/// <param name="newName">New object's name</param>
		public NameChangeEventArgs(string oldName, string newName)
		{
			OldName = oldName;
			NewName = newName;
		}

		/// <summary>New object's name.</summary>
		public string NewName { get; }

		/// <summary>Old object's name.</summary>
		public string OldName { get; }
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="TreeFile"/>.</summary>
	public class TreeFileEventArgs : EventArgs
	{
		public TreeFileEventArgs(TreeFile file)
		{
			File = file;
		}

		public TreeFile File { get; }
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="TreeCommit"/>.</summary>
	public class TreeCommitEventArgs : ObjectEventArgs<TreeCommit>
	{
		public TreeCommitEventArgs(TreeCommit commit)
			: base(commit)
		{
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="TreeDirectory"/>.</summary>
	public class TreeDirectoryEventArgs : EventArgs
	{
		public TreeDirectoryEventArgs(TreeDirectory folder)
		{
			Folder = folder;
		}

		public TreeDirectory Folder { get; }
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="Note"/>.</summary>
	public class NoteEventArgs : EventArgs
	{
		/// <summary>Create <see cref="NoteEventArgs"/>.</summary>
		/// <param name="note"><see cref="Note"/> which is related to event.</param>
		public NoteEventArgs(Note note)
		{
			Note = note;
		}

		/// <summary><see cref="Note"/> which is related to event.</summary>
		public Note Note { get; }
	}
}

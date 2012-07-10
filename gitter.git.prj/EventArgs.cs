namespace gitter.Git
{
	using System;

	/// <summary><see cref="EventArgs"/> identifying some object of type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">Type of associated object.</typeparam>
	public class ObjectEventArgs<T> : EventArgs
	{
		private readonly T _object;

		/// <summary>Create <see cref="ObjectEventArgs&lt;T&gt;"/>.</summary>
		/// <param name="object">Object, associated with event.</param>
		public ObjectEventArgs(T @object)
		{
			_object = @object;
		}

		/// <summary>Object, associated with event.</summary>
		public T Object
		{
			get { return _object; }
		}
	}

	/// <summary><see cref="EventArgs"/> cpecifying changed value of type <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">Type of associated object.</typeparam>
	public class ObjectChangedEventArgs<T> : EventArgs
	{
		#region Data

		private readonly T _oldValue;
		private readonly T _newValue;

		#endregion

		/// <summary>Create <see cref="ObjectChangedEventArgs&lt;T&gt;"/>.</summary>
		/// <param name="oldValue">Old value.</param>
		/// <param name="newValue">New value.</param>
		public ObjectChangedEventArgs(T oldValue, T newValue)
		{
			_oldValue = oldValue;
			_newValue = newValue;
		}

		/// <summary>Old value.</summary>
		public T OldValue
		{
			get { return _oldValue; }
		}

		/// <summary>New value.</summary>
		public T NewValue
		{
			get { return _newValue; }
		}
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
		/// <summary>Old branch name.</summary>
		private readonly string _oldName;

		/// <summary>Create <see cref="BranchRenamedEventArgs"/>.</summary>
		/// <param name="branch"><see cref="Branch"/> which is related to event.</param>
		public BranchRenamedEventArgs(Branch branch, string oldName)
			: base(branch)
		{
			_oldName = oldName;
		}

		/// <summary>Returns old branch name.</summary>
		/// <value>Old branch name.</value>
		public string OldName
		{
			get { return _oldName; }
		}
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

	/// <summary><see cref="EventArgs"/> identifying <see cref="Committer"/>.</summary>
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
		private readonly string _oldName;
		private readonly string _newName;

		/// <summary>Create <see cref="NameChangeEventArgs"/>.</summary>
		/// <param name="oldName">Old object's name.</param>
		/// <param name="newName">New object's name</param>
		public NameChangeEventArgs(string oldName, string newName)
		{
			_oldName = oldName;
			_newName = newName;
		}

		/// <summary>New object's name.</summary>
		public string NewName
		{
			get { return _newName; }
		}

		/// <summary>Old object's name.</summary>
		public string OldName
		{
			get { return _oldName;}
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="TreeFile"/>.</summary>
	public class TreeFileEventArgs : EventArgs
	{
		private readonly TreeFile _file;

		public TreeFileEventArgs(TreeFile file)
		{
			_file = file;
		}

		public TreeFile File
		{
			get { return _file; }
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="TreeDirectory"/>.</summary>
	public class TreeDirectoryEventArgs : EventArgs
	{
		private readonly TreeDirectory _folder;

		public TreeDirectoryEventArgs(TreeDirectory folder)
		{
			_folder = folder;
		}

		public TreeDirectory Folder
		{
			get { return _folder; }
		}
	}

	/// <summary><see cref="EventArgs"/> identifying <see cref="Note"/>.</summary>
	public class NoteEventArgs : EventArgs
	{
		private readonly Note _note;

		/// <summary>Create <see cref="NoteEventArgs"/>.</summary>
		/// <param name="note"><see cref="Note"/> which is related to event.</param>
		public NoteEventArgs(Note note)
		{
			_note = note;
		}

		/// <summary><see cref="Note"/> which is related to event.</summary>
		public Note Note
		{
			get { return _note; }
		}
	}
}

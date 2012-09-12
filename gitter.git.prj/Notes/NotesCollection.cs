namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	/// <summary>Collection of repository <see cref="Note"/> objects.</summary>
	public sealed class NotesCollection : GitObject, IEnumerable<Note>
	{
		#region Events

		/// <summary>Note created/detected.</summary>
		public event EventHandler<NoteEventArgs> Created;

		/// <summary>Note deleted/lost.</summary>
		public event EventHandler<NoteEventArgs> Deleted;

		/// <summary>Invokes <see cref="Created"/> event.</summary>
		/// <param name="note">Created <see cref="Note"/>.</param>
		private void InvokeCreated(Note note)
		{
			var handler = Created;
			if(handler != null) handler(this, new NoteEventArgs(note));
		}

		/// <summary>Invokes <see cref="Deleted"/> & other related events.</summary>
		/// <param name="note">Deleted <see cref="Note"/>.</param>
		private void InvokeDeleted(Note note)
		{
			note.MarkAsDeleted();
			var handler = Deleted;
			if(handler != null) handler(this, new NoteEventArgs(note));
		}

		#endregion

		private readonly Dictionary<string, Note> _notes;

		/// <summary>Create <see cref="NotesCollection"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		internal NotesCollection(Repository repository)
			: base(repository)
		{
			_notes = new Dictionary<string, Note>();
		}

		public object SyncRoot
		{
			get { return _notes; }
		}

		public Note this[string noteObjectName]
		{
			get { lock(SyncRoot) return _notes[noteObjectName]; }
		}

		public int Count
		{
			get { lock(SyncRoot) return _notes.Count; }
		}

		public bool Exists(string note)
		{
			Verify.Argument.IsNotNull(note, "note");

			lock(SyncRoot)
			{
				return _notes.ContainsKey(note);
			}
		}

		public bool TryGetNote(string name, out Note note)
		{
			Verify.Argument.IsNotNull(name, "name");

			lock(SyncRoot)
			{
				return _notes.TryGetValue(name, out note);
			}
		}

		public Note TryGetNote(string name)
		{
			Verify.Argument.IsNotNull(name, "name");

			Note note;
			lock(SyncRoot)
			{
				_notes.TryGetValue(name, out note);
			}
			return note;
		}

		public void Refresh()
		{
			var notes = Repository.Accessor.QueryNotes(
				new QueryNotesParameters());
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<Note, NoteData>(
					_notes,
					null,
					null,
					notes,
					noteData => ObjectFactories.CreateNote(Repository, noteData),
					ObjectFactories.UpdateNode,
					InvokeCreated,
					InvokeDeleted,
					true);
			}
		}

		internal Note Add(IRevisionPointer revision, string message)
		{
			Repository.Accessor.AppendNote(
				new AppendNoteParameters()
				{
					Revision = revision.Pointer,
					Message = message,
				});
			return null;
		}

		#region IEnumerable<Note> Members

		public IEnumerator<Note> GetEnumerator()
		{
			return _notes.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _notes.Values.GetEnumerator();
		}

		#endregion
	}
}

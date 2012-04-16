namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Object which can perform various operations on git notes.</summary>
	public interface INotesAccessor
	{
		/// <summary>Get list of all note objects.</summary>
		/// <param name="parameters"><see cref="QueryNotesParameters"/>.</param>
		/// <returns>List of all note objects.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<NoteData> QueryNotes(QueryNotesParameters parameters);

		/// <summary>Append new note to object.</summary>
		/// <param name="parameters"><see cref="AppendNoteParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void AppendNote(AppendNoteParameters parameters);
	}
}

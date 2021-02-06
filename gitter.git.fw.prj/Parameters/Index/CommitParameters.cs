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
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.Commit"/> operation.</summary>
	public sealed class CommitParameters
	{
		/// <summary>Create <see cref="CommitParameters"/>.</summary>
		public CommitParameters()
		{
		}

		/// <summary>Create <see cref="CommitParameters"/>.</summary>
		/// <param name="message">Commit message.</param>
		public CommitParameters(string message)
		{
			Message = message;
		}

		/// <summary>
		/// Automatically stage files that have been modified and deleted, but new files
		/// you have not told git about are not affected.
		/// </summary>
		public bool All { get; set; }

		/// <summary>
		/// Take an existing commit object, and reuse the log message and the authorship information
		/// (including the timestamp) when creating the commit.
		/// </summary>
		public string ReuseMessageFrom { get; set; }

		/// <summary>
		/// When used with -C/-c/--amend options, declare that the authorship of the resulting commit
		/// now belongs of the committer. This also renews the author timestamp.
		/// </summary>
		public bool ResetAuthor { get; set; }

		/// <summary>Use this as the commit message.</summary>
		public string Message { get; set; }

		/// <summary>Take the commit message from the given file.</summary>
		public string MessageFileName { get; set; }

		/// <summary>Overrides the commit author.</summary>
		public string Author { get; set; }

		/// <summary>Overrides the author date used in the commit.</summary>
		public DateTime? AuthorDate { get; set; }

		/// <summary>Add Signed-off-by line by the committer at the end of the commit log message.</summary>
		public bool SignOff { get; set; }

		/// <summary>This option bypasses the pre-commit and commit-msg hooks.</summary>
		public bool NoVerify { get; set; }

		/// <summary>Allows to create a commit with te same tree as its parent.</summary>
		public bool AllowEmpty { get; set; }

		/// <summary>Allows to create commit without any message.</summary>
		public bool AllowEmptyMessage { get; set; }

		/// <summary>Used to amend the tip of the current branch.</summary>
		public bool Amend { get; set; }
	}
}

#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Git local branch.</summary>
	public sealed class Branch : BranchBase
	{
		#region Static

		/// <summary>Validates the branch name.</summary>
		/// <param name="name">Branch name.</param>
		/// <param name="errorMessage">Error message.</param>
		/// <returns><c>true</c> if <paramref name="name"/> is a valid branch name; otherwise, <c>false</c>.</returns>
		public static bool ValidateName(string name, out string errorMessage)
		{
			if(!Reference.ValidateName(name, ReferenceType.Branch, out errorMessage))
			{
				return false;
			}
			if(name == GitConstants.HEAD)
			{
				errorMessage = Resources.ErrCannotCreateHeadManually;
				return false;
			}
			return true;
		}

		#endregion

		#region Events

		/// <summary>Occurs when branch is renamed.</summary>
		public event EventHandler<NameChangeEventArgs> Renamed;

		/// <summary>Invokes <see cref="Renamed"/> event.</summary>
		private void InvokeNameChanged(string oldName, string newName)
		{
			var handler = Renamed;
			if(handler != null) handler(this, new NameChangeEventArgs(oldName, newName));
		}

		#endregion

		#region .ctor

		/// <summary>Create <see cref="Branch"/> object.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Branch name.</param>
		/// <param name="pointer">Branch position.</param>
		/// <exception cref="ArgumentNullException">
		/// <para><paramref name="repository"/> == null or</para>
		/// <para><paramref name="position"/> == null.</para>
		/// </exception>
		internal Branch(Repository repository, string name, IRevisionPointer pointer)
			: base(repository, name, pointer)
		{
		}

		#endregion

		#region Properties

		/// <summary>Gets the type of this reference.</summary>
		/// <value><see cref="ReferenceType.LocalBranch"/>.</value>
		public override ReferenceType Type
		{
			get { return ReferenceType.LocalBranch; }
		}

		/// <summary>Gets the full branch name.</summary>
		/// <value>Full branch name.</value>
		public override string FullName
		{
			get { return GitConstants.LocalBranchPrefix + Name; }
		}

		/// <summary>Gets a value indicating whether this branch is remote.</summary>
		/// <value><c>false</c>.</value>
		public override bool IsRemote
		{
			get { return false; }
		}

		/// <summary>Gets a value indicating whether this branch is current HEAD.</summary>
		/// <value><c>true</c> if this instance is current HEAD; otherwise, <c>false</c>.</value>
		public override bool IsCurrent
		{
			get { return Repository.Head.Pointer == this; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reset this branch to position specified by <paramref name="revision"/>.
		/// </summary>
		/// <param name="revision">New branch position.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="branch"/> == null or <paramref name="revision"/> == null.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="branch"/> is not handled by this <see cref="Repository"/> or it is deleted;
		/// <paramref name="revision"/> is not handled by this <see cref="Repository"/> or it is deleted.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">
		/// Failed to dereference <paramref name="revision"/> or git reset failed.
		/// </exception>
		public void Reset(IRevisionPointer revision)
		{
			Verify.Argument.IsValidRevisionPointer(revision, Repository, "revision");
			Verify.State.IsNotDeleted(this);

			var rev = revision.Dereference();
			if(Revision != rev)
			{
				using(Repository.Monitor.BlockNotifications(
					RepositoryNotifications.BranchChanged))
				{
					Repository.Accessor.ResetBranch.Invoke(
						new ResetBranchParameters(Name, revision.Pointer));
				}
				Pointer = rev;
				NotifyRelogRecordAdded();
			}
		}

		/// <summary>Delete branch.</summary>
		/// <exception cref="T:git.BranchIsNotFullyMergedException">
		/// Branch is not fully merged and can only be deleted by calling <see cref="Delete(bool)"/>
		/// with <paramref name="force"/> == true.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">
		/// Failed to delete <paramref name="branch"/>.
		/// </exception>
		public override void Delete()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Refs.Heads.Delete(this, false);
		}

		/// <summary>Delete branch.</summary>
		/// <param name="force">Delete branch irrespective of its merged status.</param>
		/// <exception cref="T:git.BranchIsNotFullyMergedException">
		/// Branch is not fully merged and can only be deleted if <paramref name="force"/> == true.
		/// </exception>
		/// <exception cref="T:gitter.Git.GitException">
		/// Failed to delete <paramref name="branch"/>.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="Branch"/> is already deleted.
		/// </exception>
		public override void Delete(bool force)
		{
			Verify.State.IsNotDeleted(this);

			Repository.Refs.Heads.Delete(this, force);
		}

		/// <summary>
		/// Makes shure that this <see cref="Branch"/> exists and <see cref="M:Position"/> is correct.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// This <see cref="Branch"/> is deleted.
		/// </exception>
		public override void Refresh()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Refs.Heads.Refresh(this);
		}

		#endregion

		#region Overrides

		/// <summary>Rename branch.</summary>
		/// <param name="newName">New name.</param>
		protected override void RenameCore(string newName)
		{
			Verify.State.IsNotDeleted(this);

			Repository.Refs.Heads.Rename(this, newName);
		}

		/// <summary>Called after branch is renamed.</summary>
		/// <param name="oldName">Old name.</param>
		protected override void AfterRename(string oldName)
		{
			InvokeNameChanged(oldName, Name);
			Repository.Refs.Heads.NotifyRenamed(this, oldName);
		}

		#endregion
	}
}

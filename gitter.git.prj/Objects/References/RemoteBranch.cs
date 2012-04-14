namespace gitter.Git
{
	using System;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Git remote tracking branch.</summary>
	public sealed class RemoteBranch : BranchBase
	{
		/// <summary>Create <see cref="Branch"/> object.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Branch name.</param>
		/// <param name="pointer">Branch position.</param>
		/// <exception cref="ArgumentNullException">
		/// <para><paramref name="repository"/> == null or</para>
		/// <para><paramref name="position"/> == null.</para>
		/// </exception>
		internal RemoteBranch(Repository repository, string name, IRevisionPointer pointer)
			: base(repository, name, pointer)
		{
		}

		/// <summary>Gets a value indicating whether this branch is remote.</summary>
		/// <value><c>true</c>.</value>
		public override bool IsRemote
		{
			get { return true; }
		}

		/// <summary>Gets a value indicating whether this branch is current HEAD.</summary>
		/// <value><c>false</c>.</value>
		/// <remarks><see cref="RemoteBranch"/> can't be current HEAD.</remarks>
		public override bool IsCurrent
		{
			get { return false; }
		}

		/// <summary>Gets the type of this reference.</summary>
		/// <value><see cref="ReferenceType.RemoteBranch"/>.</value>
		public override ReferenceType Type
		{
			get { return ReferenceType.RemoteBranch; }
		}

		/// <summary>Gets the full branch name.</summary>
		/// <value>Full branch name.</value>
		public override string FullName
		{
			get { return GitConstants.RemoteBranchPrefix + Name; }
		}

		/// <summary>Delete branch.</summary>
		/// <exception cref="T:git.BranchIsNotFullyMergedException">Branch is not fully merged and can only be deleted by calling <see cref="Delete(bool)"/> with <paramref name="force"/> == true.</exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to delete <paramref name="branch"/>.</exception>
		public override void Delete()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(
					Resources.ExcObjectIsDeleted.UseAsFormat("RemoteBranch"));
			}

			#endregion

			Repository.Refs.Remotes.Delete(this, false);
		}

		/// <summary>Delete branch.</summary>
		/// <param name="force">Delete branch irrespective of its merged status.</param>
		/// <exception cref="T:git.BranchIsNotFullyMergedException">Branch is not fully merged and can only be deleted if <paramref name="force"/> == true.</exception>
		/// <exception cref="T:gitter.Git.GitException">Failed to delete <paramref name="branch"/>.</exception>
		/// <exception cref="InvalidOperationException">This <see cref="Branch"/> is already deleted.</exception>
		public override void Delete(bool force)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(
					Resources.ExcObjectIsDeleted.UseAsFormat("RemoteBranch"));
			}

			#endregion

			Repository.Refs.Remotes.Delete(this, force);
		}

		/// <summary>Makes shure that this <see cref="Branch"/> exists and <see cref="M:Position"/> is correct.</summary>
		/// <exception cref="InvalidOperationException">This <see cref="Branch"/> is deleted.</exception>
		public override void Refresh()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(
					Resources.ExcObjectIsDeleted.UseAsFormat("RemoteBranch"));
			}

			#endregion

			Repository.Refs.Remotes.Refresh(this);
		}
	}
}

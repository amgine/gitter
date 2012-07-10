namespace gitter.Git
{
	using System;

	using gitter.Git.AccessLayer;

	/// <summary>Git branch (local or remote).</summary>
	public abstract class BranchBase : Reference
	{
		/// <summary>Initializes a new instance of the <see cref="BranchBase"/> class.</summary>
		/// <param name="repository">Host <see cref="Repository"/>.</param>
		/// <param name="name">Reference name.</param>
		/// <param name="pointer">Referenced object.</param>
		internal BranchBase(Repository repository, string name, IRevisionPointer pointer)
			: base(repository, name, pointer)
		{
		}

		/// <summary>Gets a value indicating whether this branch is remote.</summary>
		/// <value><c>true</c> if this branch is remote; otherwise, <c>false</c>.</value>
		public abstract bool IsRemote { get; }

		/// <summary>Gets a value indicating whether this branch is current HEAD.</summary>
		/// <value><c>true</c> if this instance is current HEAD; otherwise, <c>false</c>.</value>
		public abstract bool IsCurrent { get; }

		/// <summary>Delete branch.</summary>
		public abstract void Delete();

		/// <summary>Delete branch.</summary>
		/// <param name="force">Force-remove branch.</param>
		public abstract void Delete(bool force);

		/// <summary>Refreshes cached information for this <see cref="BranchBase"/>.</summary>
		public abstract void Refresh();

		/// <summary>Notifies about external branch reset.</summary>
		/// <param name="branchInformation">Updated branch information.</param>
		/// <exception cref="ArgumentNullException"><paramref name="branchInformation"/> == <c>null</c>.</exception>
		internal void NotifyReset(BranchData branchInformation)
		{
			if(branchInformation == null) throw new ArgumentNullException("branchInformation");

			if(Revision.Name != branchInformation.SHA1)
			{
				lock(Repository.Revisions.SyncRoot)
				{
					Pointer = Repository.Revisions.GetOrCreateRevision(branchInformation.SHA1);
				}
			}
		}

		/// <summary>Filter <see cref="IRevisionPointer"/> to types supported by this <see cref="BranchBase"/>.</summary>
		/// <param name="pointer">Raw pointer.</param>
		/// <returns>Valid pointer.</returns>
		protected override IRevisionPointer PrepareInputPointer(IRevisionPointer pointer)
		{
			return pointer.Dereference();
		}

		/// <summary><see cref="ReferenceType"/>.</summary>
		/// <value><see cref="ReferenceType.RemoteBranch"/> or <see cref="ReferenceType.LocalBranch"/>.</value>
		public override ReferenceType Type
		{
			get { return IsRemote ? ReferenceType.RemoteBranch : ReferenceType.LocalBranch; }
		}

		/// <summary>Gets the full branch name.</summary>
		/// <value>Full barnch name.</value>
		public override string FullName
		{
			get { return (IsRemote ? GitConstants.RemoteBranchPrefix : GitConstants.LocalBranchPrefix) + Name; }
		}
	}
}

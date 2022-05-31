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
using System.Threading.Tasks;

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
	/// <param name="force">Force-remove branch.</param>
	public abstract void Delete(bool force = false);

	/// <summary>Delete branch.</summary>
	/// <param name="force">Force-remove branch.</param>
	public abstract Task DeleteAsync(bool force = false);

	/// <summary>Refreshes cached information for this <see cref="BranchBase"/>.</summary>
	public abstract void Refresh();

	/// <summary>Refreshes cached information for this <see cref="BranchBase"/>.</summary>
	public abstract Task RefreshAsync();

	/// <summary>Notifies about external branch reset.</summary>
	/// <param name="branchInformation">Updated branch information.</param>
	/// <exception cref="ArgumentNullException"><paramref name="branchInformation"/> == <c>null</c>.</exception>
	internal void NotifyReset(BranchData branchInformation)
	{
		Verify.Argument.IsNotNull(branchInformation);

		if(Revision.Hash != branchInformation.SHA1)
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
		Verify.Argument.IsNotNull(pointer);

		return pointer.Dereference();
	}

	/// <summary><see cref="ReferenceType"/>.</summary>
	/// <value><see cref="ReferenceType.RemoteBranch"/> or <see cref="ReferenceType.LocalBranch"/>.</value>
	public override ReferenceType Type
		=> IsRemote ? ReferenceType.RemoteBranch : ReferenceType.LocalBranch;

	/// <summary>Gets the full branch name.</summary>
	/// <value>Full branch name.</value>
	public override string FullName
		=> (IsRemote ? GitConstants.RemoteBranchPrefix : GitConstants.LocalBranchPrefix) + Name;
}

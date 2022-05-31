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
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

public sealed class RevisionTreeSource : TreeSourceBase
{
	#region Data

	private readonly IRevisionPointer _revision;

	#endregion

	#region .ctor

	public RevisionTreeSource(IRevisionPointer revision)
	{
		Verify.Argument.IsNotNull(revision);
		Verify.Argument.IsFalse(revision.IsDeleted, nameof(revision), "Specified revision is deleted.");

		_revision = revision;
	}

	#endregion

	#region Properties

	public override IRevisionPointer Revision => _revision;

	protected override Tree GetTreeCore()
	{
		return new Tree(Revision.Repository, Revision.Pointer);
	}

	public override string DisplayName
	{
		get
		{
			var pointer = Revision.Pointer;
			if(Revision.Type == ReferenceType.Revision && pointer.Length == 40)
			{
				return pointer.Substring(0, 7);
			}
			else
			{
				return pointer;
			}
		}
	}

	#endregion

	#region Methods

	public override Tree GetTree()
		=> new Tree(Revision.Repository, Revision.FullName);

	public override Task<Tree> GetTreeAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		=> Tree.GetAsync(Revision.Repository, Revision.FullName, progress, cancellationToken);

	/// <inheritdoc/>
	public override bool Equals(object obj)
		=> obj is RevisionTreeSource other && _revision == other._revision;

	/// <inheritdoc/>
	public override int GetHashCode() => _revision.GetHashCode();

	/// <inheritdoc/>
	public override string ToString() => _revision.Pointer;

	#endregion
}

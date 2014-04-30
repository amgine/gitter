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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	/// <summary>Represents a branch on remote repository (not a remote tracking branch).</summary>
	public sealed class RemoteRepositoryBranch: BaseRemoteReference
	{
		internal RemoteRepositoryBranch(RemoteReferencesCollection refs, string name, Hash hash)
			: base(refs, name, hash)
		{
		}

		public override ReferenceType ReferenceType
		{
			get { return ReferenceType.LocalBranch; }
		}

		protected override void DeleteCore()
		{
			References.RemoveBranch(this);
		}

		protected override Task DeleteCoreAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return References.RemoveBranchAsync(this, progress, cancellationToken);
		}
	}
}

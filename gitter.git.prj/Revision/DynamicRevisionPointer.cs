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

/// <summary>Dynamic revision pointer.</summary>
internal sealed class DynamicRevisionPointer : IRevisionPointer
{
	#region .ctor

	public DynamicRevisionPointer(Repository repository, string pointer)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNeitherNullNorWhitespace(pointer);

		Repository = repository;
		Pointer    = pointer;
	}

	#endregion

	#region Properties

	/// <summary>Host repository. Never null.</summary>
	public Repository Repository { get; }

	/// <summary><see cref="ReferenceType"/>.</summary>
	public ReferenceType Type => ReferenceType.Revision;

	/// <summary>Revision expression (reference name, sha1, relative expression, etc.).</summary>
	public string Pointer { get; }

	/// <summary>Returns full non-ambiguous revision name.</summary>
	public string FullName => Pointer;

	/// <summary>Object is deleted and not valid anymore.</summary>
	public bool IsDeleted => false;

	#endregion

	#region Methods

	/// <inheritdoc/>
	public Revision Dereference()
	{
		var rev = Repository.Accessor.Dereference.Invoke(
			new DereferenceParameters(Pointer));
		lock(Repository.Revisions.SyncRoot)
		{
			return Repository.Revisions.GetOrCreateRevision(rev.CommitHash);
		}
	}

	/// <inheritdoc/>
	public async ValueTask<Revision> DereferenceAsync()
	{
		var rev = await Repository.Accessor.Dereference
			.InvokeAsync(new DereferenceParameters(Pointer))
			.ConfigureAwait(continueOnCapturedContext: false);
		lock(Repository.Revisions.SyncRoot)
		{
			return Repository.Revisions.GetOrCreateRevision(rev.CommitHash);
		}
	}

	#endregion
}

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

/// <summary>Static revision pointer.</summary>
internal class StaticRevisionPointer : IRevisionPointer
{
	#region Data

	private readonly string _pointer;
	private Revision _revision;

	#endregion

	#region .ctor

	public StaticRevisionPointer(Repository repository, string pointer)
	{
		Verify.Argument.IsNotNull(repository);
		Verify.Argument.IsNeitherNullNorWhitespace(pointer);

		Repository = repository;
		_pointer   = pointer;
	}

	#endregion

	#region Properties

	public Repository Repository { get; }

	public virtual ReferenceType Type => ReferenceType.Revision;

	public virtual string Pointer => _pointer;

	public virtual string FullName => _pointer;

	public virtual bool IsDeleted => false;

	#endregion

	#region Methods

	/// <inheritdoc/>
	public virtual Revision Dereference()
	{
		if(_revision is null)
		{
			var rev = Repository.Accessor.Dereference
				.Invoke(new DereferenceParameters(Pointer));
			lock(Repository.Revisions.SyncRoot)
			{
				_revision = Repository.Revisions.GetOrCreateRevision(rev.CommitHash);
			}
		}
		return _revision;
	}

	protected virtual async ValueTask<Revision> DereferenceCoreAsync()
	{
		var rev = await Repository.Accessor.Dereference
			.InvokeAsync(new DereferenceParameters(Pointer))
			.ConfigureAwait(continueOnCapturedContext: false);
		lock(Repository.Revisions.SyncRoot)
		{
			_revision = Repository.Revisions.GetOrCreateRevision(rev.CommitHash);
		}
		return _revision;
	}

	/// <inheritdoc/>
	public ValueTask<Revision> DereferenceAsync()
		=> _revision is not null
			? new(_revision)
			: DereferenceCoreAsync();

	#endregion
}

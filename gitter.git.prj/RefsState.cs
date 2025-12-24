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
using System.Collections.Generic;

sealed class RefsState
{
	public static RefsState Capture(Repository repository, ReferenceType referenceTypes)
	{
		Verify.Argument.IsNotNull(repository);

		return new RefsState(repository, referenceTypes);
	}

	public sealed record class ReferenceState(
		ReferenceType ReferenceType,
		string        FullName,
		string        Name,
		Sha1Hash      Hash)
	{
		static Sha1Hash GetHash(Reference reference)
		{
			var pointer = reference.Pointer;
			if(pointer is null) return default;
			var revision = pointer.Dereference();
			if(revision is null) return default;
			return revision.Hash;
		}

		public static ReferenceState FromReference(Reference reference) => new(
			ReferenceType: reference.Type,
			FullName:      reference.FullName,
			Name:          reference.Name,
			Hash:          GetHash(reference));
	}

	private readonly Dictionary<string, ReferenceState> _states = [];

	public IEnumerable<ReferenceState> States => _states.Values;

	public ReferenceState? GetState(string fullName, ReferenceType type)
		=> _states.TryGetValue(fullName, out var state) && state.ReferenceType == type
			? state
			: default;

	private RefsState(Repository repository, ReferenceType referenceTypes)
	{
		Assert.IsNotNull(repository);

		if((referenceTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
		{
			CaptureReferences(repository.Refs.Heads);
		}
		if((referenceTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch)
		{
			CaptureReferences(repository.Refs.Remotes);
		}
		if((referenceTypes & ReferenceType.Tag) == ReferenceType.Tag)
		{
			CaptureReferences(repository.Refs.Tags);
		}
	}

	private void CaptureReferences<T, K>(GitObjectsCollection<T, K> collection)
		where T : Reference
		where K : ObjectEventArgs<T>
	{
		lock(collection.SyncRoot)
		{
			foreach(var reference in collection)
			{
				CaptureRefState(reference);
			}
		}
	}

	private void CaptureRefState(Reference reference)
	{
		Assert.IsNotNull(reference);

		var refState = ReferenceState.FromReference(reference);
		_states[refState.FullName] = refState;
	}
}

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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	sealed class RefsState
	{
		public static RefsState Capture(Repository repository, ReferenceType referenceTypes)
		{
			if(repository == null) throw new ArgumentNullException("repository");

			return new RefsState(repository, referenceTypes);
		}

		public sealed class ReferenceState
		{
			private readonly ReferenceType _referenceType;
			private readonly string _fullName;
			private readonly string _name;
			private readonly Hash _hash;

			public ReferenceState(ReferenceType referenceType, string fullName, string name, Hash hash)
			{
				_referenceType = referenceType;
				_fullName = fullName;
				_name = name;
				_hash = hash;
			}

			public ReferenceType ReferenceType
			{
				get { return _referenceType; }
			}

			public string FullName
			{
				get { return _fullName; }
			}

			public string Name
			{
				get { return _name;}
			}

			public Hash Hash
			{
				get { return _hash; }
			}
		}

		private readonly Dictionary<string, ReferenceState> _states;

		public IEnumerable<ReferenceState> States
		{
			get { return _states.Values; }
		}

		public ReferenceState GetState(string fullName, ReferenceType type)
		{
			ReferenceState state;
			if(_states.TryGetValue(fullName, out state))
			{
				if(state.ReferenceType != type)
				{
					state = null;
				}
			}
			return state;
		}

		private RefsState(Repository repository, ReferenceType referenceTypes)
		{
			_states = new Dictionary<string, ReferenceState>();

			if((referenceTypes & ReferenceType.LocalBranch) == ReferenceType.LocalBranch)
			{
				CaptureHeads(repository);
			}
			if((referenceTypes & ReferenceType.RemoteBranch) == ReferenceType.RemoteBranch)
			{
				CaptureRemotes(repository);
			}
			if((referenceTypes & ReferenceType.Tag) == ReferenceType.Tag)
			{
				CaptureTags(repository);
			}
		}

		private void CaptureHeads(Repository repository)
		{
			lock(repository.Refs.Heads.SyncRoot)
			{
				foreach(var head in repository.Refs.Heads)
				{
					CaptureRefState(head);
				}
			}
		}

		private void CaptureRemotes(Repository repository)
		{
			lock(repository.Refs.Remotes.SyncRoot)
			{
				foreach(var remoteHead in repository.Refs.Remotes)
				{
					CaptureRefState(remoteHead);
				}
			}
		}

		private void CaptureTags(Repository repository)
		{
			lock(repository.Refs.Tags.SyncRoot)
			{
				foreach(var tag in repository.Refs.Tags)
				{
					CaptureRefState(tag);
				}
			}
		}

		private void CaptureRefState(Reference reference)
		{
			var fullName = reference.FullName;
			var refState = new ReferenceState(reference.Type, fullName, reference.Name, reference.Pointer.Dereference().Hash);
			_states[fullName] = refState;
		}
	}
}

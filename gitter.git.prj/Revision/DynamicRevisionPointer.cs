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

	using gitter.Git.AccessLayer;

	/// <summary>Dynamic revision pointer.</summary>
	internal sealed class DynamicRevisionPointer : IRevisionPointer
	{
		#region Data

		private readonly Repository _repository;
		private readonly string _pointer;

		#endregion

		#region .ctor

		public DynamicRevisionPointer(Repository repository, string pointer)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNeitherNullNorWhitespace(pointer, "pointer");

			_repository = repository;
			_pointer = pointer;
		}

		#endregion

		#region Properties

		/// <summary>Host repository. Never null.</summary>
		public Repository Repository
		{
			get { return _repository; }
		}

		/// <summary><see cref="ReferenceType"/>.</summary>
		public ReferenceType Type
		{
			get { return ReferenceType.Revision; }
		}

		/// <summary>Revision expression (reference name, sha1, relative expression, etc.).</summary>
		public string Pointer
		{
			get { return _pointer; }
		}

		/// <summary>Returns full non-ambiguous revision name.</summary>
		public string FullName
		{
			get { return _pointer; }
		}

		/// <summary>Object is deleted and not valid anymore.</summary>
		public bool IsDeleted
		{
			get { return false; }
		}

		#endregion

		#region Methods

		/// <summary>Evaluate commit which is targeted by this <see cref="IRevisionPointer"/>.</summary>
		/// <returns>Commit which is pointed by this <see cref="IRevisionPointer"/>.</returns>
		public Revision Dereference()
		{
			var rev = _repository.Accessor.Dereference.Invoke(
				new DereferenceParameters(_pointer));
			lock(_repository.Revisions.SyncRoot)
			{
				return _repository.Revisions.GetOrCreateRevision(rev.SHA1);
			}
		}

		#endregion
	}
}

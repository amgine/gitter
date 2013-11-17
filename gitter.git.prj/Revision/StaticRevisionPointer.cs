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

	/// <summary>Static revision pointer.</summary>
	internal class StaticRevisionPointer : IRevisionPointer
	{
		#region Data

		private readonly string _pointer;
		private readonly Repository _repository;
		private Revision _revision;

		#endregion

		#region .ctor

		public StaticRevisionPointer(Repository repository, string pointer)
		{
			Verify.Argument.IsNotNull(repository, "repository");
			Verify.Argument.IsNeitherNullNorWhitespace(pointer, "pointer");

			_repository = repository;
			_pointer = pointer;
		}

		#endregion

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		public virtual ReferenceType Type
		{
			get { return ReferenceType.Revision; }
		}

		public virtual string Pointer
		{
			get { return _pointer; }
		}

		public virtual string FullName
		{
			get { return _pointer; }
		}

		public virtual bool IsDeleted
		{
			get { return false; }
		}

		#endregion

		#region Methods

		public virtual Revision Dereference()
		{
			if(_revision == null)
			{
				var rev = Repository.Accessor.Dereference.Invoke(
					new DereferenceParameters(Pointer));
				lock(Repository.Revisions.SyncRoot)
				{
					_revision = Repository.Revisions.GetOrCreateRevision(rev.SHA1);
				}
			}
			return _revision;
		}

		#endregion
	}
}

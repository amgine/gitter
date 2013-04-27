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

	sealed class RevisionFileBlameSource : BlameSourceBase
	{
		private readonly IRevisionPointer _revision;
		private readonly string _fileName;

		public RevisionFileBlameSource(IRevisionPointer revision, string fileName)
		{
			Verify.Argument.IsNotNull(revision, "revision");
			Verify.Argument.IsNeitherNullNorWhitespace(fileName, "fileName");

			_revision = revision;
			_fileName = fileName;
		}

		public string FileName
		{
			get { return _fileName; }
		}

		public override Repository Repository
		{
			get { return Revision.Repository; }
		}

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		protected override BlameFile GetBlameCore(BlameOptions options)
		{
			var accessor = _revision.Repository.Accessor;
			return accessor.QueryBlame(
				new QueryBlameParameters()
				{
					Revision = _revision.Pointer,
					FileName = _fileName,
				});
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return _revision.GetHashCode() ^ _fileName.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var ds = obj as RevisionFileBlameSource;
			if(ds == null) return false;
			return _revision == ds._revision && _fileName == ds._fileName;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if(_revision is Revision)
			{
				return _fileName + " @ " + _revision.Pointer.Substring(0, 7);
			}
			else
			{
				return _fileName + " @ " + _revision.Pointer;
			}
		}
	}
}

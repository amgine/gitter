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
			Verify.Argument.IsNotNull(revision, "revision");
			Verify.Argument.IsFalse(revision.IsDeleted, "revision");

			_revision = revision;
		}

		#endregion

		#region Properties

		public override IRevisionPointer Revision
		{
			get { return _revision; }
		}

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
		{
			return new Tree(Revision.Repository, Revision.FullName);
		}

		public override Task<Tree> GetTreeAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return Tree.GetAsync(Revision.Repository, Revision.FullName, progress, cancellationToken);
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
			var rts = obj as RevisionTreeSource;
			if(rts == null) return false;
			return (rts._revision == _revision);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return _revision.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return _revision.Pointer;
		}

		#endregion
	}
}

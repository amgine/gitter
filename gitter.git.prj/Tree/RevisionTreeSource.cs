namespace gitter.Git
{
	using System;

	public sealed class RevisionTreeSource : TreeSourceBase
	{
		private readonly IRevisionPointer _revision;

		public RevisionTreeSource(IRevisionPointer revision)
		{
			Verify.Argument.IsNotNull(revision, "revision");
			Verify.Argument.IsFalse(revision.IsDeleted, "revision");

			_revision = revision;
		}

		public override IRevisionPointer Revision
		{
			get { return _revision; }
		}

		protected override Tree GetTreeCore()
		{
			return new Tree(_revision.Repository, _revision.Pointer);
		}

		public override string DisplayName
		{
			get
			{
				if(_revision.Type == ReferenceType.Revision)
				{
					return _revision.Pointer.Substring(0, 7);
				}
				else
				{
					return _revision.Pointer;
				}
			}
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
	}
}

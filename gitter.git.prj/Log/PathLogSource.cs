namespace gitter.Git
{
	using System;

	public sealed class PathLogSource : LogSourceBase
	{
		private readonly IRevisionPointer _revision;
		private readonly string _path;

		public PathLogSource(IRevisionPointer revision, string path)
		{
			if(revision == null) throw new ArgumentNullException("repository");
			if(path == null) throw new ArgumentNullException("path");

			_revision = revision;
			_path = path;
		}

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		public string Path
		{
			get { return _path; }
		}

		protected override RevisionLog GetLogCore(LogOptions options)
		{
			var repository = _revision.Repository;
			if(repository.IsEmpty)
			{
				return new RevisionLog(repository, new Revision[0]);
			}
			else
			{
				var parameters = options.GetLogParameters();
				parameters.References = new[] { Revision.Pointer };
				parameters.Paths = new[] { _path };
				var log = repository.Accessor.QueryRevisions(parameters);
				var res = repository.Revisions.Resolve(log);
				return new RevisionLog(repository, res);
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
			var other = obj as PathLogSource;
			if(other == null) return false;
			return _revision == other._revision && _path == other._path;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return _revision.GetHashCode() ^ _path.GetHashCode();
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
				return _path + " @ " + _revision.Pointer.Substring(0, 7);
			}
			else
			{
				return _path + " @ " + _revision.Pointer;
			}
		}
	}
}

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	public sealed class PathLogSource : LogSourceBase
	{
		#region Data

		private readonly IRevisionPointer _revision;
		private readonly string _path;
		private bool _followRenames;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="PathLogSource"/> class.</summary>
		/// <param name="revision">Revision to start history log from.</param>
		/// <param name="path">Inspected path.</param>
		/// <param name="followRenames">if set to <c>true</c> follow file renames.</param>
		public PathLogSource(IRevisionPointer revision, string path, bool followRenames = true)
		{
			Verify.Argument.IsNotNull(revision, "revision");
			Verify.Argument.IsNotNull(path, "path");

			_revision = revision;
			_path = path;
			_followRenames = followRenames;
		}

		#endregion

		#region Properties

		public override Repository Repository
		{
			get { return _revision.Repository; }
		}

		public bool FollowRenames
		{
			get { return _followRenames; }
		}

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		public string Path
		{
			get { return _path; }
		}

		#endregion

		#region Overrides

		protected override RevisionLog GetLogCore(LogOptions options)
		{
			if(Repository.IsEmpty)
			{
				return new RevisionLog(Repository, new Revision[0]);
			}
			else
			{
				var parameters = options.GetLogParameters();
				parameters.References = new[] { Revision.Pointer };
				parameters.Paths = new[] { Path };
				parameters.Follow = FollowRenames;
				var log = Repository.Accessor.QueryRevisions(parameters);
				var revisions = Repository.Revisions.Resolve(log);
				return new RevisionLog(Repository, revisions);
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

		#endregion
	}
}

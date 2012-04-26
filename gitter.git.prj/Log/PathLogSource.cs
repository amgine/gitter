namespace gitter.Git
{
	using System;

	public sealed class PathLogSource : BaseLogSource
	{
		private readonly IRevisionPointer _revision;
		private readonly string _path;

		public PathLogSource(IRevisionPointer revision, string path)
		{
			if(revision == null) throw new ArgumentNullException("repository");

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

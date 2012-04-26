namespace gitter.Git
{
	using System;

	using gitter.Git.AccessLayer;

	sealed class RevisionFileBlameSource : BaseBlameSource
	{
		private readonly IRevisionPointer _revision;
		private readonly string _fileName;

		public RevisionFileBlameSource(IRevisionPointer revision, string fileName)
		{
			_revision = revision;
			_fileName = fileName;
		}

		public string FileName
		{
			get { return _fileName; }
		}

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		public override int GetHashCode()
		{
			return _revision.GetHashCode() ^ _fileName.GetHashCode();
		}

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
	}
}

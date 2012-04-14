namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class ReflogRecordData
	{
		#region Data

		private readonly int _index;
		private readonly string _message;
		private readonly RevisionData _revision;

		#endregion

		#region .ctor

		public ReflogRecordData(int index, string message, RevisionData revision)
		{
			_index = index;
			_message = message;
			_revision = revision;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return GitConstants.StashFullName + "@{" + _index + "}"; }
		}

		public string Message
		{
			get { return _message; }
		}

		public int Index
		{
			get { return _index; }
		}

		public RevisionData Revision
		{
			get { return _revision; }
		}

		#endregion

		public void Update(ReflogRecord obj)
		{
			obj.Index = _index;
			obj.Message = _message;
			Revision revision;
			lock(obj.Repository.Revisions.SyncRoot)
			{
				revision = obj.Repository.Revisions.GetOrCreateRevision(_revision.SHA1);
			}
			if(!revision.IsLoaded)
			{
				_revision.Update(revision);
			}
			obj.Revision = revision;
		}

		public ReflogRecord Construct(Repository repository, Reflog reflog)
		{
			Revision revision;
			lock(repository.Revisions.SyncRoot)
			{
				revision = repository.Revisions.GetOrCreateRevision(_revision.SHA1);
			}
			if(!revision.IsLoaded)
			{
				_revision.Update(revision);
			}
			return new ReflogRecord(repository, reflog, revision, _message, _index);
		}
	}
}

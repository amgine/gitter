namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	public sealed class StashedStateData : INamedObject, IObjectData<StashedState>
	{
		#region Data

		private readonly int _index;
		private readonly RevisionData _revision;

		#endregion

		#region .ctor

		public StashedStateData(int index, RevisionData revision)
		{
			_index = index;
			_revision = revision;
		}

		#endregion

		public string Name
		{
			get { return GitConstants.StashFullName + "@{" + _index + "}"; }
		}

		public int Index
		{
			get { return _index; }
		}

		public RevisionData Revision
		{
			get { return _revision; }
		}

		public void Update(StashedState obj)
		{
			obj.Index = _index;
		}

		public StashedState Construct(IRepository repository)
		{
			var repo = (Repository)repository;
			var revision = repo.Revisions.GetOrCreateRevision(_revision.SHA1);
			_revision.Update(revision);
			return new StashedState(repo, _index, revision);
		}
	}
}

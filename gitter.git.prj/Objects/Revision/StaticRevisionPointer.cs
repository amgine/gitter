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
			if(repository == null) throw new ArgumentNullException("repository");
			if(pointer == null) throw new ArgumentNullException("pointer");
			if(pointer == string.Empty) throw new ArgumentException("pointer");

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
				var rev = _repository.Accessor.Dereference(
					new DereferenceParameters(_pointer));
				lock(_repository.Revisions.SyncRoot)
				{
					_revision = _repository.Revisions.GetOrCreateRevision(rev.SHA1);
				}
			}
			return _revision;
		}

		#endregion
	}
}

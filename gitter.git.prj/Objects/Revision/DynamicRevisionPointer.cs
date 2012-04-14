namespace gitter.Git
{
	using System;

	using gitter.Git.AccessLayer;

	/// <summary>Dynamic revision pointer.</summary>
	internal sealed class DynamicRevisionPointer : IRevisionPointer
	{
		#region Data

		private readonly string _pointer;
		private readonly Repository _repository;

		#endregion

		#region .ctor

		public DynamicRevisionPointer(Repository repository, string pointer)
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

		public ReferenceType Type
		{
			get { return ReferenceType.Revision; }
		}

		public string Pointer
		{
			get { return _pointer; }
		}

		public string FullName
		{
			get { return _pointer; }
		}

		public bool IsDeleted
		{
			get { return false; }
		}

		#endregion

		#region Methods

		public Revision Dereference()
		{
			var rev = _repository.Accessor.Dereference(
				new DereferenceParameters(_pointer));
			lock(_repository.Revisions.SyncRoot)
			{
				return _repository.Revisions.GetOrCreateRevision(rev.SHA1);
			}
		}

		#endregion
	}
}

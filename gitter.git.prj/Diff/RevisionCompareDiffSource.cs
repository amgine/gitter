namespace gitter.Git
{
	using System;

	using gitter.Git.AccessLayer;

	sealed class RevisionCompareDiffSource : BaseDiffSource
	{
		#region Data

		private readonly IRevisionPointer _revision1;
		private readonly IRevisionPointer _revision2;

		#endregion

		#region .ctor

		public RevisionCompareDiffSource(IRevisionPointer revision1, IRevisionPointer revision2)
		{
			if(revision1 == null) throw new ArgumentNullException("revision1");
			if(revision2 == null) throw new ArgumentNullException("revision2");
			if(revision1.Repository != revision2.Repository)
				throw new ArgumentException("revision2");

			_revision1 = revision1;
			_revision2 = revision2;
		}

		#endregion

		#region Properties

		public IRevisionPointer Revision1
		{
			get { return _revision1; }
		}

		public IRevisionPointer Revision2
		{
			get { return _revision2; }
		}

		#endregion

		#region Overrides

		public override int GetHashCode()
		{
			return _revision1.GetHashCode() ^ _revision2.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var ds = obj as RevisionCompareDiffSource;
			if(ds == null) return false;
			return _revision1 == ds._revision1 && _revision2 == ds._revision2;
		}

		protected override Diff GetDiffCore(DiffOptions options)
		{
			var parameters = new QueryDiffParameters()
			{
				Revision1 = _revision1.Pointer,
				Revision2 = _revision2.Pointer,
			};
			ApplyCommonDiffOptions(parameters, options);
			return _revision1.Repository.Accessor.QueryDiff(parameters);
		}

		public override string ToString()
		{
			var r1 = (_revision1 is Revision) ? _revision1.Pointer.Substring(0, 7) : _revision1.Pointer;
			var r2 = (_revision2 is Revision) ? _revision2.Pointer.Substring(0, 7) : _revision2.Pointer;
			return "diff " + r1 + ".." + r2;
		}

		#endregion
	}
}

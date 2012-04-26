namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	sealed class RevisionCompareDiffSource : BaseDiffSource
	{
		#region Data

		private readonly IRevisionPointer _revision1;
		private readonly IRevisionPointer _revision2;
		private readonly IList<string> _paths;

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

		public RevisionCompareDiffSource(IRevisionPointer revision1, IRevisionPointer revision2, IList<string> paths)
			: this(revision1, revision2)
		{
			_paths = paths;
		}

		#endregion

		#region Properties

		public override Repository Repository
		{
			get { return _revision1.Repository; }
		}

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
			var res =  _revision1.GetHashCode() ^ _revision2.GetHashCode();
			if(_paths != null)
			{
				foreach(var path in _paths)
				{
					res ^= path.GetHashCode();
				}
			}
			return res;
		}

		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var ds = obj as RevisionCompareDiffSource;
			if(ds == null) return false;
			if(_revision1 != ds._revision1 || _revision2 != ds._revision2) return false;
			var count1 = _paths == null ? 0 : _paths.Count;
			var count2 = ds._paths == null ? 0 : ds._paths.Count;
			if(count1 != count2) return false;
			for(int i = 0; i < count1; ++i)
			{
				if(_paths[i] != ds._paths[i]) return false;
			}
			return true;
		}

		protected override Diff GetDiffCore(DiffOptions options)
		{
			var parameters = new QueryDiffParameters()
			{
				Revision1 = _revision1.Pointer,
				Revision2 = _revision2.Pointer,
				Paths = _paths,
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

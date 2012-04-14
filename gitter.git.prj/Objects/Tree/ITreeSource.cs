namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	public interface ITreeSource
	{
		string DisplayName { get; }

		Tree GetTree();

		IAsyncFunc<Tree> GetTreeAsync();
	}

	public abstract class TreeSource : ITreeSource
	{
		protected abstract Tree GetTreeCore();

		public abstract string DisplayName { get; }

		public Tree GetTree()
		{
			return GetTreeCore();
		}

		public IAsyncFunc<Tree> GetTreeAsync()
		{
			return AsyncFunc.Create(
				this,
				(data, monitor) =>
				{
					return data.GetTreeCore();
				},
				string.Empty,
				Resources.StrsFetchingTree.AddEllipsis());
		}
	}

	public class RevisionTreeSource : TreeSource
	{
		private readonly IRevisionPointer _revision;

		public RevisionTreeSource(IRevisionPointer revision)
		{
			if(revision == null) throw new ArgumentNullException("revision");
			if(revision.IsDeleted) throw new ArgumentException("revision");
			_revision = revision;
		}

		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var rts = obj as RevisionTreeSource;
			if(rts == null) return false;
			return (rts._revision == _revision) ;
		}

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		public override int GetHashCode()
		{
			return _revision.GetHashCode();
		}

		protected override Tree GetTreeCore()
		{
			return new Tree(_revision.Repository, _revision.Pointer);
		}

		public override string DisplayName
		{
			get
			{
				if(_revision.Type == ReferenceType.Revision)
				{
					return _revision.Pointer.Substring(0, 7);
				}
				else
				{
					return _revision.Pointer;
				}
			}
		}

		public override string ToString()
		{
			return _revision.Pointer;
		}
	}
}

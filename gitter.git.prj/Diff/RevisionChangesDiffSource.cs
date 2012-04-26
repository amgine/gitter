namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;
	using gitter.Git.AccessLayer;

	sealed class RevisionChangesDiffSource : BaseDiffSource
	{
		private readonly IRevisionPointer _revision;
		private readonly IList<string> _paths;

		public RevisionChangesDiffSource(IRevisionPointer revision)
		{
			if(revision == null) throw new ArgumentNullException("revision");

			_revision = revision;
		}

		public RevisionChangesDiffSource(IRevisionPointer revision, IList<string> paths)
		{
			if(revision == null) throw new ArgumentNullException("revision");

			_revision = revision;
			_paths = paths;
		}

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		public override Repository Repository
		{
			get { return _revision.Repository; }
		}

		public override int GetHashCode()
		{
			return _revision.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var ds = obj as RevisionChangesDiffSource;
			if(ds == null) return false;
			return _revision == ds._revision;
		}

		public override IEnumerable<FlowPanel> GetInformationPanels()
		{
			yield return new RevisionHeaderPanel() { Revision = _revision.Dereference() };
			yield return new FlowPanelSeparator() { Style = FlowPanelSeparatorStyle.Line };
		}

		protected override Diff GetDiffCore(DiffOptions options)
		{
			var parameters = new QueryRevisionDiffParameters(_revision.Pointer)
			{
				Paths = _paths,
			};
			ApplyCommonDiffOptions(parameters, options);
			return _revision.Repository.Accessor.QueryRevisionDiff(parameters);
		}

		public override string ToString()
		{
			if(_revision is Revision)
			{
				return "log -p " + _revision.Pointer.Substring(0, 7);
			}
			else
			{
				return "log -p " + _revision.Pointer;
			}
		}
	}
}

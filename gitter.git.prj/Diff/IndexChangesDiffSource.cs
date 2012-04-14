namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.AccessLayer;
	using gitter.Git.Gui.Controls;

	sealed class IndexChangesDiffSource : BaseDiffSource
	{
		#region Data

		private readonly Repository _repository;
		private readonly bool _cached;
		private readonly IList<string> _paths;

		#endregion

		#region .ctor

		public IndexChangesDiffSource(Repository repository, bool cached, IList<string> paths)
		{
			if(repository == null) throw new ArgumentNullException("repository");

			_repository = repository;
			_cached = cached;
			_paths = paths;
			_repository.Status.Changed += OnStatusChanged;
		}

		public IndexChangesDiffSource(Repository repository, bool cached)
			: this(repository, cached, null)
		{
		}

		#endregion

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		public bool Cached
		{
			get { return _cached; }
		}

		public IList<string> Paths
		{
			get { return _paths; }
		}

		#endregion

		#region Private

		private void OnStatusChanged(object sender, EventArgs e)
		{
			OnUpdated();
		}

		#endregion

		#region Overrides

		public override IEnumerable<FlowPanel> GetInformationPanels()
		{
			if(!_cached)
			{
				var p = new UntrackedFilesPanel(_repository.Status, _paths);
				if(p.Count != 0)
				{
					return new FlowPanel[]
					{
						p,
						new FlowPanelSeparator() { Height = 5 },
					};
				}
			}
			return null;
		}

		public override int GetHashCode()
		{
			return _repository.GetHashCode() ^ _paths.GetHashCode() ^ (_cached?1:0);
		}

		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var ds = obj as IndexChangesDiffSource;
			if(ds == null) return false;
			if(ds._repository != _repository) return false;
			if(ds._cached != _cached) return false;
			if(ds._paths == _paths) return true;
			if(ds._paths == null && _paths.Count == 0) return true;
			if(_paths == null && ds._paths.Count == 0) return true;
			if(ds._paths.Count != _paths.Count) return false;
			foreach(var path in _paths)
			{
				if(!ds._paths.Contains(path)) return false;
			}
			return true;
		}

		protected override Diff GetDiffCore(DiffOptions options)
		{
			var parameters = new QueryDiffParameters()
			{
				Cached = _cached,
				Paths = _paths,
			};
			ApplyCommonDiffOptions(parameters, options);
			return _repository.Accessor.QueryDiff(parameters);
		}

		public override void Dispose()
		{
			_repository.Status.Changed -= OnStatusChanged;
		}

		public override string ToString()
		{
			if(_cached)
				return "diff --cached";
			else
				return "diff";
		}

		#endregion
	}
}

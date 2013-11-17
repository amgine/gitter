#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.AccessLayer;

	sealed class IndexChangesDiffSource : DiffSourceBase, IIndexDiffSource
	{
		#region Data

		private readonly Repository _repository;
		private readonly bool _cached;
		private readonly IList<string> _paths;

		#endregion

		#region .ctor

		public IndexChangesDiffSource(Repository repository, bool cached, IList<string> paths)
		{
			Verify.Argument.IsNotNull(repository, "repository");

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

		public override Repository Repository
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

		/*
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
		*/

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

		private QueryDiffParameters GetParameters(DiffOptions options)
		{
			Assert.IsNotNull(options);

			var parameters = new QueryDiffParameters()
			{
				Cached = _cached,
				Paths = _paths,
			};
			ApplyCommonDiffOptions(parameters, options);
			return parameters;
		}

		protected override Diff GetDiffCore(DiffOptions options)
		{
			Assert.IsNotNull(options);

			var parameters = GetParameters(options);
			return Repository.Accessor.QueryDiff.Invoke(parameters);
		}

		protected override Task<Diff> GetDiffCoreAsync(DiffOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Assert.IsNotNull(options);

			var parameters = GetParameters(options);
			return Repository.Accessor.QueryDiff.InvokeAsync(parameters, progress, cancellationToken);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_repository.Status.Changed -= OnStatusChanged;
			}
		}

		public override string ToString()
		{
			if(_cached)
			{
				return "diff --cached";
			}
			else
			{
				return "diff";
			}
		}

		#endregion
	}
}

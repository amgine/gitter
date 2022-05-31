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

namespace gitter.Git;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

using gitter.Git.AccessLayer;

sealed class IndexChangesDiffSource : DiffSourceBase, IIndexDiffSource
{
	#region .ctor

	public IndexChangesDiffSource(Repository repository, bool cached, IList<string> paths)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;
		Cached = cached;
		Paths = paths;
		repository.Status.Changed += OnStatusChanged;
	}

	public IndexChangesDiffSource(Repository repository, bool cached)
		: this(repository, cached, null)
	{
	}

	#endregion

	#region Properties

	public override Repository Repository { get; }

	public bool Cached { get; }

	public IList<string> Paths { get; }

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

	/// <inheritdoc/>
	public override int GetHashCode()
		=> Repository.GetHashCode() ^ Paths.GetHashCode() ^ (Cached?1:0);

	/// <inheritdoc/>
	public override bool Equals(object obj)
	{
		if(obj is not IndexChangesDiffSource ds) return false;
		if(ds.Repository != Repository) return false;
		if(ds.Cached != Cached) return false;
		if(ds.Paths == Paths) return true;
		if(ds.Paths == null || ds.Paths.Count == 0)
		{
			return Paths == null || Paths.Count == 0;
		}
		if(Paths == null || Paths.Count == 0)
		{
			return ds.Paths == null || ds.Paths.Count == 0;
		}
		if(ds.Paths.Count != Paths.Count)
		{
			return false;
		}
		foreach(var path in Paths)
		{
			if(!ds.Paths.Contains(path)) return false;
		}
		return true;
	}

	private QueryDiffParameters GetParameters(DiffOptions options)
	{
		Assert.IsNotNull(options);

		var parameters = new QueryDiffParameters
		{
			Cached = Cached,
			Paths  = Paths,
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
			Repository.Status.Changed -= OnStatusChanged;
		}
	}

	/// <inheritdoc/>
	public override string ToString()
		=> Cached
			? "diff --cached"
			: "diff";

	#endregion
}

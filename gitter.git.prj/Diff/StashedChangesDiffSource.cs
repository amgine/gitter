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
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

using gitter.Git.AccessLayer;

sealed class StashedChangesDiffSource : DiffSourceBase, IRevisionDiffSource
{
	private readonly Many<string> _paths;

	public StashedChangesDiffSource(StashedState stashedState)
	{
		Verify.Argument.IsNotNull(stashedState);

		StashedState = stashedState;
	}

	public StashedChangesDiffSource(StashedState stashedState, Many<string> paths)
	{
		Verify.Argument.IsNotNull(stashedState);

		StashedState = stashedState;
		_paths = paths;
	}

	public StashedState StashedState { get; }

	public override Repository Repository => StashedState.Repository;

	IRevisionPointer IRevisionDiffSource.Revision => StashedState;

	private QueryRevisionDiffRequest GetRequest(DiffOptions options)
	{
		Assert.IsNotNull(options);

		var request = new QueryRevisionDiffRequest(StashedState.Name)
		{
			Paths = _paths,
		};
		ApplyCommonDiffOptions(request, options);
		return request;
	}

	protected override Diff GetDiffCore(DiffOptions options)
	{
		Assert.IsNotNull(options);

		var request = GetRequest(options);
		return Repository.Accessor.QueryStashDiff.Invoke(request);
	}

	protected override Task<Diff> GetDiffCoreAsync(DiffOptions options, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
	{
		Assert.IsNotNull(options);

		var request = GetRequest(options);
		return Repository.Accessor.QueryStashDiff.InvokeAsync(request, progress, cancellationToken);
	}

	/// <inheritdoc/>
	public override int GetHashCode()
		=> StashedState.GetHashCode();

	/// <inheritdoc/>
	public override bool Equals(object? obj)
		=> obj is StashedChangesDiffSource ds
		&& StashedState == ds.StashedState;

	/// <inheritdoc/>
	public override string ToString()
		=> "stash show -p " + StashedState.Name;
}

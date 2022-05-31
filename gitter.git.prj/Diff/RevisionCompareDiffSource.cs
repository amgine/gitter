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

sealed class RevisionCompareDiffSource : DiffSourceBase
{
	#region Data

	private readonly IList<string> _paths;

	#endregion

	#region .ctor

	public RevisionCompareDiffSource(IRevisionPointer revision1, IRevisionPointer revision2)
	{
		Verify.Argument.IsNotNull(revision1);
		Verify.Argument.IsNotNull(revision2);
		Verify.Argument.IsTrue(revision1.Repository == revision2.Repository,
			"revision2", "Cannot compare objects from different repositories.");

		Revision1 = revision1;
		Revision2 = revision2;
	}

	public RevisionCompareDiffSource(IRevisionPointer revision1, IRevisionPointer revision2, IList<string> paths)
		: this(revision1, revision2)
	{
		_paths = paths;
	}

	#endregion

	#region Properties

	public override Repository Repository => Revision1.Repository;

	public IRevisionPointer Revision1 { get; }

	public IRevisionPointer Revision2 { get; }

	#endregion

	#region Overrides

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var res =  Revision1.GetHashCode() ^ Revision2.GetHashCode();
		if(_paths != null)
		{
			foreach(var path in _paths)
			{
				res ^= path.GetHashCode();
			}
		}
		return res;
	}

	/// <inheritdoc/>
	public override bool Equals(object obj)
	{
		if(obj is not RevisionCompareDiffSource ds) return false;
		if(Revision1 != ds.Revision1 || Revision2 != ds.Revision2) return false;
		var count1 = _paths == null ? 0 : _paths.Count;
		var count2 = ds._paths == null ? 0 : ds._paths.Count;
		if(count1 != count2) return false;
		for(int i = 0; i < count1; ++i)
		{
			if(_paths[i] != ds._paths[i]) return false;
		}
		return true;
	}

	private QueryDiffParameters GetParameters(DiffOptions options)
	{
		Assert.IsNotNull(options);

		var parameters = new QueryDiffParameters()
		{
			Revision1 = Revision1.Pointer,
			Revision2 = Revision2.Pointer,
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

	/// <inheritdoc/>
	public override string ToString()
	{
		var r1 = (Revision1 is Revision) ? Revision1.Pointer.Substring(0, 7) : Revision1.Pointer;
		var r2 = (Revision2 is Revision) ? Revision2.Pointer.Substring(0, 7) : Revision2.Pointer;
		return "diff " + r1 + ".." + r2;
	}

	#endregion
}

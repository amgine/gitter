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

using Resources = gitter.Git.Properties.Resources;

public sealed class PathLogSource : LogSourceBase
{
	/// <summary>Initializes a new instance of the <see cref="PathLogSource"/> class.</summary>
	/// <param name="revision">Revision to start history log from.</param>
	/// <param name="path">Inspected path.</param>
	/// <param name="followRenames">if set to <c>true</c> follow file renames.</param>
	public PathLogSource(IRevisionPointer revision, string path, bool followRenames = true)
		: base(revision.Repository)
	{
		Verify.Argument.IsNotNull(revision);
		Verify.Argument.IsNotNull(path);

		Revision      = revision;
		Path          = path;
		FollowRenames = followRenames;
	}

	public bool FollowRenames { get; }

	public IRevisionPointer Revision { get; }

	public string Path { get; }

	/// <inheritdoc/>
	public override async Task<RevisionLog> GetRevisionLogAsync(LogOptions options,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		if(Repository.IsEmpty)
		{
			return RevisionLog.CreateEmpty(Repository);
		}

		var request = options.GetLogRequest();
		request.References = Revision.Pointer;
		request.Paths      = Path;
		request.Follow     = FollowRenames;

		progress?.Report(new(Resources.StrsFetchingLog.AddEllipsis()));
		var revisionData = await Repository.Accessor.QueryRevisions
			.InvokeAsync(request, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		progress?.Report(OperationProgress.Completed);
		return CreateLog(revisionData);
	}

	/// <inheritdoc/>
	public override bool Equals(object? obj)
		=> obj is PathLogSource other
		&& Revision      == other.Revision
		&& Path          == other.Path
		&& FollowRenames == other.FollowRenames;

	/// <inheritdoc/>
	public override int GetHashCode()
		=> Revision.GetHashCode() ^ Path.GetHashCode() ^ (FollowRenames ? 1 : 0);

	/// <inheritdoc/>
	public override string ToString()
		=> Revision is Revision
			? Path + " @ " + Revision.Pointer.Substring(0, 7)
			: Path + " @ " + Revision.Pointer;
}

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

using Resources = gitter.Git.Properties.Resources;

sealed class RevisionFileBlameSource : BlameSourceBase
{
	public RevisionFileBlameSource(IRevisionPointer revision, string fileName)
	{
		Verify.Argument.IsNotNull(revision);
		Verify.Argument.IsNeitherNullNorWhitespace(fileName);

		Revision = revision;
		FileName = fileName;
	}

	public override Repository Repository => Revision.Repository;

	public string FileName { get; }

	public IRevisionPointer Revision { get; }

	private QueryBlameRequest GetRequest(BlameOptions options)
		=> new()
		{
			Revision = Revision.Pointer,
			FileName = FileName,
		};

	public override BlameFile GetBlame(BlameOptions options)
	{
		Verify.Argument.IsNotNull(options);

		var request = GetRequest(options);
		return Repository.Accessor.QueryBlame.Invoke(request);
	}

	public override async Task<BlameFile> GetBlameAsync(BlameOptions options,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(options);

		progress?.Report(new OperationProgress(Resources.StrsFetchingBlame.AddEllipsis()));
		var request = GetRequest(options);
		var result = await Repository
			.Accessor
			.QueryBlame
			.InvokeAsync(request, progress, cancellationToken);
		progress?.Report(OperationProgress.Completed);
		return result;
	}

	/// <inheritdoc/>
	public override int GetHashCode() => Revision.GetHashCode() ^ FileName.GetHashCode();

	/// <inheritdoc/>
	public override bool Equals(object? obj)
	{
		if(obj is not RevisionFileBlameSource ds) return false;
		return Revision == ds.Revision && FileName == ds.FileName;
	}

	/// <inheritdoc/>
	public override string ToString()
		=> Revision is Revision
			? FileName + " @ " + Revision.Pointer.Substring(0, 7)
			: FileName + " @ " + Revision.Pointer;
}

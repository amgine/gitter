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

public sealed class RepositoryLogSource(Repository repository) : LogSourceBase(repository)
{
	/// <inheritdoc/>
	public override async Task<RevisionLog> GetRevisionLogAsync(LogOptions options,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(options);

		if(Repository.IsEmpty)
		{
			return RevisionLog.CreateEmpty(Repository);
		}

		progress?.Report(new(Resources.StrsFetchingLog.AddEllipsis()));
		var request  = options.GetLogRequest();
		var response = await Repository.Accessor.QueryRevisions
			.InvokeAsync(request, progress, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		return CreateLog(response);
	}
}

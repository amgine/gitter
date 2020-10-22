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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class RepositoryLogSource : LogSourceBase
	{
		public RepositoryLogSource(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			Repository = repository;
		}

		public override Repository Repository { get; }

		public override async Task<RevisionLog> GetRevisionLogAsync(LogOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(options, nameof(options));

			if(Repository.IsEmpty)
			{
				return await TaskUtility
					.TaskFromResult(new RevisionLog(Repository, Preallocated<Revision>.EmptyArray))
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				progress?.Report(new OperationProgress(Resources.StrsFetchingLog.AddEllipsis()));
				var parameters = options.GetLogParameters();
				var revisionData = await Repository
					.Accessor
					.QueryRevisions
					.InvokeAsync(parameters, progress, cancellationToken)
					.ConfigureAwait(continueOnCapturedContext: false);
				var revisions = Repository.Revisions.Resolve(revisionData);
				return new RevisionLog(Repository, revisions);
			}
		}
	}
}

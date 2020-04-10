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
					.TaskFromResult(new RevisionLog(Repository, new Revision[0]))
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				progress?.Report(new OperationProgress(Resources.StrsFetchingLog.AddEllipsis()));
				var parameters = options.GetLogParameters();
				var revisionData = await Repository
					.Accessor
					.QueryRevisions.InvokeAsync(parameters, progress, cancellationToken);
				var revisions = Repository.Revisions.Resolve(revisionData);
				return new RevisionLog(Repository, revisions);
				//return Repository
				//	.Accessor
				//	.QueryRevisions.InvokeAsync(parameters, progress, cancellationToken)
				//	.ContinueWith(
				//		t =>
				//		{
				//			progress?.Report(OperationProgress.Completed);
				//			var revisionData = TaskUtility.UnwrapResult(t);
				//			var revisions    = Repository.Revisions.Resolve(revisionData);
				//			var revisionLog  = new RevisionLog(Repository, revisions);

				//			return revisionLog;
				//		},
				//		cancellationToken,
				//		TaskContinuationOptions.ExecuteSynchronously,
				//		TaskScheduler.Default);
			}
		}
	}
}

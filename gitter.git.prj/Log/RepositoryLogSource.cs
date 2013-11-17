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
		#region Data

		private readonly Repository _repository;

		#endregion

		#region .ctor

		public RepositoryLogSource(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		#endregion

		#region Properties

		public override Repository Repository
		{
			get { return _repository; }
		}

		#endregion

		#region Methods

		public override Task<RevisionLog> GetRevisionLogAsync(LogOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(options, "options");

			if(Repository.IsEmpty)
			{
				return TaskUtility.TaskFromResult(new RevisionLog(Repository, new Revision[0]));
			}
			else
			{
				if(progress != null)
				{
					progress.Report(new OperationProgress(Resources.StrsFetchingLog.AddEllipsis()));
				}
				var parameters = options.GetLogParameters();
				return Repository.Accessor
								 .QueryRevisions.InvokeAsync(parameters, progress, cancellationToken)
								 .ContinueWith(
									t =>
									{
										if(progress != null)
										{
											progress.Report(OperationProgress.Completed);
										}
										var revisionData = TaskUtility.UnwrapResult(t);
										var revisions    = Repository.Revisions.Resolve(revisionData);
										var revisionLog  = new RevisionLog(Repository, revisions);

										return revisionLog;
									},
									cancellationToken,
									TaskContinuationOptions.ExecuteSynchronously,
									TaskScheduler.Default);
			}
		}

		#endregion
	}
}

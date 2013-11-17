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
	using System.Collections.Generic;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	public sealed class PathLogSource : LogSourceBase
	{
		#region Data

		private readonly IRevisionPointer _revision;
		private readonly string _path;
		private bool _followRenames;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="PathLogSource"/> class.</summary>
		/// <param name="revision">Revision to start history log from.</param>
		/// <param name="path">Inspected path.</param>
		/// <param name="followRenames">if set to <c>true</c> follow file renames.</param>
		public PathLogSource(IRevisionPointer revision, string path, bool followRenames = true)
		{
			Verify.Argument.IsNotNull(revision, "revision");
			Verify.Argument.IsNotNull(path, "path");

			_revision = revision;
			_path = path;
			_followRenames = followRenames;
		}

		#endregion

		#region Properties

		public override Repository Repository
		{
			get { return _revision.Repository; }
		}

		public bool FollowRenames
		{
			get { return _followRenames; }
		}

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		public string Path
		{
			get { return _path; }
		}

		#endregion

		#region Overrides

		public override Task<RevisionLog> GetRevisionLogAsync(LogOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			if(Repository.IsEmpty)
			{
				var tcs = new TaskCompletionSource<RevisionLog>();
				if(cancellationToken.IsCancellationRequested)
				{
					tcs.SetCanceled();
				}
				else
				{
					tcs.SetResult(new RevisionLog(Repository, new Revision[0]));
				}
				return tcs.Task;
			}
			else
			{
				var parameters = options.GetLogParameters();
				parameters.References = new[] { Revision.Pointer };
				parameters.Paths = new[] { Path };
				parameters.Follow = FollowRenames;

				if(progress != null)
				{
					progress.Report(new OperationProgress(Resources.StrsFetchingLog.AddEllipsis()));
				}
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

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var other = obj as PathLogSource;
			if(other == null) return false;
			return _revision == other._revision && _path == other._path;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return _revision.GetHashCode() ^ _path.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if(_revision is Revision)
			{
				return Path + " @ " + _revision.Pointer.Substring(0, 7);
			}
			else
			{
				return Path + " @ " + _revision.Pointer;
			}
		}

		#endregion
	}
}

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

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	sealed class RevisionFileBlameSource : BlameSourceBase
	{
		#region .ctor

		public RevisionFileBlameSource(IRevisionPointer revision, string fileName)
		{
			Verify.Argument.IsNotNull(revision, nameof(revision));
			Verify.Argument.IsNeitherNullNorWhitespace(fileName, nameof(fileName));

			Revision = revision;
			FileName = fileName;
		}

		#endregion

		#region Properties

		public override Repository Repository => Revision.Repository;

		public string FileName { get; }

		public IRevisionPointer Revision { get; }

		#endregion

		#region Methods

		private QueryBlameParameters GetParameters(BlameOptions options)
		{
			Assert.IsNotNull(options);

			var parameters = new QueryBlameParameters()
				{
					Revision = Revision.Pointer,
					FileName = FileName,
				};
			return parameters;
		}

		public override BlameFile GetBlame(BlameOptions options)
		{
			Verify.Argument.IsNotNull(options, nameof(options));

			var parameters = GetParameters(options);
			return Repository.Accessor.QueryBlame.Invoke(parameters);
		}

		public override async Task<BlameFile> GetBlameAsync(BlameOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(options, nameof(options));

			progress?.Report(new OperationProgress(Resources.StrsFetchingBlame.AddEllipsis()));
			var parameters = GetParameters(options);
			var result = await Repository
				.Accessor
				.QueryBlame
				.InvokeAsync(parameters, progress, cancellationToken);
			progress?.Report(OperationProgress.Completed);
			return result;

			//return Repository
			//	.Accessor
			//	.QueryBlame
			//	.InvokeAsync(parameters, progress, cancellationToken)
			//	.ContinueWith(
			//		t =>
			//		{
			//			progress?.Report(OperationProgress.Completed);
			//			return TaskUtility.UnwrapResult(t);
			//		},
			//		cancellationToken,
			//		TaskContinuationOptions.ExecuteSynchronously,
			//		TaskScheduler.Default);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() => Revision.GetHashCode() ^ FileName.GetHashCode();

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
			var ds = obj as RevisionFileBlameSource;
			if(ds == null) return false;
			return Revision == ds.Revision && FileName == ds.FileName;
		}

		/// <summary>Returns a <see cref="System.String"/> that represents this instance.</summary>
		/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
		public override string ToString()
		{
			return Revision is Revision
				? FileName + " @ " + Revision.Pointer.Substring(0, 7)
				: FileName + " @ " + Revision.Pointer;
		}

		#endregion
	}
}

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
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	sealed class StashedChangesDiffSource : DiffSourceBase, IRevisionDiffSource
	{
		private readonly IList<string> _paths;

		public StashedChangesDiffSource(StashedState stashedState)
		{
			Verify.Argument.IsNotNull(stashedState, nameof(stashedState));

			StashedState = stashedState;
		}

		public StashedChangesDiffSource(StashedState stashedState, IList<string> paths)
		{
			Verify.Argument.IsNotNull(stashedState, nameof(stashedState));

			StashedState = stashedState;
			_paths = paths;
		}

		public StashedState StashedState { get; }

		#region Overrides

		public override Repository Repository => StashedState.Repository;

		IRevisionPointer IRevisionDiffSource.Revision => StashedState;

		private QueryRevisionDiffParameters GetParameters(DiffOptions options)
		{
			Assert.IsNotNull(options);

			var parameters = new QueryRevisionDiffParameters(StashedState.Name)
			{
				Paths = _paths,
			};
			ApplyCommonDiffOptions(parameters, options);
			return parameters;
		}

		protected override Diff GetDiffCore(DiffOptions options)
		{
			Assert.IsNotNull(options);

			var parameters = GetParameters(options);
			return Repository.Accessor.QueryStashDiff.Invoke(parameters);
		}

		protected override Task<Diff> GetDiffCoreAsync(DiffOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Assert.IsNotNull(options);

			var parameters = GetParameters(options);
			return Repository.Accessor.QueryStashDiff.InvokeAsync(parameters, progress, cancellationToken);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
			=> StashedState.GetHashCode();

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if(obj is not StashedChangesDiffSource ds) return false;
			return StashedState == ds.StashedState;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
			=> "stash show -p " + StashedState.Name;

		#endregion
	}
}

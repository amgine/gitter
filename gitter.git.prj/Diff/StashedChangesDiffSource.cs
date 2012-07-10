namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	sealed class StashedChangesDiffSource : DiffSourceBase, IRevisionDiffSource
	{
		private readonly StashedState _stashedState;
		private readonly IList<string> _paths;

		public StashedChangesDiffSource(StashedState stashedState)
		{
			if(stashedState == null) throw new ArgumentNullException("stashedState");

			_stashedState = stashedState;
		}

		public StashedChangesDiffSource(StashedState stashedState, IList<string> paths)
		{
			if(stashedState == null) throw new ArgumentNullException("stashedState");

			_stashedState = stashedState;
			_paths = paths;
		}

		public StashedState StashedState
		{
			get { return _stashedState; }
		}

		#region Overrides

		public override Repository Repository
		{
			get { return _stashedState.Repository; }
		}

		IRevisionPointer IRevisionDiffSource.Revision
		{
			get { return _stashedState; }
		}

		protected override Diff GetDiffCore(DiffOptions options)
		{
			var parameters = new QueryRevisionDiffParameters(_stashedState.Name)
			{
				Paths = _paths,
			};
			ApplyCommonDiffOptions(parameters, options);
			return _stashedState.Repository.Accessor.QueryStashDiff(parameters);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return _stashedState.GetHashCode();
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
			var ds = obj as StashedChangesDiffSource;
			if(ds == null) return false;
			return _stashedState == ds._stashedState;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return "stash show -p " + _stashedState.Name;
		}

		#endregion
	}
}

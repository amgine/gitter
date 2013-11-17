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

	sealed class RevisionChangesDiffSource : DiffSourceBase, IRevisionDiffSource
	{
		#region Data

		private readonly IRevisionPointer _revision;
		private readonly IList<string> _paths;

		#endregion

		#region .ctor

		public RevisionChangesDiffSource(IRevisionPointer revision)
		{
			Verify.Argument.IsNotNull(revision, "revision");

			_revision = revision;
		}

		public RevisionChangesDiffSource(IRevisionPointer revision, IList<string> paths)
		{
			Verify.Argument.IsNotNull(revision, "revision");

			_revision = revision;
			_paths = paths;
		}

		#endregion

		#region Properties

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		public override Repository Repository
		{
			get { return _revision.Repository; }
		}

		#endregion

		#region Methods

		public override int GetHashCode()
		{
			return Revision.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(obj == null) return false;
			var ds = obj as RevisionChangesDiffSource;
			if(ds == null) return false;
			return _revision == ds._revision;
		}

		private QueryRevisionDiffParameters GetParameters(DiffOptions options)
		{
			var parameters = new QueryRevisionDiffParameters(_revision.Pointer)
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
			return Repository.Accessor.QueryRevisionDiff.Invoke(parameters);
		}

		protected override Task<Diff> GetDiffCoreAsync(DiffOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Assert.IsNotNull(options);

			var parameters = GetParameters(options);
			return Repository.Accessor.QueryRevisionDiff.InvokeAsync(parameters, progress, cancellationToken);
		}

		public override string ToString()
		{
			if(Revision is Revision)
			{
				return "log -p " + Revision.Pointer.Substring(0, 7);
			}
			else
			{
				return "log -p " + Revision.Pointer;
			}
		}

		#endregion
	}
}

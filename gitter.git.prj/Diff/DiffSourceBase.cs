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
	using gitter.Framework.Controls;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	abstract class DiffSourceBase : IDiffSource
	{
		#region Events

		public event EventHandler Updated;

		protected virtual void OnUpdated()
		{
			var handler = Updated;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Data

		private bool _isDisposed;

		#endregion

		#region .ctor & finalizer

		/// <summary>Initializes a new instance of the <see cref="DiffSourceBase"/> class.</summary>
		protected DiffSourceBase()
		{
		}

		/// <summary>Finalizes an instance of the <see cref="DiffSourceBase"/> class.</summary>
		~DiffSourceBase()
		{
			Dispose(false);
		}

		#endregion

		#region Properties

		public abstract Repository Repository
		{
			get;
		}

		#endregion

		public virtual IEnumerable<FlowPanel> GetInformationPanels()
		{
			return null;
		}

		protected static void ApplyCommonDiffOptions(BaseQueryDiffParameters queryParameters, DiffOptions options)
		{
			Assert.IsNotNull(queryParameters);
			Assert.IsNotNull(options);

			queryParameters.Context           = options.Context;
			queryParameters.Patience          = options.UsePatienceAlgorithm;
			queryParameters.IgnoreSpaceChange = options.IgnoreWhitespace;
			queryParameters.Binary            = options.Binary;
		}

		protected abstract Diff GetDiffCore(DiffOptions options);

		protected abstract Task<Diff> GetDiffCoreAsync(DiffOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		public Diff GetDiff(DiffOptions options)
		{
			Verify.Argument.IsNotNull(options, "options");
			Verify.State.IsFalse(IsDisposed, "Object is disposed.");

			return GetDiffCore(options);
		}

		public Task<Diff> GetDiffAsync(DiffOptions options, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(options, "options");
			Verify.State.IsFalse(IsDisposed, "Object is disposed.");

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrLoadingDiff.AddEllipsis()));
				return GetDiffCoreAsync(options, progress, cancellationToken)
					.ContinueWith(t =>
						{
							progress.Report(OperationProgress.Completed);
							return TaskUtility.UnwrapResult(t);
						},
						cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			}
			else
			{
				return GetDiffCoreAsync(options, progress, cancellationToken);
			}
		}

		/// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			return "diff";
		}

		#region IDisposable

		/// <summary>Gets a value indicating whether this instance is disposed.</summary>
		/// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		/// <summary>Releases unmanaged and - optionally - managed resources.</summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources;
		/// <c>false</c> to release only unmanaged resources.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>Releases unmanaged and - optionally - managed resources.</summary>
		public void Dispose()
		{
			if(!IsDisposed)
			{
				GC.SuppressFinalize(this);
				Dispose(true);
				IsDisposed = true;
			}
		}

		#endregion
	}
}

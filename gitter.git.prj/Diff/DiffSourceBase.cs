namespace gitter.Git
{
	using System;
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
			queryParameters.Context				= options.Context;
			queryParameters.Patience			= options.UsePatienceAlgorithm;
			queryParameters.IgnoreSpaceChange	= options.IgnoreWhitespace;
			queryParameters.Binary				= options.Binary;
		}

		protected abstract Diff GetDiffCore(DiffOptions options);

		public IAsyncFunc<Diff> GetDiffAsync()
		{
			Verify.State.IsFalse(IsDisposed, "Object is disposed.");

			return GetDiffAsync(DiffOptions.Default);
		}

		public IAsyncFunc<Diff> GetDiffAsync(DiffOptions options)
		{
			Verify.Argument.IsNotNull(options, "options");
			Verify.State.IsFalse(IsDisposed, "Object is disposed.");

			return AsyncFunc.Create(
				options,
				(opt, monitor) =>
				{
					return GetDiffCore(opt);
				},
				Resources.StrLoadingDiff.AddEllipsis(),
				string.Empty);
		}

		public Diff GetDiff()
		{
			Verify.State.IsFalse(IsDisposed, "Object is disposed.");

			return GetDiffCore(DiffOptions.Default);
		}

		public Diff GetDiff(DiffOptions options)
		{
			Verify.Argument.IsNotNull(options, "options");
			Verify.State.IsFalse(IsDisposed, "Object is disposed.");

			return GetDiffCore(options);
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

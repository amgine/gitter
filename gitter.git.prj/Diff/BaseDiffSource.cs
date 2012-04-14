namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	abstract class BaseDiffSource : IDiffSource
	{
		public event EventHandler Updated;

		protected virtual void OnUpdated()
		{
			var handler = Updated;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		public virtual IEnumerable<FlowPanel> GetInformationPanels()
		{
			return null;
		}

		protected static void ApplyCommonDiffOptions(BaseQueryDiffParameters queryParameters, DiffOptions options)
		{
			queryParameters.Context = options.Context;
			queryParameters.Patience = options.UsePatienceAlgorithm;
			queryParameters.IgnoreSpaceChange = options.IgnoreWhitespace;
		}

		protected abstract Diff GetDiffCore(DiffOptions options);

		public IAsyncFunc<Diff> GetDiffAsync()
		{
			return GetDiffAsync(DiffOptions.Default);
		}

		public IAsyncFunc<Diff> GetDiffAsync(DiffOptions options)
		{
			if(options == null) throw new ArgumentNullException("options");

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
			return GetDiffCore(DiffOptions.Default);
		}

		public Diff GetDiff(DiffOptions options)
		{
			if(options == null) throw new ArgumentNullException("options");

			return GetDiffCore(options);
		}

		public virtual void Dispose()
		{
		}

		public override string ToString()
		{
			return "diff";
		}
	}
}

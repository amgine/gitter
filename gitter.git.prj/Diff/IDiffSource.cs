namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public interface IDiffSource : IDisposable
	{
		#region Events

		event EventHandler Updated;

		#endregion

		#region Properties

		Repository Repository { get; }

		#endregion

		#region Methods

		IEnumerable<FlowPanel> GetInformationPanels();

		IAsyncFunc<Diff> GetDiffAsync();

		IAsyncFunc<Diff> GetDiffAsync(DiffOptions options);

		Diff GetDiff();

		Diff GetDiff(DiffOptions options);

		#endregion
	}
}

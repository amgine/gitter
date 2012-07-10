namespace gitter.Git
{
	using System;

	using gitter.Framework;

	public interface IDiffSource : IDisposable
	{
		#region Events

		event EventHandler Updated;

		#endregion

		#region Properties

		Repository Repository { get; }

		#endregion

		#region Methods

		IAsyncFunc<Diff> GetDiffAsync();

		IAsyncFunc<Diff> GetDiffAsync(DiffOptions options);

		Diff GetDiff();

		Diff GetDiff(DiffOptions options);

		#endregion
	}
}

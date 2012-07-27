namespace gitter.Git
{
	using System;

	using gitter.Framework;

	public interface ILogSource
	{
		#region Properties

		Repository Repository { get; }

		#endregion

		#region Methods

		IAsyncFunc<RevisionLog> GetLogAsync();

		IAsyncFunc<RevisionLog> GetLogAsync(LogOptions options);

		RevisionLog GetLog();

		RevisionLog GetLog(LogOptions options);

		#endregion
	}
}

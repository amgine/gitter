namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public interface ILogSource
	{
		Repository Repository { get; }

		IAsyncFunc<RevisionLog> GetLogAsync();

		IAsyncFunc<RevisionLog> GetLogAsync(LogOptions options);

		RevisionLog GetLog();

		RevisionLog GetLog(LogOptions options);
	}
}

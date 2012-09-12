namespace gitter.Git
{
	using System;

	using gitter.Framework;
	
	using gitter.Git.AccessLayer;
	
	using Resources = gitter.Git.Properties.Resources;
	
	public abstract class LogSourceBase : ILogSource
	{
		public abstract Repository Repository { get; }

		protected abstract RevisionLog GetLogCore(LogOptions options);

		public IAsyncFunc<RevisionLog> GetLogAsync()
		{
			return AsyncFunc.Create(
				new LogOptions(),
				(opt, monitor) =>
				{
					return GetLogCore(opt);
				},
				Resources.StrFetchingLog.AddEllipsis(),
				string.Empty);
		}

		public IAsyncFunc<RevisionLog> GetLogAsync(LogOptions options)
		{
			Verify.Argument.IsNotNull(options, "options");

			return AsyncFunc.Create(
				options,
				(opt, monitor) =>
				{
					return GetLogCore(opt);
				},
				Resources.StrFetchingLog.AddEllipsis(),
				string.Empty);
		}

		public RevisionLog GetLog()
		{
			return GetLogCore(new LogOptions());
		}

		public RevisionLog GetLog(LogOptions options)
		{
			Verify.Argument.IsNotNull(options, "options");

			return GetLogCore(options);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return "log";
		}
	}
}

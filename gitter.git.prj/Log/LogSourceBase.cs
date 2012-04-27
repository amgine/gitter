namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	
	using gitter.Framework;
	
	using gitter.Git.AccessLayer;
	
	using Resources = gitter.Git.Properties.Resources;
	
	public abstract class LogSourceBase : ILogSource
	{
		protected internal static void ApplyCommonDiffOptions(QueryRevisionsParameters queryParameters, LogOptions options)
		{
		}

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
			if(options == null) throw new ArgumentNullException("options");

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
			if(options == null) throw new ArgumentNullException("options");

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

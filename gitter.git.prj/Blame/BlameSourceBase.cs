namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	abstract class BlameSourceBase : IBlameSource
	{
		public abstract Repository Repository { get; }

		protected abstract BlameFile GetBlameCore(BlameOptions options);

		public IAsyncFunc<BlameFile> GetBlameAsync()
		{
			return GetBlameAsync(BlameOptions.Default);
		}

		public IAsyncFunc<BlameFile> GetBlameAsync(BlameOptions options)
		{
			if(options == null) throw new ArgumentNullException("options");

			return AsyncFunc.Create(
				options,
				(opt, monitor) =>
				{
					return GetBlameCore(opt);
				},
				Resources.StrLoadingDiff.AddEllipsis(),
				string.Empty);
		}

		public BlameFile GetBlame()
		{
			return GetBlameCore(BlameOptions.Default);
		}

		public BlameFile GetBlame(BlameOptions options)
		{
			if(options == null) throw new ArgumentNullException("options");

			return GetBlameCore(options);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return "blame";
		}
	}
}

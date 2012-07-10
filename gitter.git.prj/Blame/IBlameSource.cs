namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public interface IBlameSource
	{
		#region Properties

		Repository Repository { get; }

		#endregion

		#region Methods

		IAsyncFunc<BlameFile> GetBlameAsync();

		IAsyncFunc<BlameFile> GetBlameAsync(BlameOptions options);

		BlameFile GetBlame();

		BlameFile GetBlame(BlameOptions options);

		#endregion
	}
}

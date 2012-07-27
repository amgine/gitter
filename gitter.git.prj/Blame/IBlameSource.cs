namespace gitter.Git
{
	using System;

	using gitter.Framework;

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

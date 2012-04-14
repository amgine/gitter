namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public interface IBlameSource
	{
		IAsyncFunc<BlameFile> GetBlameAsync();

		IAsyncFunc<BlameFile> GetBlameAsync(BlameOptions options);

		BlameFile GetBlame();

		BlameFile GetBlame(BlameOptions options);
	}
}

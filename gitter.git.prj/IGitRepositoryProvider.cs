namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework;
	using gitter.Git.AccessLayer;

	public interface IGitRepositoryProvider : IRepositoryProvider
	{
		#region Properties

		IEnumerable<IGitAccessorProvider> GitAccessorProviders { get; }

		IGitAccessorProvider ActiveGitAccessorProvider { get; set; }

		IGitAccessor GitAccessor { get; set; }

		#endregion

		#region Methods

		bool RunInitDialog();

		bool RunCloneDialog();

		#endregion
	}
}

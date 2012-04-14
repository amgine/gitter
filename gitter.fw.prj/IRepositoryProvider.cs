namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework.Configuration;

	/// <summary>Repository provider.</summary>
	public interface IRepositoryProvider : INamedObject
	{
		/// <summary>Prepare to work in context of specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		bool LoadFor(IWorkingEnvironment environment, Section section);

		/// <summary>Save configuration to <paramref name="node"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		void SaveTo(Section section);

		/// <summary>Checks if provider can create repository for <paramref name="workingDirectory"/>.</summary>
		/// <param name="workingDirectory">Repository working directory.</param>
		/// <returns>true, if <see cref="OpenRepository()"/> can succeed for <paramref name="workingDirectory"/>.</returns>
		bool IsValidFor(string workingDirectory);

		/// <summary>Opens repository specified by <paramref name="workingDirectory"/>.</summary>
		/// <param name="workingDirectory">Working directory of repository.</param>
		/// <returns>Opened repository.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="workingDirectory"/> == <c>null</c>.</exception>
		IRepository OpenRepository(string workingDirectory);

		/// <summary>Opens repository specified by <paramref name="workingDirectory"/>.</summary>
		/// <param name="workingDirectory">Working directory of repository.</param>
		/// <returns>Opened repository.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="workingDirectory"/> == <c>null</c>.</exception>
		IAsyncFunc<IRepository> OpenRepositoryAsync(string workingDirectory);

		/// <summary>Called after repository is successfully loaded by environment.</summary>
		/// <param name="environment">Working environment.</param>
		/// <param name="repository">Loaded repository.</param>
		void OnRepositoryLoaded(IWorkingEnvironment environment, IRepository repository);

		/// <summary>Releases all resources allocated by repository if applicable.</summary>
		/// <param name="repository">Repository to close.</param>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		void CloseRepository(IRepository repository);

		/// <summary>
		/// Create a gui provider to build interactive interface for <see cref="repository"/>.
		/// It is assumed that <paramref name="repository"/> was aquired by calling <see cref="OpenRepository()"/> of this <see cref="IRepositoryProvider"/>.
		/// </summary>
		/// <param name="repository">Repository to build gui upon.</param>
		/// <returns><see cref="IRepositoryGuiProvider"/> for <paramref name="repository"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		IRepositoryGuiProvider CreateGuiProvider(IRepository repository);

		/// <summary>Get list of static repository operations (that is, operations which do not require local repository).</summary>
		IEnumerable<StaticRepositoryAction> GetStaticActions();
	}
}

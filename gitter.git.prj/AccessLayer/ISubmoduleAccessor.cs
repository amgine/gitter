namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Object which can perform various operations on git submodules.</summary>
	public interface ISubmoduleAccessor
	{
		/// <summary>Updates submodule.</summary>
		/// <param name="parameters"><see cref="SubmoduleUpdateParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void UpdateSubmodule(SubmoduleUpdateParameters parameters);

		/// <summary>Adds new submodule.</summary>
		/// <param name="parameters"><see cref="AddSubmoduleParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void AddSubmodule(AddSubmoduleParameters parameters);
	}
}

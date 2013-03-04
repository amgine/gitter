namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Collections.Generic;

	using gitter.Framework.Configuration;

	/// <summary>Provides additional services for repository.</summary>
	public interface IRepositoryServiceProvider : INamedObject
	{
		/// <summary>Returns service display name.</summary>
		/// <value>Service display name.</value>
		string DisplayName { get; }

		/// <summary>Returns service icon.</summary>
		/// <value>Service icon.</value>
		Image Icon { get; }

		/// <summary>Prepare for working inside specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		bool LoadFor(IWorkingEnvironment environment, Section section);

		/// <summary>Save configuration to <paramref name="node"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		void SaveTo(Section section);

		/// <summary>Checks if service is configured for specified repository.</summary>
		/// <param name="repository">Repository to check.</param>
		/// <returns><c>true</c>, if service is configured for the specified repository, otherwise <c>false</c>.</returns>
		bool IsValidFor(IRepository repository);

		/// <summary>Creates service setup control for the specified repository.</summary>
		/// <param name="repository">Repository to configure service for.</param>
		/// <returns>Setup control for the service.</returns>
		Control CreateSetupControl(IRepository repository);

		/// <summary>Creates GUI provider to modify application UI.</summary>
		/// <param name="repository">Specifies repository.</param>
		/// <returns>GUI provider for the service.</returns>
		IGuiProvider CreateGuiProvider(IRepository repository);
	}
}

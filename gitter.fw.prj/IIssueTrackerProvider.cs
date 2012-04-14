namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Collections.Generic;

	using gitter.Framework.Configuration;

	public interface IIssueTrackerProvider : INamedObject
	{
		string DisplayName { get; }

		Image Icon { get; }

		/// <summary>Prepare for working inside specified <paramref name="environment"/>.</summary>
		/// <param name="environment"><see cref="IWorkingEnvironment"/> to work in.</param>
		/// <param name="section">Provider configuration section.</param>
		bool LoadFor(IWorkingEnvironment environment, Section section);

		/// <summary>Save configuration to <paramref name="node"/>.</summary>
		/// <param name="section"><see cref="Section"/> for storing configuration.</param>
		void SaveTo(Section section);

		bool IsValidFor(IRepository repository);

		Control CreateSetupControl(IRepository repository);

		IGuiProvider CreateGuiProvider(IRepository repository);
	}
}

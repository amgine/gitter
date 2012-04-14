namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Configuration;

	/// <summary>Interface for gui provider/builder.</summary>
	public interface IGuiProvider
	{
		/// <summary>Build gui inside supplied <paramref name="IWorkingEnvironment"/>.</summary>
		/// <param name="environment">Environment for gui elements hosting.</param>
		void AttachToEnvironment(IWorkingEnvironment environment);

		/// <summary>Remove gui, created by this provider from <paramref name="IWorkingEnvironment"/>.</summary>
		/// <param name="environment">Environment for gui elements hosting.</param>
		void DetachFromEnvironment(IWorkingEnvironment environment);

		void SaveTo(Section section);

		void LoadFrom(Section section);
	}
}

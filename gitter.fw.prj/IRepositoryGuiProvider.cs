namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Configuration;

	/// <summary>Interface for gui provider/builder.</summary>
	public interface IRepositoryGuiProvider : IGuiProvider
	{
		void ActivateDefaultView();

		/// <summary>Repository which is currently active.</summary>
		IRepository Repository { get; set; }
	}
}

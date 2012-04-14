namespace gitter.Framework.Services
{
	using System;
	using System.Windows.Forms;

	public interface IToolTipService : IDisposable
	{
		void Register(Control control, string text);

		void Unregister(Control control);
	}
}

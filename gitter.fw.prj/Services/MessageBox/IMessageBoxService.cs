namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	public interface IMessageBoxService
	{
		DialogResult Show(IWin32Window parent, string message);

		DialogResult Show(IWin32Window parent, string message, string caption);

		DialogResult Show(IWin32Window parent, string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);

		DialogResult Show(IWin32Window parent, string message, string caption, IEnumerable<MessageBoxButton> buttons, MessageBoxIcon icon);

		DialogResult Show(IWin32Window parent, string message, string caption, MessageBoxButton button, MessageBoxIcon icon);
	}
}

namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using System.Windows.Forms;

	public sealed class CustomMessageBoxService : IMessageBoxService
	{
		public DialogResult Show(IWin32Window parent, string message)
		{
			return Show(parent, message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None);
		}

		public DialogResult Show(IWin32Window parent, string message, string caption)
		{
			return Show(parent, message, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
		}

		public DialogResult Show(IWin32Window parent, string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			using(var msgbox = new MessageBoxForm(MessageBoxButton.GetButtons(buttons), icon, message, caption))
			{
				return msgbox.ShowDialog(parent);
			}
		}

		public DialogResult Show(IWin32Window parent, string message, string caption, IEnumerable<MessageBoxButton> buttons, MessageBoxIcon icon)
		{
			using(var msgbox = new MessageBoxForm(buttons, icon, message, caption))
			{
				return msgbox.ShowDialog(parent);
			}
		}

		public DialogResult Show(IWin32Window parent, string message, string caption, MessageBoxButton button, MessageBoxIcon icon)
		{
			using(var msgbox = new MessageBoxForm(button, icon, message, caption))
			{
				return msgbox.ShowDialog(parent);
			}
		}
	}
}

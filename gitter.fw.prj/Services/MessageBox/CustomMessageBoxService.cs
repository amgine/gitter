#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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

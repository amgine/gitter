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
	using System.Linq;
	using System.Collections.Generic;
	using System.Windows.Forms;

	public sealed class SystemMessageBoxService : IMessageBoxService
	{
		private static bool CheckDialogResults(HashSet<DialogResult> hs, params DialogResult[] results)
		{
			if(hs.Count != results.Length) return false;
			for(int i = 0; i < results.Length; ++i)
			{
				if(!hs.Contains(results[i])) return false;
			}
			return true;
		}

		private static MessageBoxButtons PickButtons(IEnumerable<MessageBoxButton> buttons)
		{
			var results = new HashSet<DialogResult>();
			foreach(var b in buttons)
			{
				results.Add(b.DialogResult);
			}
			if(CheckDialogResults(results, DialogResult.OK))
			{
				return MessageBoxButtons.OK;
			}
			if(CheckDialogResults(results, DialogResult.OK, DialogResult.Cancel))
			{
				return MessageBoxButtons.OKCancel;
			}
			if(CheckDialogResults(results, DialogResult.Yes, DialogResult.No))
			{
				return MessageBoxButtons.YesNo;
			}
			if(CheckDialogResults(results, DialogResult.Yes, DialogResult.No, DialogResult.Cancel))
			{
				return MessageBoxButtons.YesNoCancel;
			}
			if(CheckDialogResults(results, DialogResult.Retry, DialogResult.Cancel))
			{
				return MessageBoxButtons.RetryCancel;
			}
			if(CheckDialogResults(results, DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore))
			{
				return MessageBoxButtons.AbortRetryIgnore;
			}
			throw new NotSupportedException();
		}

		#region IMessageBoxService Members

		public DialogResult Show(IWin32Window parent, string message)
		{
			return MessageBox.Show(parent, message);
		}

		public DialogResult Show(IWin32Window parent, string message, string caption)
		{
			return MessageBox.Show(parent, message, caption);
		}

		public DialogResult Show(IWin32Window parent, string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return MessageBox.Show(parent, message, caption, buttons, icon);
		}

		public DialogResult Show(IWin32Window parent, string message, string caption, IEnumerable<MessageBoxButton> buttons, MessageBoxIcon icon)
		{
			return MessageBox.Show(parent, message, caption, PickButtons(buttons), icon);
		}

		public DialogResult Show(IWin32Window parent, string message, string caption, MessageBoxButton button, MessageBoxIcon icon)
		{
			return MessageBox.Show(parent, message, caption, PickButtons(Enumerable.Repeat(button, 1)), icon);
		}

		#endregion
	}
}

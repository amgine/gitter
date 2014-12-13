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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace gitter
{
	public partial class MainForm : Form
	{
		private const string DefaultFieldName = "Password:";
		private const string LoginRegex = "^Username for \'.*\':";
		private const string PasswordRegex = "^Password for \'.*\':";

		public MainForm()
		{
			InitializeComponent();

			var args = Environment.GetCommandLineArgs();

			if(args.Length >= 2)
			{
				var gitPrompt = args[1];
				_lblPrompt.Text = gitPrompt;
				_lblField.Text = GetFieldName(gitPrompt);
			}

			Font = SystemFonts.MessageBoxFont;
		}

		private static void SendPassword(string password)
		{
			Console.Write(password);
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
			SendPassword(_txtPassword.Text);
			Close();
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private string GetFieldName(string gitInput)
		{
			if (Regex.IsMatch(gitInput, LoginRegex))
			{
				return "Username:";
			}
			else if (Regex.IsMatch(gitInput, PasswordRegex))
			{
				return "Password:";
			}
			else
			{
				return DefaultFieldName;
			}
		}
	}
}

﻿#region Copyright Notice
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

namespace gitter;

using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public partial class MainForm : Form
{
	static Icon LoadWindowIcon()
	{
		using var stream = typeof(MainForm)
			.Assembly
			.GetManifestResourceStream(@"gitter.Resources.icons.app.ico");
		if(stream is null) return default;
		return new Icon(stream);
	}

	static Bitmap LoadLockIcon()
	{
		using var stream = typeof(MainForm)
			.Assembly
			.GetManifestResourceStream(@"gitter.Resources.icons.lock.32.png");
		if(stream is null) return default;
		return new Bitmap(stream);
	}

	private const string DefaultFieldName = "Password:";
	private const string LoginRegex = "^Username for \'.*\':";
	private const string PasswordRegex = "^Password for \'.*\':";

	public MainForm()
	{
		InitializeComponent();
		pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
		pictureBox1.Image = LoadLockIcon();

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

	private static string GetFieldName(string gitInput)
	{
		if(Regex.IsMatch(gitInput, LoginRegex))
		{
			return "Username:";
		}
		else if(Regex.IsMatch(gitInput, PasswordRegex))
		{
			return "Password:";
		}
		else
		{
			return DefaultFieldName;
		}
	}
}

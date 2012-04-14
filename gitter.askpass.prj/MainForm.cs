using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gitter
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			var args = Environment.GetCommandLineArgs();

			if(args.Length >= 2)
			{
				_lblPrompt.Text = args[1];
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
	}
}

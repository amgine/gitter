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

namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;
	using System.ComponentModel;

	public sealed class CustomListBoxTextEditor
	{
		public event EventHandler<CancelEventArgs> Validating;

		private readonly TextBox _textBox;

		internal CustomListBoxTextEditor(CustomListBox listBox, TextBox textBox)
		{
			ListBox = listBox;
			_textBox = textBox;
			_textBox.Validating += OnValidating;
			_textBox.KeyDown += OnKeyDown;
		}

		public void OnKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape:
					Stop();
					break;
				case Keys.Enter:
					var args = new CancelEventArgs(false);
					OnValidating(_textBox, args);
					break;
			}
		}

		private void OnValidating(object sender, CancelEventArgs e)
		{
			var handler = Validating;
			if(handler != null)
			{
				var args = new CancelEventArgs(false);
				handler(this, args);
				if(args.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}
			Stop();
		}

		public CustomListBox ListBox { get; }

		public string Text
		{
			get => _textBox.Text;
			set => _textBox.Text = value;
		}

		public void Stop()
		{
			_textBox.Validating -= OnValidating;
			_textBox.KeyDown -= OnKeyDown;
			ListBox.StopTextEditor();
		}
	}
}

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
	using System.ComponentModel;
	using System.Media;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public partial class MessageBoxForm : Form
	{
		private Size MinClientSize = new Size(138, 124);
		private const int MaxClientWidth = 481;

		private readonly string _title;
		private readonly string _message;
		private readonly MessageBoxIcon _mbIcon;
		private readonly Icon _icon;
		private bool _buttonClick;
		private bool _hasCancelButton;
		private int _buttonCount;
		private IEnumerable<Button> _buttons;

		public MessageBoxForm()
		{
			InitializeComponent();

			Font = SystemFonts.MessageBoxFont;
		}

		public MessageBoxForm(MessageBoxButton button, MessageBoxIcon icon, string message, string caption)
			: this(Enumerable.Repeat(button, 1), icon, message, caption)
		{
		}

		public MessageBoxForm(IEnumerable<MessageBoxButton> buttons, MessageBoxIcon icon, string message, string title)
		{
			InitializeComponent();

			_title = title;
			_message = message;

			var s = ClientSize;
			s.Width = MaxClientWidth;
			ClientSize = s;

			Text = title;
			_mbIcon = icon;
			_icon = GetSystemIcon(icon);
			LayoutButtons(buttons);
			LayoutMessage(message);
		}

		private void LayoutButtons(IEnumerable<MessageBoxButton> buttons)
		{
			var minW = 0;
			var list = new List<Button>();
			int tabIndex = 0;
			foreach(var btn in buttons)
			{
				var control = new Button()
				{
					FlatStyle = FlatStyle.System,
					Text = btn.DisplayLabel,
					Name = btn.DisplayLabel,
					TabIndex = tabIndex++,
					Tag = btn,
				};
				control.Click += OnButtonClick;
				list.Add(control);
			}
			var cs = ClientSize;
			var size1 = new Size(75, 23);
			var size2 = new Size(106, 23);
			int x = cs.Width - 1;
			int y = cs.Height - size1.Height - 8;
			minW = 1;
			for(int i = list.Count - 1; i >= 0; --i)
			{
				var size = (TextRenderer.MeasureText(list[i].Text, Font).Width > size1.Width - 8) ?
					size2 : size1;
				x -= size.Width + 6;
				list[i].SetBounds(x, y, size.Width, size.Height);
				list[i].Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
				list[i].Parent = this;
				var btn = (MessageBoxButton)(list[i].Tag);
				if(btn.IsDefault)
				{
					AcceptButton = list[i];
				}
				if(btn.DialogResult == DialogResult.Cancel)
				{
					_hasCancelButton = true;
				}
				if(CancelButton == null && (
					btn.DialogResult == DialogResult.Abort ||
					btn.DialogResult == DialogResult.Cancel ||
					btn.DialogResult == DialogResult.No))
				{
					CancelButton = list[i];
				}
				minW += size.Width + 6;
			}
			if(list.Count == 1 && CancelButton == null)
			{
				CancelButton = list[0];
			}
			minW += 42 - 6;
			if(MinClientSize.Width < minW)
			{
				MinClientSize.Width = minW;
			}
			_buttons = list;
			_buttonCount = list.Count;
		}

		private Icon GetSystemIcon(MessageBoxIcon icon)
			=> icon switch
			{
				MessageBoxIcon.Information => SystemIcons.Information,
				MessageBoxIcon.Error       => SystemIcons.Error,
				MessageBoxIcon.Exclamation => SystemIcons.Exclamation,
				MessageBoxIcon.Question    => SystemIcons.Question,
				_ => null,
			};

		private void LayoutMessage(string message)
		{
			if(_icon == null)
			{
				_picIcon.Visible = false;
			}
			_lblMessage.Text = message;
		}

		private void OnButtonClick(object sender, EventArgs e)
		{
			var btn = (MessageBoxButton)((Button)sender).Tag;
			this.DialogResult = btn.DialogResult;
			_buttonClick = true;
			Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			if(!_buttonClick)
			{
				DialogResult = DialogResult.Cancel;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			if(_mbIcon == MessageBoxIcon.None)
			{
				_lblMessage.SetBounds(_lblMessage.Left - 53, 0, _lblMessage.Width + 53, 0,
					BoundsSpecified.X | BoundsSpecified.Width);
			}

			var minMessageWidth = MinClientSize.Width - (ClientSize.Width - _lblMessage.Width);
			var minMessageHeight = MinClientSize.Height - (ClientSize.Height - _lblMessage.Height);
			if(minMessageHeight < 32) minMessageHeight = 32;

			bool centerMessage = false;

			var size = TextRenderer.MeasureText(_message, _lblMessage.Font, _lblMessage.Size, TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
			var h = size.Height;
			int maxMessageHeight = Screen.GetBounds(this).Height * 3 / 4;
			if(h < minMessageHeight)
			{
				if(h < 32)
				{
					centerMessage = true;
				}
				h = minMessageHeight;
			}
			else if(h > maxMessageHeight)
			{
				h = maxMessageHeight;
			}
			var d = _lblMessage.Height - h;
			Height -= d;
			if(size.Width < _lblMessage.Width)
			{
				d = _lblMessage.Width - size.Width;
				if(_lblMessage.Width - d < minMessageWidth)
				{
					d = _lblMessage.Width - minMessageWidth;
				}
				Width -= d;
			}

			if(centerMessage)
			{
				_lblMessage.Top = _picIcon.Top + (31 - size.Height) / 2;
			}

			PlaySystemSound();

			base.OnLoad(e);

			if(!_hasCancelButton && _buttonCount > 1)
			{
				this.DisableCloseButton();
			}
		}

		private void PlaySystemSound()
		{
			switch(_mbIcon)
			{
				case MessageBoxIcon.Information:
					SystemSounds.Asterisk.Play();
					break;
				case MessageBoxIcon.Error:
					SystemSounds.Hand.Play();
					break;
				case MessageBoxIcon.Exclamation:
					SystemSounds.Exclamation.Play();
					break;
				case MessageBoxIcon.Question:
					SystemSounds.Question.Play();
					break;
			}
		}

		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButton button, MessageBoxIcon icon)
		{
			return Show(owner, text, caption, new[] { button }, icon);
		}

		public static DialogResult Show(IWin32Window owner, string text, string caption, IEnumerable<MessageBoxButton> buttons, MessageBoxIcon icon)
		{
			using(var form = new MessageBoxForm(buttons, icon, text, caption))
			{
				form.ShowDialog(owner);
				return form.DialogResult;
			}
		}

		private void OnIconPaint(object sender, PaintEventArgs e)
		{
			if(_icon != null)
			{
				e.Graphics.DrawIcon(_icon, 0, 0);
			}
		}

		private void CopyToClipboard()
		{
			const string separator = "---------------------------";

			var sb = new StringBuilder();
			sb.AppendLine(separator);
			sb.AppendLine(_title);
			sb.AppendLine(separator);
			sb.AppendLine(_message);
			sb.AppendLine(separator);
			foreach(var button in _buttons)
			{
				sb.Append(button.Text);
				sb.Append("   ");
			}
			sb.AppendLine();
			sb.AppendLine(separator);
			ClipboardEx.SetTextSafe(sb.ToString());
		}

		private void MessageBoxForm_KeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.C:
					if(e.Modifiers == Keys.Control)
					{
						CopyToClipboard();
						SystemSounds.Beep.Play();
					}
					break;
			}
		}
	}
}

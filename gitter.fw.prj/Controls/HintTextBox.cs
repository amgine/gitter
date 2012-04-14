using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace gitter.Framework.Controls
{
	/// <summary>TextBox which dispays grayed hint text if no text is entered.</summary>
	public class HintTextBox : TextBox
	{
		private bool _userTextEntered;
		private string _hint;

		public HintTextBox()
		{
			EnterHintMode();
			_hint = string.Empty;
		}

		private static bool TextIsValid(string text)
		{
			return !string.IsNullOrWhiteSpace(text);
		}

		private void EnterHintMode()
		{
			_userTextEntered = false;
			ForeColor = SystemColors.GrayText;
			base.Text = Hint;
		}

		private void EnterNormalMode()
		{
			_userTextEntered = true;
			ForeColor = SystemColors.WindowText;
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if(!_userTextEntered)
			{
				EnterNormalMode();
				base.Text = string.Empty;
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if(TextIsValid(base.Text))
			{
				_userTextEntered = true;
			}
			else
			{
				EnterHintMode();
			}
		}

		[DefaultValue("")]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Description("Text to display on unfocused control when no text is entered.")]
		public string Hint
		{
			get { return _hint; }
			set
			{
				_hint = value;
				if(!_userTextEntered)
				{
					base.Text = value;
				}
			}
		}

		[DefaultValue("")]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Description("Current text in the HintTextBox. If no text is entered, hint text is not returned.")]
		public override string Text
		{
			get { return _userTextEntered ? base.Text : string.Empty; }
			set
			{
				if(TextIsValid(value))
				{
					_userTextEntered = true;
					base.Text = value;
				}
				else
				{
					EnterHintMode();
				}
			}
		}
	}
}

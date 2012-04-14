namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;
	using System.ComponentModel;

	public sealed class CustomListBoxTextEditor
	{
		public event EventHandler<CancelEventArgs> Validating;

		private readonly CustomListBox _listBox;
		private readonly TextBox _textBox;

		internal CustomListBoxTextEditor(CustomListBox listBox, TextBox textBox)
		{
			_listBox = listBox;
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

		public CustomListBox ListBox
		{
			get { return _listBox; }
		}

		public string Text
		{
			get { return _textBox.Text; }
			set { _textBox.Text = value; }
		}

		public void Stop()
		{
			_textBox.Validating -= OnValidating;
			_textBox.KeyDown -= OnKeyDown;
			_listBox.StopTextEditor();
		}
	}
}

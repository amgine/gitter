namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	public sealed class CustomButtonAdapter : IButtonWidget
	{
		#region Data

		private readonly CustomButton _button;

		#endregion

		#region Events

		public event EventHandler Click;

		private void OnClick()
		{
			var handler = Click;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		public CustomButtonAdapter(CustomButtonRenderer renderer)
		{
			Verify.Argument.IsNotNull(renderer, "renderer");

			_button = new CustomButton()
			{
				Renderer = renderer,
			};
			_button.Click += OnButtonClick;
		}

		#endregion

		#region Event Handlers

		private void OnButtonClick(object sender, EventArgs e)
		{
			OnClick();
		}

		#endregion

		#region Properties

		public Control Control
		{
			get { return _button; }
		}

		public string Text
		{
			get { return _button.Text; }
			set { _button.Text = value; }
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			_button.Click -= OnButtonClick;
			_button.Dispose();
		}

		#endregion
	}
}

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class CustomCheckBoxAdapter : ICheckBoxWidget
	{
		#region Data

		private readonly CustomCheckBox _checkBox;

		#endregion

		#region Events

		public event EventHandler IsCheckedChanged;

		public event EventHandler CheckStateChanged;

		private void OnIsCheckedChanged()
		{
			var handler = IsCheckedChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		private void OnCheckStateChanged()
		{
			var handler = CheckStateChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region .ctor

		public CustomCheckBoxAdapter(CustomCheckBoxRenderer renderer)
		{
			Verify.Argument.IsNotNull(renderer, "renderer");

			_checkBox = new CustomCheckBox()
			{
				Renderer = renderer,
			};
			_checkBox.IsCheckedChanged += OnCheckBoxIsCheckedChanged;
			_checkBox.CheckStateChanged += OnCheckBoxCheckStateChanged;
		}

		#endregion

		#region Event Handlers

		private void OnCheckBoxIsCheckedChanged(object sender, EventArgs e)
		{
			OnIsCheckedChanged();
		}

		private void OnCheckBoxCheckStateChanged(object sender, EventArgs e)
		{
			OnCheckStateChanged();
		}

		#endregion

		#region Properties

		public Control Control
		{
			get { return _checkBox; }
		}

		public string Text
		{
			get { return _checkBox.Text; }
			set { _checkBox.Text = value; }
		}

		public Image Image
		{
			get { return _checkBox.Image; }
			set { _checkBox.Image = value; }
		}

		public bool IsChecked
		{
			get { return _checkBox.IsChecked; }
			set { _checkBox.IsChecked = value; }
		}

		public CheckState CheckState
		{
			get { return _checkBox.CheckState; }
			set { _checkBox.CheckState = value; }
		}

		public bool ThreeState
		{
			get { return _checkBox.ThreeState; }
			set { _checkBox.ThreeState = value; }
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			_checkBox.IsCheckedChanged -= OnCheckBoxIsCheckedChanged;
			_checkBox.CheckStateChanged -= OnCheckBoxCheckStateChanged;
			_checkBox.Dispose();
		}

		#endregion
	}
}

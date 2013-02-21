namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	public sealed class SystemCheckBoxAdapter : ICheckBoxWidget
	{
		#region Data

		private readonly CheckBox _checkBox;

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

		public SystemCheckBoxAdapter()
		{
			_checkBox = new CheckBox()
			{
				FlatStyle = FlatStyle.System,
			};
			_checkBox.CheckedChanged += OnCheckBoxCheckedChanged;
			_checkBox.CheckStateChanged += OnCheckBoxCheckStateChanged;
		}

		private void OnCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			OnIsCheckedChanged();
		}

		private void OnCheckBoxCheckStateChanged(object sender, EventArgs e)
		{
			OnCheckStateChanged();
		}

		public Control Control
		{
			get { return _checkBox; }
		}

		public string Text
		{
			get { return _checkBox.Text; }
			set { _checkBox.Text = value; }
		}

		public bool IsChecked
		{
			get { return _checkBox.Checked; }
			set { _checkBox.Checked = value; }
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

		public void Dispose()
		{
			_checkBox.CheckedChanged -= OnCheckBoxCheckedChanged;
			_checkBox.CheckStateChanged -= OnCheckBoxCheckStateChanged;
			_checkBox.Dispose();
		}
	}
}

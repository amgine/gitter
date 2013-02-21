namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	public interface ICheckBoxWidget : IDisposable
	{
		#region Events

		event EventHandler IsCheckedChanged;

		event EventHandler CheckStateChanged;

		#endregion

		#region Properties

		Control Control { get; }

		string Text { get; set; }

		bool IsChecked { get; set; }

		CheckState CheckState { get; set; }

		bool ThreeState { get; set; }

		#endregion
	}
}

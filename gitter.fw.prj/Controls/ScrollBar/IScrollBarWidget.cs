namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	public interface IScrollBarWidget : IDisposable
	{
		#region Events

		event EventHandler<ScrollEventArgs> Scroll;

		event EventHandler ValueChanged;

		#endregion

		#region Properties

		Control Control { get; }

		Orientation Orientation { get; }

		int Value { get; set; }

		int Minimum { get; set; }

		int Maximum { get; set; }

		int SmallChange { get; set; }

		int LargeChange { get; set; }

		#endregion
	}
}

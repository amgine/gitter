namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	public interface IButtonWidget : IDisposable
	{
		#region Events

		event EventHandler Click;

		#endregion

		#region Properties

		Control Control { get; }

		string Text { get; set; }

		#endregion
	}
}

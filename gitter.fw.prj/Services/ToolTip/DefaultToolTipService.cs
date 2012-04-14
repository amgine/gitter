namespace gitter.Framework.Services
{
	using System;
	using System.Windows.Forms;

	public sealed class DefaultToolTipService : IToolTipService, IDisposable
	{
		private ToolTip _toolTip;

		public DefaultToolTipService()
		{
			_toolTip = new ToolTip()
			{
				/*IsBalloon = true,*/
			};
		}

		public void Register(Control control, string text)
		{
			_toolTip.SetToolTip(control, text);
		}

		public void Unregister(Control control)
		{
			_toolTip.SetToolTip(control, string.Empty);
		}

		~DefaultToolTipService()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_toolTip != null)
				{
					_toolTip.RemoveAll();
					_toolTip.Dispose();
					_toolTip = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}

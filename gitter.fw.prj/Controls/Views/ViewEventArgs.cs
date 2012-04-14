namespace gitter.Framework.Controls
{
	using System;

	public class ViewEventArgs : EventArgs
	{
		private readonly ViewBase _view;

		public ViewEventArgs(ViewBase view)
		{
			_view = view;
		}

		public ViewBase View
		{
			get { return _view; }
		}
	}
}

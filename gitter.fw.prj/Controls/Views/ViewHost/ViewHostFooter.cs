namespace gitter.Framework.Controls
{
	using System;
	using System.Windows.Forms;

	public sealed class ViewHostFooter : Control
	{
		private readonly ViewHost _host;

		internal ViewHostFooter(ViewHost viewHost)
		{
			if(viewHost == null) throw new ArgumentNullException("viewHost");
			_host = viewHost;

			SetStyle(
				ControlStyles.ContainerControl |
				ControlStyles.Selectable |
				ControlStyles.SupportsTransparentBackColor,
				false);
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer,
				true);
		}

		public ViewHost ViewHost
		{
			get { return _host; }
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			ViewManager.Renderer.RenderViewHostFooter(this, e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			ViewHost.Activate();
			base.OnMouseDown(e);
		}
	}
}

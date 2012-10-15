namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	/// <summary><see cref="FlowPanel"/> which displays basic commit information: author, hash, date, subject, etc.</summary>
	public sealed class RevisionHeaderPanel : FlowPanel
	{
		#region Data

		private RevisionHeaderContent _content;
		private Revision _revision;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="RevisionHeaderPanel"/>.</summary>
		public RevisionHeaderPanel()
		{
			_content = new RevisionHeaderContent();
			_content.Invalidated += OnContentInvalidated;
			_content.SizeChanged += OnContentSizeChanged;
			_content.ContextMenuRequested += OnContentContextMenuRequested;
		}

		#endregion

		private void OnContentContextMenuRequested(object sender, ContentContextMenuEventArgs e)
		{
			ShowContextMenu(e.ContextMenu, e.Position.X, e.Position.Y);
		}

		private void OnContentInvalidated(object sender, ContentInvalidatedEventArgs e)
		{
			InvalidateSafe(e.Bounds);
		}

		private void OnContentSizeChanged(object sender, EventArgs e)
		{
			InvalidateSize();
		}

		protected override void OnMouseMove(int x, int y)
		{
			_content.OnMouseMove(x, y);
			base.OnMouseMove(x, y);
		}

		protected override void OnMouseLeave()
		{
			base.OnMouseLeave();
			_content.OnMouseLeave();
		}

		/// <summary>Displayed <see cref="T:gitter.Git.Revision"/>.</summary>
		public Revision Revision
		{
			get { return _revision; }
			set
			{
				if(_revision != value)
				{
					_revision = value;
					if(FlowControl != null)
					{
						_content.Revision = _revision;
					}
				}
			}
		}

		protected override void OnFlowControlAttached()
		{
			_content.Revision = _revision;
			base.OnFlowControlAttached();
		}

		protected override void OnFlowControlDetached()
		{
			_content.Revision = null;
			base.OnFlowControlDetached();
		}

		protected override void OnMouseDown(int x, int y, MouseButtons button)
		{
			base.OnMouseDown(x, y, button);
			_content.OnMouseDown(x, y, button);
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			return _content.OnMeasure(measureEventArgs.Graphics, measureEventArgs.Width);
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			_content.OnPaint(paintEventArgs.Graphics, paintEventArgs.Bounds);
		}
	}
}

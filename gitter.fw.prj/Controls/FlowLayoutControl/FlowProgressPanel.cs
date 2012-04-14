namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	public class FlowProgressPanel : FlowPanel
	{
		#region Data

		private ProcessOverlay _overlay;
		private int _height;
		private string _message;

		#endregion

		/// <summary>Create <see cref="FlowProgressPanel"/>.</summary>
		public FlowProgressPanel()
		{
			_height = 150;
			_overlay = new ProcessOverlay()
			{
				DisableHost = false,
				InvalidateHost = false,
			};
			_overlay.RepaintRequired += (sender, e) => InvalidateSafe();
		}

		protected override void OnFlowControlAttached()
		{
			_overlay.HostControl = FlowControl;
		}

		protected override void OnFlowControlDetached()
		{
			_overlay.HostControl = null;
		}

		public int Height
		{
			get { return _height; }
			set
			{
				if(_height != value)
				{
					if(value < 0) throw new ArgumentOutOfRangeException("value");
					_height = value;
					InvalidateSize();
				}
			}
		}

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public IAsyncProgressMonitor ProgressMonitor
		{
			get { return _overlay; }
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			return new Size(0, _height);
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;

			var mw = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width);
			var w = mw - 5 * 2;
			int h = _height  - 5 * 2;
			if(w > 300) w = 300;
			if(h > 85) h = 85;
			var rc = new Rectangle(
				rect.X + (mw - w) / 2,
				rect.Y + (rect.Height - h) / 2,
				w, h);
			if(_overlay != null)
			{
				if(_overlay.Visible)
				{
					_overlay.OnPaint(graphics, rc);
				}
				else
				{
					if(!string.IsNullOrEmpty(_message))
					{
						_overlay.DrawMessage(graphics, FlowControl.Font, rc, _message);
					}
				}
			}
		}
	}
}

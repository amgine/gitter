namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Panel for separating other panels.</summary>
	public class FlowPanelSeparator : FlowPanel
	{
		private int _height;
		private FlowPanelSeparatorStyle _style;

		/// <summary>Create <see cref="FlowPanelSeparator"/>.</summary>
		public FlowPanelSeparator()
		{
			_height = 16;
		}

		/// <summary>Separator height.</summary>
		public int Height
		{
			get { return _height; }
			set
			{
				if(_height != value)
				{
					_height = value;
				}
			}
		}

		public FlowPanelSeparatorStyle Style
		{
			get { return _style; }
			set
			{
				if(_style != value)
				{
					_style = value;
				}
			}
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			return new Size(0, _height);
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			switch(_style)
			{
				case FlowPanelSeparatorStyle.Line:
					{
						var y = _height / 2;
						var x = y;
						var w = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width) - 2 * x;
						if(w > 0)
						{
							x += rect.X;
							y += rect.Y;
							graphics.DrawLine(Pens.Gray, x, y, x + w, y);
						}
					}
					break;
			}
		}
	}
}

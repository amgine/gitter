#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
			_overlay.Renderer = FlowControl.Style.OverlayRenderer;
			_overlay.HostControl = FlowControl;
		}

		protected override void OnFlowControlDetached()
		{
			_overlay.HostControl = null;
			_overlay.Renderer = null;
		}

		public int Height
		{
			get { return _height; }
			set
			{
				Verify.Argument.IsNotNegative(value, "value");

				if(_height != value)
				{
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

		public IProgress<OperationProgress> ProgressMonitor
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
				if(_overlay.IsVisible)
				{
					_overlay.OnPaint(graphics, rc);
				}
				else
				{
					if(!string.IsNullOrEmpty(_message))
					{
						_overlay.DrawMessage(graphics, rc, _message);
					}
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gitter.Framework.Controls
{
	public abstract class FlowPanelHeader
	{
		private readonly FlowPanel _flowPanel;

		protected FlowPanelHeader(FlowPanel flowPanel)
		{
			Verify.Argument.IsNotNull(flowPanel, "flowPanel");

			_flowPanel = flowPanel;
		}

		public FlowPanel FlowPanel
		{
			get { return _flowPanel; }
		}

		public abstract int Height
		{
			get;
		}

		protected abstract void OnPaint(FlowPanelPaintEventArgs paintEventArgs);
	}
}

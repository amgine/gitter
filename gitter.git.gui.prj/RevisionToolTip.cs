namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	sealed class RevisionToolTip : CustomToolTip
	{
		private const int MaxWidth = 450;

		private RevisionHeaderContent _content;

		public RevisionToolTip()
		{
			_content = new RevisionHeaderContent();
		}

		public Revision Revision
		{
			get { return _content.Revision; }
			set { _content.Revision = value; }
		}

		protected override void OnPaint(DrawToolTipEventArgs e)
		{
			base.OnPaint(e);
			_content.OnPaint(e.Graphics, e.Bounds);
		}

		public override Size Size
		{
			get
			{
				var size = _content.OnMeasure(Utility.MeasurementGraphics, MaxWidth);
				size.Height += 3;
				return size;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_content.Revision = null;
			}
			base.Dispose(disposing);
		}
	}
}

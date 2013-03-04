namespace gitter.Framework.Controls
{
	using System.Drawing;

	public abstract class ProcessOverlayRenderer
	{
		protected static readonly StringFormat StringFormat = new StringFormat()
		{
			Alignment = StringAlignment.Center,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter,
			FormatFlags = StringFormatFlags.FitBlackBox,
		};

		protected static readonly StringFormat TitleStringFormat = new StringFormat(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter,
			FormatFlags = StringFormatFlags.FitBlackBox,
		};

		public static readonly ProcessOverlayRenderer Default = new DefaultOverlayRenderer();

		public static readonly ProcessOverlayRenderer MSVS2012Dark = new MSVS2012OverlayRenderer(MSVS2012OverlayRenderer.DarkColors);

		public abstract void Paint(ProcessOverlay processOverlay, Graphics graphics, Rectangle bounds);

		public abstract void PaintMessage(ProcessOverlay processOverlay, Graphics graphics, Rectangle bounds, string status);
	}
}

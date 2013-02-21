namespace gitter.Framework
{
	using System;
	using System.Drawing;

	public sealed class SystemStyleColors : IGitterStyleColors
	{
		public Color WorkArea
		{
			get { return Color.FromArgb(41, 57, 85); }
		}

		public Color Window
		{
			get { return SystemColors.Window; }
		}

		public Color ScrollBarSpacing
		{
			get { return SystemColors.Window; }
		}

		public Color Separator
		{
			get { return Color.FromArgb(209, 226, 252); }
		}

		public Color Alternate
		{
			get { return Color.WhiteSmoke; }
		}

		public Color WindowText
		{
			get { return SystemColors.WindowText; }
		}

		public Color GrayText
		{
			get { return SystemColors.GrayText; }
		}

		public Color HyperlinkText
		{
			get { return Color.Blue; }
		}

		public Color HyperlinkTextHotTrack
		{
			get { return Color.Blue; }
		}

		public Color FileHeaderColor1				{ get { return Color.FromArgb(245, 245, 245); } }
		public Color FileHeaderColor2				{ get { return Color.FromArgb(232, 232, 232); } }
		public Color FilePanelBorder				{ get { return Color.Gray; } }
		public Color LineContextForeground			{ get { return Color.FromArgb(0, 0, 0); } }
		public Color LineContextBackground			{ get { return Color.FromArgb(255, 255, 255); } }
		public Color LineAddedForeground			{ get { return Color.FromArgb(0, 100, 0); } }
		public Color LineAddedBackground			{ get { return Color.FromArgb(221, 255, 233); } }
		public Color LineRemovedForeground			{ get { return Color.FromArgb(200, 0, 0); } }
		public Color LineRemovedBackground			{ get { return Color.FromArgb(255, 238, 238); } }
		public Color LineNumberForeground			{ get { return Color.Gray; } }
		public Color LineNumberBackground			{ get { return Color.FromArgb(247, 247, 247); } }
		public Color LineNumberBackgroundHover		{ get { return LineNumberBackground.Darker(0.1f); } }
		public Color LineHeaderForeground			{ get { return Color.Gray; } }
		public Color LineHeaderBackground			{ get { return Color.FromArgb(247, 247, 247); } }
		public Color LineSelectedBackground			{ get { return Color.FromArgb(173, 214, 255); } }
		public Color LineSelectedBackgroundHover	{ get { return LineSelectedBackground.Darker(0.1f); } }
		public Color LineBackgroundHover			{ get { return LineSelectedBackground.Lighter(0.3f); } }
	}
}

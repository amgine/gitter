namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;

	internal static class ColorScheme
	{
		public static readonly Color TagBackColor = Color.FromArgb(255, 255, 155);
		public static readonly Color LocalBranchBackColor = Color.FromArgb(155, 255, 155);
		public static readonly Color RemoteBranchBackColor = Color.FromArgb(255, 155, 155);
		public static readonly Color StashBackColor = Color.FromArgb(200, 200, 255);

		public static readonly Color ModifiedBackColor = Color.FromArgb(255, 255, 155);
		public static readonly Color AddedBackColor = Color.FromArgb(155, 255, 155);
		public static readonly Color RemovedBackColor = Color.FromArgb(255, 155, 155);

		public static readonly Color LineContextText = Color.FromArgb(0, 0, 0);
		public static readonly Color LineContextBackColor = Color.FromArgb(255, 255, 255);
		public static readonly Color LineAddedText = Color.FromArgb(0, 100, 0);
		public static readonly Color LineAddedBackColor = Color.FromArgb(221, 255, 233);
		public static readonly Color LineRemovedText = Color.FromArgb(200, 0, 0);
		public static readonly Color LineRemovedBackColor = Color.FromArgb(255, 238, 238);
		public static readonly Color LineNumberText = Color.Gray;
		public static readonly Color LineNumberBackColor = Color.FromArgb(247, 247, 247);
		public static readonly Color LineNumberHoverBackColor = LineNumberBackColor.Darker(0.1f);
		public static readonly Color LineHeaderText = Color.Gray;
		public static readonly Color LineHeaderBackColor = Color.FromArgb(247, 247, 247);
		public static readonly Color LineSelectedBackColor = Color.FromArgb(173, 214, 255);
		public static readonly Color LineSelectedHoverBackColor = LineSelectedBackColor.Darker(0.1f);
		public static readonly Color LineHoverBackColor = LineSelectedBackColor.Lighter(0.3f);
	}
}

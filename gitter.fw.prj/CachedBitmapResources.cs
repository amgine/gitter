namespace gitter.Framework
{
	using System;
	using System.Resources;
	using System.Drawing;
	using System.Drawing.Drawing2D;

	/// <summary>Provides cached bitmap resources.</summary>
	public sealed class CachedBitmapResources : CachedResources<Bitmap>
	{
		public CachedBitmapResources(ResourceManager resourceManager)
			: base(resourceManager)
		{
		}

		public Bitmap CombineBitmaps(string resBackground, string resOverlay)
		{
			var res = new Bitmap(this[resBackground]);
			var ovl = this[resOverlay];
			using(var graphics = Graphics.FromImage(res))
			{
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				var rc = new Rectangle(0, 0, ovl.Width, ovl.Height);
				graphics.DrawImage(ovl, rc, rc, GraphicsUnit.Pixel);
			}
			return res;
		}
	}
}

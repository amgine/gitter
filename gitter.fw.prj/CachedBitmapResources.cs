namespace gitter.Framework
{
	using System;
	using System.Resources;
	using System.Drawing;

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
			using(var gx = Graphics.FromImage(res))
			{
				gx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				var rc = new Rectangle(0, 0, ovl.Width, ovl.Height);
				gx.DrawImage(ovl, rc, rc, GraphicsUnit.Pixel);
			}
			return res;
		}
	}
}

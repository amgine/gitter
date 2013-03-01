namespace gitter.Framework
{
	using System;
	using System.Resources;
	using System.Drawing;

	/// <summary>Provides cached icon resources.</summary>
	public sealed class CachedIconResources : CachedResources<Icon>
	{
		public CachedIconResources(ResourceManager resourceManager)
			: base(resourceManager)
		{
		}
	}
}

namespace gitter.Git.Gui
{
	using System;

	/// <summary>Object for unique color allocation.</summary>
	public interface IGraphColorProvider
	{
		int AcquireColor();

		void ReleaseColor(int color);
	}
}

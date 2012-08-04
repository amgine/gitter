namespace gitter.Framework.Controls
{
	/// <summary>Represents docking status of <see cref="ViewHost"/> control.</summary>
	public enum ViewHostStatus
	{
		/// <summary><see cref="ViewHost"/> is not present in dock model hierarchy and can be safely docked.</summary>
		Offscreen,
		/// <summary><see cref="ViewHost"/> is docked inside <see cref="ViewHostGrid"/> or <see cref="ViewSplit"/>.</summary>
		Docked,
		/// <summary><see cref="ViewHost"/> is docked inside <see cref="FloatingViewForm"/> with other hosts.</summary>
		DockedOnFloat,
		/// <summary><see cref="ViewHost"/> is docked inside <see cref="FloatingViewForm"/>.</summary>
		Floating,
		/// <summary><see cref="ViewHost"/> is docked inside <see cref="ViewDockSide"/>.</summary>
		AutoHide,
		/// <summary><see cref="ViewHost"/> is disposed as a result of docking operation.</summary>
		Disposed,
	}
}

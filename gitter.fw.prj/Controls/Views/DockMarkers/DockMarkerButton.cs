namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Button which indicates specific docking position on dock marker.</summary>
	public sealed class DockMarkerButton
	{
		#region Data

		private readonly Rectangle _bounds;
		private readonly DockResult _type;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="DockMarkerButton"/> class.</summary>
		/// <param name="bounds">Button bounds.</param>
		/// <param name="type">Button type.</param>
		internal DockMarkerButton(Rectangle bounds, DockResult type)
		{
			_bounds = bounds;
			_type = type;
		}

		#endregion

		#region Properties

		/// <summary>Gets the bounds of this <see cref="DockMarkerButton"/>.</summary>
		/// <value>Bounds of this <see cref="DockMarkerButton"/>.</value>
		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		/// <summary>Gets the docking position associated with this button.</summary>
		/// <value>Docking position associated with this button.</value>
		public DockResult Type
		{
			get { return _type; }
		}

		#endregion

		/// <summary>Paints this <see cref="DockMarkerButton"/>.</summary>
		/// <param name="graphics">The graphics surface to draw on.</param>
		/// <param name="hover">Indicates whether this button is hovered.</param>
		internal void OnPaint(Graphics graphics, bool hover)
		{
			ViewManager.Renderer.RenderDockMarkerButton(this, graphics, hover);
		}
	}
}

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	/// <summary>Represents continious drag-by-mouse process.</summary>
	public interface IMouseDragProcess
	{
		/// <summary>Action is in process.</summary>
		bool IsActive { get; }

		/// <summary>Start drag process.</summary>
		/// <param name="location">Mouse coordinates.</param>
		/// <returns>True if process started successfully.</returns>
		bool Start(Point location);

		/// <summary>Update process status.</summary>
		/// <param name="location">Mouse cordinates.</param>
		void Update(Point location);

		/// <summary>Perform action associated with this process and complete operation.</summary>
		/// <param name="location">Mouse coordinates.</param>
		void Commit(Point location);

		/// <summary>Cancel process without applying any changes.</summary>
		void Cancel();
	}
}

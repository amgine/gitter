namespace gitter.Framework
{
	using System;

	/// <summary>Dialog that may require elevated privileges to execute.</summary>
	public interface IElevatedExecutableDialog
	{
		/// <summary><see cref="RequireElevation"/> property value changed.</summary>
		event EventHandler RequireElevationChanged;

		/// <summary>Elevation is required to execute dialog in its current state.</summary>
		bool RequireElevation { get; }

		/// <summary>Action which performs operations requiring elevation.</summary>
		Action ElevatedExecutionAction { get; }
	}
}

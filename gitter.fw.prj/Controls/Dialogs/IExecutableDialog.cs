namespace gitter.Framework
{
	using System;

	/// <summary>Interface for dialog that can perform requested operation itself.</summary>
	public interface IExecutableDialog
	{
		/// <summary>Execute dialog associated action.</summary>
		/// <returns><c>true</c>, if action succeded</returns>
		bool Execute();
	}
}

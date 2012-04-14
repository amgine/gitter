namespace gitter.Framework
{
	using System;

	/// <summary>Dialog form buttons.</summary>
	[Flags]
	public enum DialogButtons
	{
		/// <summary>No buttons.</summary>
		None = 0,

		/// <summary>Ok button.</summary>
		Ok = (1 << 0),
		/// <summary>Cancel button.</summary>
		Cancel = (1 << 1),
		/// <summary>Apply button.</summary>
		Apply = (1 << 2),

		/// <summary>Ok &amp; Cancel.</summary>
		OkCancel = Ok | Cancel,
		/// <summary>Ok, Cancel &amp; Apply.</summary>
		All = Ok | Cancel | Apply,
	}
}

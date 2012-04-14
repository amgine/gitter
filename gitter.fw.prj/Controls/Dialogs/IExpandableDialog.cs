namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	/// <summary>Interface of dialog which can show/hide its part.</summary>
	public interface IExpandableDialog
	{
		/// <summary>Text, displayed on show/hide button.</summary>
		string ExpansionName { get; }

		/// <summary>Control, which contains hideable part.</summary>
		Control ExpansionControl { get; }
	}
}

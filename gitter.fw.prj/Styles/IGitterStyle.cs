namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	public interface IGitterStyle
	{
		string Name { get; }

		string DisplayName { get; }

		ToolStripRenderer ToolStripRenderer { get; }

		ViewRenderer ViewRenderer { get; }
	}
}

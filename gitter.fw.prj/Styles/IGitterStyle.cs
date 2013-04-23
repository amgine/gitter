namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	public interface IGitterStyle
	{
		string Name { get; }

		string DisplayName { get; }

		GitterStyleType Type { get; }

		IGitterStyleColors Colors { get; }

		IItemBackgroundStyles ItemBackgroundStyles { get; }

		IScrollBarWidget CreateScrollBar(Orientation orientation);

		ICheckBoxWidget CreateCheckBox();

		IButtonWidget CreateButton();

		CustomListBoxRenderer ListBoxRenderer { get; }

		ProcessOverlayRenderer OverlayRenderer { get; }

		ToolStripRenderer ToolStripRenderer { get; }

		ViewRenderer ViewRenderer { get; }
	}
}

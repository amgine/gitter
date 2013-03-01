namespace gitter.Framework
{
	using Color = System.Drawing.Color;

	public interface IGitterStyleColors
	{
		Color WorkArea						{ get; }
		Color Window						{ get; }
		Color ScrollBarSpacing				{ get; }
		Color Separator						{ get; }
		Color Alternate						{ get; }
		Color WindowText					{ get; }
		Color GrayText						{ get; }
		Color HyperlinkText					{ get; }
		Color HyperlinkTextHotTrack			{ get; }

		Color FileHeaderColor1				{ get; }
		Color FileHeaderColor2				{ get; }
		Color FilePanelBorder				{ get; }
		Color LineContextForeground			{ get; }
		Color LineContextBackground			{ get; }
		Color LineAddedForeground			{ get; }
		Color LineAddedBackground			{ get; }
		Color LineRemovedForeground			{ get; }
		Color LineRemovedBackground			{ get; }
		Color LineNumberForeground			{ get; }
		Color LineNumberBackground			{ get; }
		Color LineNumberBackgroundHover		{ get; }
		Color LineHeaderForeground			{ get; }
		Color LineHeaderBackground			{ get; }
		Color LineSelectedBackground		{ get; }
		Color LineSelectedBackgroundHover	{ get; }
		Color LineBackgroundHover			{ get; }
	}
}

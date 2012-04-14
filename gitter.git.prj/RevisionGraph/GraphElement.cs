namespace gitter.Git.Gui
{
	using System;

	[Flags]
	public enum GraphElement
	{
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		Space = (0 << 0),

		/// <summary>
		/// <para>...</para>
		/// <para>.+.</para>
		/// <para>...</para>
		/// </summary>
		Dot = (1 << GraphElementId.Dot),

		/// <summary>
		/// <para>...</para>
		/// <para>-..</para>
		/// <para>...</para>
		/// </summary>
		HorizontalLeft = (1 << GraphElementId.HorizontalLeft),
		/// <summary>
		/// <para>...</para>
		/// <para>..-</para>
		/// <para>...</para>
		/// </summary>
		HorizontalRight = (1 << GraphElementId.HorizontalRight),
		/// <summary>
		/// <para>.|.</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		VerticalTop = (1 << GraphElementId.VerticalTop),
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>.|.</para>
		/// </summary>
		VerticalBottom = (1 << GraphElementId.VerticalBottom),
		/// <summary>
		/// <para>\..</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		LeftTop = (1 << GraphElementId.LeftTop),
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>..\</para>
		/// </summary>
		RightBottom = (1 << GraphElementId.RightBottom),
		/// <summary>
		/// <para>../</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		RightTop = (1 << GraphElementId.RightTop),
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>/..</para>
		/// </summary>
		LeftBottom = (1 << GraphElementId.LeftBottom),
		/// <summary>
		/// <para>/..</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		LeftTopCorner = (1 << GraphElementId.LeftTopCorner),
		/// <summary>
		/// <para>..\</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		RightTopCorner = (1 << GraphElementId.RightTopCorner),
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>\..</para>
		/// </summary>
		LeftBottomCorner = (1 << GraphElementId.LeftBottomCorner),
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>../</para>
		/// </summary>
		RightBottomCorner = (1 << GraphElementId.RightBottomCorner),

		/// <summary>
		/// <para>...</para>
		/// <para>---</para>
		/// <para>...</para>
		/// </summary>
		Horizontal = HorizontalLeft | HorizontalRight,
		/// <summary>
		/// <para>.|.</para>
		/// <para>.|.</para>
		/// <para>.|.</para>
		/// </summary>
		Vertical = VerticalTop | VerticalBottom,
		/// <summary>
		/// <para>\..</para>
		/// <para>.\.</para>
		/// <para>..\</para>
		/// </summary>
		DiagonalLTRB = LeftTop | RightBottom,
		/// <summary>
		/// <para>../</para>
		/// <para>./.</para>
		/// <para>/..</para>
		/// </summary>
		DiagonalRTLB = RightTop | LeftBottom,
	}
}

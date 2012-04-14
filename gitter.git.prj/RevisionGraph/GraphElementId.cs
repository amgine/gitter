namespace gitter.Git.Gui
{
	public static class GraphElementId
	{
		/// <summary>
		/// <para>...</para>
		/// <para>.+.</para>
		/// <para>...</para>
		/// </summary>
		public const int Dot = 0;

		/// <summary>
		/// <para>...</para>
		/// <para>-..</para>
		/// <para>...</para>
		/// </summary>
		public const int HorizontalLeft = 1;
		/// <summary>
		/// <para>...</para>
		/// <para>..-</para>
		/// <para>...</para>
		/// </summary>
		public const int HorizontalRight = 2;
		/// <summary>
		/// <para>.|.</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int VerticalTop = 3;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>.|.</para>
		/// </summary>
		public const int VerticalBottom = 4;
		/// <summary>
		/// <para>\..</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int LeftTop = 5;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>..\</para>
		/// </summary>
		public const int RightBottom = 6;
		/// <summary>
		/// <para>../</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int RightTop = 7;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>/..</para>
		/// </summary>
		public const int LeftBottom = 8;
		/// <summary>
		/// <para>/..</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int LeftTopCorner = 9;
		/// <summary>
		/// <para>..\</para>
		/// <para>...</para>
		/// <para>...</para>
		/// </summary>
		public const int RightTopCorner = 10;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>\..</para>
		/// </summary>
		public const int LeftBottomCorner = 11;
		/// <summary>
		/// <para>...</para>
		/// <para>...</para>
		/// <para>../</para>
		/// </summary>
		public const int RightBottomCorner = 12;
	}
}

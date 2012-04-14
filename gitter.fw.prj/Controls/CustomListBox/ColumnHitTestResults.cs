namespace gitter.Framework.Controls
{
	/// <summary>Common results of hit-testing a column header (<see cref="CustomListBoxColumn"/>).</summary>
	internal static class ColumnHitTestResults
	{
		/// <summary>Header area.</summary>
		public const int Default = 0;
		/// <summary>Extender button.</summary>
		public const int Extender = -1;
		/// <summary>Column left resize grip.</summary>
		public const int LeftResizer = -2;
		/// <summary>Column right resize grip.</summary>
		public const int RightResizer = -3;
	}
}

namespace gitter.Framework.Controls
{
	/// <summary>Column size mode.</summary>
	public enum ColumnSizeMode
	{
		/// <summary>Column width is fixed and cannot be changed by user.</summary>
		Fixed,
		/// <summary>Column fills all available free space.</summary>
		Fill,
		/// <summary>Column width can be changed by user if column headers are visible.</summary>
		Sizeable,
		/// <summary>Column width is calculated based on content size.</summary>
		Auto,
	}
}

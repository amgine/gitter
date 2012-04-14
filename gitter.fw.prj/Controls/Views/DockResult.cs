namespace gitter.Framework.Controls
{
	/// <summary>Represents dock position.</summary>
	public enum DockResult
	{
		/// <summary>None.</summary>
		None = 0,

		/// <summary>Inside target dock host.</summary>
		Fill,

		/// <summary>On floating form.</summary>
		Float,

		/// <summary>To the left of dock host.</summary>
		Left,
		/// <summary>To the top of dock host.</summary>
		Top,
		/// <summary>To the bottom of dock host.</summary>
		Bottom,
		/// <summary>To the right of dock host.</summary>
		Right,

		/// <summary>Inside new document host to the left of dock host.</summary>
		DocumentLeft,
		/// <summary>Inside new document host to the top of dock host.</summary>
		DocumentTop,
		/// <summary>Inside new document host to the bottom of dock host.</summary>
		DocumentBottom,
		/// <summary>Inside new document host to the right of dock host.</summary>
		DocumentRight,

		/// <summary>Inside auto-hiding container at the left side of dock host.</summary>
		AutoHideLeft,
		/// <summary>Inside auto-hiding container at the top side of dock host.</summary>
		AutoHideTop,
		/// <summary>Inside auto-hiding container at the bottom side of dock host.</summary>
		AutoHideBottom,
		/// <summary>Inside auto-hiding container at the right side of dock host.</summary>
		AutoHideRight,
	}
}

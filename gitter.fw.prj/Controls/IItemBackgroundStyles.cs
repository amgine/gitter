namespace gitter.Framework.Controls
{
	public interface IItemBackgroundStyles
	{
		/// <summary>Focused item style.</summary>
		IBackgroundStyle Focused { get; }

		/// <summary>Focused+Selected item style.</summary>
		IBackgroundStyle SelectedFocused { get; }

		/// <summary>Selected item style.</summary>
		IBackgroundStyle Selected { get; }

		/// <summary>Selected without control focus.</summary>
		IBackgroundStyle SelectedNoFocus { get; }

		/// <summary>Hovered item status.</summary>
		IBackgroundStyle Hovered { get; }

		/// <summary>Hovered+Focused item status.</summary>
		IBackgroundStyle HoveredFocused { get; }
	}
}
